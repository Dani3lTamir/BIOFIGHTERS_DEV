using UnityEngine;

public class DCDifficultyManager : BaseDifficultyManager
{
    public const string LEVEL_TYPE = "DC";
    protected override string LevelType => LEVEL_TYPE;

    [System.Serializable]
    public class DifficultySettings
    {
        public int triesLeft;
        public float time;
        public float scoreMulti;
        public bool hideAntiBodyOnExit;
        public int numberOfTCells;
    }

    [Header("DC Difficulty Presets")]
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

        GameCountManager.Instance.SetCounterValue("AttemptsLeft", settings.triesLeft);
        Timer timer = GameObject.FindWithTag("LevelTimer").GetComponent<Timer>();
        timer.countdownTime = settings.time;
        ScoreManager.Instance.scoreMultiplier = settings.scoreMulti;

        SleepingTCellSpawner spawner = GameObject.FindFirstObjectByType<SleepingTCellSpawner>();
        spawner.isHideAntiBodyOnExit = settings.hideAntiBodyOnExit;
        spawner.numberOfTCells = settings.numberOfTCells;
    }
}