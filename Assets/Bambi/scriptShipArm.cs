using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptShipArm : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Trash"))
		{
			collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
		}
	}
}
