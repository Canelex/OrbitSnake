using UnityEngine;

public class Prefs : PlayerPrefs
{
    public static int bestScore
    {
        get {return Prefs.GetInt("best_score_points", 0);}
        set {Prefs.SetInt("best_score_points", value);}
    }

    public static int lastScore
    {
        get {return Prefs.GetInt("last_score_points", 0);}
        set {Prefs.SetInt("last_score_points", value);}
    }

    public static bool bigTicketPurchased
    {
        get {return GetBool("big_ticket_purchased", false); }
        set {SetBool("big_ticket_purchased", value); }
    }

    public static bool smallTicketPurchased
    {
        get {return GetBool("small_ticket_purchased", false); }
        set {SetBool("small_ticket_purchased", value); }
    }

    public static string activeSkin
    {
        get {return GetString("active_skin", "default");}
        set {SetString("active_skin", value);}
    }

    public static int credits
    {
        get {return GetInt("credits", 0);}
        set {SetInt("credits", value);}
    }

    public static bool musicEnabled
    {
        get {return GetBool("music_enabled", true);}
        set {SetBool("music_enabled", value);}
    }

    public static bool soundEnabled
    {
        get {return GetBool("sound_enabled", true);}
        set {SetBool("sound_enabled", value);}
    }

    public static bool GetBool(string key, bool defValue)
    {
        return GetInt(key, defValue ? 1 : 0) == 1;
    }

    public static void SetBool(string key, bool value)
    {
        SetInt(key, value ? 1 : 0);
    }
}