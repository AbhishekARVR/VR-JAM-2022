using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ActionBasedController))]
public class ControllerInput : MonoBehaviour
{
	public bool debugging;

	public ActionBasedController controller;
	public InputActionReference touchFaceReference;

	public enum HandPose
	{
		Empty,
		CalibrationTool,
		Paper,
		AmmoBag,
		Pistol
	}

	public HandPose currentPose;

	//fingys
	public readonly List<Finger> gripFingers = new List<Finger>()
	{
		new Finger(FingerType.Middle),
		new Finger(FingerType.Ring),
		new Finger(FingerType.Pinky)
	};
	public readonly List<Finger> triggerFingers = new List<Finger>()
	{
		new Finger(FingerType.Index)
	};
	public readonly List<Finger> thumbFingers = new List<Finger>()
	{
		new Finger(FingerType.Thumb)
	};

	private void Start()
	{
		//controller
		if (controller == null)
			Debug.LogError("Controller missing ActionBasedController component.", this);

		//touch face
		if (touchFaceReference == null)
			Debug.LogError("Controller missing touch face InputActionReference component.", this);
	}

	// Update is called once per frame
	void Update()
	{
		//Get inputs
		CheckGrip();
		CheckTrigger();
		CheckThumb();
	}

	private void CheckGrip()
	{
		SetFingerTargets(gripFingers, controller.selectAction.action.ReadValue<float>());
	}

	private void CheckTrigger()
	{
		float actionValue = controller.activateAction.action.ReadValue<float>();
		SetFingerTargets(triggerFingers, actionValue);
	}

	private void CheckThumb()
	{
		SetFingerTargets(thumbFingers, touchFaceReference.action.ReadValue<float>());
	}

	private void SetFingerTargets(List<Finger> fingers, float value)
	{
		foreach (Finger f in fingers)
		{
			if (debugging && value > .001)
				Debug.Log($"Type: {f.type}, Current: {f.current}, Target: {f.target}");

			f.target = value;
		}
	}

	public void SetPose(HandPose pose)
	{
		//validate that the 
		currentPose = pose;
	}
}