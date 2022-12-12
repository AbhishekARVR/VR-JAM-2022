using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

	public scriptDashBoard dash;
	[SerializeField]
	private AudioSource srcDash;

	public int playerFunds = 0;
	public int trashCount = 0;

	public int trashCapacity;
	public int maxTrashCapacity;
	public int trashCapacityIncrement;

	public float speedMultiplier = 1f;
	public float maxSpeedMultiplier;
	public float speedMultiplierIncrement;

	public float fuelLevel = 100f;
	public float maxFuel = 100f;

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
		{
			dash = dashObj.GetComponent<scriptDashBoard>();
			srcDash = dashObj.GetComponentInChildren<AudioSource>();

			if (srcDash == null)
				Debug.LogError("No dash audio source found.", this);
		}
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

	public bool updateTrash(int amount)
	{
		int newTrashCount = trashCount + amount;
		
		if (newTrashCount <= trashCapacity && newTrashCount >= 0) //only update trash if we have room for it.
		{
			if (newTrashCount > trashCount)
			{
				//play trash added audio
			}
			else if (newTrashCount < trashCount)
			{
				//play trash removed audio
			}
			
			//update trash amount
			trashCount = newTrashCount;

			//update ui
			dash.updateTrashAmount(trashCount);

			return true;
		}

		//Otherwise the trash has been rejected
		//play trash rejected audio

		return false;
	}

    public void updatePlayerFunds(int value)
	{
		playerFunds += value;

		//play coin drop audio if we are gaining funds.
		if (value > 0)
			srcDash.PlayOneShot(AudioManager.Instance.GetRandCoinSound());

		//Update Dash UI
		dash.updateFundsAmount(playerFunds);
	}

	/// <summary>
	/// Set the fuel level to the passed in value.
	/// </summary>
	/// <param name="value"></param>
    public void updateFuel(float value)
	{
		fuelLevel = value;
		
		//Update Dash UI
		dash.updateFuelAmount(fuelLevel);
	}

	public void updateTrashCapacity(int increaseAmount)
	{
		trashCapacity += increaseAmount;

		//Update Dash UI
		dash.updateTrashAmount(trashCount);
	}

	public void updateSpeed(float increaseAmount)
	{
		speedMultiplier += increaseAmount;

	}

	public void useFuel(float value)
	{
		fuelLevel -= value;

		//Update Dash UI
		dash.updateFuelAmount(fuelLevel);
	}

	/// <summary>
	/// Top off the tank and take the players money.
	/// </summary>
	/// <param name="cost"></param>
	public void buyFuel(int cost)
	{
		if (playerFunds >= cost && fuelLevel < maxFuel)
		{
			updatePlayerFunds(-(cost));
			updateFuel(100);
		}
	}
	
	public void buyTrashCapacityUpgrade(int cost)
	{
		if (playerFunds >= cost && trashCapacity < maxTrashCapacity)
		{
			updatePlayerFunds(-(cost));
			updateTrashCapacity(trashCapacityIncrement);
		}
	}

	public void buySpeedUpgrade(int cost)
	{
		if (playerFunds >= cost && speedMultiplier < maxSpeedMultiplier)
		{
			updatePlayerFunds(-(cost));
			updateSpeed(speedMultiplierIncrement);
		}
	}
}
