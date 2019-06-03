using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    private IEnumerator StartGame()
    {
        AssetManager assetManager = AssetManager.Instance;
        assetManager.Init();
        Shutters shutters = Shutters.Instance;
        shutters.Init();
        yield return new WaitForSeconds(2F);
        SceneManager.LoadScene("Menu");
    }

    void Start()
    {
        StartCoroutine(StartGame());
    }
}
