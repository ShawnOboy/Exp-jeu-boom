using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MessagesJeuReseau : NetworkBehaviour
{
   GestionnaireMessagesJeu gestionnaireMessagesJeu;

    
    void Start()
    {
        
    }

    public void EnvoieMessageJeuRPC(string nomDuJoueur, string leMessage)
    {
        //éxécuté par serveur uniquement
        print("fonction pour envoie de message RPC activée");
        RPC_MessagesJeu(nomDuJoueur,leMessage);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_MessagesJeu(string nomDuJoueur,string leMessage, RpcInfo infos = default)
    {
        //Envoyé par le serveur, reçu par tout le monde
        Debug.Log($"RPC MessageJeu <b>{nomDuJoueur}</b> {leMessage}");

        if (gestionnaireMessagesJeu == null)
            gestionnaireMessagesJeu = JoueurReseau.Local.gestionnaireCameraLocale.GetComponentInChildren<GestionnaireMessagesJeu>();

        if(gestionnaireMessagesJeu != null)
        {
            gestionnaireMessagesJeu.ReceptionMessage($"<b>{nomDuJoueur}</b> {leMessage}");
        }

        if(leMessage == "a quitté la partie")
        {
            //FindFirstObjectByType<GestionnaireAffichagePointage>().SupprimeJoueur(nomDuJoueur);
        }


    }

    
}
