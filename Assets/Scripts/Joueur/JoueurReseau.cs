using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion; // namespace pour utiliser les classes de Fusion
using TMPro;
/* 
 * 1.Les objets réseau ne doivent pas dériver de MonoBehavior, mais bien de NetworkBehavior
 * Importation de l'interface IPlayerLeft
 * 2.Variable pour mémoriser l'instance du joueur
 * 3.Fonction Spawned() : Semblable au Start(), mais pour les objets réseaux
 * Sera exécuté lorsque le personnage sera créé (spawn)
 * Test si le personnage créé est le personnage contrôlé par l'utilisateur local.
 * HasInputAuthority permet de vérifier cela.
 * Retourne true si on est sur le client qui a généré la création du joueur
 * Retourne false pour les autres clients
 * 4. Lorsqu'un joueur se déconnecte du réseau, on élimine (Despawn) son joueur.
 */
public class JoueurReseau : NetworkBehaviour, IPlayerLeft //1.
{
    /* Remplacer votre ancienne variable nomDujoueur par ceci.
     * Ajout d'une variable synchronisée (Networked) de type NetworkString avec un maximum de
     * 16 caractères. Lorsque la variable est modifiée, appel de la fonction ChangementDeNom_static()
     */
    [Networked(OnChanged = nameof(ChangementDeNom_static))]
    public NetworkString<_16> nomDujoueur { get; set; }

    public TextMeshProUGUI nomDuJoueurTxt;

    //Variable qui sera automatiquement synchronisée par le serveur sur tous les clients
    [Networked] public Color maCouleur { get; set; }

    bool messageEnvoieDuNom;
    MessagesJeuReseau messagesJeuReseau;

    public static JoueurReseau Local;  //.2

    //Ajout d'une variable public Transform. Dans Unity, glisser l'objet "visuel" du prefab du joueur
    public Transform modeleJoueur;

    public GestionnaireCameraLocale gestionnaireCameraLocale;
    public GameObject LocalUI;

    

    private void Awake()
    {
        messagesJeuReseau = GetComponent<MessagesJeuReseau>();
    }

    /*
     * Au départ, on change la couleur du joueur. La variable maCouleur sera définie
     * par le serveur dans le script GestionnaireReseau. La fonction Start() sera
     * appelée après la fonction Spawned().
     */
    private void Start()
    {
        GetComponentInChildren<MeshRenderer>().material.color = maCouleur;
    }

    

    public override void Spawned() //3.
    {
        if(Object.HasInputAuthority)
        {
            Local = this;

            //Si c'est le joueur du client, on appel la fonction pour le rendre invisible
            Utilitaires.SetRenderLayerInChildren(modeleJoueur, LayerMask.NameToLayer("JoueurLocal"));

            //On désactive la mainCamera. Assurez-vous que la caméra de départ possède bien le tag MainCamera
            Camera.main.gameObject.SetActive(false);
            //Apple d'un RPC (fonction remote procedure call) en passant le nom du joueur enregistré
            RPC_ChangementdeNom(PlayerPrefs.GetString("NomDuJoueur"));
           
           
        }
        else
        {
            //Si le joueur créé est contrôlé par un autre joueur, on désactive le component caméra de cet objet
            Camera camLocale = GetComponentInChildren<Camera>();
            camLocale.enabled = false;

            // On désactive aussi le component AudioListener
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            LocalUI.SetActive(false);

            Debug.Log("Un joueur réseau a été créé");
        }

        //Définir le joueur pour le joueur local
        Runner.SetPlayerObject(Object.InputAuthority, Object);

        transform.name = $"Joueur_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player) //.4
    {
        /* À partir du paramètre "player" reçu en paramètre, on récupére le NetworkObject qui lui est
         * associé. On peut ainsi aller chercher la variable "nomDujoueur" dans son script JoueurReseau.
         * Une fois cela fait, on appelle la fonction SupprimeJoueur() du script 
         * GestionnairePointage en lui passant le nom du joueur qui quitte.
         */
        
        if (Runner.TryGetPlayerObject(player, out NetworkObject leJoueurQuiPart))
        {
            string nomJoueurQuiPart = leJoueurQuiPart.GetBehaviour<JoueurReseau>().nomDujoueur.ToString();
            FindFirstObjectByType<GestionnairePointage>().SupprimeJoueur(nomJoueurQuiPart);
        }

        if (Object.HasStateAuthority)
        {
            if(Runner.TryGetPlayerObject(player, out NetworkObject leJoueurQuiQuitte))
            {
                if (leJoueurQuiQuitte == Object)
                    Local.GetComponent<MessagesJeuReseau>().EnvoieMessageJeuRPC(leJoueurQuiQuitte.GetComponent<JoueurReseau>().nomDujoueur.ToString(), "a quitté la partie");
            }
        }
            //messagesJeuReseau.EnvoieMessageJeuRPC(nomDujoueur.ToString(), "a quitté la partie");

        if(player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    /* Fonction static qui sera appelée automatiquement lors la variable nomDuJoueur sera changé. 
     * Sera appelé sur tous les clients puisque la variable nomDuJoueur est synchronisée (Networked)
     * 1.Appel de la fonction instanciée ChangementDeNom()
     */
    static void ChangementDeNom_static(Changed<JoueurReseau> changed)
    {
        //1.
        changed.Behaviour.ChangementDeNom();
    }

    /* Fonction changement de nom qui appelle la fonction EnregistrementNom() du script
     * GestionnairePointage en passant le nom du joueur en paramètre.
     * 
     */
    private void ChangementDeNom()
    {
        nomDuJoueurTxt.text = nomDujoueur.ToString();
        GetComponent<GestionnairePointage>().EnregistrementNom(nomDujoueur.ToString());
    }

    /* Déclenchement d'un remote procedure call (RPC). Ceci permet à un joueur (clien) de déclencher
     * une fonction sur un autre client. Entre paranthèses, on peut spécifier la source et la cible. Si
     * on ne le fait pas, il s'agit alors d'un RPC static qui sera envoyé à tous les clients.
     * Ici, le message est envoyé par le joueur qui possède le InputAuthority, c'est-à-dire le joueur
     * local qui vient d'entrer son nom. Le message est destiné uniquement au serveur (StateAuthority).
     * Concrètement, ce script sera exécuté seulement sur le serveur, dans le script du joueur qui
     * vient de se joindre.
     * - Paramètres :
     * - string leNom : le nom du joueur reçu en paramètre
     * - RpcInfo infos = propre à Fusion. Contientra différentes informations pouvant être utilisées,
     * telles : le tick précis de l'envoie, la source (playerRef) qui  a envoyé le message, etc.)
     * 1. On défini le nom du joueur. Seulement sur le serveur, mais cette variable est Networked et
     * sera synchronisée.
     */
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ChangementdeNom(string leNom, RpcInfo infos = default)
    {
        print("RPC_ChangementdeNom");
        this.nomDujoueur = leNom;
        if(!messageEnvoieDuNom)
        {
            
            messagesJeuReseau.EnvoieMessageJeuRPC(leNom, "a rejoint la partie");
            messageEnvoieDuNom = true;
        }
    }
}
