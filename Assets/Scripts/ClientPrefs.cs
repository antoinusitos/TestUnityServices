using UnityEngine;

public class ClientPrefs : MonoBehaviour
{
    public static string GetGuid()
    {
        if (PlayerPrefs.HasKey("client_guid"))
        {
            return PlayerPrefs.GetString("client_guid");
        }

        var guid = System.Guid.NewGuid();
        var guidString = guid.ToString();

        PlayerPrefs.SetString("client_guid", guidString);
        return guidString;
    }
}
