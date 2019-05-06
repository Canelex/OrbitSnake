using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    private AssetManager assetManager;
    private Shutters shutters;

    void Start()
    {
        shutters = Shutters.Instance;
        shutters.Init();
        shutters.SetShutters(1F);

        assetManager = AssetManager.Instance;
        assetManager.Init();

        StartCoroutine(shutters.Open(0.3F));
    }
}
