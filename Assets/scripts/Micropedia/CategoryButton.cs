using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CategoryButton : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text categoryNameText;

    public void Initialize(string label, Sprite icon, UnityAction onClick)
    {
        iconImage.sprite = icon;
        categoryNameText.text = label;
        GetComponent<Button>().onClick.AddListener(onClick);
    }
}