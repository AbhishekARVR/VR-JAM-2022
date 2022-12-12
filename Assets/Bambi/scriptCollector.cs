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

	private AudioSource srcTrashZone;
	private AudioSource srcTrashJostle;
	public GameObject trashSuckNode;

	public List<GameObject> collectedTrash;

	private void Start()
	{
		//validation
		if (dashBoard == null)
			Debug.LogError("No dash board reference set.", this);

		if (trashDumpPoint == null)
			Debug.LogError("No trash dump point set.", this);

		srcTrashJostle = GetComponent<AudioSource>();

		if (srcTrashJostle == null)
			Debug.LogError("No audio source component set.", this);

		if (trashSuckNode == null)
			Debug.LogError("No trash suck node set.", this);


		//initialization
		collectedTrash = new List<GameObject>();

		srcTrashZone = trashZone.gameObject.GetComponent<AudioSource>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Trash"))
		{
			StartCoroutine("processTrash", collision.gameObject);
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag("Trash"))
		{
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
		if (GameManager.Instance.updateTrash(1))
		{
			//Play trash collection sound
			srcTrashJostle.clip = trashObj.GetComponent<scriptTrash>().collectionSound;
			srcTrashJostle.PlayOneShot(srcTrashJostle.clip);
			
			//Remove trash from its chunk
			scriptOceanManager.Instance.RemoveCollectedTrash(trashObj.transform.position, trashObj);
		
			//Prepare trash to fall into the basket
			trashObj.SetActive(false);
			trashObj.transform.parent = transform.parent.parent; //parent the trash to the boat so physics don't go crazy on us
			Destroy(trashObj.GetComponent<scriptOceanBob>());
			prepareRigidbody(trashObj);
			trashObj.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);

			//Start the trash suck audio sequence
			var trashNodeObj = Instantiate(trashSuckNode, transform.position, Quaternion.identity);
			var trashNode = trashNodeObj.GetComponent<TrashSuckNode>();
			trashNode.destination = trashZone.transform;
			trashNode.GetTaSuckin();

			//Let trash move down the pipe
			while (trashNode != null)
			{
				yield return null;
			}
			//yield return new WaitForSeconds(tubeTime);

			//Dump trash
			trashObj.SetActive(true);

			//Jostle the cage
			srcTrashZone.PlayOneShot(srcTrashZone.clip);

			//Place randomly within the trash dump zone
			trashObj.transform.position = new Vector3(
				Random.Range(trashZone.bounds.min.x, trashZone.bounds.max.x),
				Random.Range(trashZone.bounds.min.y, trashZone.bounds.max.y),
				Random.Range(trashZone.bounds.min.z, trashZone.bounds.max.z)
			);

			collectedTrash.Add(trashObj);
		}
	}
}
