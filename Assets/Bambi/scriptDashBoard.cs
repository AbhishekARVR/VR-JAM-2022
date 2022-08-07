using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scriptDashBoard : MonoBehaviour
{
	public Text txtTrashAmount;
	public Text txtFuelAmount;

	// Start is called before the first frame update
	void Start()
    {
		//validation
		if (txtTrashAmount == null)
			Debug.LogError("Missing trash amount text reference.", this);

		if (txtFuelAmount == null)
			Debug.LogError("Missing fuel amount text reference.", this);
	}

	public void updateTrashAmount(int trashCount)
	{
		txtTrashAmount.text = $"Trash: {trashCount}";
	}

	public void updateFuelAmount(int trashCount)
	{
		txtTrashAmount.text = $"Trash: {trashCount}";
	}
}
