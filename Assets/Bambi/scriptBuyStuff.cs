using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptBuyStuff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void buyFuel(int cost)
	{
		GameManager.Instance.buyFuel(cost);
	}

	public void buyTrashCapacityUpgrade(int cost)
	{
		GameManager.Instance.buyTrashCapacityUpgrade(cost);
	}
}
