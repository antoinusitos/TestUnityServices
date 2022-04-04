using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private string playerNameValue = "";

    public GameObject playerNameMenu = null;
    public GameObject mainMenu = null;
    public GameObject lobbyMenu = null;

    public InputField playerNameInputField = null;
    public Button playerNameValidateButton = null;

    public GameObject gameManager = null;
    public Text playerNameLabel = null;

    public AudioSource buttonClick = null;

    private void Start()
    {
        if(PlayerPrefs.HasKey("PlayerName"))
        {
            playerNameValue = PlayerPrefs.GetString("PlayerName");
            GoToMainMenu();
        }
        else
        {
            GoToPlayerNameMenu();
        }
    }

    public void GoToPlayerNameMenu()
    {
        playerNameMenu.SetActive(true);
        mainMenu.SetActive(false);
        lobbyMenu.SetActive(false);

        playerNameInputField.text = "";
        playerNameValidateButton.interactable = false;
    }

    public void GoToMainMenu()
    {
        playerNameMenu.SetActive(false);
        mainMenu.SetActive(true);
        lobbyMenu.SetActive(false);

        playerNameLabel.text = "Welcome " + PlayerPrefs.GetString("PlayerName") + " !";
    }

    public void GoToLobbyMenu()
    {
        playerNameMenu.SetActive(false);
        mainMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }

    public void SavePlayerName()
    {
        playerNameValue = playerNameInputField.text;
        PlayerPrefs.SetString("PlayerName", playerNameValue);
    }

    public void Play()
    {
        GoToLobbyMenu();
        gameManager.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayButtonClick()
    {
        if(buttonClick.isActiveAndEnabled)
            buttonClick.Play();
    }
}
