using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDeathControl : MonoBehaviour {

  GameObject activeCard;

  private void Update() {
    activeCard = GameObject.FindGameObjectWithTag("Active Card");
  }

  private void OnCollisionEnter(Collision collisionObject) {

    Debug.Log(collisionObject.gameObject.name);

    if(collisionObject.gameObject.name == "Ocean") {
      activeCard.GetComponent<RandomCard>().randomCard.cardEffectManager.Death();
    }
  }

}
