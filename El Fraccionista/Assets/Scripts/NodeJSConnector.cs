using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class NodeJSConnector : MonoBehaviour
{

    private static NodeJSConnector _instance;

    public static NodeJSConnector Instance
    {
        get
        {
            if (_instance == null)
            {
                // If no instance exists, try to find one in the scene
                _instance = FindObjectOfType<NodeJSConnector>();

                // If still no instance exists, create a new GameObject and add the script
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("NodeJSConnector");
                    _instance = singletonObject.AddComponent<NodeJSConnector>();
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        // Ensure there is only one instance
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void Post(string uri, string data)
    {
        StartCoroutine(PostRequest(uri, data));
    }

    IEnumerator PostRequest(string uri, string data)
    {
        UnityWebRequest www = UnityWebRequest.Post(uri, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Server response: " + www.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }
}
