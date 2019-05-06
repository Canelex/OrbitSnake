using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shutters : Singleton<Shutters>
{
    [Range(0, 1F)]
    public float percent;
    private Texture2D texture;

    public void OnGUI()
    {
        Camera camera = Camera.main;
        float width = camera.pixelWidth * percent / 2;
        GUI.DrawTexture(new Rect(0, 0, width, camera.pixelHeight), texture);
        GUI.DrawTexture(new Rect(camera.pixelWidth - width, 0, width, camera.pixelHeight), texture);
    }

    public void SetShutters(float percent)
    {
        this.percent = percent;
    }

    public IEnumerator Open(float time)
    {
        percent = 1;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            percent = 1 - t / time;
            yield return null;
        }

        percent = 0;
    }

    public IEnumerator Close(float time)
    {
        percent = 0;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            percent = t / time;
            yield return null;
        }

        percent = 1;
    }

    private IEnumerator Scene(string scene)
    {
        yield return Close(0.5F);
        SceneManager.LoadScene(scene);
        yield return Open(0.5F);
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(Scene(scene));
    }

    public void Init()
    {
        texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture.SetPixel(0, 0, Color.black);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Point;
        texture.Apply();
    }
}
