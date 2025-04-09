using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public Image tutorialImage;
    public Button continueButton;
    public Button exitButton;
    public TMP_Text continueButtonText;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public string defaultContinueText = "המשך";
    public string finalContinueText = "קיבלתי!";
    public bool isTutorialActive = false;
    private List<TutorialStep> tutorialSequence = new List<TutorialStep>();
    private int currentStepIndex = -1;
    private bool isTyping = false;
    private string fullMessage;
    private Coroutine typingCoroutine;
    private float previousTimeScale;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        continueButton.onClick.AddListener(ContinueTutorial);
        exitButton.onClick.AddListener(HideTutorial);
    }

    public void StartTutorialSequence(List<TutorialStep> steps)
    {
        if (steps == null || steps.Count == 0)
        {
            Debug.LogWarning("No tutorial steps provided!");
            return;
        }
        isTutorialActive = true;
        PauseGame();
        tutorialSequence = steps;
        currentStepIndex = -1;
        ShowNextStep();
    }

    public void ShowNextStep()
    {
        currentStepIndex++;

        if (currentStepIndex >= tutorialSequence.Count)
        {
            HideTutorial();
            return;
        }

        TutorialStep currentStep = tutorialSequence[currentStepIndex];
        tutorialPanel.SetActive(true);
        tutorialImage.sprite = currentStep.image;
        fullMessage = currentStep.message;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(currentStep.message));

        bool isLastStep = currentStepIndex == tutorialSequence.Count - 1;
        continueButtonText.text = isLastStep ? finalContinueText : defaultContinueText;
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        tutorialText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            tutorialText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    public void ContinueTutorial()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            tutorialText.text = fullMessage;
            isTyping = false;
            return;
        }
        ShowNextStep();
    }

    private void PauseGame()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = previousTimeScale;
    }

    public void HideTutorial()
    {
        isTutorialActive = false;
        tutorialPanel.SetActive(false);
        currentStepIndex = -1;
        ResumeGame();
    }


}