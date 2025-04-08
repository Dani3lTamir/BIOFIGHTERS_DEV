using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntryButton : MonoBehaviour
{
    [SerializeField] Image entryIcon;
    [SerializeField] TMP_Text entryNameText;

    public void Initialize(MicroPediaEntry entry, MicroPediaUI ui, bool isUnlocked)
    {
        entryIcon.sprite = entry.iconSprite;
        entryNameText.text = isUnlocked ? entry.displayName : "???";

        if (isUnlocked)
        {
            GetComponent<Button>().onClick.AddListener(() => ui.ShowEntryDetails(entry));
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }
}