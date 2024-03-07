using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Factories;
using Photon.Pun;
using Photon.Realtime;

namespace GameUtils
{
    // Clase que se encarga de agarrar un bloque y moverlo consigo.
    public class BlockDragger : MonoBehaviourPunCallbacks
    {
        // L�gica del singleton
        private static BlockDragger _instance;
        public static BlockDragger Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Try to find an existing BlockDragger in the scene
                    _instance = FindObjectOfType<BlockDragger>();

                    // If it doesn't exist, create a new one on a GameObject
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("Block Dragger");
                        _instance = obj.AddComponent<BlockDragger>();
                    }
                }
                return _instance;
            }
        }

        // Atributo que almacena la capa en la que los bloques se encuentran (Capa "Blocks")
        [SerializeField] LayerMask blockLayer;
        [SerializeField] LayerMask _stackLayerMask;
        [SerializeField] Stack _stack;

        // Atributo que almacena el bloque que se est� seleccionando
        [SerializeField] Block _blockBeingDragged;

        // Start is called before the first frame update
        void Start()
        {
            _blockBeingDragged = null;
        }

        // Update is called once per frame
        private bool wasButtonPressed = false;
        void FixedUpdate()
        {
            bool isButtonPressed = Input.GetMouseButton(0);

            Block blockOn = GetBlockOn();
            Block blockPicked = GetBlockPicked();

            if (isButtonPressed && blockOn != null && blockPicked == null)
            {
                // El bot�n se acaba de presionar en este fotograma.
                Debug.Log("Bot�n del rat�n presionado.");
                PickUpBlock(blockOn);
            }
            else if (!isButtonPressed && blockPicked != null)
            {
                // El bot�n se acaba de soltar en este fotograma.
                Debug.Log("Bot�n del rat�n soltado.");
                DropOffBlock();
            }

            wasButtonPressed = isButtonPressed;

            HandleDraggedBlockMovement();
        }

      

        // Funci�n que se enncarga de mover el bloque seleccionado junto a la flecha del mouse
        void HandleDraggedBlockMovement()
        {
            Debug.Log("HandleDraggedBlockMovement");
            if (_blockBeingDragged == null)
            {
                return;
            }
            Debug.Log("Moviendo bloque!");

            Vector2 mousePosition = Input.mousePosition;
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            _blockBeingDragged.SetPosition(new Vector3(worldPosition.x, worldPosition.y, _blockBeingDragged.transform.position.z));
        }

        // Funci�n para obtener el bloque que se esta agarrando
        public Block GetBlockPicked()
        {
            return _blockBeingDragged;
        }

        // Funci�n que se encarga de seleccionar un bloque
        public void PickUpBlock(Block block)
        {
            _blockBeingDragged = block;
            Debug.Log("Agarrando Bloque: " + _blockBeingDragged);
        }


        public void DropOffBlock()
        {
            Debug.Log("Soltando Bloque: " + _blockBeingDragged);

            // Usar un raycast para verificar si estamos sobre la pila
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, _stackLayerMask);

            if (!(hit.collider != null && (hit.collider.CompareTag("Player1") || hit.collider.CompareTag("Player2"))))
            {
                Destroy(_blockBeingDragged.gameObject);
                //Debug.LogError("Bloque eliminado porque no está sobre la pila.");

            }

            _blockBeingDragged = null;
        }

        // Funci�n para detectar que el mouse esta posicionado sobre 
        public Block GetBlockOn()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, blockLayer);

            // Cuando el rayo detecta una collisi�n
            if (hit.collider != null)
            {
                GameObject obj = hit.collider.gameObject;
                Debug.Log("Cursor sobre " + obj.name);
                // Si esa collisi�n es con un bloque...
                if (obj.CompareTag("Block"))
                {
                    return obj.GetComponent<Block>();
                }
            }
            return null;
        }
    }

}