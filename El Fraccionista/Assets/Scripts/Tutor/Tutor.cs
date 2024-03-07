using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutor : MonoBehaviour
{
    
    private Animator animator;
    private ControlStartGame controlStartGame;
    [SerializeField] private TextMeshPro text_message;
    [SerializeField] private GameObject cloud;
    

    void Start()
    {
        animator = GetComponent<Animator>();
        controlStartGame = FindObjectOfType<ControlStartGame>();
        cloud.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(controlStartGame.turnoActual == 0)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180 + 1f, transform.eulerAngles.z);
        }else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0 + 1f, transform.eulerAngles.z);
        }
    }

    public void ShowMessage(string message, float displayTime = 5f)
    {
        StartCoroutine(ShowMessageCoroutine(message, displayTime));
    }

    private IEnumerator ShowMessageCoroutine(string message, float displayTime)
    {
        // Mostrar el mensaje
        cloud.SetActive(true);
        text_message.text = message;

        // Esperar durante el tiempo especificado
        yield return new WaitForSeconds(displayTime);

        // Ocultar el mensaje después del tiempo especificado
        cloud.SetActive(false);
    }

}
