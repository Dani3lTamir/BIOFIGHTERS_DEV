using UnityEngine;
using System.Collections.Generic;

public class MicroPediaDatabase : MonoBehaviour
{
    public static MicroPediaDatabase Instance;

    public List<MicroPediaEntry> allEntries = new List<MicroPediaEntry>();
    private Dictionary<string, MicroPediaEntry> _entryDictionary = new Dictionary<string, MicroPediaEntry>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeDictionary()
    {
        foreach (var entry in allEntries)
        {
            _entryDictionary.Add(entry.entryID, entry);
        }
    }

    public List<MicroPediaEntry> GetEntriesByCategory(EntryCategory category)
    {
        return allEntries.FindAll(e => e.category == category);
    }

    public void UnlockEntry(string entryID)
    {
        if (_entryDictionary.TryGetValue(entryID, out MicroPediaEntry entry))
        {
            entry.unlocked = true;
            PlayerPrefs.SetInt("Unlocked_" + entryID, 1);
        }
    }

    public void LoadUnlockedStates()
    {
        foreach (var entry in allEntries)
        {
            entry.unlocked = PlayerPrefs.GetInt("Unlocked_" + entry.entryID, 0) == 1;
        }
    }
}