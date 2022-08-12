using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptTrashChute : MonoBehaviour
{
	public float suckPower;
	public GameObject trashSucker;

	private Animator trashSuckerAnim;
	private AudioSource oneShots;
	private AudioSource loop;
	
	List<Rigidbody> trashRBs = new List<Rigidbody>();

	//coroutines
	private IEnumerator suctionTrashRoutine;

	private void Start()
	{
		//validation
		if (trashSucker == null)
			Debug.LogError("No trash sucker assigned.", this);

		trashSuckerAnim = trashSucker.GetComponent<Animator>();
		trashSuckerAnim.speed = 0;

		var aScrs = trashSucker.GetComponents<AudioSource>();
		loop = aScrs[0];
		oneShots = aScrs[1];
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Boat") && suctionTrashRoutine == null)
		{
			Debug.Log("Entering trash suction zone.");

			var test = other.GetComponentInChildren<scriptCollector>();

			//get trash amount on player
			suctionTrashRoutine = SuctionTrashRoutine(test.collectedTrash);

			StartCoroutine(suctionTrashRoutine);
		}

		if (other.CompareTag("Trash"))
		{
			var trash = other.GetComponent<scriptTrash>();

			GameManager.Instance.updatePlayerFunds(trash.value);

			trashRBs.Remove(other.GetComponent<Rigidbody>());

			Destroy(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Boat"))
		{
			Debug.Log("Exiting trash suction zone.");

			if (suctionTrashRoutine != null)
			{
				StopCoroutine(suctionTrashRoutine);
				suctionTrashRoutine = null;
			}
			
			//Stop jiggle animation
			trashSuckerAnim.speed = 0;

			//Play die down sound effect
			oneShots.PlayOneShot(AudioManager.Instance.trashSuckerSpinDown);
			loop.Stop();
		}
	}

	//coroutines
	private IEnumerator SuctionTrashRoutine(List<GameObject> trashObjs)
	{
		//Clear out any dead references and set colliders to not be triggers
		List<GameObject> trashToRemove = new List<GameObject>();
		foreach (var trash in trashObjs)
		{
			if (trash == null)
				trashToRemove.Add(trash);
			else
				trash.GetComponent<Collider>().isTrigger = false;
		}
		foreach(var trash in trashToRemove)
		{
			Debug.Log("Removing old trash reference.", this);
			trashObjs.Remove(trash);
		}
		
		//Play start up sound, wait the duration of the clip length
		oneShots.PlayOneShot(AudioManager.Instance.trashSuckerStartUp);
		yield return new WaitForSeconds(AudioManager.Instance.trashSuckerStartUp.length);

		//Start jiggle animation
		trashSuckerAnim.speed = 3; //3 seems good?

		//Start audio
		loop.clip = AudioManager.Instance.trashSuckerRunning;
		loop.Play();

		//Update UI
		GameManager.Instance.removeTrash(trashObjs.Count);

		//prepare to suck trash
		foreach (GameObject trash in trashObjs)
		{
			var rb = trash.GetComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.useGravity = false;
			trashRBs.Add(rb);
		}

		//Suck trash
		while (trashRBs.Count > 0)
		{
			foreach(Rigidbody rb in trashRBs)
			{
				Vector3 diff = trashSucker.transform.position - rb.transform.position;
				rb.AddForce(diff * suckPower);
			}

			yield return null;
		}

		//Stop jiggle animation
		trashSuckerAnim.speed = 0;

		//Stop the audio
		oneShots.PlayOneShot(AudioManager.Instance.trashSuckerSpinDown);
		loop.Stop();
		yield return new WaitForSeconds(AudioManager.Instance.trashSuckerSpinDown.length);

		Debug.Log("Done taking trash.");

		suctionTrashRoutine = null;
	}
}
