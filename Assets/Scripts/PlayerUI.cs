using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text timeTime = null;
    public float time = 0;

    public Text gameModeText = null;

    public Text ammoText = null;

    public Slider lifeSlider = null;
    public Text lifeText = null;

    public GameObject scorePanel = null;
    public Transform panelLobbyPlayer = null;
    public Transform scoreTextPrefab = null;

    private void Update()
    {
        time -= Time.deltaTime;
        float min = time / 60;
        min = Mathf.Floor(min);
        float sec = time - (min * 60);
        sec = Mathf.Floor(sec);
        timeTime.text = $"Time Left :   {min}:{sec}";
    }

    public void ShowScore(bool newState)
    {
        scorePanel.SetActive(newState);
    }

    public void UpdateMagazineSize(int current, int total)
    {
        ammoText.text = current + " / " + total;
    }

    public void UpdateLife(float life, float maxLife)
    {
        lifeText.text = life.ToString();
        lifeSlider.value = life / maxLife;
    }

    public void SetGameMode(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.FFA:
                gameModeText.text = "Free For All";
                break;
        }
    }

    public void UpdatePlayerList(PlayerState[] playerStates)
    {
        if (playerStates.Length != panelLobbyPlayer.childCount)
        {
            for (int i = 0; i < panelLobbyPlayer.childCount; i++)
            {
                Destroy(panelLobbyPlayer.GetChild(i));
            }

            for (int i = 0; i < playerStates.Length; i++)
            {
                Instantiate(scoreTextPrefab, panelLobbyPlayer);
            }
        }

        for (int i = 0; i < playerStates.Length; i++)
        {
            panelLobbyPlayer.GetChild(i).GetComponent<PlayerScoreText>().UpdateTexts(playerStates[i].playerName, playerStates[i].kills, playerStates[i].deaths);
        }
    }
}
