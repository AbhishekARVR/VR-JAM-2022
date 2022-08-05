using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TogglePauseMenu : MonoBehaviour
{
    public static TogglePauseMenu Instance;

    public InputActionReference activateMenuRef = null;


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
        // adding the script to button
        activateMenuRef.action.started += ActivateMenu;
    }

    //removing script to button
    private void OnDestroy()
    {
        activateMenuRef.action.started -= ActivateMenu;
    }

    private void ActivateMenu(InputAction.CallbackContext context)
    {
        // toggle menu
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);
    }
}
