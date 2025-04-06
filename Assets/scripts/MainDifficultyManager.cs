using UnityEngine;
using TMPro;

public class MainDifficultyManager : BaseDifficultyManager
{
    public const string LEVEL_TYPE = "Main";
    protected override string LevelType => LEVEL_TYPE;

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

    protected override void ApplySettings()
    {
        var settings = CurrentDifficulty switch
        {
            Difficulty.Easy => easy,
            Difficulty.Medium => medium,
            _ => hard
        };

        // Apply timer settings
        Timer timer = GameObject.FindWithTag("LevelTimer").GetComponent<Timer>();
        timer.countdownTime = settings.time;

        // Apply score and reward multipliers
        ScoreManager.Instance.scoreMultiplier = settings.scoreMulti;
        RewardSystem.Instance.rewardMultiplier = settings.rewardMulti;

        // Apply boss settings
        BossSpawner bossSpawner = FindObjectOfType<BossSpawner>(true);
        if (bossSpawner == null)
        {
            Debug.LogError("BossSpawner not found in scene!");
            return;
        }
        bossSpawner.healthMultiplier = settings.bossHealthMulti;
        bossSpawner.damageMultiplier = settings.bossDamageMulti;

        Debug.Log($"Applied {CurrentDifficulty} settings");
    }

}