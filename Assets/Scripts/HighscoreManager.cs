using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HighscoreManager : MonoBehaviour
{
    private void Start()
    {
        DatabaseManager.CreateTable<Highscore>();
        Highscore score = new Highscore()
        {
            name = "Mikko",
            score = 150,
            date = DateTime.Now.ToString()
        };
        DatabaseManager.InsertIntoTable<Highscore>(score);
    }
}

public class Highscore
{
    public string name;
    public int score;
    public string date;
}
