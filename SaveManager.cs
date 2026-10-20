using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public Transform player;
    public bool loadOnStart = true;

    private const string Prefix = "StreetLife_";

    private void Start()
    {
        if (loadOnStart) LoadGame();
    }

    public void SaveGame()
    {
        if (player != null)
        {
            Vector3 p = player.position;
            PlayerPrefs.SetFloat(Prefix + "PlayerX", p.x);
            PlayerPrefs.SetFloat(Prefix + "PlayerY", p.y);
            PlayerPrefs.SetFloat(Prefix + "PlayerZ", p.z);
            PlayerPrefs.SetFloat(Prefix + "PlayerYaw", player.eulerAngles.y);
        }

        PlayerPrefs.SetInt(Prefix + "HasSave", 1);
        PlayerPrefs.Save();
        Debug.Log("StreetLife progress saved.");
    }

    public void LoadGame()
    {
        if (player == null || PlayerPrefs.GetInt(Prefix + "HasSave", 0) == 0) return;

        Vector3 p = new Vector3(
            PlayerPrefs.GetFloat(Prefix + "PlayerX", 0),
            PlayerPrefs.GetFloat(Prefix + "PlayerY", 1),
            PlayerPrefs.GetFloat(Prefix + "PlayerZ", 0));

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.position = p;
        player.rotation = Quaternion.Euler(0, PlayerPrefs.GetFloat(Prefix + "PlayerYaw", 0), 0);
        if (cc != null) cc.enabled = true;

        Debug.Log("StreetLife progress loaded.");
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteKey(Prefix + "PlayerX");
        PlayerPrefs.DeleteKey(Prefix + "PlayerY");
        PlayerPrefs.DeleteKey(Prefix + "PlayerZ");
        PlayerPrefs.DeleteKey(Prefix + "PlayerYaw");
        PlayerPrefs.DeleteKey(Prefix + "HasSave");
        PlayerPrefs.Save();
    }
}
