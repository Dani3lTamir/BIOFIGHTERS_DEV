using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro components
using System.Collections;
using System.IO;

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
        triviaUI.SetActive(false); // Hide trivia UI at start
        LoadTriviaQuestions();
        InvokeRepeating("ShowRandomTrivia", triviaInterval, triviaInterval); // Show trivia every 'triviaInterval' seconds
    }

    public void LoadTriviaQuestions()
    {
        // Read the JSON file
        string filePath = Path.Combine(  Application.streamingAssetsPath, jsonFileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            // Deserialize the JSON into a TriviaQuestionList object
            TriviaQuestionList questionList = JsonUtility.FromJson<TriviaQuestionList>(json);
            triviaQuestions = questionList.questions; // Assign the questions to the triviaQuestions array
        }
        else
        {
            Debug.LogError("Trivia questions file not found at " + filePath);
        }
    }


    public void ShowRandomTrivia()
    {
        Time.timeScale = 0; // Pause the game
        triviaUI.SetActive(true); // Show the trivia UI

        currentQuestion = triviaQuestions[Random.Range(0, triviaQuestions.Length)];
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
            ScoreManager.Instance.UpdateScoreForObject("RightAnswer");//add points for right answer
            powerUpManager.ActivateRandomPowerUp(); // Activate a random power-up
        }
        else
        {
            Debug.Log("Wrong Answer!");
        }

        // Hide trivia UI and resume the game
        Time.timeScale = 1; // Resume the game
        triviaUI.SetActive(false); // Hide the trivia UI
    }
}
