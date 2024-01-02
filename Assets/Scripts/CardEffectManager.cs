using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour {

  public GameObject cardPrefab;

  private void Update() {
    GameObject activeCard = GameObject.FindGameObjectWithTag("Active Card");

    if(Input.GetKeyDown(KeyCode.Space)) {
      activeCard.GetComponent<RandomCard>().randomCard.pickedUpByPlayer = true;
    }
  }

  public void Moon() {
    Debug.Log("Effet Moon Activé");
  }

  public void Sun() {
    Debug.Log("Effet Sun Activé");
  }

  public void Fool() {
    Debug.Log("Effet Fool Activé");
  }

  public void World() {
    Debug.Log("Effet World Activé");
  }

  public void Star() {
    Debug.Log("Effet Star Activé");
  }

  public void HighPriestess() {
    Debug.Log("Effet High Priestess Activé");
  }

  public void HangedMan() {
    Debug.Log("Effet Hanged Man Activé");
  }

  public void WheelOfFortune() {
    Debug.Log("Effet Wheel of Fortune Activé");
  }

}
