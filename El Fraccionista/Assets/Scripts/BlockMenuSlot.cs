using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Factories;
using GameUtils;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BlockMenuSlot : MonoBehaviour
{
    [SerializeField] private float value;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Generar el bloque al momento de presionar el botón
    void OnMouseDown()
    {
        // Obteniendo las coordenadas del mouse
        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        // Generando el bloque (en la misma positión del mouse)
        Block newBlock = BlockFactory.Instance.CreateBlock(value, worldPosition);
        // BlockDragger.Instance.PickUpBlock(newBlock);
    }
}
