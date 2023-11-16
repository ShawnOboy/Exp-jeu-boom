using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System;
using System.Threading.Tasks;

public class GestionnaireReseau : MonoBehaviour, INetworkRunnerCallbacks
{
    //Contient référence au component NetworkRunner
    NetworkRunner _runner;

    // pour mémoriser le component GestionnaireMouvementPersonnage du joueur
    GestionnaireInputs gestionnaireInputs;

    // Contient la référence au script JoueurReseau du Prefab
    public JoueurReseau joueurPrefab;

   // Tableau de couleurs à difinir dans l'inspecteur
    public Color[] couleurJoueurs;
   // Pour compter le nombre de joueurs connectés
    public int nbJoueurs = 0;

    GestionnaireListeSessions gestionnaireListeSessions;

    

    private void Awake()
    {
        // Si déjà créé, on ne veut pas recréer le networkrunner
        NetworkRunner RunnerDejaActif = FindFirstObjectByType<NetworkRunner>();
        gestionnaireListeSessions = FindFirstObjectByType<GestionnaireListeSessions>(FindObjectsInactive.Include); //true pour les objets inactifs

        if (RunnerDejaActif != null)
        {
            _runner = RunnerDejaActif;
            Destroy(gameObject);
        }
           
    }

    void Start()
    {
        // Création d'une partie dès le départ
        if(_runner == null)
        {
            /*  1.Ajout du component NetworkRunne au gameObject. On garde en mémoire
            la référence à ce component dans la variable _runner.
            2.Indique au NetworkRunner qu'il doit fournir les inputs au simulateur (Fusion)
        */
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;
            
        }
        if(SceneManager.GetActiveScene().name != "Accueil")
        {
           // CreationPartie(GameMode.AutoHostOrClient, "TestSession",1);
            
        }

    }

    /* Fonction asynchrone pour démarrer Fusion et créer une partie. Est appelé par le script
     * 
     * Paramètres reçus : 
     * - GameMode : Valeur possible : Client, Host, Server, AutoHostOrClient, etc.)
     * - nomSession : Nom de la partie (session) 
     * - indexScene : l'index de la scène de jeu
     * 1. Méthode du NetworkRunner qui permet d'initialiser une nouvelle partie (session)
     * 2. On utilise les paramètres pour définir les différente arguments (GameMode, SessionName, scene)
     * 3. Le nom du Lobby. Pourrait être passé en paramètre éventuellement
     * 4. On limite le nombre de joueur connecté à 10
     * 5.SceneManager : référence au component script NetworkSceneManagerDefault qui 
     * est ajouté au même moment
     */
    async void CreationPartie(GameMode mode, string nomSession, int indexScene)
    {
        //1.
        await _runner.StartGame(new StartGameArgs()
        {
            //2.
            GameMode = mode,
            SessionName = nomSession,
            //3.
            CustomLobbyName = "MonFPS_Lobby",
            Scene = indexScene,
            //4.
            PlayerCount = 10, //limite de 10 joueurs
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        }); ;
    }

   
    /* Lorsqu'un joueur se connecte au serveur
     * 1.On vérifie si ce joueur est aussi le serveur. Si c'est le cas, on spawn un prefab de joueur.
     * Bonne pratique : la commande Spawn() devrait être utilisé seulement par le serveur
    */
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        if(_runner.IsServer)
        {
            //Debug.Log($"Création du joueur {player.PlayerId} par le serveur");
            /*On garde la référence au nouveau joueur créé par le serveur. La variable locale
             créée est de type JoueurReseau (nom du script qui contient la fonction Spawned()*/
            JoueurReseau leNouveuJoueur =  _runner.Spawn(joueurPrefab, Utilitaires.GetPositionSpawnAleatoire(), Quaternion.identity, player);
            /*On change la variable maCouleur du nouveauJoueur et on augmente le nombre de joueurs connectés
            Comme j'ai seulement 10 couleurs de définies, je m'assure de ne pas dépasser la longueur de mon
            tableau*/

       
            leNouveuJoueur.maCouleur = couleurJoueurs[nbJoueurs];

            nbJoueurs++;
            if (nbJoueurs >= 10) nbJoueurs = 0;
        }
        else
        {
           // Debug.Log("Un joueur s'est connecté comme client. Spawn d'un joueur");
        }
    } 

    

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //Debug.Log($"Le joueur {player.PlayerId} a quitté la partie");
       
    }

    /*
     * Fonction du Runner pour définir les inputs du client dans la simulation
     * 1. On récupère le component GestionnaireInputs du joueur local
     * 2. On définit (set) le paramètre input en lui donnant la structure de données (struc) qu'on récupère
     * en appelant la fonction GestInputReseau du script GestionnaireInputs. Les valeurs seront mémorisées
     * et nous pourrons les utilisées pour le déplacement du joueur dans un autre script.Ouf...
     */
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //1.
        if(gestionnaireInputs == null && JoueurReseau.Local !=null)
        {
            
            gestionnaireInputs = JoueurReseau.Local.GetComponent<GestionnaireInputs>();
        }

        //2.
        if(gestionnaireInputs !=null)
        {
            input.Set(gestionnaireInputs.GetInputReseau());
        }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }
    /*
     * Fonction appelée lorsqu'une connexion réseau est refusée ou lorsqu'un client perd
     * la connexion suite à une erreur réseau. Le paramètre ShutdownReason est une énumération (enum)
     * contenant différentes causes possibles.
     */
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if(shutdownReason == ShutdownReason.GameIsFull)
        {
            //Debug.Log("Le maximum de joueur est atteint. Réessayer plus tard.");
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //Debug.Log("OnConnectedToServer");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
       // Debug.Log("OnOnDisconnectedFromServer");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //Debug.Log("Connection demandée par = " + runner.GetPlayerUserId());
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //Debug.Log("Connection refusée par = " + runner.GetPlayerUserId());
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
       
    }

    /* Fonction de PhotonFusion qui est automatiquement appelée lorsqu'on rejoint un lobby et lorsqu'une
     * nouvelle partie (session) est créée.
     * Paramètres :
     * - Runner : référence au client serveur
     * - List<SessionInfo> sessionList : une liste contenant la liste des parties (sessions)
     * 1. Par mesure préventive. On quitte la fonction si la référence au script gestionnaireListeSessions
     * n'a pas été défini.
     * 2.Si la liste est vide, on appelle la fonction AucuneSessionTrouvee() du script 
     * gestionnaireListeSessions;
     * 3.Si des parties (sessions) sont en cours, on efface les anciennes informations du tableau des
     * listes de session en appelant EffaceListe() du script gestionnaireListeSessions. Ensuite, le
     * foreach s'exécutera. Il permet de passer les éléments de la liste "sessionList" un par un. Pour
     * chaque itération, on appelle la fonction AjouteListe du script gestionnaireListeSessions en lui 
     * passant en paramètre l'élément de la liste qui contient les informations sur une partie (session).
     * Le script gestionnaireListeSessions créera alors un nouvel élément dans le PanelListeSessions qui
     * présente les informations de la partie (nom, nombre de joueurs connectés).
     */
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //1.
        if (gestionnaireListeSessions == null)
            return;
        //2.
        if(sessionList.Count == 0)
        {
            gestionnaireListeSessions.AucuneSessionTrouvee();
        }
        //3.
        else
        {
            gestionnaireListeSessions.EffaceListe();

            foreach(SessionInfo sessionInfo in sessionList)
            {
                gestionnaireListeSessions.AjouteListe(sessionInfo);
            }
        }
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
       
    }

    /*
     * Fonction publique qui est appelée par le script GestionnaireMenuAccueil lorsque l'utilisateur
     * clique sur le bouton "Trouver une partie".
     * Création d'une task. Un obet task permet des opérations asynchrone (qui prennent un certain temps)
     * comme les opérations réseau. Le thred principal (autre code) continue de s'exécuter.
     * Appel de la méthode asynchrone ConnexionAuLobby qui renverra une task récupérée par la variable
     * tacheConnexionLobby.
     */
    public void RejoindreLeLobby()
    {
        Task tacheConnexionLobby = ConnexionAuLobby();
    }
    /*
    * Fontion asynchrone qui appelle la fonction Fusion pour se joindre à un lobby
    * 1.Le nom du lobby à rejoindre. Il faut utiliser le même lorsqu'on crée le lobby!
    * 2.Appel de la fonction Fusion JoinSessionLobby en indiquant le nom de la session à joindre comme
    * deuxième paramètre. Le mot clé "await" permet de suspensder l'exécution du code tant que la
    * réponse n'est pas reçue. Le résultat sera enmagasiné dans la variable "resultat" qui est de type
    * StartGameResult (une classe propre a Fusion)
    * 3.Vérifiation de l'état de l'opération (réussie ou pas)
    * Notez que lorsque l'opération "JoinSessionLobby" est réussie, la fonction "OnSessionListUpdated"
    * sera automatiquement exécutée.
    * Notez que si le lobby n'existe pas, il sera créé.Si le lobby existe, le joueur s'y joindra
    */
    private async Task ConnexionAuLobby()
    {
        //1.
        string nomDuLobby = "MonFPS_Lobby";
        //2.
        StartGameResult resultat = await _runner.JoinSessionLobby(SessionLobby.Custom, nomDuLobby);
        //3.
        if(resultat.Ok)
        {
            //Debug.Log("Connexion au lobby effectuée avec succès");
        }
        else
        {
            //Debug.LogError($"Incapable de se connecter au lobby {nomDuLobby}");
        }
    }

    /*
     * Fonction appelé de l'extérieur par le script GestionnaireMenuAccueil lors que le joueur a appuyé
     * sur le bouton pour créer une nouvelle partie (session).
     * Paramètres :
     * string nomSession : le nom de la partie (session) qui sera créée
     * string nomScene : le nom de la scène dans lequel on devra être dirigé une fois la partie créé.
     * Dans ce projet, il s'agit de la scène "Jeu".
     * 1.Récupérartion du build index de la scène "Jeu". Notez l'utilisation de la commande 
     * SceneUtility.GetBuildIndexByScenePath()
     * 2.Appel de la fonction CreationPartie pour créer une nouvelle partie (session). On précise en 
     * paramètres que nous serons l'hôte de la partie et non un client. On indique également le nom
     * de la partie (session) et l'index de la scène qui doit être chargée.
     */
    public void InfosCreationPartie(string nomSession, string nomScene)
    {
        //1.
        int indexScene = SceneUtility.GetBuildIndexByScenePath($"Scenes/{nomScene}");
        //2.
        CreationPartie(GameMode.Host, nomSession, indexScene);
    }

    /* Fonction appelée de l'extérieur par le script GestionnaireListeSession et lorsque le joueur
     * appuie sur le bouton pour rejoindre une partie.
     * Paramètre :
     * - SessionInfo sessionInfo : Les informations sur la partie à rejoindre
     * 1.Appel de la fonction CreationPartie. Dans ce cas, on va plutôt rejoindre une partie
     * existante. On précise donc en paramètres que nous serons Client (et non serveur), le nom de la
     * partie (sesion) à rejoindre. Le dernière paramètre représente l'index de la scène. Sa valeur n'a
     * pas d'importance et sera géré par Fusion pour nous diriger à la bonne scène.
     */
    public void RejoindrePartie(SessionInfo sessionInfo)
    {
        CreationPartie(GameMode.Client, sessionInfo.Name, 0);
    }

    
}
