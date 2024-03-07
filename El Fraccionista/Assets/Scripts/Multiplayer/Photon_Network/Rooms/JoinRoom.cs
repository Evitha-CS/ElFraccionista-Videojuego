using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class JoinRoom : MonoBehaviourPunCallbacks
{
    //Script antiguo, borrar luego

    public int number;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void JoiningaRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Se unió a una sala de forma aleatoria");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No se pudo unir a ninguna sala, se creará una nueva sala");
        CreateRooms();
    }
    public void CreateRooms()
    {
        number = Random.Range(1, 100);
        Debug.Log("Se va a CREAR una nueva sala");
        PhotonNetwork.JoinOrCreateRoom("Sala n°." + number, new RoomOptions() { MaxPlayers = 2 }, TypedLobby.Default);
        Debug.Log("Se ha CREADO la sala n°." + number);

        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("No se pudo crear la sala. Volviendo a intentar...");
        CreateRooms();
    }
}
