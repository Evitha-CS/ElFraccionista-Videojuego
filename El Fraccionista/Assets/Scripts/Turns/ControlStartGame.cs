using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;
using GameUtils;
using Photon.Realtime;

public class ControlStartGame : MonoBehaviourPunCallbacks
{
    public TextMeshPro mensajeEspera;
    public TextMeshPro movimientosText;
    public TextMeshPro movimientosTotalesText;
    public Button botonIniciar;
    public Button botonCambiarTurno;
    public int turnoActual = 0; // 0 para el jugador 1, 1 para el jugador 2
    private int movimientosRealizados = 0;
    public int limiteMovimientos = 3;
    private Dictionary<int, int> movimientosTotales = new Dictionary<int, int>();
    private VictoryManager victoryManager;
    private Tutor tutor;
    public TextMeshPro timerText;
    private float timeElapsed;
    public string totalTime;
    private int minutes, seconds, cents;
    private IsPlaying isPlaying = new IsPlaying();
    private IsinRoom isinRoom = new IsinRoom();



    void Start()
    {

        isPlaying.id_usuario = (int)PhotonNetwork.LocalPlayer.CustomProperties["userId"];
        isPlaying.nombre_usuario = PhotonNetwork.NickName;
        isPlaying.play = true;

        string json = JsonUtility.ToJson(isPlaying);
        //Debug.LogError($"JSON data: {json}");

        NodeJSConnector.Instance.Post("http://localhost:5000/api/usuario-isplaying", json);

        movimientosTotales[0] = 0;
        movimientosTotales[1] = 0;

        photonView.RPC(nameof(InicializarMovimientosTotales), RpcTarget.AllBuffered, movimientosTotales);

        DesactivarCollidersYCambiarColor("Player1");
        DesactivarCollidersYCambiarColor("Player2");

        victoryManager = FindObjectOfType<VictoryManager>();
        tutor = FindObjectOfType<Tutor>();

        // Mostrar mensaje de espera al jugador 2 solo si es el jugador local
        if (!PhotonNetwork.IsMasterClient)
        {
            mensajeEspera.text = $"Esperando a {PhotonNetwork.MasterClient.NickName}";
            tutor.ShowMessage(mensajeEspera.text);
            botonIniciar.interactable = false;
            botonCambiarTurno.gameObject.SetActive(false);
            botonIniciar.GetComponentInChildren<TextMeshPro>().text = "Esperando a iniciar...";
        }

        if (PhotonNetwork.IsMasterClient)
        {
            botonCambiarTurno.gameObject.SetActive(false);
            RoomUtilities.SaveRoomData(isinRoom, "usuario-isinRoom"); //Se mandan los datos recolectados de la sala a Nodejs
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            string winner = PhotonNetwork.NickName;

            victoryManager.DisableInGameElements();

            if (GetIndexMaximumMovements() == 0)
            {
                winner = PhotonNetwork.MasterClient.NickName;
            }

            victoryManager.WinnerAndSummry(winner);
        }

    }

    private IEnumerator StartTimer()
    {
        while (true)
        {
            timeElapsed += Time.deltaTime;
            minutes = (int)(timeElapsed / 60f);
            seconds = (int)(timeElapsed - minutes * 60f);
            cents = (int)((timeElapsed - (int)timeElapsed) * 100f);

            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, cents);
            totalTime = timerText.text;

            if (victoryManager.victoryPanel.activeSelf) break;

            yield return null;
        }
    }

    public int GetIndexMaximumMovements()
    {
        if (movimientosTotales.Count == 2)
        {
            // Comparar los dos elementos y devolver el índice del mayor
            return (movimientosTotales[0] > movimientosTotales[1]) ? 0 : 1;
        }

        return -1;
    }

    //  RPCs para : MOVIMIENTOS TOTALES

    [PunRPC]
    void InicializarMovimientosTotales(Dictionary<int, int> movimientosIniciales)
    {
        movimientosTotales = movimientosIniciales;
    }

    [PunRPC]
    void ActualizarMovimientosTotales(Dictionary<int, int> nuevosMovimientosTotales)
    {
        movimientosTotales = nuevosMovimientosTotales;

        // Actualizar el TextMeshPro con el nuevo contador total de movimientos
        movimientosTotalesText.text = $"Movimientos Totales: {movimientosTotales[turnoActual]}";

        if (turnoActual == 0)
        {
            victoryManager.UpdatePlayer1Moves(movimientosTotales[0]);
        }
        else
        {
            victoryManager.UpdatePlayer2Moves(movimientosTotales[1]);
        }
    }


    public void IniciarPartida()
    {
        // Enviar un evento para iniciar la partida
        photonView.RPC(nameof(RPCIniciarPartida), RpcTarget.All, turnoActual);
        botonIniciar.gameObject.SetActive(false);
        photonView.RPC(nameof(RPCEstablecerMovimientos), RpcTarget.All, 0);
        botonCambiarTurno.gameObject.SetActive(true);
    }

    public void CambiarTurno()
    {
        if (PhotonNetwork.IsMasterClient && turnoActual == 0)
        {

            botonCambiarTurno.gameObject.SetActive(true);
            movimientosRealizados = 0;

            // Solo el MasterClient puede cambiar el turno al jugador 2
            photonView.RPC(nameof(RPCCambiarTurno), RpcTarget.All, 1);
            photonView.RPC(nameof(RPCEstablecerMovimientos), RpcTarget.All, 0);
            //Debug.LogError("Turno cambiado a jugador 2 " + turnoActual);
            botonCambiarTurno.gameObject.SetActive(false);

        }
        else if (!PhotonNetwork.IsMasterClient && turnoActual == 1)
        {

            botonCambiarTurno.gameObject.SetActive(true);
            movimientosRealizados = 0;

            // Solo el jugador 2 puede cambiar el turno al MasterClient
            photonView.RPC(nameof(RPCCambiarTurno), RpcTarget.All, 0);
            photonView.RPC(nameof(RPCEstablecerMovimientos), RpcTarget.All, 0);
            //Debug.LogError("Turno cambiado a jugador 1 " + turnoActual);
            botonCambiarTurno.gameObject.SetActive(false);

        }
    }


    [PunRPC]
    void RPCIniciarPartida(int nuevoTurno, PhotonMessageInfo info)
    {
        // Inicializar los movimientos totales al inicio de la partida
        movimientosTotales[0] = 0;
        movimientosTotales[1] = 0;

        // Establecer el primer turno
        turnoActual = nuevoTurno;
        //Debug.LogError("Turno Actual dentro de RPCIniciarPartida = " + turnoActual);
        mensajeEspera.text = $"Turno del Jugador {turnoActual + 1}";
        tutor.ShowMessage(mensajeEspera.text);

        if (!PhotonNetwork.IsMasterClient)
        {
            DesactivarCollidersYCambiarColor("Player1");
            DesactivarCollidersYCambiarColor("Player2");
            botonIniciar.gameObject.SetActive(false);
        }
        else
        {
            ActivarCollidersYRestaurarColor("Player1");
            DesactivarCollidersYCambiarColor("Player2");
        }

        StartCoroutine(StartTimer()); //Comienza a contar el Timer

    }

    [PunRPC]
    void RPCCambiarTurno(int nuevoTurno, PhotonMessageInfo info)
    {
        turnoActual = nuevoTurno;

        photonView.RPC(nameof(ActualizarMovimientosTotales), RpcTarget.All, movimientosTotales);

        //Debug.LogError("Turno Actual dentro de RPCCambiarTurno = " + turnoActual);
        // Actualizar el mensaje de espera con el nuevo turno
        mensajeEspera.text = $"Turno del Jugador {turnoActual + 1}";
        tutor.ShowMessage(mensajeEspera.text);

        // Gestionar los colliders y colores según el nuevo turno
        if (turnoActual == 0 && PhotonNetwork.IsMasterClient)
        {
            ActivarCollidersYRestaurarColor("Player1");
            DesactivarCollidersYCambiarColor("Player2");
            botonCambiarTurno.gameObject.SetActive(true);
        }
        if ((turnoActual == 0 && !PhotonNetwork.IsMasterClient) || (turnoActual == 1 && PhotonNetwork.IsMasterClient))
        {
            DesactivarCollidersYCambiarColor("Player1");
            DesactivarCollidersYCambiarColor("Player2");
            botonCambiarTurno.gameObject.SetActive(false);
        }
        if (turnoActual == 1 && !PhotonNetwork.IsMasterClient)
        {
            ActivarCollidersYRestaurarColor("Player2");
            DesactivarCollidersYCambiarColor("Player1");
            botonCambiarTurno.gameObject.SetActive(true);
        }

        // Actualizar el TextMeshPro con el nuevo contador total de movimientos
        movimientosTotalesText.text = $"Movimientos Totales: {movimientosTotales[turnoActual]}";

    }


    void DesactivarCollidersYCambiarColor(string tag)
    {
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject jugador in jugadores)
        {
            // Obtener todos los objetos hijos del jugador
            Transform[] objetosHijos = jugador.GetComponentsInChildren<Transform>(true);

            foreach (Transform objetoHijo in objetosHijos)
            {
                // Filtrar por nombres específicos
                if (objetoHijo.name == "Stack" ||
                    objetoHijo.name == "Block Menu Slot 1" ||
                    objetoHijo.name == "Block Menu Slot 2" ||
                    objetoHijo.name == "Block Menu Slot 3" ||
                    objetoHijo.name == "Block Menu Slot 4")
                {
                    // Desactivar el collider
                    Collider2D collider = objetoHijo.GetComponent<Collider2D>();
                    if (collider != null)
                    {

                        collider.enabled = false;
                    }

                    // Cambiar el color a gris
                    Renderer renderer = objetoHijo.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = Color.gray;
                    }
                }
            }
        }
    }

    void ActivarCollidersYRestaurarColor(string tag)
    {
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject jugador in jugadores)
        {
            // Obtener todos los objetos hijos del jugador
            Transform[] objetosHijos = jugador.GetComponentsInChildren<Transform>(true);

            foreach (Transform objetoHijo in objetosHijos)
            {
                // Filtrar por nombres específicos
                if (objetoHijo.name == "Stack" ||
                    objetoHijo.name == "Block Menu Slot 1" ||
                    objetoHijo.name == "Block Menu Slot 2" ||
                    objetoHijo.name == "Block Menu Slot 3" ||
                    objetoHijo.name == "Block Menu Slot 4")
                {
                    // Activar el collider
                    Collider2D collider = objetoHijo.GetComponent<Collider2D>();
                    if (collider != null)
                    {
                        collider.enabled = true;
                    }

                    // Restaurar el color original 
                    Renderer renderer = objetoHijo.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = Color.white;
                    }
                }
            }
        }
    }


    public void IncrementarMovimientos()
    {
        // Verificar si se ha superado el límite de movimientos
        if (movimientosRealizados < limiteMovimientos)
        {
            movimientosRealizados++;
            // Debug.LogError("Movimientos realizados: " + movimientosRealizados);
            // Incrementar los movimientos totales del jugador actual
            movimientosTotales[turnoActual]++;

            photonView.RPC(nameof(ActualizarMovimientosTotales), RpcTarget.All, movimientosTotales);

            // Actualizar el TextMeshPro con el nuevo número de movimientos
            movimientosText.text = movimientosRealizados.ToString();

            // Verificar si se superó el límite de movimientos después de incrementar
            if (movimientosRealizados >= limiteMovimientos)
            {
                // Desactivar la interacción con los objetos
                DesactivarInteraccion();

                // Llamar al método RPC para sincronizar el contador de movimientos entre los jugadores
                photonView.RPC(nameof(ActualizarContadorMovimientos), RpcTarget.All, movimientosRealizados);
            }
            else
            {
                // Llamar al método RPC para sincronizar el contador de movimientos entre los jugadores
                photonView.RPC(nameof(ActualizarContadorMovimientos), RpcTarget.All, movimientosRealizados);
            }
        }

        // Actualizar el TextMeshPro con el nuevo contador total de movimientos
        //movimientosTotalesText.text = $"Movimientos Totales: {movimientosTotales[turnoActual]}";

    }

    void DesactivarInteraccion()
    {
        if (turnoActual == 0)
        {
            DesactivarCollidersYCambiarColor("Player1");
        }
        else if (turnoActual == 1)
        {
            DesactivarCollidersYCambiarColor("Player2");
        }

    }

    [PunRPC]
    void ActualizarContadorMovimientos(int nuevosMovimientos)
    {
        // Esta función se llama para sincronizar el contador de movimientos entre todos los jugadores
        movimientosRealizados = nuevosMovimientos;
        movimientosText.text = movimientosRealizados.ToString();
        // Debug.LogError("Contador aumentado a: " + movimientosRealizados);

        //movimientosTotales[turnoActual] = movimientosRealizados;
    }

    [PunRPC]
    void RPCEstablecerMovimientos(int nuevosMovimientos)
    {
        // Este método RPC se llama para establecer el valor de movimientosRealizados en todos los clientes
        movimientosRealizados = nuevosMovimientos;
        movimientosText.text = movimientosRealizados.ToString();
        // Debug.LogError("Movimientos reiniciados a: " + movimientosRealizados);
    }


    //Por si se desconecta en medio de la partida
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Desconectado del servidor. Causa: {cause}");

        isPlaying.id_usuario = (int)PhotonNetwork.LocalPlayer.CustomProperties["userId"];
        isPlaying.nombre_usuario = PhotonNetwork.NickName;
        isPlaying.play = false;

        string json = JsonUtility.ToJson(isPlaying);
        //Debug.LogError($"JSON data: {json}");

        NodeJSConnector.Instance.Post("http://localhost:5000/api/usuario-isplaying", json);
        //NodeJSConnector.Instance.Post("http://146.83.194.142:1477/api/usuario-isplaying", json);

        RoomUtilities.SaveRoomData(isinRoom,"usuario-isNotInRoom"); //Se mandan los datos recolectados de la sala a Nodejs

    }

}
