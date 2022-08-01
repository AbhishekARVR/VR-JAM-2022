using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptCollector : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Trash"))
		{
			Destroy(collision.gameObject);
		}
	}
}
