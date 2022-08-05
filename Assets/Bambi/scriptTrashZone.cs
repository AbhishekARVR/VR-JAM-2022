using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptTrashZone : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Trash"))
		{
			if (other.TryGetComponent<scriptTrash>(out scriptTrash trash))
			{
				trash.isCollected = true;
			}
			else
				Debug.LogError("No trash script found on trash object.", other);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Trash"))
		{
			if (other.TryGetComponent<scriptTrash>(out scriptTrash trash))
			{
				trash.isCollected = false;
			}
			else
				Debug.LogError("No trash script found on trash object.", other);
		}
	}
}
