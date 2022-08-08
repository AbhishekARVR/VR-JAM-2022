using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

	public scriptDashBoard dash;

	public int playerFunds = 0;

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

    public void updatePlayerFunds(int value)
	{
		playerFunds += value;

		//Update Dash UI
		dash.updateFundsAmount(playerFunds);
	}

	public void buySomething(int cost)
	{
		if (playerFunds >= cost)
		{
			updatePlayerFunds(-(cost));
		}
	}
}
