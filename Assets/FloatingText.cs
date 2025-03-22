using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public Animator animator;

    public void ShowText(string message, Color color, bool isUp)
    {
        textMesh.text = message;
        textMesh.color = color;
        if (isUp)
        {
            animator.SetTrigger("Show"); // Trigger Up animation
        }
        else
        {
            animator.SetTrigger("ShowDown"); // Trigger Down animation
        }
        Destroy(gameObject, 5f); // Destroy after animation
    }
}
