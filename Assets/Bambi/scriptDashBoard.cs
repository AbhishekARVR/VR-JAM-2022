using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scriptDashBoard : MonoBehaviour
{
	public Text txtTrashAmount;
	public Text txtFuelAmount;
	public Text txtFundsAmount;

	// Start is called before the first frame update
	void Start()
    {
		//validation
		if (txtTrashAmount == null)
			Debug.LogError("Missing trash amount text reference.", this);

		if (txtFuelAmount == null)
			Debug.LogError("Missing fuel amount text reference.", this);

		if (txtFundsAmount == null)
			Debug.LogError("Missing funds amount text reference.", this);
	}

	public void updateTrashAmount(int trashCount)
	{
		txtTrashAmount.text = $"Trash: {trashCount}";
	}

	public void updateFuelAmount(int fuelCount)
	{
		txtTrashAmount.text = $"Fuel: {fuelCount}";
	}

	public void updateFundsAmount(int fundsAmount)
	{
		txtFundsAmount.text = $"Funds: {fundsAmount}";
	}
}
