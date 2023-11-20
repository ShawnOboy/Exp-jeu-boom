using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // accès aux objets du XR Interaction Toolkit
using UnityEngine.InputSystem; // pour utiliser le nouveau InputSyteme
using UnityEngine.Events;
using System;

/*
 * Description générale
 * Script simple qui d'activer et de désactiver le rayon tracteur lorsqu'on appuie/relache le bouton grip du contrôleur
 * Mathieu Dionne
 * Derniére modifications : 12 septembre 2021
 */

public class GestionRayonTracteur : MonoBehaviour
{
    // [SerializeField] Permet de "sérialiser" une variable privée. Elle sera accessible dans l'inspecteur de Unity
    // l'objet qui possèdent le rayon tracteur (component XRRayInteractor, XRInteractorLineVisual, etc.)
    [SerializeField]
    GameObject ControleurTracteur;

    // L'action du contrôleur qui active/désactive le rayon. Peut être autre chose que le grip. Action à définir dans le tableau InputAction
    [SerializeField]
    InputActionReference inputActionReference_ActiveGrip; 



  
    private void OnEnable()
    {
        // s'exécute lorsque le script devient actif (enable)
        // ajout de la fonction qui sera appelée lorsque l'action sera effectuée
        inputActionReference_ActiveGrip.action.performed += ActiveRayon;
    }

    private void OnDisable()
    {
        // s'exécute lorsque le script devient inactif (disable)
        // retire la fonction qui sera appelée lorsque l'action sera effectuée
        inputActionReference_ActiveGrip.action.performed -= ActiveRayon;
    }

    private void Start()
    {

        //Par défaut, on désactive le rayon
        ControleurTracteur.GetComponent<XRRayInteractor>().enabled = false;
        ControleurTracteur.GetComponent<XRInteractorLineVisual>().enabled = false;
    }

    // Méthode qui s'exécute lorsque l'action sera effectuée (typiquement enfoncer le bouton grip).
	// Dans le tableau InputAction,l'action doit être configurée de type "button" et le binding avec un TriggerBehavior à PressAndRelease.
    // Lorsque la méthode sera appelée, le paramètre obj sera égal à 1f (bouton enfoncé) ou 0f (bouton relâché)
    private void ActiveRayon(InputAction.CallbackContext obj)
    {
       
        // Bouton enfoncé, on active le rayon
        // ReadValue<float> permet de récuperé la valeur de type float contenu dans le paramètre obj
        if (obj.ReadValue<float>() == 1f)
        {
            ControleurTracteur.GetComponent<XRRayInteractor>().enabled = true;
            ControleurTracteur.GetComponent<XRInteractorLineVisual>().enabled = true;
        }
        // Bouton relaché, on désactive le rayon
        else if (obj.ReadValue<float>() == 0f)
        {
            ControleurTracteur.GetComponent<XRRayInteractor>().enabled = false;
            ControleurTracteur.GetComponent<XRInteractorLineVisual>().enabled = false;
        }
    }
}
