using UnityEngine;

public interface IDifficultyManager
{
    Difficulty CurrentDifficulty { get; }
    void InitializeLevel();
    void RecordFailure();
    void RecordSuccess(); 
}

