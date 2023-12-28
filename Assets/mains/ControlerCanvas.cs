using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

public class ControlerCanvas : MonoBehaviour
{
    [SerializeField]
    GameObject LeControleur;


    [SerializeField]
    InputActionReference inputActionReference_UISwitcher;

    bool estActif = false;

    [SerializeField]
    GameObject LeCanvas;

  
    private void OnEnable()
    {
        inputActionReference_UISwitcher.action.performed += ActivateUIMode;
        print("enable");
    }
    private void OnDisable()
    {
        inputActionReference_UISwitcher.action.performed -= ActivateUIMode;
        print("disable");
    }

    private void Start()
    {
        //Deactivating UI Canvas Gameobject by default
        if (LeCanvas !=null)
        {
            LeCanvas.SetActive(false);
        }

    }

    /// <summary>
    /// This method is called when the player presses UI Switcher Button which is the input action defined in Default Input Actions.
    /// When it is called, UI interaction mode is switched on and off according to the previous state of the UI Canvas.
    /// </summary>
    /// <param name="obj"></param>
    private void ActivateUIMode(InputAction.CallbackContext obj)
    {
        if (!estActif)
        {
            estActif = true;

            //Activating the UI Canvas Gameobject
            LeCanvas.SetActive(true);
        }
        else
        {
            estActif = false;

            //De-Activating the UI Canvas Gameobject
            LeCanvas.SetActive(false);
        }

    }
}
