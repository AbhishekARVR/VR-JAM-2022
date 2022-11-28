using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WheelTest : MonoBehaviour
{
	//List<IXRSelectInteractor> interactors;
	public Transform hand;

	[Tooltip("The object we wish to interact with.")]
	public Transform interactable;
	[Tooltip("The initial rotation  of the interactable.")]
	private Quaternion initialRotation;
	public float maxRotation;
	public float minRotation;
	[Tooltip("When releasing the object, should the object return to its initial rotation.")]
	public bool returnToNeutral;
	[Tooltip("Speed at which the wheel returns to neutral.")]
	public float returnSpeed;

	private void Start()
	{
		initialRotation = interactable.localRotation;
	}

	private void Update()
	{
		if (hand != null) //hands are on the wheel
			HandleObjectRotation(hand, interactable);
		else if (returnToNeutral) //hands are off the wheel
			HandleReturnToNeutral(interactable);
	}

	private void HandleObjectRotation(Transform hand, Transform interactable)
	{
		//Check how far we've turned the wheel
		var hp = hand.localPosition;
		var wp = interactable.localPosition;

		var dir = hp - wp;

		var lookRot = Quaternion.LookRotation(dir, interactable.up);

		//Check if our new rotation is within the accepted boundaries, if not kick out
		var lookRotNegated = lookRot.eulerAngles.y > 180 ? lookRot.eulerAngles.y - 360 : lookRot.eulerAngles.y;
		if (lookRotNegated > maxRotation || lookRotNegated < minRotation)
			return;

		//Apply the rotation
		interactable.localEulerAngles = new Vector3(interactable.localEulerAngles.x, lookRot.eulerAngles.y, interactable.localEulerAngles.z);
	}

	private void HandleReturnToNeutral(Transform interactable)
	{
		//Check if we've reached neutral, kick out if so
		if (interactable.localRotation.eulerAngles == initialRotation.eulerAngles)
			return;

		//Progressively return to neutral
		interactable.localRotation = Quaternion.RotateTowards(interactable.localRotation, initialRotation, returnSpeed * Time.deltaTime);
	}
}
