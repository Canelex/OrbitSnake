using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetManager : Singleton<AssetManager>
{
    private Dictionary<string, string> localizedText;
    public Texture2D detailsTexture;
    private Sprite[] details;
    public AudioSource soundSource;
    public AudioSource musicSource;
    public Audio[] audios;
    public Skin[] skins;

    #region Localization

    public void LoadLocalizedText(string fileName)
    {
        // Custom loading script for android
        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(LoadLocalizedTextOnAndroid(fileName));
            return;
        }

        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath); // read json string
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(json);
            foreach (LocalizationData.LocalizationItem item in loadedData.items)
            {
                localizedText.Add(item.key, item.value); // Add each json key-value pair
            }

            Debug.Log("Loaded '" + fileName + "' with " + localizedText.Count + " entries.");
        }
        else
        {
            Debug.LogError("File '" + fileName + "' does not exist!");
        }
    }

    private IEnumerator LoadLocalizedTextOnAndroid(string fileName)
    {
        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath + "/", fileName);
        string json;

        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            Debug.Log("UNITY:" + System.Environment.NewLine + filePath);
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            json = www.downloadHandler.text;
        }
        else
        {
            json = File.ReadAllText(filePath);
        }

        LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(json);
        foreach (LocalizationData.LocalizationItem item in loadedData.items)
        {
            localizedText.Add(item.key, item.value); // Add each json key-value pair
        }

        Debug.Log("Loaded '" + fileName + "' with " + localizedText.Count + " entries.");
    }

    private string GetLocalizedFile()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified:
                return "localized_cs.json";
            case SystemLanguage.Russian:
                return "localized_ru.json";
            case SystemLanguage.Spanish:
                return "localized_es.json";
        }

        return "localized_en.json";
    }

    public string GetLocalizedString(string key)
    {
        if (localizedText != null && localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }

        return "null";
    }

    #endregion
    #region Textures/details/skins

    private void CreateSprites(int numSprites)
    {
        details = new Sprite[numSprites];

        for (int i = 0; i < numSprites; i++)
        {
            Rect randomRect = new Rect(Random.Range(0, 512), Random.Range(0, 512), 512, 512);
            details[i] = Sprite.Create(detailsTexture, randomRect, new Vector2(0.5F, 0.5F), 512);
        }
    }

    public Sprite GetRandomTexture()
    {
        return details[Random.Range(0, details.Length)];
    }

    public Skin GetSkin(string name)
    {
        foreach (Skin skin in skins)
        {
            if (skin.name == name) return skin;
        }

        return null;
    }

    #endregion
    #region Sound/music

    private void Play(string name, AudioSource source)
    {
        if (source.isPlaying)
        {
            source.Stop(); // Stop the current clip if it's running
        }

        foreach (Audio sound in audios)
        {
            if (sound.name == name)
            {
                // Play the sound.
                source.loop = sound.loop;
                source.volume = sound.volume;
                source.clip = sound.clip;
                source.Play();
                return;
            }
        }
    }

    public void PlaySound(string name)
    {
        Play(name, soundSource);
    }

    public void PlayMusic(string name)
    {
        Play(name, musicSource);
    }

    public void StopSound()
    {
        soundSource.Stop();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public bool IsSoundPlaying()
    {
        return soundSource.isPlaying;
    }

    public bool IsMusicPlaying()
    {
        return musicSource.isPlaying;
    }

    public void UpdateAudio()
    {
        soundSource.mute = !Prefs.soundEnabled;
        musicSource.mute = !Prefs.musicEnabled;
    }

    #endregion

    public new void Init()
    {
        LoadLocalizedText(GetLocalizedFile()); // load language file
        CreateSprites(10); // create planet sprites
        PlayMusic("Music"); // Play music
        UpdateAudio(); // Sync to settings

        IsReady = true;
        Debug.Log("Finshed initializing AssetManager.");
    }

}
