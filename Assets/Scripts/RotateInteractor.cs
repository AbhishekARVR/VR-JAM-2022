using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Custom rotator that assumes all local rotations will remain at 0.
/// </summary>
public class RotateInteractor : XRBaseInteractable
{
	public enum RotType
	{
		X,
		Y,
		Z,
	}
	public RotType rotType;
	[Tooltip("The set function will clean up the rotation to only fit within the set range of motion and snap to the set rotation axis.")]
	private Func<Transform, float, float, Transform> rotAdjFunc;
	
	[Tooltip("The object we wish to interact with.")]
	public Transform interactable;
	[Tooltip("The anchor initializing point, should be at the tip of the forward arrow.")]
	public Transform anchorSpawnPoint;
	[Tooltip("The initial rotation  of the interactable.")]
	private Quaternion initialRotation;
	public float maxRotation;
	public float minRotation;
	[Tooltip("When releasing the object, should the object return to its initial rotation.")]
	public bool returnToNeutral;
	[Tooltip("Speed at which the wheel returns to neutral.")]
	public float returnSpeed;

	private List<IXRSelectInteractor> interactors;
	private List<Transform> anchors;
	private Vector3 anchorCurrentPos;
	public GameObject anchorPrefab;

	public void Start()
	{
		//Init anchors
		anchors = new List<Transform>();

		//Cache the starting rotation when we being, this will hold the point to return to after letting go.
		initialRotation = interactable.localRotation;

		//Set the rotation function we want to use at runtime. Saves us from having to 'if' this behavoir every loop.
		switch (rotType)
		{
			case RotType.X:
				rotAdjFunc = applyXRotation;
				break;
			case RotType.Y:
				rotAdjFunc = applyYRotation;
				break;
			case RotType.Z:
				rotAdjFunc = applyZRotation;
				break;
		}
	}

	protected override void OnSelectEntered(SelectEnterEventArgs args)
	{
		base.OnSelectEntered(args);

		interactors = interactorsSelecting;

		SpawnAnchorPoint(interactors);
	}

	protected override void OnSelectExited(SelectExitEventArgs args)
	{
		base.OnSelectExited(args);

		interactors = interactorsSelecting;

		RemoveAnchorPoint(interactors, anchors);
	}

	private void Update()
	{
		if (interactors != null && interactors.Count > 0) //hands are on the wheel
			HandleObjectRotation(anchors, interactable);
		else if (returnToNeutral) //hands are off the wheel
			HandleReturnToNeutral(interactable);
	}

	private void HandleObjectRotation(List<Transform> anchors, Transform interactable)
	{
		//Check how far we've turned the wheel
		anchorCurrentPos = anchors[0].position;

		//average all anchors together to get the mid point
		if (anchors.Count > 1)
		{
			for(int i = 1; i < anchors.Count; i++)
			{
				var anchor = anchors[i];

				anchorCurrentPos.x += anchor.position.x;
				anchorCurrentPos.y += anchor.position.y;
				anchorCurrentPos.z += anchor.position.z;
			}

			anchorCurrentPos = new Vector3(anchorCurrentPos.x / anchors.Count, anchorCurrentPos.y / anchors.Count, anchorCurrentPos.z / anchors.Count);
		}

		ApplyRotation(interactable, anchorCurrentPos, maxRotation, minRotation);
	}

	private void ApplyRotation(Transform interactable, Vector3 anchorPos, float maxRotation, float minRotation)
	{
		interactable.LookAt(anchorPos);

		rotAdjFunc.Invoke(interactable, maxRotation, minRotation);
	}

	/// <summary>
	/// Uses and snaps to the x axis.
	/// </summary>
	private Func<Transform, float, float, Transform> applyXRotation = (interactable, maxRotation, minRotation) =>
	{
		var lookRotRelegated = interactable.localEulerAngles.x > 180 ? interactable.localEulerAngles.x - 360 : interactable.localEulerAngles.x;

		if (lookRotRelegated > maxRotation)
			lookRotRelegated = maxRotation;

		if (lookRotRelegated < minRotation)
			lookRotRelegated = minRotation;

		interactable.localEulerAngles = new Vector3(lookRotRelegated, 0, 0);

		return interactable;
	};

	/// <summary>
	/// Uses and snaps to the y axis.
	/// </summary>
	private Func<Transform, float, float, Transform> applyYRotation = (interactable, maxRotation, minRotation) =>
	{
		var lookRotRelegated = interactable.localEulerAngles.y > 180 ? interactable.localEulerAngles.y - 360 : interactable.localEulerAngles.y;

		if (lookRotRelegated > maxRotation)
			lookRotRelegated = maxRotation;

		if (lookRotRelegated < minRotation)
			lookRotRelegated = minRotation;

		interactable.localEulerAngles = new Vector3(0, lookRotRelegated, 0);

		return interactable;
	};

	/// <summary>
	/// Uses and snaps to the z axis.
	/// </summary>
	private Func<Transform, float, float, Transform> applyZRotation = (interactable, maxRotation, minRotation) =>
	{
		var lookRotRelegated = interactable.localEulerAngles.z > 180 ? interactable.localEulerAngles.z - 360 : interactable.localEulerAngles.z;

		if (lookRotRelegated > maxRotation)
			lookRotRelegated = maxRotation;

		if (lookRotRelegated < minRotation)
			lookRotRelegated = minRotation;

		interactable.localEulerAngles = new Vector3(0, 0, lookRotRelegated);

		return interactable;
	};

	/// <summary>
	/// Return interactable to initial rotation after letting go with both hands.
	/// </summary>
	/// <param name="interactable"></param>
	private void HandleReturnToNeutral(Transform interactable)
	{
		//Check if we've reached neutral, kick out if so
		if (interactable.localRotation.eulerAngles == initialRotation.eulerAngles)
			return;

		//Progressively return to neutral
		interactable.localRotation = Quaternion.RotateTowards(interactable.localRotation, initialRotation, returnSpeed * Time.deltaTime);
	}

	/// <summary>
	/// When the player grabs the object, set new anchor point so that we move the object reletive to the hand instead of snapping to it.
	/// </summary>
	/// <returns></returns>
	private void SpawnAnchorPoint(List<IXRSelectInteractor> interactors)
	{
		//var anchor = Instantiate(new GameObject("Anchor"), anchorSpawnPoint.position, anchorSpawnPoint.rotation);
		var anchor = Instantiate(anchorPrefab, anchorSpawnPoint.position, anchorSpawnPoint.rotation);
		anchor.name = "anchor";

		//Find the new hand that just grabbed and parent the new anchor to that hand.
		foreach(var interactor in interactors)
		{
			if (interactor.transform.Find("anchor") == null)
			{
				anchor.transform.parent = interactor.transform;
				break;
			}
		}

		anchors.Add(anchor.transform);
	}

	/// <summary>
	/// When the player lets go of the object, remove that anchor point.
	/// We also want to shift the current holding hand's anchor to the average point so
	/// the wheel doesn't snap to the remaining controller's anchor.
	/// </summary>
	/// <returns></returns>
	private void RemoveAnchorPoint(List<IXRSelectInteractor> interactors, List<Transform> anchors)
	{
		Transform anchorToRemove = null;

		foreach(var anchor in anchors)
		{
			var anchorParent = anchor.parent;
			var found = false;

			//Find the anchor that is no longer in use
			foreach (var interactor in interactors)
			{
				if (interactor.transform == anchorParent)
				{
					found = true;

					anchor.position = anchorCurrentPos; //set the position to the current position so that we don't jerk the wheel when one hand lets go.

					break; //found the parent so move on
				}
			}

			if (!found) //then this anchor is no longer in use
				anchorToRemove = anchor;
		}

		//Remove the anchor
		if (anchorToRemove != null) 
		{
			anchors.Remove(anchorToRemove);

			Destroy(anchorToRemove.gameObject);
		}
	}
}
