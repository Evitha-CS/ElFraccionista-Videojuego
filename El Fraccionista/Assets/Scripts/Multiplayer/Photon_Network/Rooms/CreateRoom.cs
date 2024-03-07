using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CreateRoom : MonoBehaviourPunCallbacks
{

    //Script antiguo, borrar luego

    public int number;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CreateRooms()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        number = Random.Range(1, 100);
        Debug.Log("Se va a CREAR una nueva sala");
        PhotonNetwork.JoinOrCreateRoom("Sala n°." + number, new RoomOptions() { MaxPlayers= 2}, TypedLobby.Default);
        Debug.Log("Se ha CREADO la sala n°." + number);

       // PhotonNetwork.LoadLevel("Game");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("No se pudo crear la sala. Volviendo a intentar...");
        CreateRooms();
    }
}
