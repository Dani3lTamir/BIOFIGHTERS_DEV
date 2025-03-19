using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton instance
    private Dictionary<string, int> objectScoreValues; //maps objects to their score value
    public int score = 0;
    public bool isDoublePoints = false;

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

    public int AddScore(int points)
    {
        if (points > 0 && isDoublePoints) points *= 2; //checks for double score power up
        if ((score + points) >= 0) score += points; //disable negative score
        else score = 0;
        return points;
    }

    public int GetObjectScore(string Tag)
    {
        return objectScoreValues.ContainsKey(Tag) ? objectScoreValues[Tag] : 0;
    }

    public void UpdateScoreForObject(string obj)
    {
        int value = GetObjectScore(obj);
        int points = AddScore(value);
        Debug.Log( obj +": " + points);
    }

    public int GetScore()
    {
        return score;
    }

    public void ActivateDoublePoints()
    {
        isDoublePoints = true;
    }

    public void DeactivateDoublePoints()
    {
        isDoublePoints = false;
    }



}
