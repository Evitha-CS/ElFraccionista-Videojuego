using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InstructionData
{
    public string instructionText;
    public GameObject associatedObject;
}

public class Instructions : MonoBehaviour
{
    public GameObject instructions;
    public GameObject menu;
    void Start()
    {

    }

    void Update()
    {

    }

    public void OnClickInstructions()
    {
        instructions.SetActive(true);
        menu.SetActive(false);
    }

    public void OnClickBack()
    {
        instructions.SetActive(false);
        menu.SetActive(true);
    }


}

