using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AntigenGenerator : MonoBehaviour
{
    public GameObject antigenPrefab; // Prefab for the antigen
    public Image antigenUIImage; // Reference to the UI Image component for the antigen

    public void GenerateAntigen(Dictionary<SleepingTCell, AntibodyGenerator> tCellAntibodyMap)
    {
        if (tCellAntibodyMap == null)
        {
            Debug.LogError("No antibodies provided to generate antigen.");
            return;
        }

        // Randomly select one entry from the dictionary
        List<SleepingTCell> tCells = new List<SleepingTCell>(tCellAntibodyMap.Keys);
        int correctIndex = Random.Range(0, tCells.Count);
        SleepingTCell correctTCell = tCells[correctIndex];
        AntibodyGenerator correctAntibody = tCellAntibodyMap[correctTCell];
        //set the t-cell as having the correct antibody
        correctTCell.SetCorrectAntibody(true);
        // Create the antigen
        GameObject antigen = Instantiate(antigenPrefab, new Vector3(0, 5, 0), Quaternion.identity);

        // Generate the antigen's shape to complement the correct antibody
        Texture2D antigenTexture = CreateComplementaryShapeTexture(correctAntibody);
        Sprite antigenSprite = Sprite.Create(antigenTexture, new Rect(0, 0, antigenTexture.width, antigenTexture.height), Vector2.one * 0.5f);
        antigen.GetComponent<SpriteRenderer>().sprite = antigenSprite;

        // Update the UI Image with the antigen sprite
        if (antigenUIImage != null)
        {
            antigenUIImage.sprite = antigenSprite;
        }
    }

    Texture2D CreateComplementaryShapeTexture(AntibodyGenerator antibody)
    {
        // Use the antibody's total width as the base width
        int baseWidth = Mathf.CeilToInt(antibody.GetTotalWidth());
        int baseHeight = antibody.baseHeight;
        int textureWidth = baseWidth;
        int textureHeight = baseHeight + 15; // Fixed pin length (e.g., 15 pixels)
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        // Fill the texture with a solid color (antigen base)
        Color[] pixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        // Calculate the total width of the antibody's pins and spacings
        float totalWidth = antibody.GetTotalWidth();

        // Calculate the starting X position to center the gaps
        float currentX = 0; // Start from the left edge


        // Create gaps (negative space) for the antibody's pins
        for (int i = 0; i < antibody.numPins; i++)
        {
            int pinWidth = antibody.pinWidths[i];

            for (int y = baseHeight; y < textureHeight; y++)
            {
                for (int x = (int)currentX; x < (int)currentX + pinWidth; x++)
                {
                    if (x >= 0 && x < textureWidth && y >= 0 && y < textureHeight)
                    {
                        pixels[y * textureWidth + x] = Color.clear; // Clear the gap
                    }
                }
            }
            if (i < antibody.numPins - 1)
            {
                currentX += pinWidth + antibody.pinSpacings[i];
            }
        }

        // Apply the pixels to the texture
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}