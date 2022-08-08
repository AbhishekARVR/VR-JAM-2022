using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptTrashSucker : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Trash"))
		{
			var trash = other.GetComponent<scriptTrash>();

			GameManager.Instance.updatePlayerFunds(trash.value);

			Destroy(other);
		}
	}
}
