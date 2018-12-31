using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astronaut : MonoBehaviour
{
	public bool collected;
	private Collider2D myCollider;

	private void Start()
	{
		myCollider = GetComponent<Collider2D>();
	}

    private void Update()
    {
		Collider2D[] results = new Collider2D[8];
		int num = myCollider.OverlapCollider(new ContactFilter2D(), results);
		for (int i = 0; i < num; i++)
		{
			if (results[i].tag == "Planet")
			{
				transform.position = Vector3.MoveTowards(transform.position, results[0].transform.position, -2F * Time.deltaTime);
				break;
			}
		}
    }

	public void FlyAtRocket(Transform rocket)
	{
		StartCoroutine(FlyAtRocket(rocket, 4F));
	}

	private IEnumerator FlyAtRocket(Transform rocket, float speed)
	{
		while (Vector3.Distance(transform.position, rocket.position) > 0.25F)
		{
			transform.position = Vector3.MoveTowards(transform.position, rocket.position, speed * Time.deltaTime);
			yield return null;
		}

		Destroy(gameObject);
	}
}
