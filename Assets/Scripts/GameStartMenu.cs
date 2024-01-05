using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class GameStartMenu : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button quitButton;
    // Start is called before the first frame update
    void Start()
    {
        //Hook events
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);

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

    public void StartGame()
    {
        // Récupérer l'index de la scène actuelle
        int indexSceneActuelle = SceneManager.GetActiveScene().buildIndex;

        // Charger la scène suivante
        SceneManager.LoadScene(1);
    }
}
