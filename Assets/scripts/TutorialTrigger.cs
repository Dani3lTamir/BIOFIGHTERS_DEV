using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Tutorial Configuration")]
    public TutorialSequence tutorialSequence;
    public bool playOnEnable = true;
    public float startDelay = 5f;

    [Header("Completion Settings")]
    public bool onlyShowOnce = true;
    public string playerPrefKey = "";

    void OnEnable()
    {
        if (playOnEnable && ShouldShowTutorial())
        {
            StartCoroutine(StartTutorialWithDelay());
        }
    }

    IEnumerator StartTutorialWithDelay()
    {
        yield return new WaitForSecondsRealtime(startDelay);
        TutorialManager.Instance.StartTutorialSequence(tutorialSequence.steps);
    }

    bool ShouldShowTutorial()
    {
        if (!onlyShowOnce) return true;
        if (string.IsNullOrEmpty(playerPrefKey)) return true;
        return PlayerPrefs.GetInt(playerPrefKey, 0) == 0;
    }

    void MarkAsShown()
    {
        if (onlyShowOnce && !string.IsNullOrEmpty(playerPrefKey))
        {
            PlayerPrefs.SetInt(playerPrefKey, 1);
        }
    }

    public void StartTutorial()
    {
        if (ShouldShowTutorial())
        {
            TutorialManager.Instance.StartTutorialSequence(tutorialSequence.steps);
        }
    }
}