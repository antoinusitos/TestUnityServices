using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreText : MonoBehaviour
{
    public Text playerName = null;
    public Text playerKills = null;
    public Text playerDeaths = null;

    public void UpdateTexts(string name_In, int kills_In, int deaths_In)
    {
        playerName.text = name_In;
        playerKills.text = kills_In.ToString();
        playerDeaths.text = deaths_In.ToString();
    }
}
