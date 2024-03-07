using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using GameUtils;
using UnityEngine.UIElements;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(RectTransform))]


public class Block : MonoBehaviour
{
    // Datos base del bloque
    [SerializeField]float _value;
    public float Value { get => _value; }
    private float lifeTimer = 0.2f;

    // Componentes del bloque
    SpriteRenderer _spriteRenderer;
    BoxCollider2D _boxCollider;
    Rigidbody2D _rb2D;
    // RectTransform _rectTransform;

    // Inicializar componentes
    private void InitProperties()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        _boxCollider = gameObject.GetComponent<BoxCollider2D>();

        _rb2D = gameObject.GetComponent<Rigidbody2D>();

        // _rectTransform = gameObject.GetComponent<RectTransform>();
    }
    // Inicializar Eventos
    void InitEvents(){
        
    }
    // Start is called before the first frame update
    void Awake()
    {
        InitProperties();
        InitEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Vector3 newPosition)
    {
        if(newPosition == null){
            return;
        }
        this.transform.position = new Vector3(newPosition.x, newPosition.y, this.transform.position.z);
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
}
