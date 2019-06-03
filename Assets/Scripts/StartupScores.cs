using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartupScores : MonoBehaviour
{
    public Text textBest;

    void Start()
    {
        textBest.text = Prefs.bestScore.ToString();
    }
}
