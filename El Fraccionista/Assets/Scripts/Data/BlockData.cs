using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Data
{
    [CreateAssetMenu(fileName = "Block Data", menuName = "El Fraccionista/Block Data")]
    public class BlockData : ScriptableObject
    {
        // El valor del bloque
        public float Value;
        // El sprite del bloque
        public Sprite Sprite;
    }
}

