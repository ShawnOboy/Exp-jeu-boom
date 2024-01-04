using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class CardOrientationManager : MonoBehaviour {

  GameObject activeCard;
  public GameObject cardSpawn;
  public GameObject player;
  public float defaultOrientation;

  void Update() {

    activeCard = GameObject.FindGameObjectWithTag("Active Card");
    Transform activeCardParent = activeCard.transform.parent.transform.parent;

    if(player.transform.GetWorldPose().position.x > cardSpawn.transform.GetWorldPose().position.x) {
      cardSpawn.transform.localEulerAngles = new Vector3(
        cardSpawn.transform.rotation.x,
        defaultOrientation + 180f,
        -90f
      );
      if(!activeCard.GetComponent<RandomCard>().randomCard.pickedUpByPlayer) {
        activeCardParent.transform.localEulerAngles = new Vector3(
          activeCardParent.transform.rotation.x,
          defaultOrientation + 180f,
          -90f
        );
      }
    }

    if(player.transform.GetWorldPose().position.x < cardSpawn.transform.GetWorldPose().position.x) {
      cardSpawn.transform.localEulerAngles = new Vector3(
        cardSpawn.transform.rotation.x,
        defaultOrientation,
        -90f
      );

      if(!activeCard.GetComponent<RandomCard>().randomCard.pickedUpByPlayer) {
        activeCardParent.transform.localEulerAngles = new Vector3(
          activeCardParent.transform.rotation.x,
          defaultOrientation,
          -90f
        );
      }
    }
  }
}
