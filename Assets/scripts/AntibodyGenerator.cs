using UnityEngine;
using System.Collections.Generic;

public class AntibodyGenerator : MonoBehaviour
{
    public int baseHeight = 20;   // Height of the base
    public int minPins = 3;       // Minimum number of pins
    public int maxPins = 4;       // Maximum number of pins
    public int minPinWidth = 1;   // Minimum width of a pin
    public int maxPinWidth = 3;   // Maximum width of a pin
    public int minPinSpacing = 2; // Minimum spacing between pins
    public int maxPinSpacing = 5; // Maximum spacing between pins
    public Color shapeColor = Color.white;

    public int numPins;           // Number of pins
    public int[] pinWidths;       // Width of each pin
    public int[] pinSpacings;   // Spacing between pins
    public int baseWidth;         // Width of the base (calculated dynamically)

    public void Initialize()
    {
        // Randomize the antibody's shape
        RandomizeShape();

        // Calculate baseWidth based on the total width of the antibody
        baseWidth = Mathf.CeilToInt(GetTotalWidth());

        // Generate the antibody's texture
        Texture2D texture = CreateAntibodyTexture();

        if (texture != null)
        {
            // Create a sprite from the texture
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            // Check if a SpriteRenderer component already exists
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                // Add a SpriteRenderer component to the antibody
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            spriteRenderer.sortingLayerName = "Above Character"; // Set the sorting layer

            // Assign the sprite to the SpriteRenderer
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogError("Failed to create antibody texture.");
        }
    }

    void RandomizeShape()
    {
        // Randomize the number of pins
        numPins = Random.Range(minPins, maxPins + 1);

        // Randomize the pin widths and spacings
        pinWidths = new int[numPins];
        pinSpacings = new int[numPins - 1]; // Spacings are between pins

        for (int i = 0; i < numPins; i++)
        {
            pinWidths[i] =  Random.Range(minPinWidth, maxPinWidth + 1);
            if (i < numPins - 1)
            {
                pinSpacings[i] =  Random.Range(minPinSpacing, maxPinSpacing);
            }
        }

    }

    Texture2D CreateAntibodyTexture()
    {
        // Validate baseWidth and baseHeight
        if (baseWidth <= 0 || baseHeight <= 0)
        {
            return null; // Return null to avoid creating an invalid texture
        }

        int textureWidth = baseWidth;
        int textureHeight = baseHeight + 15; // Fixed pin length (e.g., 15 pixels)

        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        // Fill the texture with a transparent background
        Color[] pixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }

        // Draw the base
        for (int y = 0; y < baseHeight; y++)
        {
            for (int x = 0; x < baseWidth; x++)
            {
                pixels[y * textureWidth + x] = shapeColor;
            }
        }

        // Draw the pins
        float currentX = 0; // Start from the left edge

        for (int i = 0; i < numPins; i++)
        {
            int pinWidth = pinWidths[i];
            for (int y = baseHeight; y < textureHeight; y++)
            {
                for (int x = (int)currentX; x < (int)currentX + pinWidth; x++)
                {
                    if (x >= 0 && x < textureWidth && y >= 0 && y < textureHeight)
                    {
                        pixels[y * textureWidth + x] = shapeColor;
                    }
                }
            }
            if (i < numPins - 1)
            {
                currentX += pinWidth + pinSpacings[i];
            }
        }

        // Apply the pixels to the texture
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    public float GetTotalWidth()
    {
        if (numPins <= 0 || pinWidths == null || pinSpacings == null)
        {
            return 0; // Return 0 to avoid invalid calculations
        }

        float totalWidth = 0;
        for (int i = 0; i < numPins; i++)
        {
            totalWidth += pinWidths[i];
            if (i < numPins - 1)
            {
                totalWidth += pinSpacings[i];
            }
        }
        return totalWidth;
    }

    // Check if an antibody is unique based on its pins and spacings
    public static bool IsUniqueAntibody(List<AntibodyGenerator> antibodyList, AntibodyGenerator antibody)
    {
        foreach (AntibodyGenerator existingAntibody in antibodyList)
        {
            if (existingAntibody.numPins == antibody.numPins &&
                AreArraysEqual(existingAntibody.pinWidths, antibody.pinWidths) &&
                AreArraysEqual(existingAntibody.pinSpacings, antibody.pinSpacings))
            {
                return false;
            }
        }
        return true;
    }

    private static bool AreArraysEqual<T>(T[] array1, T[] array2)
    {
        if (array1 == null || array2 == null)
        {
            return false;
        }

        if (array1.Length != array2.Length)
        {
            return false;
        }

        for (int i = 0; i < array1.Length; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(array1[i], array2[i]))
            {
                return false;
            }
        }
        return true;
    }

}