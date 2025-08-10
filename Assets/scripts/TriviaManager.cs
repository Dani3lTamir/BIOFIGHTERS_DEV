using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro components
using System.Collections;
using System.IO;
using System;
using UnityEngine.Networking;



[System.Serializable]
public class TriviaQuestion
{
    public string questionText;
    public string[] options;
    public int correctAnswerIndex;
}

[System.Serializable]
public class TriviaQuestionList
{
    public TriviaQuestion[] questions;
}



public class TriviaManager : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour powerUpManagerObject; // Assign any MonoBehaviour implementing IPowerUpManager in the Inspector
    private IPowerUpManager powerUpManager;
    public string jsonFileName = "TriviaQuestions.json"; // Name of the JSON file
    public TriviaQuestion[] triviaQuestions; // Array of trivia questions
    public GameObject triviaUI; // UI panel for the trivia question
    public TextMeshProUGUI questionText; // TextMeshPro field for the question
    public Button[] optionButtons; // Buttons for the options
    public TextMeshProUGUI timerText; // TextMeshPro field to show the time left
    public float triviaInterval = 30f; // Interval between trivia questions (in seconds)
    public float answerTimeLimit = 10f; // Time limit to answer the trivia (in seconds)
    private TriviaQuestion currentQuestion;
    private float currentAnswerTimeLeft;
    private Coroutine countdownCoroutine; // To store the countdown timer coroutine

    private AudioManager audioManager; // Reference to the AudioManager

    private void Awake()
    {
        // Ensure the assigned MonoBehaviour implements the IPowerUpManager interface
        if (powerUpManagerObject is IPowerUpManager manager)
        {
            powerUpManager = manager;
        }
        else
        {
            Debug.LogError("Assigned object does not implement IPowerUpManager!");
        }
    }

    private void Start()
    {
        audioManager = AudioManager.Instance; // Get the AudioManager instance
        triviaUI.SetActive(false); // Hide trivia UI at start
        StartCoroutine(LoadTriviaQuestions());
        InvokeRepeating("ShowRandomTrivia", triviaInterval, triviaInterval); // Show trivia every 'triviaInterval' seconds
    }

    public IEnumerator LoadTriviaQuestions()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        string json = string.Empty;

        // WebGL loading path
#if UNITY_WEBGL && !UNITY_EDITOR
        using (UnityWebRequest request = UnityWebRequest.Get(filePath))
        {
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"WebGL load failed: {request.error}");
                yield break;
            }
            
            json = request.downloadHandler.text;
        }
#else
        // Standalone/Editor path
        if (File.Exists(filePath))
        {
            json = File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError($"File not found at: {filePath}");
            yield break;
        }
#endif

        try
        {
            triviaQuestions = JsonUtility.FromJson<TriviaQuestionList>(json).questions;
            Debug.Log($"Loaded {triviaQuestions.Length} questions");
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON parse error: {e.Message}");
        }
    }

    public void ShowRandomTrivia()
    {
        Time.timeScale = 0; // Pause the game
        triviaUI.SetActive(true); // Show the trivia UI
        audioManager.Play("Clock"); 

        currentQuestion = triviaQuestions[UnityEngine.Random.Range(0, triviaQuestions.Length)];
        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < currentQuestion.options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.options[i];

                int index = i; // Capture the index for the button callback
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => AnswerTrivia(index));
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }

        // Start the countdown timer
        currentAnswerTimeLeft = answerTimeLimit;
        countdownCoroutine = StartCoroutine(CountdownTimer());
    }

    private IEnumerator CountdownTimer()
    {
        while (currentAnswerTimeLeft > 0)
        {
            timerText.text = Mathf.Ceil(currentAnswerTimeLeft).ToString() + "s"; // Update the timer UI
            currentAnswerTimeLeft -= Time.unscaledDeltaTime; // Use unscaledDeltaTime to keep the timer running even when the game is paused
            yield return null;
        }

        // If the time runs out, treat it as a wrong answer
        AnswerTrivia(-1); // Pass an invalid index to trigger wrong answer logic
    }

    public void AnswerTrivia(int selectedIndex)
    {
        // Stop the countdown timer coroutine if it's running
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        // If the player selected the correct answer
        if (selectedIndex == currentQuestion.correctAnswerIndex)
        {
            Debug.Log("Correct Answer!");
            // Play correct answer sound
            audioManager.Play("CorrectAnswer"); // Play correct answer sound
            ScoreManager.Instance.UpdateScoreForObject("RightAnswer");//add points for right answer
            powerUpManager.ActivateRandomPowerUp(); // Activate a random power-up
        }
        else
        {
            audioManager.Play("WrongAnswer"); // Play wrong answer sound
            Debug.Log("Wrong Answer!");
        }
        audioManager.Stop("Clock"); // Stop the clock sound

        // Hide trivia UI and resume the game
        Time.timeScale = 1; // Resume the game
        triviaUI.SetActive(false); // Hide the trivia UI
    }
}
