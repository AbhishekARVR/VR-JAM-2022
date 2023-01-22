using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using static SpectatorFollowPoint;

/// <summary>
/// Use the spectator camera to capture gameplay in cool ways.
/// Use follow points to set up different camera angles.
/// The space bar or the 'X' button on a VR controller will switch to the next camera angle.
/// </summary>
public class SpectatorManager : MonoBehaviour
{
	public Transform spectatorCamera;
	public List<Transform> followPoints;
	public Transform currentFollowPoint;
	public int followPointIndex = 0;

	private SpectatorFollowPoint currentFollowPointConfig;
	private Vector3 velocity = Vector3.zero;

	public InputAction pressXReference;

	private void OnEnable()
	{
		pressXReference.Enable();
		pressXReference.performed += PressX;
	}

	private void OnDisable()
	{
		pressXReference.Disable();
	}

	private void PressX (InputAction.CallbackContext context)
	{
		CutToFollowPoint(followPointIndex + 1);
	}

	// Start is called before the first frame update
	private void Start()
    {
		if (spectatorCamera == null)
			Debug.LogError("Spectator manager is missing a camera reference.", this);

		//Get all of the points positioned in the scene.
		foreach(var point in followPoints)
		{
			var followPointConfig = point.GetComponent<SpectatorFollowPoint>();
			ParentFollowPoint(point, followPointConfig.followPointParent);
		}

		//set up the current follow point
		CutToFollowPoint(followPointIndex);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (currentFollowPoint == null)
		{
			RemoveFollowPoint(followPointIndex);
			return;
		}
		
		if (currentFollowPointConfig.move)
			HandleMovement(currentFollowPointConfig.moveSmoother);

		if (currentFollowPointConfig.look)
			HandleLooking(currentFollowPointConfig.lookSmoother);
    }

	private void HandleMovement(float smoother)
	{
		if (currentFollowPointConfig.followPointParent != null)
			currentFollowPoint.localPosition = Vector3.zero + currentFollowPointConfig.followPointParentOffset;

		spectatorCamera.position = Vector3.SmoothDamp(spectatorCamera.position, currentFollowPoint.position + currentFollowPointConfig.followPointOffset, ref velocity, smoother);
	}

	private void HandleLooking(float smoother)
	{
		switch (currentFollowPointConfig.lookType)
		{
			case LookType.lookAtFollowPoint:
				var dist = (spectatorCamera.position - currentFollowPoint.position).sqrMagnitude;
				if (dist < currentFollowPointConfig.lookThreshold)
					return;

				Vector3 dir = currentFollowPoint.position - spectatorCamera.position;
				Quaternion rot = Quaternion.LookRotation(dir, currentFollowPoint.up);

				if (currentFollowPointConfig.maintainHorizon)
					rot = Quaternion.Euler(0, rot.eulerAngles.y, 0);

				spectatorCamera.rotation = Quaternion.Slerp(spectatorCamera.rotation, rot, smoother);
				break;
			case LookType.lookWherePointLooks:
				rot = currentFollowPoint.rotation;

				if (currentFollowPointConfig.maintainHorizon)
					rot = Quaternion.Euler(0, rot.eulerAngles.y, 0);

				spectatorCamera.rotation = Quaternion.Slerp(spectatorCamera.rotation, rot, smoother);
				break;
		}
	}

	/// <summary>
	/// Snap the spectator camera to the current angle and position.
	/// </summary>
	public void SnapCamera()
	{
		//position
		if (currentFollowPointConfig.move)
		{
			if (currentFollowPointConfig.followPointParent != null)
				currentFollowPoint.localPosition = Vector3.zero + currentFollowPointConfig.followPointParentOffset;

			spectatorCamera.position = currentFollowPoint.position + currentFollowPointConfig.followPointOffset;
		}
		else
			spectatorCamera.position = currentFollowPointConfig.initialPosition;

		//rotation
		switch (currentFollowPointConfig.lookType)
		{
			case LookType.lookAtFollowPoint:
				Vector3 dir = currentFollowPoint.position - spectatorCamera.position;
				Quaternion rot = Quaternion.LookRotation(dir, currentFollowPoint.up);

				if (currentFollowPointConfig.maintainHorizon)
					rot = Quaternion.Euler(0, rot.eulerAngles.y, 0);

				spectatorCamera.rotation = rot;
				break;
			case LookType.lookWherePointLooks:
				rot = currentFollowPoint.rotation;

				if (currentFollowPointConfig.maintainHorizon)
					rot = Quaternion.Euler(0, rot.eulerAngles.y, 0);

				spectatorCamera.rotation = rot;
				break;
		}
	}

	/// <summary>
	/// Initilizes everything needed to start interfacing with the requested camera position.
	/// </summary>
	/// <param name="index"></param>
	public void CutToFollowPoint(int index)
	{
		if (followPoints == null || followPoints.Count == 0)
		{
			Debug.LogError("Spectator manager has no follow points.", this);
			return;
		}

		//cycle the index
		followPointIndex = index % followPoints.Count;

		currentFollowPoint = followPoints[followPointIndex];
		currentFollowPointConfig = currentFollowPoint.GetComponent<SpectatorFollowPoint>();

		ParentFollowPoint(currentFollowPoint, currentFollowPointConfig.followPointParent);

		SnapCamera();
	}

	private void ParentFollowPoint(Transform point, Transform parent)
	{
		if (parent == null)
			return;

		point.parent = parent;
		point.localPosition = Vector3.zero;
	}

	public void RemoveFollowPoint(int indexToRemove)
	{
		followPoints.Remove(followPoints[indexToRemove]);

		//Move to the point that filled this point's spot.
		if (followPointIndex == indexToRemove)
			CutToFollowPoint(followPointIndex);
	}
}
