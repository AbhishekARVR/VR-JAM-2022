using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingBarge : MonoBehaviour
{
	[Tooltip("Distance from the player where we'll destroy the starting barge as they are now out on their adventure and no longer need it.")]
	public float destroyThreshold = 75000;

	[SerializeField] private Transform player;

	private void Start()
	{
		var playerObj = GameObject.FindGameObjectWithTag("Player");

		if (playerObj == null)
			Debug.LogError("No player object found.", this);

		player = playerObj.transform;
	}
	// Update is called once per frame
	void Update()
    {
		var dist = (transform.position - player.position).sqrMagnitude;

		if (dist > destroyThreshold)
			Destroy(this.gameObject);
    }
}
