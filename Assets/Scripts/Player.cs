using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private bool isInPause = false;
    public GameObject pauseMenu = null;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause(!isInPause);
        }
    }

    public void SetPause(bool state)
    {
        isInPause = state;
        pauseMenu.SetActive(isInPause);
        if (isInPause)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public bool GetIsInPause()
    {
        return isInPause;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }
}
