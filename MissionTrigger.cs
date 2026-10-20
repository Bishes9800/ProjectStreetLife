using UnityEngine;
using UnityEngine.Events;

public class MissionTrigger : MonoBehaviour
{
    public string missionTitle = "First Delivery";
    [TextArea] public string description = "Reach the marker to complete your first mission.";
    public int rewardCash = 250;
    public bool destroyAfterComplete = true;
    public UnityEvent onMissionComplete;

    private bool completed;

    private void OnTriggerEnter(Collider other)
    {
        if (completed || !other.CompareTag("Player")) return;
        completed = true;

        int cash = PlayerPrefs.GetInt("StreetLife_Cash", 0);
        PlayerPrefs.SetInt("StreetLife_Cash", cash + rewardCash);
        PlayerPrefs.Save();

        Debug.Log($"Mission complete: {missionTitle}. Reward: ${rewardCash}. {description}");
        onMissionComplete?.Invoke();

        if (destroyAfterComplete) Destroy(gameObject);
    }
}
