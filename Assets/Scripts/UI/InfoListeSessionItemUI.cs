using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System; // pour utiliser les Action

/* Script du prefab ItemListeSession. Ce prefab sera instancié une ou plusieurs fois, en fonction
 * du nombre de partie (session) active sur les serveurs de Photon Fusion
 * Variables :
 * Les trois premières variables doivent être définie dans l'insepcteur en glissant les gameObjects
 * du prefab ItemListeSession. Il s'agit des 3 objets enfants de l'objet ItemListeSession
 * - TextMeshProUGUI txt_NomSession : Référence au texte pour afficher le nom de la partie
 * - TextMeshProUGUI txt_NombreJoueurs : Référence au texte pour afficher le nombre du joueur dans
 * une partie
 * btn_Joindre : référence au bouton pour joindre la partie
 * SessionInfo sessionInfos : Classe propre a fusion pour mémoriser les informations de session
 * event Action<SessionInfo> OnRejoindreSession : Evenement OnRejoindreSsession
 */
public class InfoListeSessionItemUI : MonoBehaviour
{
    public TextMeshProUGUI txt_NomSession;
    public TextMeshProUGUI txt_NombreJoueurs;
    public Button btn_Joindre;

    SessionInfo sessionInfos; //classe fusion

    public event Action<SessionInfo> OnRejoindreSession;

    /*
     * Fonction qui sera appelée par le script GestionnaireListeSessions qui affichera les infos
     * d'une partie (session de jeu), soit le nom et le nombrede joueur.
     * 1. Mémorisation des informations sur la session reçues en paramètre
     * 2. Affichage du nom de la partie et du nombre de joueurs connectés dans les champs textes
     * 3. Vérification du nombre de joueurs. Si le nombre maximal est atteint (10 dans cet exemple)
     * le bouton ne sera pas visible. Sinon, le bouton sera visible et permettra de se joindre.
     */
    public void EnregistreInfos(SessionInfo sessionInfos)
    {
        //1.
        this.sessionInfos = sessionInfos;
        //2.
        txt_NomSession.text = $"{sessionInfos.Name}";
        txt_NombreJoueurs.text = $"{sessionInfos.PlayerCount.ToString()}/{sessionInfos.MaxPlayers.ToString()}";
        //3.
        bool boutonJoindreActif = true;

        if (sessionInfos.PlayerCount >= sessionInfos.MaxPlayers)
            boutonJoindreActif = false;

        btn_Joindre.gameObject.SetActive(boutonJoindreActif);
    }

    /*
     * Fonction publique déclenché lorsqu'on appuie sur le bouton pour rejoindre la session. À définir
     * dans l'inspecteur dans le gameObject BoutonJoindre.
     * Invoke la fonction qui aura été associé à l'événement OnRejoindreSession, plus précisément la
     * fonction NouveauInfoListeSessionItemUI_OnRejoindreSession du script GestionnaireListeSessions.
     */
    public void OnClick()
    {
        //Invoquer l'événement OnRejoindreSession
        OnRejoindreSession?.Invoke(sessionInfos);
    }

}
