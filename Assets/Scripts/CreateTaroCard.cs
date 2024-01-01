#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreateTaroCard", menuName = "Taro Card", order = 0)]
public class CreateTaroCard : ScriptableObject {
  [SerializeField] string cardName;

  public string CardName {
    get { return string.IsNullOrEmpty(cardName) ? base.name : cardName; }
    set { cardName = value; }
  }

// Création d'un bouton dans l'éditeur pour mettre le nom de la carte sur "cardName"
#if UNITY_EDITOR
  [CustomEditor(typeof(CreateTaroCard))]
  public class CreateTaroCardEditor : Editor {
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      CreateTaroCard refScript = (CreateTaroCard)target; // Prend référence au Script "CreateTaroCard" via l'élément de l'inspecteur visé (target)

      if(GUILayout.Button("Set Card Name")) { // Cré un bouton qui appel le code qu'il contient
        SetCardName(refScript);
      }
    }

    private void SetCardName(CreateTaroCard refScript) {
      refScript.CardName = refScript.name;
      EditorUtility.SetDirty(refScript); // Dit à l'asset (ScriptableObject -> CreateTaroCard) d'être sauvegardé
    }
  }
#endif
// Fin du bouton

  public Material cardArtwork;
  public EffectTypeEnum effectType;

  [Min(0)]
  public int cardPointValue;

  public enum EffectTypeEnum {
    buff,
    debuff
  }

  public bool pickedUpByPlayer = false;
}
