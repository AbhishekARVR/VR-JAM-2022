using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptTrash : MonoBehaviour
{
	public int value;
	public bool isCollected;

	private void Awake()
	{
		isCollected = false;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "GarbageCollector")
		{
			isCollected = true;
			collision.gameObject.transform.parent.GetComponent<BoatController>().AddTrashValue(value);
			Destroy(this.gameObject);
		}
	}
}
