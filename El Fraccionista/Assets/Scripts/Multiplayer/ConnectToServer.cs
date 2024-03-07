using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

/** CUANDO TODO FUNCIONE CAMBIAR CODIGO POR EL QUE ESTA EN LA CARPETA DE PROYECTO DE TITULO, PARA QUE LLAME AL USUARIO DESDE REACT**/
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public Text usernameString;
    public TMP_InputField usernameInput;
    public TextMeshProUGUI buttonText;
    private int userId;
    private string userName;



    public static ConnectToServer instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnClickConnect()
    {
        /*
                if (usernameInput.text.Length >= 1)
                {
                    PhotonNetwork.NickName = usernameInput.text;
                    buttonText.text = "Conectando...";
                    PhotonNetwork.AutomaticallySyncScene = true;
                    PhotonNetwork.ConnectUsingSettings();
                }

        */

        if (!String.IsNullOrEmpty(usernameString.text))
        {
            PhotonNetwork.NickName = userName;
            PhotonNetwork.LocalPlayer.CustomProperties["userId"] = userId;
            buttonText.text = "Conectando...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }

    }



    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby"); //Se debe crear la escena
    }

    public void SetString(string userIdAndName)
    {
        usernameString.text = userIdAndName;


        // Dividir la cadena utilizando la coma como separador
        string[] parts = userIdAndName.Split(',');

        // Verificar si se dividió correctamente y obtener la ID y el nombre
        if (parts.Length == 2)
        {
            // Convertir la ID a entero
            if (int.TryParse(parts[0], out userId))
            {
                userName = parts[1].Trim(); // Eliminar espacios en blanco alrededor del nombre
                // Aquí puedes usar userId y userName según tus necesidades
            }
            else
            {
                Debug.LogError("Error al convertir la ID a entero");
            }
        }
        else
        {
            Debug.LogError("Formato de cadena no válido");
        }
    }

}
