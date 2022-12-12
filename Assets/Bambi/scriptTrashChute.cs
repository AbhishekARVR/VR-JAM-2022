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
	
	private scriptCollector collector;
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
			collector = other.GetComponentInChildren<scriptCollector>();

			//get trash amount on player
			suctionTrashRoutine = SuctionTrashRoutine(collector);

			StartCoroutine(suctionTrashRoutine);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (trashSuckerAnim.speed > 0) //we only want to take the trash if we've actually started sucking trash
		{
			if (other.CompareTag("Trash"))
			{
				//Play thunk sound
				oneShots.PlayOneShot(AudioManager.Instance.trashSuckerPop);
				
				var trash = other.GetComponent<scriptTrash>();

				if (GameManager.Instance.updateTrash(-1))
					GameManager.Instance.updatePlayerFunds(trash.value);

				//Clean up game references
				collector.collectedTrash.Remove(other.gameObject);
				trashRBs.Remove(other.GetComponent<Rigidbody>());

				Destroy(other.gameObject);
			}
		}
	}

	//coroutines
	private IEnumerator SuctionTrashRoutine(scriptCollector collector)
	{
		//Clear out any dead references and set colliders to not be triggers
		List<GameObject> trashToRemove = new List<GameObject>();
		foreach (var trash in collector.collectedTrash)
		{
			if (trash == null)
				trashToRemove.Add(trash);
			else
				trash.GetComponent<Collider>().isTrigger = false;
		}
		foreach(var trash in trashToRemove)
		{
			Debug.Log("Removing old trash reference.", this);
			collector.collectedTrash.Remove(trash);
		}
		
		//Play start up sound, wait the duration of the clip length
		oneShots.PlayOneShot(AudioManager.Instance.trashSuckerStartUp);
		yield return new WaitForSeconds(AudioManager.Instance.trashSuckerStartUp.length);

		//Start audio
		loop.clip = AudioManager.Instance.trashSuckerRunning;
		loop.Play();

		//Start jiggle animation
		trashSuckerAnim.speed = 3; //3 seems good?

		//prepare to suck trash
		foreach (GameObject trash in collector.collectedTrash)
		{
			trash.transform.parent = null;//remove boat parenting so the physics don't go crazy.
			
			var rb = trash.GetComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.useGravity = false;
			rb.drag = 1;
			trashRBs.Add(rb);
		}

		//Suck trash
		Transform suckPoint = trashSucker.transform.Find("SuckPoint");
		while (trashRBs.Count > 0)
		{
			foreach(Rigidbody rb in trashRBs)
			{
				Vector3 diff = suckPoint.position - rb.transform.position;
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
