using UnityEngine;
using TMPro;

public class TotalScore : MonoBehaviour
{
    public TextMeshProUGUI totalScoreText;

    void Start()
    {
        if (LevelManager.Instance != null)
        {
            totalScoreText.text = "" + LevelManager.Instance.GetTotalScore();
        }
    }
}
