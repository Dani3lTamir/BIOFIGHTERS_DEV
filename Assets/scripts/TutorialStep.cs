using UnityEngine;

[System.Serializable]
public class TutorialStep
{
    [TextArea(3, 5)]
    public string message;
    public Sprite image;

    [Header("Optional")]
    public AudioClip soundEffect;
    public float autoAdvanceAfter = -1f; // -1 means manual advance
}
