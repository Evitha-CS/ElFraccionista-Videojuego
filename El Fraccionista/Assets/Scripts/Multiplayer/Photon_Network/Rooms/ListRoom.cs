using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListRoom : MonoBehaviour
{
    //Script antiguo, borrar luego

    [SerializeField]
    private Text _text;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        _text.text = roomInfo.MaxPlayers + ", " + roomInfo.Name;
    }
}
