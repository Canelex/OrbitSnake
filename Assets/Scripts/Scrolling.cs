using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour
{
	private GameController game;

	private void Awake()
	{
		game = FindObjectOfType<GameController>();
	}

	public void Scroll(Vector2 distance)
	{
		transform.Translate(distance);

		if (transform.position.y < game.GetScreenHeight() * -2) // Twice of the screen
		{
			Destroy(gameObject);
		}
	}
}
