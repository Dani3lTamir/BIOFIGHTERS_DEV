using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using TMPro;

public class TimerPlayModeTests
{
    // Mock FloatingTextManager to prevent NullReferenceException
    private class MockFloatingTextManager : MonoBehaviour
    {
        public static MockFloatingTextManager Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        public void ShowFloatingText(string text, RectTransform transform, Color color, bool flag) { }
    }

    private GameObject CreateTestTimerObject(float countdownTime = 10f)
    {
        // Create mock FloatingTextManager
        var floatingTextManagerObj = new GameObject("FloatingTextManager");
        floatingTextManagerObj.AddComponent<MockFloatingTextManager>();

        // Create game objects
        var timerObject = new GameObject("TimerTest");
        var timerComponent = timerObject.AddComponent<Timer>();

        // Create a text object for the timer
        var textObject = new GameObject("TimerText");
        var tmpText = textObject.AddComponent<TextMeshProUGUI>();
        timerComponent.timerText = tmpText;

        // Set initial timer properties
        timerComponent.countdownTime = countdownTime;

        return timerObject;
    }

    [UnityTest]
    public IEnumerator Timer_InitializesCorrectly()
    {
        // Arrange
        var timerObject = CreateTestTimerObject();
        var timer = timerObject.GetComponent<Timer>();

        // Wait a frame to ensure Start() is called
        yield return null;

        // Assert
        Assert.AreEqual(timer.countdownTime, timer.GetTimeRemaining(), 0.1f, "Timer should start with initial countdown time");
        Assert.IsFalse(timer.isTimeUp, "Timer should not be marked as time up initially");
    }


    [UnityTest]
    public IEnumerator Timer_ReachesZero()
    {
        // Arrange
        var timerObject = CreateTestTimerObject(2f);
        var timer = timerObject.GetComponent<Timer>();

        // Wait longer than the timer duration
        yield return new WaitForSeconds(3f);

        // Assert
        Assert.IsTrue(timer.isTimeUp, "Timer should be marked as time up");
        Assert.AreEqual(0f, timer.GetTimeRemaining(), 0.1f, "Timer should reach zero");
    }

    [UnityTest]
    public IEnumerator Timer_FormatsTimeCorrectly()
    {
        // Arrange
        var timerObject = CreateTestTimerObject(125f);
        var timer = timerObject.GetComponent<Timer>();

        // Wait a frame to ensure initialization
        yield return null;

        // Assert formatting
        string formattedTime = timer.FormatTime(125f);
        Assert.AreEqual("02:05", formattedTime, "Timer should format time correctly");
    }


    // Clean up after tests
    [TearDown]
    public void Cleanup()
    {
        var timers = GameObject.FindObjectsOfType<Timer>();
        foreach (var timer in timers)
        {
            Object.Destroy(timer.gameObject);
        }

        var floatingTextManagers = GameObject.FindObjectsOfType<MockFloatingTextManager>();
        foreach (var manager in floatingTextManagers)
        {
            Object.Destroy(manager.gameObject);
        }
    }
}