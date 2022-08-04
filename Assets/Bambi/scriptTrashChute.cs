using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptTrashChute : MonoBehaviour
{
	public float speed;

	//coroutines
	private IEnumerator suctionTrashRoutine;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Boat") && suctionTrashRoutine == null)
		{
			Debug.Log("Entering trash suction zone.");

			//get trash amount on player
			//suctionTrashRoutine = SuctionTrashRoutine(GameManger.trashAmount);
			suctionTrashRoutine = SuctionTrashRoutine(10);

			StartCoroutine(suctionTrashRoutine);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Boat"))
		{
			Debug.Log("Exiting trash suction zone.");

			if (suctionTrashRoutine != null)
				StopCoroutine(suctionTrashRoutine);

			//Play die down sound effect

		}
	}

	//coroutines
	private IEnumerator SuctionTrashRoutine(int trashCount)
	{
		//Play start up sound, wait the duration of the clip length
		yield return new WaitForSeconds(3);

		//Suck trash
		while (trashCount > 0)
		{
			trashCount--;
			Debug.Log(trashCount);

			//Add money to player

			yield return new WaitForSeconds(speed);
		}

		Debug.Log("Done taking trash.");

		suctionTrashRoutine = null;
	}
}
