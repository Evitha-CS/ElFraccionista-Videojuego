using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;
using Photon.Pun;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]


public class Stack : MonoBehaviourPunCallbacks
{

    // Componentes de la Pila
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider;
    Rigidbody2D _rb2D;
    RectTransform _rectTransform;

    // Propiedades de la Pila
    [SerializeField] private List<Block> _blocks;
    [SerializeField] private List<Block> _blocksPhoton;

    private PhotonView _photonview;
    private ControlStartGame controlStartGame;

    public readonly float MaxValue = 2f;
    public int player1failMovesCount = 0, player2failMovesCount = 0;

    private VictoryManager victoryManager;
    private Tutor tutor;
    private string localPlayerName;
    private float currentSum = 0f; //suma actual de bloques



    public float CurrentValue
    {
        get
        {
            float currentValue = 0f;
            foreach (Block block in _blocks)
            {
                currentValue += block.Value;
            }
            return currentValue;
        }
    }

    private void InitProperties()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _boxCollider = gameObject.GetComponent<BoxCollider2D>();
        _rb2D = gameObject.GetComponent<Rigidbody2D>();
        _blocks = new List<Block>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        InitProperties();
        // Buscar la instancia de ControlStartGame al inicio
        controlStartGame = FindObjectOfType<ControlStartGame>();
        victoryManager = FindObjectOfType<VictoryManager>();
        tutor = FindObjectOfType<Tutor>();
        localPlayerName = PhotonNetwork.NickName;
    }

    // Funci�n que Gestiona las colisiones 
    void OnTriggerStay2D(Collider2D other)
    {

        Debug.Log("OnTriggerStay2D");
        // Verificar que el GameObject tiene la etiqueta "Block" (Verificar que es un bloque)
        if (!other.gameObject.CompareTag("Block"))
        {
            Debug.Log("Exit 1");
            return;
        }

        // Verificar que el GameObject posee un script Block.cs
        Block block = other.gameObject.GetComponent<Block>();
        if (block == null)
        {
            Debug.Log("Exit 2");
            return;
        }

        // Verificar que este bloque no est� siendo agarrado por el usuario
        if (BlockDragger.Instance.GetBlockPicked() != block)
        {
            AddBlock(block);
            UpdateBlocksPosition();
            UpdateBlocksOnPhoton();
        }
        Debug.Log("OnTriggerStay2D END");
    }

    void UpdateBlocksPosition()
    {
        // Variables para calcular la coordenada Y en la que cada bloque deber� colocarse
        float stackScale = _rectTransform.localScale.x;

        // float yRelativeBottom = -(_rectTransform.rect.height * stackScale) / 2f;
        float yRelativeBottom = -0.62f;  
        float relY = yRelativeBottom;
        float xPos = _rectTransform.position.x;
        float zPos = _rectTransform.position.z;
        float verticalSpacing = 0.004f; 

        // Se recorre cada bloque almacenado en la pila
        foreach (Block block in _blocks)
        {
            RectTransform blockRT = (RectTransform)block.transform;

            float blockHeight = blockRT.rect.height * blockRT.localScale.y;
            relY += (blockHeight / 2f) + verticalSpacing;

            blockRT.anchoredPosition = new Vector3(0, relY, 0); //Posicion final del bloque

            relY += (blockHeight / 2f);

        }

        if (CheckTwoWholeBlocks())
        {
            //Finaliza la partida
            Debug.LogError("¡Partida terminada!");
            victoryManager.DisableInGameElements();
        }
    }

    public bool CheckTwoWholeBlocks()
    {
        int wholeBlockCount = 0;

        // Contar el número de bloques enteros en la pila
        foreach (Block block in _blocks)
        {
            if (block.Value == 1f)
            {
                wholeBlockCount++;
            }
        }

        // Verificar si hay dos bloques enteros
        //return wholeBlockCount == 2;

        // Verificar si hay dos bloques enteros
        if (wholeBlockCount == 2)
        {
            // Llama a la función para mostrar el ganador y resumen
            victoryManager.WinnerAndSummry(localPlayerName);
            return true;
        }

        return false;
    }
    private void UpdateBlocksOnPhoton()
    {
        // Eliminando bloques instanciados con photon
        foreach (Block blockPhoton in _blocksPhoton)
        {
            PhotonNetwork.Destroy(blockPhoton.gameObject);
        }
        _blocksPhoton.Clear();
        // Instanciando bloques en el lado del otro jugador
        foreach (Block block in _blocks)
        {
            RectTransform blockRT = (RectTransform)block.transform;
            Block newBlockPhoton = PhotonNetwork.Instantiate(block.gameObject.name.Replace("(Clone)", ""), blockRT.TransformPoint(Vector3.zero), Quaternion.identity).GetComponent<Block>();
            _blocksPhoton.Add(newBlockPhoton);
        }
    }

    private float GetNextYRel(float value)
    {
        float yRel = 0; //extraValue = -2;
        switch (value)
        {
            case 0.125f:
                yRel = 8f - 2f;
                break;
            case 0.25f:
                yRel = 16f - 4f;
                break;
            case 0.5f:
                yRel = 32f - 8f;
                break;
            case 1f:
                yRel = 64f - 16f;
                break;
            default:
                yRel = 0f;
                break;
        }
        return yRel;
    }

    public float GetHeight()
    {
        if (_boxCollider != null)
        {
            return _boxCollider.size.y;
        }
        // If not using a collider, you can use the renderer's bounds
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.y;
        }
        return 0f;
    }
    public float GetRealHeight()
    {
        // Get the block's local scale
        Vector3 localScale = this.transform.localScale;

        // Calculate the height by taking into account the Y scale component
        float blockHeight = this.GetHeight() * localScale.y;
        return blockHeight;
    }


    // Funci�n que almacena un bloque en la pila
    public bool AddBlock(Block newBlock, int index = -1)
    {
        // Validaciones
        if (newBlock == null)
        {
            return false;
        }
        if (newBlock.Value == 0.125f && (newBlock.Value + this.CurrentValue > this.MaxValue))
        {
            return false;
        }


        // Verificar si la suma actual más el valor del nuevo bloque es igual a 2
        if (currentSum + newBlock.Value == 2f)
        {
            tutor.ShowMessage("Ya has llenado la pila, fusiona los bloques dentro de ella para ganar");
        }

        // Proceso para fusionar un bloque
        if (newBlock.Value > 0.125f)
        {
            if (!AddBlockToFuse(newBlock))
            {
                RemoveInvalidBlock(newBlock);
                return false;
            }
            tutor.ShowMessage("Muy bien, has logrado fusionar dos bloques!!!");
        }
        else
        {
            // Proceso para almacenar el bloque
            if (index < 0)
            {
                _blocks.Add(newBlock);
            }
            else
            {
                _blocks.Insert(index, newBlock);
            }

            // Actualizar la suma actual solo si es un bloque de valor 0.125f
            currentSum += (newBlock.Value == 0.125f) ? newBlock.Value : 0f;

        }

        newBlock.tag = "Stacked Block";
        newBlock.transform.SetParent(this.transform);

        // Incrementar el contador de movimientos del jugador actual
        IncrementarContadorMovimientos();
        // Debug.LogError("Se ejecutó : IncrementarContadorMovimientos");

        return true;
    }


    private void IncrementarContadorMovimientos()
    {
        // Verificar si la instancia existe y pertenece al jugador local
        if (controlStartGame != null)
        {
            // Incrementar el contador de movimientos
            controlStartGame.IncrementarMovimientos();
            //  Debug.LogError("Se llamó a : IncrementarMovimientos");
        }
        else
        {
            //  Debug.LogError("ControlStartGame = null");
        }
    }

    public bool RemoveBlock(Block block)
    {
        _blocks.Remove(block);

        GameObject.Destroy(block.gameObject);

        return true;
    }

    // Funci�n que se encarga de verificar si un bloque es apto para ser fusionado
    private bool AddBlockToFuse(Block newBlock)
    {
        string newfraction, actualfraction;
        // Inicializar las variables b�sicas
        BoxCollider2D newBlockCollider = newBlock.GetComponent<BoxCollider2D>();
        Dictionary<Block, float> blockDistances = new Dictionary<Block, float>();

        // Revisar si el collider2D del bloque nuevo encaja con alguno de los collider de los bloques de la pila
        foreach (Block block in _blocks)
        {
            SpriteRenderer spriteRenderer = block.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.white;

            // Verificar si este bloque esta siendo tocado por el nuevo
            BoxCollider2D blockCollider = block.gameObject.GetComponent<BoxCollider2D>();
            if (newBlockCollider.IsTouching(blockCollider))
            {
                newfraction = ConvertToFraction(newBlock.Value);
                actualfraction = ConvertToFraction(block.Value);
                tutor.ShowMessage($"Recuerda que el bloque de {newfraction} no puede ser fusionado con uno de {actualfraction}");

                Vector2 newBlockCenter = newBlockCollider.bounds.center;
                Vector2 blockCenter = blockCollider.bounds.center;
                float distance = Vector2.Distance(newBlockCenter, blockCenter);
                blockDistances.Add(block, distance);
            }
            else
            {
                newfraction = ConvertToFraction(newBlock.Value);
                tutor.ShowMessage($"¡No hagas trampa!, el bloque {newfraction} solo puede ser usado para fusionar otros bloques");
            }

        }

        // Ordenando las distancias de menor a mayor y obteniendo las 2 distancias m�s cortas
        var sortedBlockDistances = blockDistances.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value); ;
        var nearestDistances = sortedBlockDistances.Take(2);
        float nearestBlocksTotalValue = 0f;

        // Iterando sobre los 2 bloques elegidos anteriormente
        foreach (KeyValuePair<Block, float> pair in nearestDistances)
        {
            // Pintando los bloques de rojo
            Block nearestBlock = pair.Key;
            SpriteRenderer spriteRenderer = nearestBlock.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.red;

            // Sumando el valor total de los 2 bloques
            nearestBlocksTotalValue += nearestBlock.Value;
        }

        if (nearestDistances.ToList().Count != 2)
        {
            return false;
        }

        if (nearestBlocksTotalValue != newBlock.Value)
        {
            // Llama a la función para eliminar el bloque soltado
            RemoveInvalidBlock(newBlock);
            return false;
        }

        return ReplaceBlocks(newBlock, nearestDistances.FirstOrDefault().Key, nearestDistances.ElementAtOrDefault(1).Key);
    }

    //Funcion para pasar los decimales a fraccion
    string ConvertToFraction(double decimalNumber)
    {
        if (decimalNumber == 0.125)
        {
            return "1/8";
        }
        else if (decimalNumber == 0.25)
        {
            return "1/4";
        }
        else if (decimalNumber == 0.5)
        {
            return "1/2";
        }
        else
        {
            return "1 Entero";
        }
    }



    // Funci�n que se encarga de reemplazar 2 bloques antiguos por uno nuevo
    private bool ReplaceBlocks(Block newBlock, Block oldBlock1, Block oldBlock2)
    {
        // Validar los valores totales de los bloques

        // Reemplazar los bloques
        int oldBlock1Index = _blocks.IndexOf(oldBlock1);
        int oldBlock2Index = _blocks.IndexOf(oldBlock2);
        int newBlockIndex;

        //Debug.Log(oldBlock1Index);
        //Debug.Log(oldBlock2Index);

        if (oldBlock2Index < oldBlock1Index)
        {
            newBlockIndex = oldBlock2Index;
        }
        else
        {
            newBlockIndex = oldBlock1Index;
        }

        print("New Block Index:" + newBlockIndex);

        RemoveBlock(_blocks[newBlockIndex]);
        RemoveBlock(_blocks[newBlockIndex]);

        _blocks.Insert(newBlockIndex, newBlock); // <-- Aqu� esta el problema

        newBlock.gameObject.tag = "Stacked Block";
        newBlock.gameObject.transform.SetParent(this.transform);

        Debug.Log("�El bloque ha sido reemplazado exitosamente!");

        // IncrementarContadorMovimientos();

        return true;
    }


    // Función para eliminar el bloque que no se puede fusionar
    private void RemoveInvalidBlock(Block invalidBlock)
    {
        _blocks.Remove(invalidBlock);
        Destroy(invalidBlock.gameObject);  // Destruye el objeto del bloque

        // Incrementar el contador de fallos individual del jugador en la red
        photonView.RPC("RPC_IncrementarFailMovesCount", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_IncrementarFailMovesCount()
    {

        // Incrementar el contador de fallos individual del jugador
        if (gameObject.CompareTag("Player1"))
        {
            player1failMovesCount++;
            victoryManager.UpdatePlayer1Fails(player1failMovesCount);
            //Debug.LogError("Bloque eliminado debido a una fusión no válida. Contador de fallo para Player 1 = " + player1failMovesCount);

        }

        if (gameObject.CompareTag("Player2"))
        {
            player2failMovesCount++;
            victoryManager.UpdatePlayer2Fails(player2failMovesCount);
            //Debug.LogError("Bloque eliminado debido a una fusión no válida. Contador de fallo para Player 2 = " + player2failMovesCount);

        }


    }


    // Función para verificar si se puede agregar un bloque único sin fusionar
    private bool CanAddSingleBlock(Block newBlock)
    {
        if (newBlock.Value == 0.125f)
        {
            return true;
        }
        else
        {
            // Llama a la función para eliminar el bloque que no se puede fusionar
            RemoveInvalidBlock(newBlock);
            return false;
        }
    }


}
