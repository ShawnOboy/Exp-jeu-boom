using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardEffectManager : MonoBehaviour {
  public static bool isDead = false;

  GameObject activeCard;
  public Material shaderDissolve;
  private Material shaderDissolveVisual;
  private Material shaderDissolveFront;
  private Material shaderDissolveBack;
  public Material transparent;
  public Texture2D [] textureDissolve;
  private bool dissolving = false;
  public AudioClip burningCard;

  [Header ("Player Ref")]
  public GameObject player;

  [Header ("Card Prefab")]
  public GameObject cardPrefab;

  [Header ("Moon / Sun")]
  public Light dirLight;
  public Material [] skyBoxMaterial;
  [SerializeField] int skyBoxIndex = 2;
  public GameObject flash;
  public AudioClip fastWhoosh;

  [Header ("World")]
  public GameObject icebergMap;
  public GameObject [] bigIceberg;

  [Header ("Fool")]
  public Material [] fakeArtwork;
  public Material shaderFool;
  private Material foolShader;
  private bool foolEffect;

  [Header ("Star")]
  public GameObject fireworks;
  public AudioClip fireworkWhistle;
  public AudioClip fireworkBurst;


  private void Start() {
    SetDefaultEffect();
    foolShader = new(shaderFool);
    shaderDissolveVisual = new(shaderDissolve);
    shaderDissolveFront = new(shaderDissolve);
    shaderDissolveBack = new(shaderDissolve);
    fireworks.GetComponent<ParticleSystem>().Pause();
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
        StartCoroutine(nameof(PlayerDeath));
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
        StartCoroutine(nameof(PlayerDeath));
      }
    }

    // Gestion Effets World

    if(icebergMap.transform.position.y < -1.25f) {
      StartCoroutine(nameof(PlayerDeath));
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

    // Gestion Dissolve

    if(dissolving) {
      if(shaderDissolveVisual.GetFloat("_Cutoff_Height") > -2) {
        shaderDissolveVisual.SetFloat("_Cutoff_Height", shaderDissolveVisual.GetFloat("_Cutoff_Height") - 0.1f);
      }
      if(shaderDissolveFront.GetFloat("_Cutoff_Height") > -2) {
        shaderDissolveFront.SetFloat("_Cutoff_Height", shaderDissolveFront.GetFloat("_Cutoff_Height") - 0.1f);
      }
      if(shaderDissolveBack.GetFloat("_Cutoff_Height") > -2) {
        shaderDissolveBack.SetFloat("_Cutoff_Height", shaderDissolveBack.GetFloat("_Cutoff_Height") - 0.1f);
      }
    }

    if(isDead) {
      
    }
  }

  public void Moon() {
    skyBoxIndex -= 1;
    Material moonMat = new(flash.GetComponent<MeshRenderer>().material) {
      color = new Color(0f, 0f, 0f, 1f)
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
    GameObject cardParent = activeCard.transform.parent.transform.parent.gameObject;
    cardParent.GetComponent<AudioSource>().PlayOneShot(fastWhoosh);
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

    yield return new WaitForSeconds(1.5f);

    GameObject cardParent = activeCard.transform.parent.transform.parent.gameObject;
    cardParent.GetComponent<AudioSource>().PlayOneShot(burningCard);

    yield return new WaitForSeconds(0.5f);

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
    fireworks.GetComponent<ParticleSystem>().Play();
    StartCoroutine(FireworkSound(4));
  }

  private IEnumerator FireworkSound(int repeatAmount) {
    for (int i = 0; i < repeatAmount; i++) {
      yield return new WaitForSeconds(1f);
      fireworks.GetComponent<AudioSource>().PlayOneShot(fireworkWhistle);
      Invoke(nameof(FireworkBurst), 2.25f);
    }
  }

  private void FireworkBurst() {
    fireworks.GetComponent<AudioSource>().PlayOneShot(fireworkBurst);
  }

  public void HighPriestess() {
    Material highPriestessMat = new(flash.GetComponent<MeshRenderer>().material) {
      color = new Color(1f, 1f, 1f, 1f)
    };
    ActivateFlash(highPriestessMat);
    SetDefaultEffect();

    player.transform.position = new Vector3(
      player.transform.position.x,
      player.transform.position.y + 0.5f,
      player.transform.position.z
    );
  }

  public void Death() {
    StartCoroutine(nameof(PlayerDeath));
  }

  public void WheelOfFortune() {
    Debug.Log("Effet Wheel of Fortune Activé");
  }

  private IEnumerator PlayerDeath() {
    isDead = true;

    yield return new WaitForSeconds(2f);

    int indexSceneActuelle = SceneManager.GetActiveScene().buildIndex;
    SceneManager.LoadScene(indexSceneActuelle - 1);
}

  // Fonction de reset des valeur
  public void SetDefaultEffect() {
    isDead = false;

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

  public IEnumerator DissolveCardEffect(GameObject dissolvedElement, string cardPart) {

    if(cardPart == "cardVisual") {
      List<Material> materialsList = new() {
        transparent,
        shaderDissolveVisual
      };

      shaderDissolveVisual.SetTexture("_Texture2D", textureDissolve[0]);

      dissolvedElement.GetComponent<MeshRenderer>().SetMaterials(materialsList);
    }

    else if(cardPart == "cardBack") {
      List<Material> materialsList = new() {
        transparent,
        shaderDissolveBack
      };
      
      shaderDissolveBack.SetTexture("_Texture2D", textureDissolve[1]);

      dissolvedElement.GetComponent<MeshRenderer>().SetMaterials(materialsList);
    }
    else {
      List<Material> materialsList = new() {
        transparent,
        shaderDissolveFront
      };
      
      if(cardPart == "Fool") shaderDissolveFront.SetTexture("_Texture2D", textureDissolve[2]);
      else if(cardPart == "Moon") shaderDissolveFront.SetTexture("_Texture2D", textureDissolve[3]);
      else if(cardPart == "Sun") shaderDissolveFront.SetTexture("_Texture2D", textureDissolve[4]);
      else if(cardPart == "World") shaderDissolveFront.SetTexture("_Texture2D", textureDissolve[5]);
      else if(cardPart == "Star") shaderDissolveFront.SetTexture("_Texture2D", textureDissolve[6]);
      else if(cardPart == "HighPriestess") shaderDissolveFront.SetTexture("_Texture2D", textureDissolve[7]);
      else if(cardPart == "Death") shaderDissolveFront.SetTexture("_Texture2D", textureDissolve[8]);
      else if(cardPart == "WheelOfFortune") shaderDissolveFront.SetTexture("_Texture2D", textureDissolve[9]);

      dissolvedElement.GetComponent<MeshRenderer>().SetMaterials(materialsList);
    }

    dissolving = true;
    shaderDissolveVisual.SetFloat("_Cutoff_Height", 5.3f);
    shaderDissolveFront.SetFloat("_Cutoff_Height", 5.3f);
    shaderDissolveBack.SetFloat("_Cutoff_Height", 5.3f);

    yield return new WaitForSeconds(1f);

    dissolving = false;

  }
}
