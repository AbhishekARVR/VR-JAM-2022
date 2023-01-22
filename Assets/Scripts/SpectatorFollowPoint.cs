using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorFollowPoint : MonoBehaviour
{
	private SpectatorManager spectatorManager;
	
	public bool move;

	[Tooltip("Approximately the time it will take to reach the target. A smaller value will reach the target faster.")]
	public float moveSmoother = .3f;

	[Tooltip("World position to set the camera when assigned this follow point. Only used when move is unchecked to make sure we snap to the correct postion. Uses the follow points initial position to set.")]
	public Vector3 initialPosition;

	[Tooltip("Allows you to offset from the follow point, of you wanted to, for instance, keep a certain distance from the follow point.")]
	public Vector3 followPointOffset;

	[Tooltip("Shifts the follow point if parented. for instance, to look over the users shoulder.")]
	public Vector3 followPointParentOffset;

	[Tooltip("Parents the follow point to an object to dynamically track it.")]
	public Transform followPointParent;

	public bool look;
	public enum LookType
	{
		lookAtFollowPoint,
		lookWherePointLooks
	}

	public LookType lookType;

	[Tooltip("Value between 0 and 1 that defines the interpolated points to reach the destination. A smaller value means it will reach the target slower.")]
	[Range(0, 1)]
	public float lookSmoother = .3f;

	[Tooltip("When the object gets so close, stop rotating. Mostly just keeps camera from getting too close and getting a wonky rotation.")]
	public float lookThreshold = .01f;

	public bool maintainHorizon;

	private void Awake()
	{
		spectatorManager = transform.GetComponentInParent<SpectatorManager>();

		if (spectatorManager == null)
			Debug.LogError("Follow point could not find SpectatorManager component in parent. Make sure follow points start childed to the spectator manager.", this);
		
		initialPosition = transform.position;
	}
}
