using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Classe comprenant des fonctions statiques générales utilisées
 par plusieurs scripts
*/

public static class Utilitaires
{
    // Fonction statique qui retourne un vector3 aléatoire
    public static Vector3 GetPositionSpawnAleatoire()
    {
        return new Vector3(Random.Range(-20, 20), 4, Random.Range(-20, 20));
    }

    public static void SetRenderLayerInChildren(Transform transform, int numLayer)
    {
        foreach(Transform trans in transform.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = numLayer;
        }
    }
}

