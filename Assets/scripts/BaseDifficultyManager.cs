using UnityEngine;
using TMPro;

public abstract class BaseDifficultyManager : MonoBehaviour, IDifficultyManager
{
    public TextMeshProUGUI difficultyText;

    [Header("Difficulty Thresholds")]
    public int easyToMedium = 1;
    public int mediumToEasy = 1;
    public int mediumToHard = 1;
    public int hardToMedium = 1;

    public Difficulty CurrentDifficulty { get; protected set; } = Difficulty.Medium;
    protected abstract string LevelType { get; }

    protected int _failures;
    protected int _successes;

    protected virtual void Start()
    {
        if (!LevelManager.Instance)
        {
            InitializeLevel();
        }
        UpdateDifficultyText();
    }

    public virtual void InitializeLevel()
    {
        _failures = PlayerPrefs.GetInt($"Failures_{LevelType}", 0);
        _successes = PlayerPrefs.GetInt($"Successes_{LevelType}", 0);
        CurrentDifficulty = (Difficulty)PlayerPrefs.GetInt($"CurrentDifficulty_{LevelType}", (int)Difficulty.Medium);
        ApplySettings();
        Debug.Log($"Current difficulty: {CurrentDifficulty}");
    }

    public virtual void RecordFailure()
    {
        _failures++;
        PlayerPrefs.SetInt($"Failures_{LevelType}", _failures);

        switch (CurrentDifficulty)
        {
            case Difficulty.Hard when _failures >= hardToMedium:
                SetDifficulty(Difficulty.Medium);
                break;
            case Difficulty.Medium when _failures >= mediumToEasy:
                SetDifficulty(Difficulty.Easy);
                break;
        }

        ApplySettings();
    }

    public virtual void RecordSuccess()
    {
        _successes++;
        PlayerPrefs.SetInt($"Successes_{LevelType}", _successes);

        switch (CurrentDifficulty)
        {
            case Difficulty.Easy when _failures < easyToMedium:
                SetDifficulty(Difficulty.Medium);
                break;
            case Difficulty.Medium when _failures <= mediumToHard:
                SetDifficulty(Difficulty.Hard);
                break;
        }

        ResetCounters();
        ApplySettings();
    }

    protected virtual void SetDifficulty(Difficulty newDifficulty)
    {
        CurrentDifficulty = newDifficulty;
        PlayerPrefs.SetInt($"CurrentDifficulty_{LevelType}", (int)newDifficulty);
        UpdateDifficultyText();
        ResetCounters();
    }

    protected void ResetCounters()
    {
        _failures = 0;
        _successes = 0;
        PlayerPrefs.SetInt($"Failures_{LevelType}", 0);
        PlayerPrefs.SetInt($"Successes_{LevelType}", 0);
    }

    protected void UpdateDifficultyText()
    {
        if (difficultyText != null)
        {
            difficultyText.text = CurrentDifficulty.ToString();
        }
    }

    protected abstract void ApplySettings();
}