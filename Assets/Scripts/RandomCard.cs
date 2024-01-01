using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomCard : MonoBehaviour {
  CreateTaroCard [] taroCards;
  void Start() {
    LoadTaroCards();
    // taroCards = Resources.LoadAll<CreateTaroCard>("Resources");
    
    GetRandomCard();
    CreateTaroCard randomCard = GetRandomCard();

    Renderer renderer = GetComponent<Renderer>();
    renderer.material = randomCard.cardArtwork;
  }

  void LoadTaroCards() {
#if UNITY_EDITOR
  string folderPath = "Assets/Taro Cards/Cards";
  string [] guids = AssetDatabase.FindAssets("t:CreateTaroCard", new[] { folderPath });

  taroCards = new CreateTaroCard[guids.Length];

  for (int i = 0; i < guids.Length; i++) {
    string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
    taroCards[i] = AssetDatabase.LoadAssetAtPath<CreateTaroCard>(assetPath);
  }
#endif
  }

  CreateTaroCard GetRandomCard() {
    int randomIndex = Random.Range(0, taroCards.Length);
    return taroCards[randomIndex];
  }
}
