using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerProxy : MonoBehaviour {
    public AudioClip[] clips;

    // Use this for initialization
    void Start () {
        foreach (AudioClip clip in clips)
            AudioManager.instance.LoadFX(clip);
	}
	
    public void PlayClip(int id) {
        if (id < clips.Length)
            AudioManager.instance.PlayFX(clips[id]);
    }
}
