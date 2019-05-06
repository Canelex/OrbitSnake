using UnityEngine;

[System.Serializable]
public class Audio
{
    public string name;
    public AudioClip clip;
    [Range(0, 1F)]
    public float volume;
    public bool loop;
}