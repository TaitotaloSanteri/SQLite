using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    // Muuttuja, johon tallennettaan aktiivinen yhteys tietokantaan 
    private static SQLiteConnection dbConnection;
    // Muuttuja, johon tallennettaan SQL-komennot, jotka lähetään tietokantaan
    private static SQLiteCommand dbCommand;
    // Tietokannan tiedoston nimi
    private const string dbName = "TestDatabase.db";
    // Dictionary, jota käytetään C# muuttujatyyppien vaihtamiseen SQL-muotoon
    private static Dictionary<Type, string> TypeToSQL = new Dictionary<Type, string>()
    {
        {typeof(string), "VARCHAR"},
        {typeof(int), "INT" },
        {typeof(float), "FLOAT" }
    };

    private static SQLiteDataReader ExecuteReader(string cmd)
    {
        dbCommand.CommandText = cmd;
        // Lähetetään käsky tietokantaan. Jos tulee virhe, niin näytetään
        // se Unityn konsolissa.
        try
        {
            return dbCommand.ExecuteReader();
        }
        catch (SQLiteException error)
        {
            Debug.Log(error);
            return null;
        }
    }

    // Funktio, joilla lähetään tietokantaan käskyjä
    private static void ExecuteCommand(string cmd)
    {
        // Tallennettaan dbCommand muuttujaan käsky
        dbCommand.CommandText = cmd;
        // Lähetetään käsky tietokantaan. Jos tulee virhe, niin näytetään
        // se Unityn konsolissa.
        try
        {
            dbCommand.ExecuteNonQuery();
        }
        catch (SQLiteException error)
        {
            Debug.Log(error);
        }
    }

    public static void CloseDatabase() => dbConnection.Close(); 

    public static void UpdateTable<T>(string setText, string whereText, string name = "")
    {
        string tableName = name == "" ? typeof(T).ToString() : name;
        FieldInfo[] tableFields = typeof(T).GetFields();
        string cmd = $"UPDATE {tableName} SET {setText} WHERE {whereText}";
        ExecuteCommand(cmd);
    }
    
    /// <summary>
    /// Ottaa sisään kokonaisen SQL -komennon 
    /// </summary>
    public static void ExecutePureSQL(string cmd) => ExecuteCommand(cmd);

    public static void DropTable<T>(string name = "")
    {
        string tableName = name == "" ? typeof(T).ToString() : name;
        string cmd = $"DROP TABLE {tableName}";
        ExecuteCommand(cmd);
    }

    public static List<T> ReadAllDataFromTable<T>(string name = "") where T : new()
    {
        string tableName = name == "" ? typeof(T).ToString() : name;
        FieldInfo[] tableFields = typeof(T).GetFields();
        // Aloitetaan SQL haku hakemalla taulusta kaikki siihen kuuluvat tiedot
        string cmd = $"SELECT * FROM {tableName}";
        // Tehdään SQLiteDataReader -tyyppinen muuttuja, johon haetaan kaikki
        // taulusta löytyneet tiedot
        SQLiteDataReader reader = ExecuteReader(cmd);
        // Tehdään lista sitä varten, että voidaan muuttaa SQL -tiedot C# -muotoon.
        List<T> data = new List<T>();
        // Käydään reader -muuttujaa läpi tietuerivi kerrallaan.
        while(reader.Read())
        {
            // Lisätään joka rivi kohdalla data listaan uusi rivi.
            data.Add(new T());
            // Määritetään indeksi jonka kohdalle data listassa tieto lisätään
            int index = data.Count - 1;
            // Käydään jokainen luokkaan kuuluva muuttujanimi läpi
            for (int i = 0; i < tableFields.Length; i++)
            {
                // Käydään tietueen jokainen sarake läpi, ja lisätään ne "data" -listaan.
                typeof(T).GetField(tableFields[i].Name).SetValue(data[index], reader.GetValue(i));
            }
        }
        reader.Close();
        return data;
    }

    public static void InsertIntoTable<T>(T data, string name = "")
    {
        // Käytetään geneerisen tyypin nimeä taulun nimeämiseksi,
        // jollei määritetä taululle omaa nimeä.
        string tableName = name == "" ? typeof(T).ToString() : name;
        // Haetaan tyypin luokasta kaikki siihen kuuluvat "public"
        // tyyliset muuttujat. HighScore luokan tapauksessa nämä
        // olisivat name, score, date.
        FieldInfo[] tableFields = typeof(T).GetFields();
        // Aloitetaan SQL-komennon luonti INSERT INTO lausekkeella.
        string cmd = $"INSERT INTO {tableName} (";
        // Käydään geneerisen tyypin muuttujanimet läpi ja lisätään
        // ne SQL-komentoon.
        for (int i = 1; i < tableFields.Length; i++)
        {
            if (i > 1) cmd += ", ";
            cmd += $"{tableFields[i].Name}";
        }
        cmd += ") VALUES (";

        for (int i = 1; i < tableFields.Length; i++)
        {
            if (i > 1) cmd += ", ";
            // Haetaan reflektioiden avulla arvot data -muuttujasta. Reflektioita
            // joudutaan käyttämään, koska ohjelmamme ei etukäteen tiedä
            // minkätyyppinen objekti on kyseessä (geneerinen tyyppi).
            cmd += $"'{typeof(T).GetField(tableFields[i].Name).GetValue(data)}'";
        }
        cmd += ")";
        ExecuteCommand(cmd);
    }
    private static bool CheckForId(FieldInfo id)
    {
        if (id.Name != "id")
        {
            Debug.Log("The first member of the class has to be ID");
        }
        return id.Name == "id";
    }
    // Funktio, joilla luodaan uusia tauluja tietokantaan. Käytetään 
    // geneeristä tyyppiä <T>, jotta voidaan käyttää samaa funktiota
    // monenlaisten taulujen luontiin.
    public static void CreateTable<T>(string name = "")
    {
        // Käytetään geneerisen tyypin nimeä taulun nimeämiseksi,
        // jollei määritetä taululle omaa nimeä.
        string tableName = name == "" ? typeof(T).ToString() : name;
        // Haetaan tyypin luokasta kaikki siihen kuuluvat "public"
        // tyyliset muuttujat. HighScore luokan tapauksessa nämä
        // olisivat name, score, date.
        FieldInfo[] tableFields = typeof(T).GetFields();
        if (!CheckForId(tableFields[0])) return;
        
        // Lisätään SQL-komennon alku.
        string cmd = $"CREATE TABLE IF NOT EXISTS {tableName} (ID INTEGER PRIMARY KEY AUTOINCREMENT, ";
        // Käydään for -loopilla läpi kaikki luokkaan kuuluvat muuttujat.
        for (int i = 1; i < tableFields.Length; i++)
        {
            if (i > 1) cmd += ", ";
            cmd += $"{tableFields[i].Name} {TypeToSQL[tableFields[i].FieldType]}";
        
        }
        cmd += ")";
        Debug.Log(cmd);
        ExecuteCommand(cmd);
    }

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
