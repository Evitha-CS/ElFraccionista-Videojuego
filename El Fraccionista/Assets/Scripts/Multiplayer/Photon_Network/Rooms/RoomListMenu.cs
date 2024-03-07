using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListMenu : MonoBehaviourPunCallbacks
{
    //Script antiguo, borrar luego

    [SerializeField]
    private ListRoom roomListing;
    [SerializeField]
    private Transform content;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            ListRoom listing = Instantiate(roomListing, content);
            if(listing != null)
            {
                listing.SetRoomInfo(roomInfo);
            }
        }
    }
}
