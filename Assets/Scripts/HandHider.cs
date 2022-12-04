using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandHider : MonoBehaviour
{
	public GameObject handObject;
	public XRInteractorLineVisual lineVisual;
	public XRRayInteractor interactor;

	private void OnEnable()
	{
		interactor.selectEntered.AddListener(SelectHide);
		interactor.selectExited.AddListener(SelectShow);

		interactor.hoverEntered.AddListener(HoverShow);
		interactor.hoverExited.AddListener(HoverHide);
	}

	private void OnDisable()
	{
		interactor.selectEntered.RemoveListener(SelectHide);
		interactor.selectExited.RemoveListener(SelectShow);

		interactor.hoverEntered.RemoveListener(HoverShow);
		interactor.hoverExited.RemoveListener(HoverHide);
	}

	/// <summary>
	/// Triggered when pressing the grip button
	/// </summary>
	/// <param name="args"></param>
	private void SelectShow(SelectExitEventArgs args)
	{
		setHandVisibility(true);
	}

	/// <summary>
	/// Triggered when releasing the grip button
	/// </summary>
	private void SelectHide(SelectEnterEventArgs args)
	{
		setHandVisibility(false);
	}

	/// <summary>
	/// Triggered when hovering over an interactable.
	/// </summary>
	/// <param name="args"></param>
	private void HoverShow(HoverEnterEventArgs args)
	{
		if (!args.interactableObject.transform.CompareTag("MainControl"))
			setRayVisibility(true);
	}

	/// <summary>
	/// Triggered when exiting a hover over an interactable.
	/// </summary>
	/// <param name="args"></param>
	private void HoverHide(HoverExitEventArgs args)
	{
		if (!args.interactableObject.transform.CompareTag("MainControl"))
			setRayVisibility(false);
	}

	public void setHandVisibility(bool isActive)
	{
		handObject.SetActive(isActive);
	}

	public void setRayVisibility(bool isEnabled)
	{
		lineVisual.enabled = isEnabled;
	}
}
