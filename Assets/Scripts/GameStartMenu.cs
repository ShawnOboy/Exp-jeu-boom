using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
        Application.Quit();
    }

    public void StartGame()
    {
        // R�cup�rer l'index de la sc�ne actuelle
        int indexSceneActuelle = SceneManager.GetActiveScene().buildIndex;

        // Charger la sc�ne suivante
        SceneManager.LoadScene(indexSceneActuelle + 1);
    }
}
