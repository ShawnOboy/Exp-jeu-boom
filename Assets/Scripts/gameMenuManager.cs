using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class gameMenuManager : MonoBehaviour
{
    // Variable statique pour stocker le score
    public static int score = 0;

    public Transform head;
    public float spawnDistance = 2;
    public GameObject menu;
    public InputActionProperty showButton;

    public Button quitButton;
    public Button endButton;

    // R�f�rence au texte sur le Canvas pour afficher le score
    public TMP_Text texteScore;

    public GameObject manetteGRay;
    public GameObject manetteDRay;

    // Fonction pour augmenter le score
    public void AugmenterScore(int points)
    {
        score += points;
        // Mettre � jour le texte du score sur le Canvas
        if (texteScore != null)
        {
            texteScore.text = "Score : " + score.ToString();
        }
    }
    void Start()
    {
        //Hook events
        endButton.onClick.AddListener(End);
        quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (showButton.action.WasPerformedThisFrame())
        {

            Debug.Log("bouton appuy�");
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
        Application.Quit();
    }
    
    //Pour finir le jeu et garder ton score
    public void End()
    {
        // R�cup�rer l'index de la sc�ne actuelle
        int indexSceneActuelle = SceneManager.GetActiveScene().buildIndex;

        // Charger la sc�ne suivante
        SceneManager.LoadScene(indexSceneActuelle + 1);
    }
}
