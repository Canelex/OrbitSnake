using UnityEngine;

[System.Serializable]
public class Skin
{
    public string name;
    public Sprite sprite;
    public int cost;

    public int GetCost()
    {
        if (Prefs.bigTicketPurchased)
        {
            return (int)(cost * 0.20);
        }
        else if (Prefs.smallTicketPurchased)
        {
            return (int)(cost * 0.60);
        }
        else
        {
            return cost;
        }
    }
}