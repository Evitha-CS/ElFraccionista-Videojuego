using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomPlayers;
    public Button join_btn;
    public Sprite full;
    LobbyManager manager;
    public RoomInfo RoomInfo { get; private set; }


    public void Start()
    {
        manager = FindAnyObjectByType<LobbyManager>();

        join_btn.onClick.AddListener(() =>
        {
            manager.JoinRoom(roomName.text);
        });

    }
    /*
    public void Update()
    {
        if (this.RoomInfo.PlayerCount == RoomInfo.MaxPlayers)
        {
            join_btn.image.sprite = full;
        }
    }
    */
    //NO SE ESTA USANDO 
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        
        RoomInfo = roomInfo;
        roomName.text = roomInfo.Name + "Jugadores "+ roomInfo.MaxPlayers;
        //roomInfo.Name + " " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;

    }
    
    public void SetRoomName(string _roomName, int _roomPlayers, int _roomMaxplayers)
    {
     
        roomName.text = _roomName;
        roomPlayers.text = "Jugadores " + _roomPlayers +"/"+_roomMaxplayers;
        //roomName.text = RoomInfo.Name + " " + RoomInfo.PlayerCount + "/" + RoomInfo.MaxPlayers;
    }

    /*
    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
    */
}
