using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manage uninterrumped music through scenes and play sound effects from a pre-loaded audio pool.
/// </summary>
public class AudioManager : MonoBehaviour {
    const int DEFAULT_FX_POOLSIZE = 3;
    private static AudioManager audioManager = null;

    [SerializeField]
    private AudioClip musicClip;
    private AudioSource musicSource;

    private Dictionary<AudioClip, IndexedAudioSourceList> fxSourceDictionary = null;

    /// <summary>
    /// Get the AudioManager instance.
    /// </summary>
    public static AudioManager instance {
        get {
            Assert.IsNotNull(audioManager, "No AudioManager has been created before. Have a GameObject on scene with the AudioManager script");
            return audioManager;
        }
    }

    /// Constructor deactivated. Attach component to scene GameObject for use.
    protected AudioManager() { }

    /// <summary>
    /// Initialize singleton design.
    /// </summary>
    public void Awake() {
        if (audioManager == null) {
            audioManager = this;
            DontDestroyOnLoad(this.gameObject);

            if (musicClip != null)
                PlayMusic(musicClip);
            fxSourceDictionary = new Dictionary<AudioClip, IndexedAudioSourceList>();
        } else {
            // If a new AudioManager is found and it has a different musicClip attached, play the new musicClip.
            if (audioManager != this) {
                if (musicClip != audioManager.musicClip)
                    audioManager.PlayMusic(musicClip);
                Destroy(this.gameObject);
            }
        }
    }

    /// <summary>
    ///  Plays a looped music clip.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <param name="pan"></param>
    /// <param name="priority"></param>

    public void PlayMusic(AudioClip clip, float volume = 1, float pitch = 1, float pan = 0, int priority = 128) {
        if (musicSource != null) {
            musicSource.Stop();
            Destroy(musicSource);
            musicSource = null;
        }
        
        musicClip = clip;
        if (musicClip != null) {
            musicSource = CreateAudioSource(this.gameObject, clip, volume, true);
            musicSource.Play();
            musicSource.pitch = pitch;
            musicSource.panStereo = pan;
            musicSource.priority = priority;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Pauses the current playing music if one exists.
    /// </summary>

    public void PauseMusic() {
        if (musicSource != null) {
            musicSource.Pause();
        }
    }

    /// <summary>
    /// Unpauses the current playing music if one exists.
    /// </summary>

    public void UnPauseMusic() {
        if (musicSource != null) {
            musicSource.UnPause();
        }
    }

    /// <summary>
    /// Creates a pool of audio clips for multiple simultaneous playbacks
    /// </summary>
    /// <param name="clip"> The audio clip to play </param>
    /// <param name="poolSize"> The maximum ammount of AudioSources played from this clip at any given time </param>

    public void LoadFX(AudioClip clip, int poolSize = DEFAULT_FX_POOLSIZE) {
        if (poolSize <= 0)
            poolSize = DEFAULT_FX_POOLSIZE;

        IndexedAudioSourceList audioList;
        if (!fxSourceDictionary.TryGetValue(clip, out audioList)) {
            audioList = new IndexedAudioSourceList(this.gameObject, clip, poolSize);
            fxSourceDictionary.Add(clip, audioList);
        }
    }

    /// <summary>
    /// Plays an audio clip from an existing pool. 
    /// If no pool exists, one is created with default parameters.
    /// Audio sources in the pool are played in sequence and if the current audio to play is already playing, the audio is reset.
    /// Advanced customizations from PlayFXCustom aren't modified.
    /// </summary>
    /// <param name="clip"> The audio </param>

    public void PlayFX(AudioClip clip) {
        IndexedAudioSourceList audioList;
        if (!fxSourceDictionary.TryGetValue(clip, out audioList)) {
            audioList = new IndexedAudioSourceList(gameObject, clip, DEFAULT_FX_POOLSIZE);
            fxSourceDictionary.Add(clip, audioList);
        }
        
        AudioSource audioSource = audioList.Next();
        audioSource.Play();
    }

    /// <summary>
    /// Plays an audio clip from an existing pool with advanced audio customizations. 
    /// If no pool exists, one is created with deafult parameters.
    /// Audio sources in pool are played in sequence and if the current audio to play is already playing, the audio is reset.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <param name="pan"></param>
    /// <param name="priority"></param>

    public void PlayFXCustom(AudioClip clip, float volume = 1, float pitch = 1, float pan = 0, int priority = 128) {
        IndexedAudioSourceList audioList;
        if (!fxSourceDictionary.TryGetValue(clip, out audioList)) {
            audioList = new IndexedAudioSourceList(gameObject, clip, DEFAULT_FX_POOLSIZE);
            fxSourceDictionary.Add(clip, audioList);
        }
        
        AudioSource audioSource = audioList.Next();
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.panStereo = pan;
        audioSource.priority = priority;
        audioSource.Play();
    }

    /// <summary>
    /// Stops all audio sources currently playing from a specific clip or all sources if none is specified.
    /// </summary>
    /// <param name="clip"></param>

    public void StopFX(AudioClip clip = null) {
        if (clip != null) {
            IndexedAudioSourceList audioList;
            if (!fxSourceDictionary.TryGetValue(clip, out audioList)) {
                foreach (AudioSource audioSource in audioList.audioSourceList)
                audioSource.Stop();
            }
        } else {
            foreach (IndexedAudioSourceList audioList in fxSourceDictionary.Values)
                foreach (AudioSource audioSource in audioList.audioSourceList)
                    audioSource.Stop();
        }
    }

    /// <summary>
    /// Removes the asociated pool from a specific AudioClip or all pools if none is specified.
    /// </summary>
    /// <param name="clip"></param>

    public void ClearFx(AudioClip clip = null) {
        if (clip != null) {
            IndexedAudioSourceList audioList;
            if (!fxSourceDictionary.TryGetValue(clip, out audioList)) {
                audioList.Destroy();
                fxSourceDictionary.Remove(clip);
            }
        } else {
            foreach (IndexedAudioSourceList audioList in fxSourceDictionary.Values)
               audioList.Destroy();
            fxSourceDictionary.Clear();
        }
    }

    /// <summary>
    /// Creates and AudioSource from an AudioClip and attaches it to the specified GameObject
    /// </summary>
    /// <param name="gameObject">Attach the AudioSource to the specified GameObject</param>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="loop"></param>
    /// <param name="playOnAwake"></param>
    /// <returns></returns>

    public static AudioSource CreateAudioSource(GameObject gameObject, AudioClip clip, float volume = 1f, bool loop = false, bool playOnAwake = false) {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
        source.playOnAwake = playOnAwake;
        return source;
    }

    /// <summary>
    /// Pool container class.
    /// </summary>
    class IndexedAudioSourceList {
        public List<AudioSource> audioSourceList;
        public int index;

        /// <summary>
        /// Create an AudioSource pool from an AudioClip.
        /// AudioSource components are attached to the specified GameObject.
        /// </summary>
        /// <param name="hostObject">The GameObject which AudioSources will be attached.</param>
        /// <param name="audioClip">The AudioClip from which the AudioSources will be played.</param>
        /// <param name="size">The size of the pool.</param>
        public IndexedAudioSourceList(GameObject hostObject, AudioClip audioClip, int size) {
            audioSourceList = new List<AudioSource>(size);

            for (int i = 0; i < size; ++i)
                audioSourceList.Add(CreateAudioSource(hostObject, audioClip));
        }

        /// <summary>
        /// Get the next available AudioSource.
        /// </summary>
        /// <returns></returns>

        public AudioSource Next() {
            AudioSource audioSource = audioSourceList[index];
            index = (index + 1) % audioSourceList.Count;
            return audioSource;
        }

        /// <summary>
        /// Get the AudioSource at the current index.
        /// </summary>
        /// <returns></returns>

        public AudioSource Current() {
            return audioSourceList[index];
        }

        /// <summary>
        /// Destroy all created AudioSource components.
        /// </summary>

        public void Destroy() {
            foreach (AudioSource audioSource in audioSourceList)
                GameObject.Destroy(audioSource);
        }
    }
}