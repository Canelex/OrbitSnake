using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mass : MonoBehaviour
{
	[SerializeField]
	private float mass;

	public float GetMass()
	{
		return mass;
	}

	public void ChangeMass(float deltaMass)
	{
		mass += deltaMass;
	}

	public void SetMass(float newMass)
	{
		mass = newMass;
	}
}
