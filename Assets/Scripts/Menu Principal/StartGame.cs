using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using UnityEngine.Networking;
using UnityEditor.PackageManager;

public class StartGame : NetworkBehaviour {
  public int playerID;
  void Start()
  {
    Button button = GetComponent<Button>();
    button.onClick.AddListener(OnClick);
  }

  void OnClick()
  {
    Debug.Log("Player " + playerID + " clicked the button!");
  }
  public void StartGameButton() {

    if(Object.HasStateAuthority) {
      Debug.Log("AYOOOOOOOOOOOOOOOOO");
    }
  }


}
// {
//     public int playerID;


// }
