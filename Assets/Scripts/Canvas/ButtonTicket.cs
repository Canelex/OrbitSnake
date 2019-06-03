using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTicket : MonoBehaviour
{
    private Image image;
    public Sprite spriteDefault;
    public Sprite spriteSmall;
    public Sprite spriteBig;

    void Update()
    {
        if (Prefs.bigTicketPurchased)
        {
            image.sprite = spriteBig;
        }
        else if (Prefs.smallTicketPurchased)
        {
            image.sprite = spriteSmall;
        }
        else
        {
            image.sprite = spriteDefault;
        }
    }

    void Start()
    {
        image = GetComponent<Image>();
    }
}
