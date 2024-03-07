using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Linq;
using GameUtils;


public class VictoryManager : MonoBehaviourPunCallbacks
{

    public GameObject victoryPanel;
    public GameObject gamePanel;
    [SerializeField] private List<Block> _blocksPhoton;
    public TextMeshPro winner;
    public TextMeshPro totalTime;
    public TextMeshPro player1totalFails;
    public TextMeshPro player2totalFails;
    public TextMeshPro player1totalMoves;
    public TextMeshPro player2totalMoves;
    private bool captureErrors = false;
    private ControlStartGame controlStartGame;
    private Stack stack;
    private GameData gameData = new GameData();
    private IsPlaying isPlaying = new IsPlaying();
    private IsinRoom isinRoom = new IsinRoom();


    // Start is called before the first frame update
    void Start()
    {
        controlStartGame = FindObjectOfType<ControlStartGame>();
        stack = FindObjectOfType<Stack>();
        InitializeFailText(player1totalFails, PhotonNetwork.MasterClient.NickName);
        InitializeFailText(player2totalFails, PlayerUtilities.GetOtherPlayerName());
        InitializeMovesText(player1totalMoves, PhotonNetwork.MasterClient.NickName);
        InitializeMovesText(player2totalMoves, PlayerUtilities.GetOtherPlayerName());
    }

    private void InitializeFailText(TextMeshPro textField, string playerName)
    {
        textField.text = $"Total de fallos cometidos por {playerName}: 0";
    }
    private void InitializeMovesText(TextMeshPro textField, string playerName)
    {
        textField.text = $"Total de movimientos hechos por {playerName}: 0";
    }

    /*
    private int GetOtherPlayerID()
    {
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Player player in players)
        {
            if (!player.IsMasterClient)
            {
                return (int)player.CustomProperties["userId"];
            }
        }
        return -1; // Manejar el caso en el que no se encuentre otro jugador
    }*/

    // Para desactivar los elementos del juego
    public void DisableInGameElements()
    {
        photonView.RPC("RPC_DisableInGameElements", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_DisableInGameElements()
    {
        gamePanel.SetActive(false);
        victoryPanel.SetActive(true);

    }

    public void WinnerAndSummry(string winnerName)
    {
        string time = controlStartGame.totalTime;
        //int player1Fails = stack.player1failMovesCount;
        //int player2Fails = stack.player2failMovesCount;

        photonView.RPC("RPC_WinnerAndSummry", RpcTarget.AllBuffered, winnerName, time);
    }


    [PunRPC]
    private void RPC_WinnerAndSummry(string winnerName, string time)
    {
        winner.text = $"El ganador es: {winnerName}";
        totalTime.text = $"La partida duró: {time} minutos";

        if (PhotonNetwork.IsMasterClient)
        {
            if (!string.IsNullOrEmpty(winnerName) && !string.IsNullOrEmpty(time))
            {
                gameData.usuario_ganador = winnerName;
                gameData.partida_duracion = time;

                // Guardar ID de Jugadores
                if(PhotonNetwork.MasterClient.CustomProperties["userId"] != null)
                {
                    gameData.p1_ID = (int)PhotonNetwork.MasterClient.CustomProperties["userId"];
                }
                else
                {
                    Debug.LogError("ID de Jugador 1 perdida...");
                }
                

                if (PlayerUtilities.GetOtherPlayerID() != -1)
                {
                    gameData.p2_ID = PlayerUtilities.GetOtherPlayerID();
                }
                else
                {
                    Debug.LogError("ID de Jugador 2 perdida...");
                }
                // Mostrar información de depuración
                //Debug.LogError("Data class: " + $"usuario_ganador: {gameData.usuario_ganador}, partida_duracion: {gameData.partida_duracion}");

                //Debug.LogError($"Datos de la partida: usuario_ganador = {winnerName}, partida_duracion = {time}");

                //Se guarda la instancia de gameData junto a todos sus atributos recolectados
                string json = JsonUtility.ToJson(gameData);
                //Debug.LogError($"JSON data: {json}");

                NodeJSConnector.Instance.Post("http://localhost:5000/api/guardar-datos-partida", json);
            }
            else
            {
                Debug.Log("Datos de la partida: estan vacios...");
            }

        }

    }

    public void UpdatePlayer1Fails(int count)
    {
        gameData.p1TotalFails = count;
        player1totalFails.text = $"Total de fallos cometidos por {PhotonNetwork.MasterClient.NickName}: {count}";
    }

    public void UpdatePlayer2Fails(int count)
    {
        string otherPlayerName = PlayerUtilities.GetOtherPlayerName();
        gameData.p2TotalFails = count;

        player2totalFails.text = $"Total de fallos cometidos por {otherPlayerName}: {count}";
    }

    public void UpdatePlayer1Moves(int count)
    {
        gameData.TotalMovesp1 = count;
        player1totalMoves.text = $"Total de movimientos hechos por {PhotonNetwork.MasterClient.NickName}: {count}";
    }

    public void UpdatePlayer2Moves(int count)
    {
        string otherPlayerName = PlayerUtilities.GetOtherPlayerName();
        gameData.TotalMovesp2 = count;
        player2totalMoves.text = $"Total de movimientos hechos por {otherPlayerName}: {count}";
    }

    public void BackToLobby()
    {
        captureErrors = true; // Comienza a capturar mensajes de error

        // Verificar si el jugador local es el Master Client
        if (PhotonNetwork.IsMasterClient)
        {
            // Si es el Master Client, todos los jugadores deben abandonar la sala
            PhotonNetwork.CurrentRoom.IsOpen = false; // Cierra la sala para que nuevos jugadores no puedan unirse
            PhotonNetwork.CurrentRoom.IsVisible = false; // Hace que la sala no sea visible en la lista de salas


            // Abandonar la sala (esto también hace que los demás jugadores abandonen la sala)
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            // Si no es el Master Client, simplemente abandona la sala
            PhotonNetwork.LeaveRoom();
        }

        isPlaying.id_usuario = (int)PhotonNetwork.LocalPlayer.CustomProperties["userId"];
        isPlaying.nombre_usuario = PhotonNetwork.NickName;
        isPlaying.play = false;

        string json = JsonUtility.ToJson(isPlaying);
        //Debug.LogError($"JSON data: {json}");

        NodeJSConnector.Instance.Post("http://localhost:5000/api/usuario-isplaying", json);

        RoomUtilities.SaveRoomData(isinRoom,"usuario-isNotInRoom"); //Se mandan los datos recolectados de la sala a Nodejs
        
    }

    public override void OnLeftRoom()
    {
        captureErrors = false;
        StartCoroutine(WaitLeaveRoom());
    }

    IEnumerator WaitLeaveRoom()
    {
        // Esperar hasta que PhotonNetwork esté completamente fuera de la sala
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }

        // Cargar la escena del lobby después de salir de la sala
        SceneManager.LoadScene("Lobby");
    }

    public override void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    public override void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (captureErrors && type == LogType.Error)
        {
            // Debug.LogError(logString);
        }
    }
}
