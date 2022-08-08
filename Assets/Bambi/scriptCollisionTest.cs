using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptCollisionTest : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		var test = collision;
	}

	private void OnTriggerEnter(Collider other)
	{
		var test = other;
	}
}
