using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DCUIManager : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public TextMeshProUGUI AttemptsLeft;
    public Image antigenImage; // Reference to the UI Image component for the Antigen icon

    private void Update()
    {
        timer.text = "" + ScoreManager.Instance.GetScore();
        AttemptsLeft.text = "" + GameCountManager.Instance.GetCounterValue("AttemptsLeft");
    }

}