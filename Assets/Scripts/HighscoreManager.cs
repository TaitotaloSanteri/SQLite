using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class HighscoreManager : MonoBehaviour
{
    private List<Highscore> highscores;
    [SerializeField]
    private Transform addScorePanel, showScorePanel, scorePrefab;
    [SerializeField]
    private InputField nameField, scoreField;


    private void Start()
    {
        //DatabaseManager.DropTable<Highscore>();
        DatabaseManager.CreateTable<Highscore>();
        DatabaseManager.ExecutePureSQL("UPDATE Highscore SET name = 'Juan' WHERE name = 'Mikko'");
    }

    public void AddScore()
    {
        showScorePanel.gameObject.SetActive(false);
        addScorePanel.gameObject.SetActive(true);
    }
    public void SubmitScore()
    {
        Highscore newScore = new Highscore()
        {
            name = nameField.text,
            score = int.Parse(scoreField.text),
            date = DateTime.Now.ToString()
        };
        DatabaseManager.InsertIntoTable<Highscore>(newScore);
        nameField.text = "";
        scoreField.text = "";
    }
    public void ShowScores()
    {
        addScorePanel.gameObject.SetActive(false);
        RectTransform[] rects = showScorePanel.GetComponentsInChildren<RectTransform>();
        for (int i = 1; i < rects.Length; i++ )
        {
            Destroy(rects[i].gameObject);
        }
        // Haetaan kaikki highscore tietueet tietokannasta, ja järjestään
        // ne System.Linq kirjastosta löytyvällä komennolla isoimmasta
        // pistemäärästä pienimpään
        highscores = DatabaseManager.ReadAllDataFromTable<Highscore>()
                                    .OrderByDescending(obj => obj.score)
                                    .ToList();

        showScorePanel.gameObject.SetActive(true);
        for (int i = 0; i < highscores.Count; i++)
        {
            Transform score = Instantiate(scorePrefab, showScorePanel);
            Text[] texts = score.GetComponentsInChildren<Text>();
            texts[0].text = $"{i + 1}.";
            texts[1].text = $"{highscores[i].name}";
            texts[2].text = $"{highscores[i].score}";
            texts[3].text = $"{highscores[i].date}";
            score.position = new Vector2(showScorePanel.position.x, 
                                         showScorePanel.position.y - i * 50);
        }
    }
}

public class Highscore
{
    public Int64 id;
    public string name;
    public int score;
    public string date;
}
