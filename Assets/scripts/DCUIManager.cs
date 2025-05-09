using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DCUIManager : MonoBehaviour
{
    public TextMeshProUGUI AttemptsLeftText;
    public Image wake;
    public static DCUIManager Instance;

    private void Awake()
    {
        // Initialize the Instance field
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
        AttemptsLeftText.text = "" + GameCountManager.Instance.GetCounterValue("AttemptsLeft");
    }


    public void ShowWakeUI()
    {
        wake.gameObject.SetActive(true);
    }
    public void HideWakeUI()
    {
        wake.gameObject.SetActive(false);
    }
}