using UnityEngine;

public class SnakeNode : MonoBehaviour
{
    //[System.NonSerialized]
    public SnakeNode after;
    //[System.NonSerialized]
    public SnakeNode before;

    public void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        AssetManager assetManager = AssetManager.Instance;
        renderer.sprite = assetManager.GetSkin(Prefs.activeSkin).sprite;
    }
}