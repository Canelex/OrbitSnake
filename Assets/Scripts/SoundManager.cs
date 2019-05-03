using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    public AudioSource sound;
    public AudioSource music;
    public Sound[] sounds;

    private void Update()
    {
        if (IsMusicPlaying() && !Prefs.GetBool("music_enabled", true))
        {
            StopMusic();
        }

        if (!IsMusicPlaying() && Prefs.GetBool("music_enabled", true))
        {
            PlayMusic("Music");
        }
    }

    private void Play(string name, AudioSource source)
    {
        if (source.isPlaying)
        {
            source.Stop(); // Stop the current clip if it's running
        }

        foreach (Sound sound in sounds)
        {
            if (sound.name == name)
            {
                // Play the sound.
                source.loop = sound.loop;
                source.volume = sound.volume;
                source.clip = sound.source;
                source.Play();
                return;
            }
        }
    }

    public void PlaySound(string name)
    {
        if (!Prefs.GetBool("sound_enabled", true)) return;

        Play(name, sound);
    }

    public void PlayMusic(string name)
    {
        Play(name, music);
    }

    public bool IsSoundPlaying()
    {
        return sound.isPlaying;
    }

    public bool IsMusicPlaying()
    {
        return music.isPlaying;
    }

    public void StopSound()
    {
        sound.Stop();
    }

    public void StopMusic()
    {
        music.Stop();
    }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip source;
        [Range(0, 1)]
        public float volume;
        public bool loop;
    }
}