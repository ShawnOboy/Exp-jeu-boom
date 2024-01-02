using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomCard : MonoBehaviour {
  CreateTarotCard [] tarotCards;

  public CreateTarotCard randomCard;
  private bool activatedEffect = false;

  void Start() {
    LoadTarotCards();
    
    // GetRandomCard();
    randomCard = GetRandomCard();

    gameObject.tag = "Active Card";
    randomCard.cardEffectManager = GameObject.FindGameObjectWithTag("Effect Manager").GetComponent<CardEffectManager>();

    SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
    sRenderer.sprite = randomCard.cardArtwork;
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

    if(randomCard.CardName == "Fool") randomCard.cardEffectManager.Fool();
    else if(randomCard.CardName == "Moon") randomCard.cardEffectManager.Moon();
    else if(randomCard.CardName == "Sun") randomCard.cardEffectManager.Sun();
    else if(randomCard.CardName == "World") randomCard.cardEffectManager.World();
    else if(randomCard.CardName == "Star") randomCard.cardEffectManager.Star();
    else if(randomCard.CardName == "HighPriestess") randomCard.cardEffectManager.HighPriestess();
    else if(randomCard.CardName == "Death") randomCard.cardEffectManager.Death();
    else if(randomCard.CardName == "WheelOfFortune") randomCard.cardEffectManager.WheelOfFortune();
    
    yield return new WaitForSeconds(3);

    Transform cardParent = transform.parent.transform.parent;
    randomCard.cardEffectManager = GameObject.FindGameObjectWithTag("Effect Manager").GetComponent<CardEffectManager>();
    GameObject spawnLocation = GameObject.FindGameObjectWithTag("Card Spawn");

    Instantiate(randomCard.cardEffectManager.cardPrefab, spawnLocation.transform.position, spawnLocation.transform.rotation);
    Destroy(cardParent.gameObject);

  }
}
