# Unity-AudioManager
Simple scene-persistent music and audio pooling.

## Why
As a way to have more control of the audio resources employed in Unity, a centralized approach was taken.
With the AudioManager component you can play music through multiple scenes and preload ready-to-use AudioSources for sound effects.

When direct access to the AudioManager is not available, you can use the AudioManagerProxy instead, as in Unity's built-in UI system and animation events.

## How To Use
Add the Assets/AudioManager folder to your project or download the repo and open it on Unity.

Place a GameObject (preferably a prefab) with the AudioManager component in at least your starting scene to initialize the singleton reference. After that access using **AudioManager.instance** and any of the methods bellow:
  
  * **PlayMusic** (*AudioClip* clip)
  * **PauseMusic** ()
  * **UnPauseMusic** ()
  * **LoadFX** (*AudioClip* clip)
  * **PlayFX** (*AudioClip* clip)
  * **PlayFXCustom** (*AudioClip* clip, *float* volume, *float* pitch, *float* pan, *int* priority)
  * **StopFX** (*AudioClip* clip)
  * **ClearFx** (*AudioClip* clip)

For music tracks preferably put the AudioClip you want to play on the AudioManager component preferably or manually use the **PlayMusic** method in a script.

![AduioManager component](https://github.com/twyblade64/Unity-AudioManager/blob/master/Images/AudioManagerComponent.png?raw=true "AudioManager component")

For sound effects, first load the AudioClips using **LoadFX** and use **PlayFX** whenever you want to play them.

Alternatively, put them within the AudioManagerProxy and play them using **AudioManagerProxy.PlayClip**(*int* index).

![AudioManagerProxy component](https://github.com/twyblade64/Unity-AudioManager/blob/master/Images/AudioManagerProxyComponent.png?raw=true "AudioManagerProxy component")

![AudioManagerProxy use](https://github.com/twyblade64/Unity-AudioManager/blob/master/Images/AudioManagerProxyUse.png?raw=true "AudioManagerProxy use")

## Examples
Included in the Assets/Examples folder, you can check three different examples:
1. *01_SoundEffects*: A small demo with balls falling and playing a sound on OnCollisionEnter events.
2. *02_AudioManagerProxy*: Multiple buttons using the Unity UI, each one playing a different sound on click.
3. *03_Music*: Four different scenes showing the interactions with different music clips on the AudioManager.


## To Do
  * Add volume management for music and sounds.
  * Option to clear sounds on scene change.
  * Add 3D sound support.
