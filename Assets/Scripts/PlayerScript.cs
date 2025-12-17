using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    int characterIndex;
    public GameObject spawnPoint;
    int[] otherPlayers;
    int index;
    private const string textFileName = "PlayerNames";

    void Start()
    {
        characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        GameObject mainCharacter = Instantiate(playerPrefabs[characterIndex], spawnPoint.transform.position, Quaternion.identity);
        mainCharacter.GetComponent<NameScript>().SetName(PlayerPrefs.GetString("PlayerName", "John"));

        var humanCtrl = mainCharacter.GetComponent<PlayerController>();
        if (humanCtrl == null) humanCtrl = mainCharacter.AddComponent<PlayerController>();
        humanCtrl.isHuman = true;

        List<PlayerMove> ordered = new List<PlayerMove>();
        ordered.Add(mainCharacter.GetComponent<PlayerMove>());

        otherPlayers = new int[PlayerPrefs.GetInt("PlayerCount")];
        string[] nameArray = ReadLinesFromFile(textFileName);

        for (int i = 0; i < otherPlayers.Length - 1; i++)
        {
            spawnPoint.transform.position += new Vector3(0.2f, 0, 0.08f);
            index = Random.Range(0, playerPrefabs.Length);

            GameObject otherPlayer = Instantiate(playerPrefabs[index], spawnPoint.transform.position, Quaternion.identity);
            otherPlayer.GetComponent<NameScript>().SetName(nameArray[Random.Range(0, nameArray.Length)]);

            var botCtrl = otherPlayer.GetComponent<PlayerController>();
            if (botCtrl == null) botCtrl = otherPlayer.AddComponent<PlayerController>();
            botCtrl.isHuman = false;

            ordered.Add(otherPlayer.GetComponent<PlayerMove>());
        }

        TurnManager.Instance.RegisterPlayersInOrder(ordered);
    }
    string[] ReadLinesFromFile(string fileName)
    {
        TextAsset textAsset=Resources.Load<TextAsset>(fileName);

        if(textAsset != null)
        {
            return textAsset.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogWarning("File not found: " + fileName);
            return new string[0];
        }
    }
}
