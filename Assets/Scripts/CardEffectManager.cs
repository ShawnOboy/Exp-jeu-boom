using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class CardEffectManager : MonoBehaviour {

  GameObject activeCard;

  [Header ("Player Ref")]
  public GameObject player;

  [Header ("Card Prefab")]
  public GameObject cardPrefab;

  [Header ("Moon / Sun")]
  public Light dirLight;
  public Material [] skyBoxMaterial;
  [SerializeField] int skyBoxIndex = 2;
  public GameObject flash;

  [Header ("World")]
  public GameObject icebergMap;
  public GameObject [] bigIceberg;

  [Header ("Fool")]
  public Material [] fakeArtwork;
  public Material shaderFool;
  private Material foolShader;
  
  private bool foolEffect;


  private void Start() {
    SetDefaultEffect();
    foolShader = new(shaderFool);
  }

  private void Update() {

    // Recherche continue de la carte active dans la hiérarchie
    activeCard = GameObject.FindGameObjectWithTag("Active Card");

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

    // Gestion Effets World

    if(icebergMap.transform.position.y < -1.25f) {
      // Fonction Mort du joueur
    }

    // Gestion Fool

    if(foolEffect) {
      if(foolShader.GetFloat("_Cutoff_Height") < 2) {
        foolShader.SetFloat("_Cutoff_Height", foolShader.GetFloat("_Cutoff_Height") + 0.015f);
      }
    }
    else {
      if(foolShader.GetFloat("_Cutoff_Height") > 0.4f) {
        foolShader.SetFloat("_Cutoff_Height", foolShader.GetFloat("_Cutoff_Height") - 0.01f);
      }
    }

    if(Input.GetKeyDown(KeyCode.V)) {
      Moon();
    }
    if(Input.GetKeyDown(KeyCode.B)) {
      Sun();
    }

  }

  public void Moon() {
    skyBoxIndex -= 1;
    Material moonMat = new(flash.GetComponent<MeshRenderer>().material) {
      color = new Color(0f, 0f, 0f, 0f)
    };
    ActivateFlash(moonMat);
  }

  public void Sun() {
    skyBoxIndex += 1;
    Material sunMat = new(flash.GetComponent<MeshRenderer>().material) {
      color = new Color(1f, 1f, 1f, 1f)
    };
    ActivateFlash(sunMat);
  }

  private void ActivateFlash(Material color) {
    flash.GetComponent<MeshRenderer>().material = color;
    flash.GetComponent<Animator>().enabled = true;
    Invoke(nameof(DisableFlash), 1f);
  }

  private void DisableFlash() {
    flash.GetComponent<Animator>().enabled = false;
  }

  public void Fool() {
    StartCoroutine(CardIsFake());
  }

  IEnumerator CardIsFake() {
    int randomIndex = (int)Mathf.Floor(Random.Range(0, 7));

    List<Material> materialsList = new() {
      fakeArtwork[randomIndex],
      foolShader
    };

    activeCard.GetComponent<MeshRenderer>().SetMaterials(materialsList);

    yield return new WaitForSeconds(1f);

    foolEffect = true;
    foolShader.SetFloat("_Cutoff_Height", 0.9f);
    foolShader.SetFloat("_Noise_Scale", 5f);
    foolShader.SetFloat("_Gloom", 1.25f);

    yield return new WaitForSeconds(0.75f);

    foolEffect = false;
    foolShader.SetFloat("_Noise_Scale", 25f);

    materialsList[0] = activeCard.GetComponent<RandomCard>().randomCard.cardArtwork;
    activeCard.GetComponent<MeshRenderer>().SetMaterials(materialsList);
  }

  public void World() {
    StartCoroutine(SinkIceberg());
    StartCoroutine(ShrinkIceberg());
  }

  IEnumerator SinkIceberg() {
    float randomSinkingAmount = Random.Range(0.25f, 0.5f);
    float tempSinkingAmount = 0f;

    while (randomSinkingAmount > tempSinkingAmount) {
      tempSinkingAmount += 0.001f;
      icebergMap.transform.position = new Vector3(
        icebergMap.transform.position.x,
        icebergMap.transform.position.y - 0.001f,
        icebergMap.transform.position.z
      );
      yield return new WaitForSeconds(0.001f);
    }
  }

  IEnumerator ShrinkIceberg() {
    float randomShrinkingAmount = Random.Range(0.1f, 0.3f);
    float tempShrinkingAmount = 0f;

    while (randomShrinkingAmount > tempShrinkingAmount) {
      tempShrinkingAmount += 0.001f;
      foreach (GameObject iceberg in bigIceberg) {
        iceberg.transform.localScale = new Vector3(
          iceberg.transform.localScale.x - 0.001f,
          iceberg.transform.localScale.y - 0.001f,
          iceberg.transform.localScale.z - 0.001f
        );
      }
      yield return new WaitForSeconds(0.001f);
    }
  }

  public void Star() {
    Debug.Log("Effet Star Activé");
  }

  public void HighPriestess() {
    SetDefaultEffect();

    player.transform.position = new Vector3(
      player.transform.position.x,
      player.transform.position.y + 0.5f,
      player.transform.position.z
    );
  }

  public void Death() {
    // Fonction Mort du joueur
  }

  public void WheelOfFortune() {
    Debug.Log("Effet Wheel of Fortune Activé");
  }

  // Fonction de reset des valeur
  public void SetDefaultEffect() {
    // Gestion Effets Moon / Sun

    dirLight.intensity = 1;

    for (int i = 0; i < skyBoxMaterial.Length; i++) {
      Material skyMaterial = skyBoxMaterial[i];
      RenderSettings.skybox = skyMaterial;

      if(skyMaterial.GetFloat("_Exposure") != 1f) {
        skyMaterial.SetFloat("_Exposure", 1f);
      }
    }

    // Gestion Effets World

    icebergMap.transform.position = Vector3.zero ;
    foreach (GameObject iceberg in bigIceberg) {
      iceberg.transform.localScale = Vector3.one;
    }
  }
}
