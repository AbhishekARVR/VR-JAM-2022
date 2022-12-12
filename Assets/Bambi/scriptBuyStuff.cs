using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scriptBuyStuff : MonoBehaviour
{
	public enum ButtonType
	{
		Fuel,
		Capacity,
		Speed
	}
	public ButtonType type;

	public int cost;

	public AudioSource srcChaChing;
	
	private Button button;

	// Start is called before the first frame update
	void Start()
    {
		button = GetComponent<Button>();

		if (button == null)
			Debug.LogError("No button component found.", this);

		if (srcChaChing == null)
			Debug.LogError("No audio source found.", this);
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		//disable the button if we don't have the funds to pay.
		button.interactable = GameManager.Instance.playerFunds >= cost;
	}

	public void buyFuel()
	{
		srcChaChing?.PlayOneShot(srcChaChing.clip);

		GameManager.Instance.buyFuel(cost);
	}

	public void buyTrashCapacityUpgrade()
	{
		srcChaChing?.PlayOneShot(srcChaChing.clip);

		GameManager.Instance.buyTrashCapacityUpgrade(cost);
	}

	public void buySpeedUpgrade()
	{
		srcChaChing?.PlayOneShot(srcChaChing.clip);

		GameManager.Instance.buySpeedUpgrade(cost);
	}
}
