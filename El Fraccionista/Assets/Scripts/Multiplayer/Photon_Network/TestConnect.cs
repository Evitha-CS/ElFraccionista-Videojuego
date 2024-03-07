using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestConnect : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    private void Start()
    {
        print("Conectando al servidor...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("Servidor conectado");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //Si ocurre un error al conectarse con el servidor
        print("Desconectando del servidor." + cause.ToString());
    }


}
