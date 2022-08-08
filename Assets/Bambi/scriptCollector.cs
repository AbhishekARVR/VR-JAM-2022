using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptCollector : MonoBehaviour
{
	public scriptDashBoard dashBoard;
	public Transform trashDumpPoint; //point trash will try to fall into trash basket
	public float tubeTime; //time it takes to get down the tube
	public float settleTime; //how long to keep the rigidbody active to settle trash into place
	public float tubeExitForce;
	public BoxCollider trashZone;

	public List<GameObject> collectedTrash;

	private void Start()
	{
		//validation
		if (dashBoard == null)
			Debug.LogError("No dash board reference set.", this);

		if (trashDumpPoint == null)
			Debug.LogError("No trash dump point set.", this);

		collectedTrash = new List<GameObject>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Trash"))
		{
			//Debug.Log("Got trash.");
			
			StartCoroutine("processTrash", collision.gameObject);
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag("Trash"))
		{
			//Debug.Log("Got trash.");

			StartCoroutine("processTrash", collision.gameObject);
		}
	}

	/// <summary>
	/// There may already be a rigidbody on the trash if it collided with one of the collector arms.
	/// If it did tweak the existing rigidbody. If not, add one.
	/// </summary>
	/// <param name="trashObj"></param>
	private void prepareRigidbody(GameObject trashObj)
	{
		if (trashObj.TryGetComponent<Rigidbody>(out Rigidbody curRb)) //update the existing rigidbody
		{
			//Remove the y constaint so it can fall into the trash basket.
			curRb.constraints = RigidbodyConstraints.None;
			curRb.isKinematic = true;
			curRb.useGravity = false;
			curRb.velocity = Vector3.zero;
			curRb.angularVelocity = Vector3.zero;
		}
		else //add a rigidbody
		{
			var newRb = trashObj.AddComponent<Rigidbody>();
			newRb.isKinematic = true;
			newRb.useGravity = false;
		}
	}

	//coroutines
	private IEnumerator processTrash(GameObject trashObj)
	{
		//Remove trash from chunk
		scriptOceanManager.Instance.RemoveCollectedTrash(trashObj.transform.position, trashObj);
		
		//Turn the trash obj off
		trashObj.SetActive(false);

		//Prepare trash to fall into the basket
		trashObj.transform.parent = transform.parent.parent; //parent the trash to the boat so physics don't go crazy on us
		Destroy(trashObj.GetComponent<scriptOceanBob>());
		prepareRigidbody(trashObj);
		trashObj.transform.position = trashDumpPoint.position;

		//Let trash move down the pipe
		yield return new WaitForSeconds(tubeTime);

		//Dump trash
		trashObj.SetActive(true);

		//Place randomly within the trash dump zone, so it doesn't disrupt the boat
		trashObj.transform.position = new Vector3(
			Random.Range(trashZone.bounds.min.x, trashZone.bounds.max.x),
			Random.Range(trashZone.bounds.min.y, trashZone.bounds.max.y),
			Random.Range(trashZone.bounds.min.z, trashZone.bounds.max.z)
		);

		//Apply a little launching force
		//trashObj.GetComponent<Rigidbody>().AddForce(-transform.up * tubeExitForce); //assuming spout is on the left side of the boat facing in.

		//Wait for trash to settle
		//yield return new WaitForSeconds(settleTime);

		//If we landed in the basked, cool turn off physics, if not destory the object.
		//Gives us a fun way to limit the amount of trash, without just having to set a specific trash limit.
		//var trash = trashObj.GetComponent<scriptTrash>();

		//if (trash.isCollected)
		//{
		//	trashObj.GetComponent<Rigidbody>().isKinematic = true;

		//	//Add trash to trashCollection
			collectedTrash.Add(trashObj);
		//	GameManager.Instance.trashCount++;

		//	//Update Dash UI
		//	dashBoard.updateTrashAmount(collectedTrash.Count);
		//}
		//else
		//{
		//	Destroy(trashObj);
		//}
	}
}
