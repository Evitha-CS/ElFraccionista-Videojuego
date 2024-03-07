using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Photon.Pun;

namespace Factories 
{
    public class BlockFactory : MonoBehaviour
    {
        // Lista de los valores de los bloques que esta f�brica puede crear (Se pueden cambiar desde el inspector)
        [SerializeField] private List<float> _blockValuesList = new List<float>();

        // Lista de los prefabs de los bloques que esta f�brica puede crear (Se pueden cambiar desde el inspector)
        [SerializeField] private List<GameObject> _blockPrefabsList = new List<GameObject>();
        
        // Diccionario de bloques para generar los bloques
        Dictionary<float, GameObject> _blockPrefabs = new Dictionary<float, GameObject>();

        // L�gica del singleton de esta f�brica
        private static BlockFactory _instance;
        public static BlockFactory Instance
        {
            get
            {
                if(_instance == null)
                {
                    // Try to find an existing BlockDragger in the scene
                    _instance = FindObjectOfType<BlockFactory>();

                    // If it doesn't exist, create a new one on a GameObject
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("Block Factory");
                        _instance = obj.AddComponent<BlockFactory>();
                    }
                }
                return _instance;
            }
        }

        // Funci�n para inicializar el diccionario de bloques
        private void InitPrefabsDictionary(){
            for(int i = 0; i < _blockValuesList.Count; i++)
            {
                _blockPrefabs.Add(_blockValuesList[i], _blockPrefabsList[i]);
            }
            _blockValuesList.Clear();
            _blockPrefabsList.Clear();
        }

        // Funci�n para instanciar bloques
        public Block CreateBlock(float blockValue, Vector3 position)
        {
            GameObject instance = Instantiate(_blockPrefabs[blockValue], position, Quaternion.identity);
            return instance.GetComponent<Block>();
        }

        private void Awake(){
            InitPrefabsDictionary();
        }


    }
}