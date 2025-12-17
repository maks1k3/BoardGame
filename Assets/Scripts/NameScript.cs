using UnityEngine;
using TMPro;

public class NameScript : MonoBehaviour
{
    private TextMeshPro tMP;

    public string PlayerName { get; private set; } = "Unknown";

    void Awake()
    {
        tMP = transform.Find("NameField").gameObject.GetComponent<TextMeshPro>();
    }

    public void SetName(string name)
    {
        PlayerName = name;

        tMP.text = name;
        tMP.color = new Color32(
            (byte)Random.Range(0, 256),
            (byte)Random.Range(0, 256),
            (byte)Random.Range(0, 256),
            255
        );
    }
}
