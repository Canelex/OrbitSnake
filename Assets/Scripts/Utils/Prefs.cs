using UnityEngine;

class Prefs : PlayerPrefs
{
    public static bool GetBool(string key, bool defValue)
    {
        return GetInt(key, defValue ? 1 : 0) == 1;
    }

    public static void SetBool(string key, bool value)
    {
        SetInt(key, value ? 1 : 0);
    }
}