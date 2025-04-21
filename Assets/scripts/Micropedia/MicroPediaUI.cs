using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MicroPediaUI : MonoBehaviour
{
    [Header("References")]
    public Transform categoryButtonsParent;
    public GameObject categoryButtonPrefab;
    public GameObject entriesView; // Main entries container
    public Transform entriesGrid;
    public GameObject entryButtonPrefab;
    public GameObject entryDetailPanel; // Dedicated detail view panel
    public Image entryIconDisplay;
    public TMP_Text entryNameText;
    public TMP_Text entryDescriptionText;

    [Header("Navigation")]
    [SerializeField] private Button backToCategoriesButton;
    [SerializeField] private Button backToEntriesButton;
    [SerializeField] private Button exitButton;


    [Header("Category Icons")]
    public Sprite pathogensIcon;
    public Sprite immuneCellsIcon;
    public Sprite processesIcon;

    private void Start()
    {
        InitializeDatabase();
        InitializeUI();
        SetupNavigation();
    }

    private void InitializeDatabase()
    {
        if (MicroPediaDatabase.Instance == null)
        {
            Instantiate(Resources.Load<GameObject>("MicroPedia/MicroPediaDatabase"));
        }
        //  MicroPediaDatabase.Instance.LoadUnlockedStates();
    }

    private void InitializeUI()
    {
        categoryButtonsParent.gameObject.SetActive(true);
        entriesView.SetActive(false);
        entryDetailPanel.SetActive(false);
        InitializeCategoryButtons();
    }

    private void SetupNavigation()
    {
        backToCategoriesButton.onClick.AddListener(ReturnToCategories);
        backToEntriesButton.onClick.AddListener(ReturnToEntries);
        exitButton.onClick.AddListener(DisableSelf);

        // button sound effects
        AddBackButtonFeedback(backToCategoriesButton);
        AddBackButtonFeedback(backToEntriesButton);
        AddBackButtonFeedback(exitButton);
    }

    private void InitializeCategoryButtons()
    {
        CreateCategoryButton("פתוגנים", EntryCategory.Pathogens, pathogensIcon);
        CreateCategoryButton("תאי חיסון", EntryCategory.ImmuneCells, immuneCellsIcon);
        CreateCategoryButton("תהליכים ומושגים", EntryCategory.Processes, processesIcon);
    }

    private void CreateCategoryButton(string label, EntryCategory category, Sprite icon)
    {
        GameObject button = Instantiate(categoryButtonPrefab, categoryButtonsParent);
        button.GetComponent<CategoryButton>().Initialize(label, icon, () => ShowCategory(category));

    }

    public void ShowCategory(EntryCategory category)
    {
        PlayButtonFeedback();
        categoryButtonsParent.gameObject.SetActive(false);
        entriesView.SetActive(true);
        ClearEntriesGrid();

        var entries = MicroPediaDatabase.Instance.GetEntriesByCategory(category);
        foreach (var entry in entries)
        {
            GameObject entryButton = Instantiate(entryButtonPrefab, entriesGrid);
            entryButton.GetComponent<EntryButton>().Initialize(entry, this, true);
        }
    }

    public void ShowEntryDetails(MicroPediaEntry entry)
    {
        PlayButtonFeedback();
        entryDetailPanel.SetActive(true);

        entryIconDisplay.sprite = entry.iconSprite;
        entryNameText.text = entry.displayName;
        entryDescriptionText.text = entry.description;
    }

    public void ReturnToCategories()
    {
        entryDetailPanel.SetActive(false);
        entriesView.SetActive(false);
        categoryButtonsParent.gameObject.SetActive(true);
    }

    public void ReturnToEntries()
    {
        entryDetailPanel.SetActive(false);
    }

    private void ClearEntriesGrid()
    {
        foreach (Transform child in entriesGrid)
        {
            Destroy(child.gameObject);
        }
    }

    private void PlayButtonFeedback()
    {

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play("ButtonPress");
        }
        else
        {
            Debug.LogError("AudioManager instance not found in the scene.");
        }


    }


    private void AddBackButtonFeedback(Button button)
    {
        button.onClick.AddListener(() =>
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play("BackButtonPress");
            }
            else
            {
                Debug.LogError("AudioManager instance not found in the scene.");
            }
        }
);
    }



    void DisableSelf()
    {

        // This disables the entire GameObject the script is attached to
        gameObject.SetActive(false);

        // All components and child objects will be disabled
    }
}