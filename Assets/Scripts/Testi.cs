using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testi : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DatabaseManager.CreateTable<PlayerData>();

        PlayerData player = new PlayerData()
        {
            playerName = "Jussi",
            xPosition = 2938.2f,
            yPosition = 9182.1f,
            health = 100,
            attackPower = 50
        };
        DatabaseManager.InsertIntoTable<PlayerData>(player);
        List<PlayerData> saveFiles = DatabaseManager.ReadAllDataFromTable<PlayerData>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class PlayerData
{
    public string playerName;
    public float xPosition, yPosition;
    public int health;
    public int attackPower;
}
