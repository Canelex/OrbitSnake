using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shutters : Singleton<Shutters>
{
    [Range(0, 1F)]
    public float percent;
    private Texture2D texture;
    private bool loadingScene;

    public bool LoadingScene
    {
        get {return loadingScene;}
        set {loadingScene = value;}
    }

    public void OnGUI()
    {
        Camera camera = Camera.main;
        float width = Mathf.Ceil(camera.pixelWidth * percent / 2F);
        GUI.DrawTexture(new Rect(0, 0, width, camera.pixelHeight), texture);
        GUI.DrawTexture(new Rect(camera.pixelWidth - width, 0, width, camera.pixelHeight), texture);
    }

    public void SetShutters(float percent)
    {
        this.percent = percent;
    }

    public IEnumerator OpenShutters(float time)
    {
        percent = 1;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            percent = 1 - t / time;
            yield return null;
        }

        percent = 0;
    }

    public IEnumerator CloseShutters(float time)
    {
        percent = 0;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            percent = t / time;
            yield return null;
        }

        percent = 1;
    }

    private IEnumerator GoToScene(string scene)
    {
        LoadingScene = true;
        yield return CloseShutters(0.5F);
        SceneManager.LoadScene(scene);
        yield return OpenShutters(0.5F);
        LoadingScene = false;
    }

    public void LoadSceneWithShutters(string scene)
    {
        StartCoroutine(GoToScene(scene));
    }

    public new void Init()
    {
        texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture.SetPixel(0, 0, Color.black);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        IsReady = true;
        Debug.Log("Finished initializing Shutters.");
    }
}
