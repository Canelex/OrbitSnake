using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
	public float animSpeed;
	public string defaultOpen;
	[SerializeField]
	private Page[] pages;
	[SerializeField]
	private Field[] fields;
	private Page active;

	private void Start()
	{
		active = GetPage(defaultOpen);
		active.Toggle(true);
	}

	public Page GetPage(string name)
	{
		foreach (Page page in pages)
		{
			if (page.name == name)
			{
				return page;
			}
		}

		return null;
	}

	public void OpenPage(string name)
	{
		Page page = GetPage(name);

		if (active != null)
		{
			active.Toggle(false);
		}

		page.Toggle(true);
		active = page;
	}

	public Field GetField(string name)
	{
		foreach (Field field in fields)
		{
			if (field.name == name)
			{
				return field;
			}
		}

		return null;
	}

	[System.Serializable]
	public class Page
	{
		public string name;
		public GameObject obj;

		public void Toggle(bool enabled)
		{
			obj.SetActive(enabled);
		}
	}

	[System.Serializable]
	public class Field
	{
		public string name;
		public Text textField;
		public Image imageField;

		public void SetText(string text)
		{
			textField.text = text;
		}

		public void SetImage(Sprite sprite)
		{
			imageField.sprite = sprite;
		}
	}
}
