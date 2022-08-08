using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptTrashChute : MonoBehaviour
{
	public float speed;
	public GameObject trashSucker;

	private Animator trashSuckerAnim;
	private AudioSource oneShots;
	private AudioSource loop;

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
	private IEnumerator SuctionTrashRoutine(int trashCount)
	{
		//Play start up sound, wait the duration of the clip length
		oneShots.PlayOneShot(AudioManager.Instance.trashSuckerStartUp);
		yield return new WaitForSeconds(AudioManager.Instance.trashSuckerStartUp.length);

		//Start jiggle animation
		trashSuckerAnim.speed = 3; //3 seems good?

		//Start audio
		loop.clip = AudioManager.Instance.trashSuckerRunning;
		loop.Play();

		//Suck trash
		while (trashCount > 0)
		{
			trashCount--;
			Debug.Log(trashCount);

			//Add money to player

			yield return new WaitForSeconds(speed);
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
