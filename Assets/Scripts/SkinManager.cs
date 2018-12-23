using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
	public Skin[] skins;

	public bool IsSkinUnlocked(string name)
	{
		return PlayerPrefs.GetInt("Skin" + name, 0) == 1;
	}

	public Skin GetSkinByName(string name)
	{
		foreach (Skin skin in skins)
		{
			if (name == skin.name) return skin;
		}

		return null;
	}
}
