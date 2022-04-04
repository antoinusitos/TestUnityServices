using UnityEngine;

public class HandleAnimationSound : MonoBehaviour
{
    public AudioSource stepSound = null;

    public void Step()
    {
        if (stepSound)
            stepSound.Play();
    }
}
