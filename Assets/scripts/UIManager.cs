using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
   // public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI InfluenzaLeft;
    public TextMeshProUGUI AlliesLeft;

    //  public TextMeshProUGUI timeElapsedText;

    private void Update()
    {
        scoreText.text = "" + ScoreManager.Instance.GetScore();
        // enemiesKilledText.text = "Enemies Killed: " + GameStatsManager.Instance.GetEnemiesKilled();
        InfluenzaLeft.text = "" + GameCountManager.Instance.GetCounterValue("InfluenzaLeft");
        AlliesLeft.text = "" + GameCountManager.Instance.GetCounterValue("AlliesLeft");

        //timeElapsedText.text = "Time: " + Mathf.FloorToInt(GameStatsManager.Instance.GetTimeElapsed()) + "s";
    }
}
