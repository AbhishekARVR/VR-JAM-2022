using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

	public scriptDashBoard dash;

	public int playerFunds = 0;
	public int trashCount = 0;

    private void Awake()
    {
        //singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
		//validation
		var dashObj = GameObject.FindGameObjectWithTag("Dash");

		if (dashObj == null)
			Debug.LogError("No dash object found in scene.", this);
		else
			dash = dashObj.GetComponent<scriptDashBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

	public void removeTrash(int amountToRemove)
	{
		dash.updateTrashAmount(trashCount - amountToRemove);
	}

    public void updatePlayerFunds(int value)
	{
		playerFunds += value;

		//Update Dash UI
		dash.updateFundsAmount(playerFunds);
	}

    public void updateFuel(int value)
	{
		//Update Dash UI
		dash.updateFuelAmount(value);
	}

	public void buyFuel(int cost)
	{
		if (playerFunds >= cost)
		{
			updatePlayerFunds(-(cost));
			updateFuel(100);
		}
	}
}
