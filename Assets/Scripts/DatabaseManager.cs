using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private static SQLiteConnection dbConnection;
    private static SQLiteCommand dbCommand;
    private const string dbName = "TestDatabase.db";



    [RuntimeInitializeOnLoadMethod]
    private static void InitializeDatabase()
    {
        // Varmistetaan, että ei luoda kuin yksi yhteys tietokantaan.
        if (dbConnection != null)
        {
            Debug.Log("Tietokantayhteys on jo olemassa.");
            return;
        }
        // Määritetään tietokannan tiedoston sijainti.
        string dbPath = $"Data Source={Application.persistentDataPath}/{dbName}";
        // Luodaan yhteys tietokantaan. Jos tiedostoa ei ole olemassa,
        // tämä komento luo uuden tiedoston.
        dbConnection = new SQLiteConnection(dbPath);
        // Avataan yhteys tietokantaan.
        dbConnection.Open();
        // Luodaan yhteys SQL-komentojen ja juuri luodun tietokannan välille.
        dbCommand = new SQLiteCommand(dbConnection);
        Debug.Log("Yhteys tietokantaan muodostettu. " + dbPath);
    }
}
