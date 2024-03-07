using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace GameUtils
{
    public static class RoomUtilities
    {


        public static void SaveRoomData(IsinRoom isinRoom, string serverRuta)
        {
            //GUARGAR DATOS DE SALA VIRTUAL
            isinRoom.nombre_sala = PhotonNetwork.CurrentRoom.Name;

            // Guardar ID de Jugadores
            if (PhotonNetwork.MasterClient.CustomProperties["userId"] != null)
            {
                isinRoom.usuario1_Id = (int)PhotonNetwork.MasterClient.CustomProperties["userId"];
            }
            else
            {
                Debug.Log("ID de Jugador 1 perdida...");
            }


            if (PlayerUtilities.GetOtherPlayerID() != -1)
            {
                isinRoom.usuario2_Id = PlayerUtilities.GetOtherPlayerID();
            }
            else
            {
                Debug.Log("ID de Jugador 2 perdida...");
            }

            //Se guarda la instancia de isinRoom junto a todos sus atributos recolectados
            string json = JsonUtility.ToJson(isinRoom);
            //Debug.Log($"JSON data: {json}");

            // Concatena la URL del servidor con la ruta específica
            string fullUrl = $"http://localhost:5000/api/{serverRuta}";
            //Debug.Log(fullUrl);
            NodeJSConnector.Instance.Post(fullUrl, json);
        }
    }
}

