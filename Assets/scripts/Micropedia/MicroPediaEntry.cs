// MicroPediaEntry.cs
using UnityEngine;

[System.Serializable]
public class MicroPediaEntry
{
    public string entryID; // "ecoli", "macrophage" etc.
    public string displayName;
    [TextArea(3, 8)] public string description;
    public Sprite iconSprite;
    public EntryCategory category;
    public bool unlocked;
}

public enum EntryCategory
{
    Pathogens,
    ImmuneCells,
    Processes
}