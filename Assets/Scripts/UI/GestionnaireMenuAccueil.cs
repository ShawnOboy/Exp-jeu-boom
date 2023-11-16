using UnityEngine;
using TMPro;
/* Script de l'accueil qui permet de gérer l'enregistrement de l'utilisateur et déclencher la recherche
 * de sessions de jeu sur les serveurs de Photon.
 * Variables :
 * - Variables pour mémoriser les différents panneaux du canvas :
 * panelNomDuJoueur : pour saisir le nom du joueur
 * panelListeSessions : pour l'affichage des parties actuellment en cours
 * panelCreationSession : pour créer sa propre partie (session de jeu)
 * panelEtat : pour afficher des informations sur l'état des requêtes
 * 
 * inputField_NomDuJoueur : référence au inputField qui permet au joueur d'inscrire son nom
 * inputField_NomSession : référence au inputField qui permet au joueur d'inscrire le nom de la
 * partie qu'il souhaite créer (invisible au départ)
 * 
 */
public class GestionnaireMenuAccueil : MonoBehaviour
{
    [Header("Pannels")]
    public GameObject panelNomDuJoueur;
    public GameObject panelListeSessions;
    public GameObject panelCreationSession;
    public GameObject panelEtat;

    [Header("InfosJoueurs")]
    public TMP_InputField inputField_NomDuJoueur;

    [Header("NouvelleSession")]
    public TMP_InputField inputField_NomSession;


    /*
     * Vérification d'un nom déjà enregistré localement. Si c'est le cas, on affiche
     * ce nom dans la zone de texte.
     */
    void Start()
    {
        if (PlayerPrefs.HasKey("NomDuJoueur"))
            inputField_NomDuJoueur.text = PlayerPrefs.GetString("NomDuJoueur");
    }

    /*
     * Fonction qui permet de masquer tous les panneaux
     */
    void CacheTousLesPanels()
    {
        panelNomDuJoueur.SetActive(false);
        panelListeSessions.SetActive(false);
        panelCreationSession.SetActive(false);
        panelEtat.SetActive(false);
    }

    /*
     * Fonction déclenchée lorsqu'on appuie sur le bouton "Trouver une partie". À définir dans
     * l'inspecteur, dans le "OnClick()" du bouton.
     * 1.Enregistrement local du nom du joueur
     * 2.On récupère le component-script GestionnaireReseau et on appelle la fonction RejoindreLobby().
     * Cette fonction devra donc être ajouté au script GestionnaireReseau.
     * 3.On cache tous les panneaux et on active le panneau qui afficher la liste des parties
     * 4.On cherche le component-script GestionnaireListeSessions et on appelle sa fonction
     * ChercheDesSessions(). Notez bien la syntaxe particulière pour inclure les objets désactivés.
     */
    public void BtnTrouveParties()
    {
        //1.
        PlayerPrefs.SetString("NomDuJoueur", inputField_NomDuJoueur.text);
        PlayerPrefs.Save();
        //2.
        GestionnaireReseau gestionnaireReseau = FindFirstObjectByType<GestionnaireReseau>();
        gestionnaireReseau.RejoindreLeLobby();
        //3.
        CacheTousLesPanels();
        panelListeSessions.SetActive(true);
        //4.
        FindFirstObjectByType<GestionnaireListeSessions>(FindObjectsInactive.Include).ChercheDesSessions();
    }

    /*
     * Fonction déclenchée lorsqu'on appuie sur le bouton "Créer une nouvelle partie" dans le panneau 
     * "PanelListeSessions". À définir dans l'inspecteur, dans le "OnClick()" du bouton.
     * 1.On cache tous les panneaux et on active seulement le panneau de creation de session.
     */
    public void BtnNouvellePartie()
    {
        CacheTousLesPanels();
        panelCreationSession.SetActive(true);
    }

    /*
     * Fonction déclenchée lorsqu'on appuie sur le bouton "Créer une partie" dans le panneau 
     * "PanelCreationSession". À définir dans l'inspecteur, dans le "OnClick()" du bouton.
     * 1.On récupère le component-script GestionnaireReseau et on appelle la fonction 
     * CreationPartie en passant en paramètre le nom du joueur et le nom de la scène qui 
     * devra être lancée.
     * 2.On cache tous les panneaux et on active seulement le panneau affichant les messages d'état.
     */
    public void BtnCreationNouvelleSession()
    {
        //1.
        GestionnaireReseau gestionnaireReseau = FindFirstObjectByType<GestionnaireReseau>();
        gestionnaireReseau.InfosCreationPartie(inputField_NomSession.text, "Jeu");
        //2.
        CacheTousLesPanels();
        panelEtat.SetActive(true);
    }

    /*
     * Fonction déclenchée qui sera appelée de l'extérieur, par le script GestionnaireListeSessions.
     * Cache tous les panneaux pour afficher seulement le panneau d'état
     */
    public void RejoindreServeur()
    {
        CacheTousLesPanels();
        panelEtat.SetActive(true);
    }
}
