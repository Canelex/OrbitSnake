using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackerIcon : MonoBehaviour
{
	public GameManager gameManager;
	public RectTransform icon;

    public void UpdateTracker(Transform rocket, Transform astronaut)
    {
		// Keep icon upright
		icon.rotation = Quaternion.identity;

		if (!CanSeePoint(astronaut.position))
		{
			// Show marker
			transform.localScale = Vector3.one;

			// Calculate angle to astronaut
			Vector3 difference = astronaut.position - rocket.position;
			float radians = Mathf.Atan2(difference.x, -difference.y);
			radians -= rocket.rotation.eulerAngles.z * Mathf.Deg2Rad;

			// Calculate position of marker on screen
			float halfWidth = Camera.main.scaledPixelWidth / 2F;
			float halfHeight = Camera.main.scaledPixelHeight / 2F;
			float radius = Mathf.Sqrt(halfHeight * halfHeight + halfWidth * halfWidth);
			float xPos = Mathf.Clamp(Mathf.Sin(radians) * radius, -halfWidth, halfWidth);
			float yPos = Mathf.Clamp(Mathf.Cos(radians) * -radius, -halfHeight, halfHeight);

			// Apply position and rotation
			transform.localPosition = new Vector2(xPos, yPos);
			transform.rotation = Quaternion.Euler(0, 0, radians * Mathf.Rad2Deg);
		}
		else
		{
			// Hide marker
			transform.localScale = Vector3.zero;
		}
    }

	private bool CanSeePoint(Vector3 position)
	{
		position = Camera.main.WorldToViewportPoint(position);
		return position.x > 0 && position.x < 1 && position.y > 0 && position.y < 1;
	}
}
