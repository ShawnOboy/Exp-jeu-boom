using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour {

  [Header ("Card Prefab")]
  public GameObject cardPrefab;

  [Header ("Moon / Sun")]
  public Light dirLight;
  public Material [] skyBoxMaterial;
  [SerializeField] int skyBoxIndex = 2;

  private void Start() {
    SetDefaultEffect();
  }

  private void Update() {

    // Recherche continue de la carte active dans la hiérarchie
    GameObject activeCard = GameObject.FindGameObjectWithTag("Active Card");

    if(Input.GetKeyDown(KeyCode.Space)) {
      activeCard.GetComponent<RandomCard>().randomCard.pickedUpByPlayer = true;
    }

    // Gestion Effets Moon / Sun

    Material skyMaterial = skyBoxMaterial[skyBoxIndex];
    RenderSettings.skybox = skyMaterial;

    if(skyBoxIndex == 0) {
      if(skyMaterial.GetFloat("_Exposure") > 0) {
        skyMaterial.SetFloat("_Exposure", skyMaterial.GetFloat("_Exposure") - 0.0025f);
      }
      else {
        // Fonction Mort du joueur
      }
    }
    else if(skyBoxIndex == 4) {
      if(dirLight.intensity < 20) {
        dirLight.intensity += 0.025f;
      }
      if(skyMaterial.GetFloat("_Exposure") < 2) {
        skyMaterial.SetFloat("_Exposure", skyMaterial.GetFloat("_Exposure") + 0.0025f);
      }
      else {
        // Fonction Mort du joueur
      }
    }

  }

  public void Moon() {
    skyBoxIndex -= 1;
  }

  public void Sun() {
    skyBoxIndex += 1;
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

  public void Death() {
    Debug.Log("Effet Death Activé");
  }

  public void WheelOfFortune() {
    Debug.Log("Effet Wheel of Fortune Activé");
  }

  // Fonction de reset des valeur
  void SetDefaultEffect() {
    // Gestion Effets Moon / Sun

    dirLight.intensity = 1;

    for (int i = 0; i < skyBoxMaterial.Length; i++) {
      Material skyMaterial = skyBoxMaterial[i];
      RenderSettings.skybox = skyMaterial;

      if(skyMaterial.GetFloat("_Exposure") != 1f) {
        skyMaterial.SetFloat("_Exposure", 1f);
      }
    }
  }
}
