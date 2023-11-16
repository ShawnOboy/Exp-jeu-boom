using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
/* Script sur l'objet PanelListeSessions dans le prefab Canvas. Ce script s'occuppe essentiellement de
 * gérer l'affichage des informations obtenues par le GestionnaireReseau sur les parties (sessions) en
 * cours sur le serveur de Photon Fusion.
 * Variables:
 * - TextMeshProUGUI txtEtat : zone texte pour l'affichage de l'état de l'opération en cours
 * - GameObject ListeItemSessionPrefab : Référence au prefab qui sera instancié. Le nombre de prefab
 * instancié sera égal au nombre de parties (sessions) en cours sur le serveur.
 * - VerticalLayoutGroup verticalLayoutGroup : référence au component verticalLayoutGroup du gameObject
 * LayoutGroupeVertical. Permet une gestion dynamique de l'affichage vertical qui s'ajuste automatiquement
 * en fonction du nombre d'élément (ListeItemSession) qui sera ajouté.
 */
public class GestionnaireListeSessions : MonoBehaviour
{
    public TextMeshProUGUI txtEtat;
    public GameObject ListeItemSessionPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    /*
     * Appel de la fonction EffaceListe() qui détruira tous les objet ItemListeSession présente dans
     * le groupe vertical (verticalLayoutGroup)
     */
    private void Awake()
    {
        EffaceListe();
    }

    /*
     * Fonction de nettoyage avant un nouveal affichage des parties (sessions) disponibles. 
     * 1. Le foreach permet de passer les enfants du groupe vertical un par un et de les supprimer. 
     * 2. On désactive le texte affichant l'état des opérations.
     */
    public void EffaceListe()
    {
        //1.
        foreach(Transform elementListe in verticalLayoutGroup.transform)
        {
            Destroy(elementListe.gameObject);
        }
        //2.
        txtEtat.gameObject.SetActive(false);
    }

    /*
    * Fonction appelée de l'extérieur, par le script GestionnaireReseau qui permet d'ajouter une
    * nouvelle entrée de partie dans le panneau PanelListeSessions.
    * Parmamètre SessionInfo sessionInfo : contient les différentes informations sur la session
    * 1. Instanciation d'un nouveau objet ListeItemSession à partir du prefab. Le deuxième argument
    * (verticalLayoutGroup.transform) permet de placer le nouveal objet comme enfant du groupe vertical.
    * L'objet créé est mémorisé dans la variable locale nouvelItemListe.
    * 2. On mémorise dans la variable locale nouveauInfoListeSessionItemUI la référence 
    * au component-script InfoListeSessionItemUI
    * 3.Appel de la fonction EnregistreInfos du script InfoListeSessionItemUI en passant en paramètre
    * les informations de la session. Cette fonction se chargera de l'affichage des informations.
    * 4.On ajoute une fonction à l'action OnRejoindreSession du script InfoListeSessionItemUI.C'est
    * cette fonction (NouveauInfoListeSessionItemUI_OnRejoindreSession) qui sera appelée lorsque le bouton
    * "joindre" sera cliqué... Ouf!
    */
    public void AjouteListe(SessionInfo sessionInfo)
    {
        //1. 
        GameObject nouvelItemListe = Instantiate(ListeItemSessionPrefab, verticalLayoutGroup.transform);
        //2.
        InfoListeSessionItemUI nouveauInfoListeSessionItemUI = nouvelItemListe.GetComponent<InfoListeSessionItemUI>();
        //3.
        nouveauInfoListeSessionItemUI.EnregistreInfos(sessionInfo);
        //4.
        nouveauInfoListeSessionItemUI.OnRejoindreSession += NouveauInfoListeSessionItemUI_OnRejoindreSession;
    }

    /*
    * Fonction appelé par lorsque l'utilisateur clique sur le bouton joindre d'une partie (session).
    * Paramètre SessionInfo sessionInfo : les informations de la session à rejoindre
    * 1.Récupération du GestionnaireReseau (script) avec la commande FindFirstObjectByType
    * 2.Apple de la fonction RejoindrePartie() dans le script GestionnaireReseau. On passe en paramètre
    * les informations de la session à rejoindre.
    * 3.Récupération du GestionnaireMenuAccueil (script) et appel de sa fonction RejoindreServeur().
    * Le rôle de cette fonction est simplement de cacher les panneaux et de montrer le message d'état
    * de l'opération.
    */
    private void NouveauInfoListeSessionItemUI_OnRejoindreSession(SessionInfo sessionInfo)
    {
        //1.
        GestionnaireReseau gestionnaireReseau = FindFirstObjectByType<GestionnaireReseau>();
        //2.
        gestionnaireReseau.RejoindrePartie(sessionInfo);
        //3.
        GestionnaireMenuAccueil gestionnaireMenuAccueil = FindFirstObjectByType<GestionnaireMenuAccueil>();
        gestionnaireMenuAccueil.RejoindreServeur();
    }

    /* Fonction appelée de l'extérieur, par le script GestionnaireReseau lorsqu'aucune partie (session)
     * n'est trouvé. On vide l'affichage des sessions dans le tableau et on affiche un message d'état
     * pour indiquer qu'aucune session de jeu n'a été trouvée.
     */
    public void AucuneSessionTrouvee()
    {
        EffaceListe();
        txtEtat.text = "Aucune session de jeu trouvée";
        txtEtat.gameObject.SetActive(true);
    }
    /* Fonction appelée de l'extérieur, par le script GestionnaireReseau lorsque la commande pour
     * rechercher des sessions a été envoyée et qu'on attend une réponse.
     * On vide l'affichage des sessions dans le tableau et on affiche un message d'état
     * pour indiquer qu'une recherche est en cours.
     */
    public void ChercheDesSessions()
    {
        EffaceListe();
        txtEtat.text = "Recherche de partie en cours...";
        txtEtat.gameObject.SetActive(true);
    }
}
