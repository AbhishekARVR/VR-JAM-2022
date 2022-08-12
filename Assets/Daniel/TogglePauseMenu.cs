using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TogglePauseMenu : MonoBehaviour
{
    public static TogglePauseMenu Instance;

    [SerializeField] Transform pausePos;
    [SerializeField] GameObject PauseCanvas;

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

    private void Update()
    {
        PauseCanvas.transform.position = pausePos.transform.position;
		PauseCanvas.transform.rotation = pausePos.transform.rotation;
	}


    public void ActivateMenu()
    {
        // toggle menu
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
