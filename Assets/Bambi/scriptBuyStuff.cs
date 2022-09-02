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
	
	private Button button;

	// Start is called before the first frame update
	void Start()
    {
		button = GetComponent<Button>();

		if (button == null)
			Debug.LogError("No button component found.", this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		//disable the button if we don't have the funds to pay.
		button.interactable = GameManager.Instance.playerFunds >= cost;
	}

	public void buyFuel()
	{
		GameManager.Instance.buyFuel(cost);
	}

	public void buyTrashCapacityUpgrade()
	{
		GameManager.Instance.buyTrashCapacityUpgrade(cost);
	}

	public void buySpeedUpgrade()
	{
		GameManager.Instance.buySpeedUpgrade(cost);
	}
}
