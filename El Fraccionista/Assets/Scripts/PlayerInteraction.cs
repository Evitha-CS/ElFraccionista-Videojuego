using Photon.Pun;
using UnityEngine;

//ESTA CLASE YA NO SE UTILIZA BORRAR DESPUES
public class PlayerInteraction : MonoBehaviourPun
{
    private void Start()
    {

        // Accede a la etiqueta del objeto actual.
        string objectTag = gameObject.tag;

        // Comprueba si la etiqueta coincide con la identidad del jugador.
        if (objectTag == "Player1" && PhotonNetwork.IsMasterClient)
        {
            // Este objeto pertenece al jugador 1, permite la interacción.
            EnableInteraction();
        }
        else if (objectTag == "Player2" && !PhotonNetwork.IsMasterClient)
        {
            // Este objeto pertenece al jugador 2, permite la interacción.
            EnableInteraction();
        }
        else
        {
            // Este objeto no pertenece al jugador local, desactiva la interacción.
            DisableInteraction();
        }

    }

    private void EnableInteraction()
    {

        GetComponent<Collider2D>().enabled = true;
        Debug.Log("Interacción Activada");
    }
    private void DisableInteraction()
    {

        GetComponent<Collider2D>().enabled = false;
        Debug.Log("Interacción Desactivada");
    }
}
