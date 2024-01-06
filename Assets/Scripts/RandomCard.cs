using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.XR.Interaction.Toolkit;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomCard : MonoBehaviour {
  CreateTarotCard [] tarotCards;

  public CreateTarotCard randomCard;
  private bool activatedEffect = false;

  void Start() {
    LoadTarotCards();
    
    randomCard = GetRandomCard();

    gameObject.tag = "Active Card";
    randomCard.cardEffectManager = GameObject.FindGameObjectWithTag("Effect Manager").GetComponent<CardEffectManager>();

    MeshRenderer mRenderer = GetComponent<MeshRenderer>();
    mRenderer.material = randomCard.cardArtwork;

    Transform cardParent = transform.parent.transform.parent;
    XRGrabInteractable grabInteractable = cardParent.GetComponent<XRGrabInteractable>();
    grabInteractable.selectEntered.AddListener(CardGrab);
  }
  void CardGrab(SelectEnterEventArgs interactor) {
    randomCard.pickedUpByPlayer = true;
  }

  void Update() {
    if(randomCard.pickedUpByPlayer && !activatedEffect) {
      StartCoroutine(CardEffect());
    }
  }


  void LoadTarotCards() {
#if UNITY_EDITOR
  string folderPath = "Assets/Tarot Cards/Cards";
  string [] guids = AssetDatabase.FindAssets("t:CreateTarotCard", new[] { folderPath });

  tarotCards = new CreateTarotCard[guids.Length];

  for (int i = 0; i < guids.Length; i++) {
    string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
    tarotCards[i] = AssetDatabase.LoadAssetAtPath<CreateTarotCard>(assetPath);
  }
#endif
  }

  CreateTarotCard GetRandomCard() {
    int randomIndex = Random.Range(0, tarotCards.Length);
    return tarotCards[randomIndex];
  }

  IEnumerator CardEffect() {

    activatedEffect = true;
    randomCard.pickedUpByPlayer = false;
    GameObject cardParent = transform.parent.transform.parent.gameObject;

    int randomSound = Mathf.FloorToInt(Random.Range(0f, randomCard.cardEffectManager.cardFlip.Length));
    cardParent.GetComponent<AudioSource>().PlayOneShot(randomCard.cardEffectManager.cardFlip[randomSound], 2f);

    if(randomCard.CardName == "Fool") randomCard.cardEffectManager.Fool();

    yield return new WaitForSeconds(1);

    if(randomCard.CardName == "Moon") randomCard.cardEffectManager.Moon();
    else if(randomCard.CardName == "Sun") randomCard.cardEffectManager.Sun();
    else if(randomCard.CardName == "World") randomCard.cardEffectManager.World();
    else if(randomCard.CardName == "Star") randomCard.cardEffectManager.Star();
    else if(randomCard.CardName == "HighPriestess") randomCard.cardEffectManager.HighPriestess();
    else if(randomCard.CardName == "Death") randomCard.cardEffectManager.Death();
    else if(randomCard.CardName == "WheelOfFortune") randomCard.cardEffectManager.WheelOfFortune();

    gameMenuManager.score += randomCard.cardPointValue;
    
    yield return new WaitForSeconds(1.5f);

    cardParent.GetComponent<AudioSource>().PlayOneShot(randomCard.cardEffectManager.burningCard);

    yield return new WaitForSeconds(0.5f);

    GameObject visualElement = gameObject.transform.parent.gameObject;
    GameObject frontElement = gameObject;
    GameObject backElement = gameObject.transform.parent.GetChild(1).gameObject;
    
    StartCoroutine(randomCard.cardEffectManager.DissolveCardEffect(visualElement, "cardVisual"));
    StartCoroutine(randomCard.cardEffectManager.DissolveCardEffect(frontElement, randomCard.CardName));
    StartCoroutine(randomCard.cardEffectManager.DissolveCardEffect(backElement, "cardBack"));

    yield return new WaitForSeconds(2);

    randomCard.cardEffectManager = GameObject.FindGameObjectWithTag("Effect Manager").GetComponent<CardEffectManager>();
    GameObject spawnLocation = GameObject.FindGameObjectWithTag("Card Spawn");

    GameObject newCard = Instantiate(randomCard.cardEffectManager.cardPrefab, spawnLocation.transform.position, spawnLocation.transform.rotation);

    Destroy(cardParent);

  }
}
