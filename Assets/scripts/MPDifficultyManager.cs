using UnityEngine;

public class MPDifficultyManager : BaseDifficultyManager
{
    public const string LEVEL_TYPE = "MP";
    protected override string LevelType => LEVEL_TYPE;

    [System.Serializable]
    public class DifficultySettings
    {
        public float enemySpeed;
        public int alliesLeft;
        public float time;
        public float scoreMulti;
    }

    [Header("MP Difficulty Presets")]
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

        EnemyPool[] allPools = FindObjectsByType<EnemyPool>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        foreach (EnemyPool enemyPool in allPools)
        {
            enemyPool.ChangeEnemiesSpeed(settings.enemySpeed);
        }

        GameCountManager.Instance.SetCounterValue("AlliesLeft", settings.alliesLeft);

        Timer timer = GameObject.FindWithTag("LevelTimer").GetComponent<Timer>();
        timer.countdownTime = settings.time;

        ScoreManager.Instance.scoreMultiplier = settings.scoreMulti;
    }
}