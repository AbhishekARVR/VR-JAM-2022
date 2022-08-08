using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptShipArm : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		var other = collision.gameObject;

		if (other.CompareTag("Trash"))
		{
			//Add a rigidbody so it bounces around.
			if (!other.TryGetComponent<Rigidbody>(out Rigidbody rb))
			{
				rb = other.AddComponent<Rigidbody>();
				rb.useGravity = false;
				rb.isKinematic = false;
				rb.constraints = RigidbodyConstraints.FreezePositionY;
			}
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		var other = collision.gameObject;

		if (other.CompareTag("Trash"))
		{
			//Add a rigidbody so it bounces around.
			if (!other.TryGetComponent<Rigidbody>(out Rigidbody rb))
			{
				rb = other.AddComponent<Rigidbody>();
				rb.useGravity = false;
				rb.isKinematic = false;
				rb.constraints = RigidbodyConstraints.FreezePositionY;
			}
		}
	}
}
