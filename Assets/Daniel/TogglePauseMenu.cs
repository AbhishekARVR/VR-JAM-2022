using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TogglePauseMenu : MonoBehaviour
{
    public static TogglePauseMenu Instance;

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


    public void ActivateMenu()
    {
        // toggle menu
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);
    }
}
