using UnityEngine.UI;
using UnityEngine;



public class RollerNumberScript : MonoBehaviour
{
    DiceRollScript diceRollScript;
    [SerializeField] Text rolledNumberText;


    void Awake()
    {
        diceRollScript= FindFirstObjectByType<DiceRollScript>();
    }

    void Update()
    {
        if (diceRollScript != null)
        {
            if (diceRollScript.isLanded)
                rolledNumberText.text = diceRollScript.diceFaceNum;

            else
                rolledNumberText.text = "?";
        }
        else
            Debug.LogWarning("DiceScript not found!");
    }
}

