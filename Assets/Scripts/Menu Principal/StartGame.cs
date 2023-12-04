using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using UnityEngine.Networking;
using UnityEditor.PackageManager;
using UnityEngine.SceneManagement;

public class StartGame : NetworkBehaviour {
  // void Start() {
    
  // }

  public void StartGameButton() {

    if(Object.HasStateAuthority) {
      Debug.Log("AYOOOOOOOOOOOOOOOOO");
      SceneManager.LoadScene("Game");
    }
    else {
      Debug.Log("HAHAHAHAHAHAHAHAHAHA");
    }
  }


}
