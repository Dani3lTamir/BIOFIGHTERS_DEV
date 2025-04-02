using UnityEngine;
using TMPro;


public class MainDifficultyManager : MonoBehaviour, IDifficultyManager
{
    public const string LEVEL_TYPE = "Main";
    public TextMeshProUGUI difficultyText;

    [System.Serializable]
    public class DifficultySettings
    {
        public float time;
        public float scoreMulti;
        public float rewardMulti;
        public float bossHealthMulti;
        public float bossDamageMulti;
    }

    [Header("Main Difficulty Presets")]
    public DifficultySettings easy;
    public DifficultySettings medium;
    public DifficultySettings hard;

    public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Medium;

    [Header("Difficulty Thresholds")]
    public int easyToMedium = 1;
    public int mediumToEasy = 1;
    public int mediumToHard = 5;
    public int hardToMedium = 1;


    private int _failures;
    private int _successes;

    void Start()
    {
        // Fallback initialization if scene loads before LevelManager
        if (!LevelManager.Instance)
        {
            InitializeLevel();
        }
        difficultyText.text = CurrentDifficulty.ToString();

    }


    public void InitializeLevel()
    {
        _failures = PlayerPrefs.GetInt($"Failures_{LEVEL_TYPE}", 0);
        _successes = PlayerPrefs.GetInt($"Successes_{LEVEL_TYPE}", 0);
        CurrentDifficulty = (Difficulty)PlayerPrefs.GetInt($"CurrentDifficulty_{LEVEL_TYPE}", (int)Difficulty.Medium);
        ApplySettings();
        Debug.Log($"Current difficulty: {CurrentDifficulty}");
    }

    public void RecordFailure()
    {
        _failures++;
        PlayerPrefs.SetInt($"Failures_{LEVEL_TYPE}", _failures);

        // Difficulty adjustment rules
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

    public void RecordSuccess()
    {
        _successes++;
        PlayerPrefs.SetInt($"Successes_{LEVEL_TYPE}", _successes);

        // Difficulty adjustment rules
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

    private void SetDifficulty(Difficulty newDifficulty)
    {
        CurrentDifficulty = newDifficulty;
        PlayerPrefs.SetInt($"CurrentDifficulty_{LEVEL_TYPE}", (int)newDifficulty);
        difficultyText.text = CurrentDifficulty.ToString();
        ResetCounters();
    }

    private void ResetCounters()
    {
        _failures = 0;
        _successes = 0;
        PlayerPrefs.SetInt($"Failures_{LEVEL_TYPE}", 0);
        PlayerPrefs.SetInt($"Successes_{LEVEL_TYPE}", 0);
    }

    private void ApplySettings()
    {
        var settings = CurrentDifficulty switch
        {
            Difficulty.Easy => easy,
            Difficulty.Medium => medium,
            _ => hard
        };
        // Apply ALL level-specific parameters
        Debug.Log("Applying settings for " + CurrentDifficulty);
        Timer timer = GameObject.FindWithTag("LevelTimer").GetComponent<Timer>();
        timer.countdownTime = settings.time;
        ScoreManager.Instance.scoreMultiplier = settings.scoreMulti;
        RewardSystem.Instance.rewardMultiplier = settings.rewardMulti;
        // Find Boss Spawner
        BossSpawner bossSpawner = FindObjectOfType<BossSpawner>(true);
        if (bossSpawner == null)
        {
            Debug.LogError("BossSpawner not found in scene!");
            return;
        }
        bossSpawner.healthMultiplier = settings.bossHealthMulti;
        bossSpawner.damageMultiplier = settings.bossDamageMulti;
    }
}