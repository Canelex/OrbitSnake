using UnityEngine;

class PlanetTextures : MonoBehaviour
{
    public static PlanetTextures Instance;
    public Texture2D texture;
    public int numSprites;
    private Sprite[] sprites;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void GenerateSprites()
    {
        sprites = new Sprite[numSprites];
        for (int i = 0; i < numSprites; i++)
        {
            Rect randomRect = new Rect(Random.Range(0, 512), Random.Range(0, 512), 512, 512);
        	sprites[i] = Sprite.Create(texture, randomRect, new Vector2(0.5F, 0.5F), 512);
        }
    }

    public Sprite GetRandomTexture()
    {
        return sprites[Random.Range(0, numSprites)];
    }
}