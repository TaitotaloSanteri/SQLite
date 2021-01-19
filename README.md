# SQLite
SQLiten käyttö Unityn kanssa.

Kopioi Assets/Plugins/ kansiosta löytyvät kaksi .DLL tiedostoa oman projektisi vastaavaan kansioon.
Kopioi DataManager.cs tiedosto omaan projektiisi Assets kansioon

**Luo uusi taulukko:**

`DataManager.CreateTable<OmaLuokka>();`

**Lisää taulukkoon tietoa:**

`DataManager.InsertIntoTable<OmaLuokka>(data);`
(data -parametri on tässä tapauksess OmaLuokka -kuuluva objekti

**Hae taulukosta kaikki tiedot**:

`DataManager.ReadAllDataFromTable<OmaLuokka>();`

Korvaa **OmaLuokka** sana yllä olevissa esimerkeissä omalla luokallasi, esim:

`DataManager.CreateTable<HighScore>();`

```
public class HighScore{
  public string name; 
  public int score; 
  public string date; 
}


