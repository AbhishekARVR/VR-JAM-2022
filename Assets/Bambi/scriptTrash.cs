using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptTrash : MonoBehaviour
{
	public int value;
	public bool isCollected;
	public AudioClip collectionSound;

	private void Awake()
	{
		isCollected = false;
	}
}
