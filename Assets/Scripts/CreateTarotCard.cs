#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreateTarotCard", menuName = "Tarot Card", order = 0)]
public class CreateTarotCard : ScriptableObject {
  [SerializeField] string cardName;

  public string CardName {
    get { return string.IsNullOrEmpty(cardName) ? base.name : cardName; }
    set { cardName = value; }
  }

// Création d'un bouton dans l'éditeur pour mettre le nom de la carte sur "cardName"
#if UNITY_EDITOR
  [CustomEditor(typeof(CreateTarotCard))]
  public class CreateTarotCardEditor : Editor {
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      CreateTarotCard refScript = (CreateTarotCard)target; // Prend référence au Script "CreateTarotCard" via l'élément de l'inspecteur visé (target)

      if(GUILayout.Button("Set Card Name")) { // Cré un bouton qui appel le code qu'il contient
        SetCardName(refScript);
      }
    }

    private void SetCardName(CreateTarotCard refScript) {
      refScript.CardName = refScript.name;
      EditorUtility.SetDirty(refScript); // Dit à l'asset (ScriptableObject -> CreateTarotCard) d'être sauvegardé
    }
  }
#endif
// Fin du bouton

  public Material cardArtwork;

  [Min(0)]
  public int cardPointValue;

  public bool pickedUpByPlayer = false;

  [HideInInspector] public CardEffectManager cardEffectManager;
}
