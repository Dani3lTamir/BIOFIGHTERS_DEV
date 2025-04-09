using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTutorialSequence", menuName = "Tutorials/Tutorial Sequence")]
public class TutorialSequence : ScriptableObject
{
    public List<TutorialStep> steps = new List<TutorialStep>();
}