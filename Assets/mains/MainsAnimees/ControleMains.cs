using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControleMains : MonoBehaviour
{
    [SerializeField] InputActionReference gripInputAction;
    [SerializeField] InputActionReference triggerInputAction;

    Animator handAnimator;

    void Awake()
    {
        handAnimator = GetComponent<Animator>(); 
    }


    private void OnEnable()
    {
        gripInputAction.action.performed += GripPressed;
        gripInputAction.action.canceled += GripRelease;

        triggerInputAction.action.performed += TriggerPressed;
        triggerInputAction.action.canceled += TriggerRelease;
    }

    private void OnDisable()
    {
        gripInputAction.action.performed -= GripPressed;
        triggerInputAction.action.performed -= TriggerPressed;
    }



    private void TriggerPressed(InputAction.CallbackContext obj) => handAnimator.SetFloat("Trigger", obj.ReadValue<float>());
    private void TriggerRelease(InputAction.CallbackContext obj) => handAnimator.SetFloat("Trigger", 0f);

    private void GripPressed(InputAction.CallbackContext obj)=> handAnimator.SetFloat("Grip", obj.ReadValue<float>());
    private void GripRelease(InputAction.CallbackContext obj) => handAnimator.SetFloat("Grip", 0f);


    
}
