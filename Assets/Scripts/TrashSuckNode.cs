using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the trash as it gets sucked from the collector to the trash cage.
/// </summary>
public class TrashSuckNode : MonoBehaviour
{
	public Transform destination;
	public float speed = 1;
	//how close the node needs to be in order to destroy
	public float destroyThreshold = .01f;
	private AudioSource srcTrashSucking;

	[SerializeField]
	private bool sucking = false;

    // Start is called before the first frame update
    void Start()
    {
		srcTrashSucking = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
		if (!sucking) //don't do anything unless we've started the sucking process.
			return;

		CheckForDeath();

		MoveTowardsDestination();
    }

	private void CheckForDeath()
	{
		//if the node is sufficiently close to target, destory it
		var dist = (transform.position - destination.position).sqrMagnitude;

		if (dist < destroyThreshold)
			Destroy(this.gameObject);
	}

	private void MoveTowardsDestination()
	{
		transform.position = Vector3.MoveTowards(transform.position, destination.position, speed * Time.deltaTime);
	}

	public void GetTaSuckin()
	{
		if (srcTrashSucking == null)
			srcTrashSucking = gameObject.GetComponent<AudioSource>();

		srcTrashSucking.Play();

		sucking = true;
	}
}
