using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;

public class gameMenuManager : MonoBehaviour
{
    // Variable statique pour stocker le score
    public static int score = 0;

    public Transform head;
    public float spawnDistance = 2;
    public GameObject menu;
    public InputActionProperty showButton;

    public Button quitButton;

    // Référence au texte sur le Canvas pour afficher le score
    public TMP_Text texteScore;

    public GameObject manetteGRay;
    public GameObject manetteDRay;

    // Fonction pour augmenter le score
    public void AugmenterScore(int points)
    {
        score += points;
        // Mettre à jour le texte du score sur le Canvas
        if (texteScore != null)
        {
            texteScore.text = "Score : " + score.ToString();
        }
    }
    void Start()
    {
        //Hook events
        quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (showButton.action.WasPerformedThisFrame())
        {

            Debug.Log("bouton appuyé");
            menu.SetActive(!menu.activeSelf);
            
            menu.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z).normalized * spawnDistance;

            //Ray actif pour utiliser le menu
            manetteGRay.SetActive(!manetteGRay.activeSelf);
            manetteDRay.SetActive(!manetteDRay.activeSelf);
        }
        menu.transform.LookAt(new Vector3(head.position.x, menu.transform.position.y, head.position.z));
        menu.transform.forward *= -1;
    }
    public void QuitGame()
    {
        // Vérifier si l'application est en mode VR
        if (XRSettings.enabled)
        {
            // Désactiver le mode VR avant de quitter
            XRSettings.enabled = false;
        }

        // Quitter l'application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    
}
}
