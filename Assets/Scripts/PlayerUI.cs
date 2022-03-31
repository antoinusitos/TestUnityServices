using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text timeTime = null;
    public float time = 0;

    private void Update()
    {
        time -= Time.deltaTime;
        float min = time / 60;
        min = Mathf.Floor(min);
        float sec = time - (min * 60);
        sec = Mathf.Floor(sec);
        timeTime.text = $"Time Left :   {min}:{sec}";
    }
}
