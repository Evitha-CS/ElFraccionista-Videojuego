using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Reflection;
using TMPro;
using System.Text.RegularExpressions;
using GameUtils;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomInputField;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public Text roomName;


    public RoomItem roomItemPrefab;
    public Sprite full;
    List<RoomItem> roomList = new List<RoomItem>();
    public Transform contentObject;
    public float updateTime = 1.5f;
    float nextUpdateTime;

    public List<PlayerName> playerNamesList = new List<PlayerName>();
    public PlayerName playerNamePrefab;
    public Transform playerNameParent;

    public GameObject playButton;
    public TextMeshProUGUI errorText;
    private IsinRoom isinRoom = new IsinRoom();


    public void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    //Para crear la sala virtual
    public void OnClickCreate()
    {
        if (IsRoomNameValid(roomInputField.text))
        {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 2 });
        }
    }

    private bool IsRoomNameValid(string roomName)
    {
        // Verificar longitud mínima
        if (roomName.Length < 4)
        {
            DisplayError("El nombre de la sala debe tener al menos 4 caracteres.", Color.red);
            return false;
        }

        // Verificar caracteres especiales utilizando expresiones regulares
        if (!Regex.IsMatch(roomName, "^[a-zA-Z0-9_]*$"))
        {
            DisplayError("El nombre de la sala no debe contener caracteres especiales.", Color.red);
            return false;
        }

        return true;
    }

    private void DisplayError(string message, Color color)
    {
        // Mostrar el mensaje de error en el TextMeshPro con el color especificado
        if (errorText != null)
        {
            errorText.text = message;
            errorText.color = color;
        }
        else
        {
            Debug.LogError("ErrorText no está asignado en el Inspector.");
        }
    }

    //Sala de espera
    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Sala : " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
        //RoomUtilities.SaveRoomData(isinRoom,"usuario-isinRoom"); //Se mandan los datos recolectados de la sala a Nodejs
    }

    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {


        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + updateTime;
        }


    }

    private void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomList)
        {
            Destroy(item.gameObject);
        }
        roomList.Clear();

        foreach (RoomInfo room in list)
        {
            //removed from rooms list
            if (room.RemovedFromList)
            {
                int index = roomList.FindIndex(x => x.RoomInfo.Name == room.Name);
                if (index != -1)
                {
                    Destroy(roomList[index].gameObject);
                    roomList.RemoveAt(index);
                }
            }
            //Added to rooms list
            else
            {
                RoomItem listing = Instantiate(roomItemPrefab, contentObject);

                if (listing != null)
                {

                    listing.SetRoomName(room.Name, room.PlayerCount, room.MaxPlayers);
                    //listing.SetRoomName(room.Name);
                    roomList.Add(listing);


                    //Cambia el boton si la sala esta llena o no
                    if (room.PlayerCount == room.MaxPlayers)
                    {
                        Image joinButtonImage = listing.join_btn.image;
                        if (joinButtonImage != null)
                        {
                            joinButtonImage.sprite = full;
                        }

                    }

                }




            }
        }

    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        //RoomUtilities.SaveRoomData(isinRoom,"usuario-isNotInRoom"); //Se mandan los datos recolectados de la sala a Nodejs
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Servidor conectado.");

        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //Si ocurre un error al conectarse con el servidor
        Debug.Log("Desconectando del servidor." + cause.ToString());
    }

    void UpdatePlayerList()
    {
        foreach (PlayerName item in playerNamesList)
        {
            Destroy(item.gameObject);
        }
        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Photon.Realtime.Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerName newPlayerName = Instantiate(playerNamePrefab, playerNameParent);
            newPlayerName.SetPlayerInfo(player.Value);
            playerNamesList.Add(newPlayerName);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayerList();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }

    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel("GameRoom");

    }

   
}
