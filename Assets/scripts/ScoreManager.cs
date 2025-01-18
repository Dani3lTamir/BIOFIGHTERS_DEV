using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton instance
    private Dictionary<string, int> objectScoreValues; //maps objects to their score value
    public int score = 0;

    [System.Serializable]
    public class ObjectScore //to make scores available for change in the inspector
    {
        public string Tag;
        public int scoreValue;
    }

    public ObjectScore[] levelObjectScores;

    private void Awake()
    {
        // Ensure there's only one instance of the ScoreManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps the score manager across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        objectScoreValues = new Dictionary<string, int>();
        foreach (var objectScore in levelObjectScores)
        {
            objectScoreValues[objectScore.Tag] = objectScore.scoreValue;
        }

    }

    private void Start()
    {
    }

    public void AddScore(int points)
    {
        if ((score + points) >= 0) score += points; //disable negative score
        else score = 0;
    }

    public int GetObjectScore(string Tag)
    {
        return objectScoreValues.ContainsKey(Tag) ? objectScoreValues[Tag] : 0;
    }

    public int GetScore()
    {
        return score;
    }


}
