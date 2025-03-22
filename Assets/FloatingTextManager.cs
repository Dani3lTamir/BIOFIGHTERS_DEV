using UnityEngine;
using TMPro;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    public GameObject floatingTextPrefab; // Assign in Inspector
    public Transform canvasTransform; // Assign your UI Canvas here

    private void Awake()
    {
        // Singleton enforcement
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }
    }

    public void ShowFloatingText(string message, Transform uiElement, Color color, bool isUp = true)
    {
        if (floatingTextPrefab == null || canvasTransform == null || uiElement == null) return;

        // Instantiate inside the Canvas
        GameObject floatingTextObj = Instantiate(floatingTextPrefab, canvasTransform);
        // Set as a child of the UI element
        floatingTextObj.transform.SetParent(uiElement, false);

        // Call ShowText() to set message and start animation
        FloatingText floatingText = floatingTextObj.GetComponent<FloatingText>();
        if (floatingText != null)
        {
            floatingText.ShowText(message, color, isUp);
        }
    }
}
