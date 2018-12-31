using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOverlay : MonoBehaviour
{
	private Vector3 position;
	private Button button;
	private bool pointerDown;
	private bool pointerOver;
	public float verticalOffset;

    private void Start()
    {
		button = transform.parent.GetComponent<Button>();
		position = transform.localPosition;
    }

	private void PointerUpdate()
	{
		if (pointerOver && pointerDown && button.interactable)
		{
			transform.localPosition = position + Vector3.up * verticalOffset;
		}
		else
		{
			transform.localPosition = position;
		}
	}

	public void PointerDown()
	{
		pointerDown = true;
		PointerUpdate();
	}

	public void PointerUp()
	{
		pointerDown = false;
		PointerUpdate();
	}

	public void PointerEnter()
	{
		pointerOver = true;
		PointerUpdate();
	}

	public void PointerExit()
	{
		pointerOver = false;
		PointerUpdate();
	}
}
