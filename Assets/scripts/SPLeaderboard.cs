using UnityEngine;
using TMPro;
using Dan.Main;
using Dan.Models;


public class SPLeaderboard : MonoBehaviour
{
    [Header("Leaderboard Settings")]
    [SerializeField] private string publicLeaderboardKey = "dab58b11037ca090b699eb46c7fc12eccfc05f8f4e6455a070ed4f2faaaeb717";

    [Header("UI References")]
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Transform entryParent;

    void Start()
    {
        GetLeaderboard();
    }

    public void SubmitScore()
    {
        if (string.IsNullOrEmpty(usernameInputField.text))
        {
            Debug.LogError("Username cannot be empty.");
            return;
        }

        SetLeaderboardEntry(usernameInputField.text);
    }

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, OnEntriesLoaded);
    }

    public void SetLeaderboardEntry(string username)
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogError("LevelManager instance is null.");
            return;
        }

        LeaderboardCreator.UploadNewEntry(
            publicLeaderboardKey,
            username,
            LevelManager.Instance.GetTotalScore(),
            OnScoreUploadComplete
        );
    }

    private void OnScoreUploadComplete(bool success)
    {
        if (success)
        {
            GetLeaderboard();
        }
        else
        {
            Debug.LogError("Score upload failed.");
        }
    }

    private void OnEntriesLoaded(Entry[] entries)
    {
        ClearLeaderboard();

        if (entries == null || entries.Length == 0)
        {
            Debug.Log("No leaderboard entries found.");
            return;
        }

        foreach (Entry entry in entries)
        {
            CreateLeaderboardEntry(entry);
        }
    }

    private void ClearLeaderboard()
    {
        foreach (Transform child in entryParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateLeaderboardEntry(Entry entry)
    {
        GameObject entryObj = Instantiate(entryPrefab, entryParent);
        var entryUI = entryObj.GetComponent<ScoreEntryUI>();

        if (entryUI == null)
        {
            Debug.LogError("Entry prefab missing EntryUI component!");
            return;
        }

        // Set basic entry info
        entryUI.nameText.text = $"{entry.Rank}. {entry.Username}";
        entryUI.scoreText.text = entry.Score.ToString("N0");

        // Apply highlighting
        if (entry.IsMine())
        {
            HighlightEntry(entryUI, Color.green);
        }
        else if (entry.Rank <= 3)
        {
            HighlightEntry(entryUI, Color.yellow);
            entryUI.nameText.fontStyle = FontStyles.Bold;
        }
    }

    private void HighlightEntry(ScoreEntryUI entry, Color color)
    {
        entry.nameText.color = color;
        entry.scoreText.color = color;
    }
}

