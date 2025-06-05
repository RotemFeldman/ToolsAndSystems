using UnityEngine;
using UnityEngine.UI;

namespace UnityUtils
{
    public static class Create
    {
        

        /// <summary>
        /// Creates a world space text object using TextMeshPro and returns the associated component.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="position">The position of the text in world space.</param>
        /// <param name="rotation">The rotation of the text object. Defaults to the identity rotation.</param>
        /// <param name="fontSize">The font size of the text. Defaults to 36.</param>
        /// <param name="color">The color of the text. If null, the default TextMeshPro color is used.</param>
        /// <param name="parent">The parent GameObject for the text object. If null, the text object will have no parent.</param>
        /// <returns>Returns the <see cref="TMPro.TextMeshPro"/> component associated with the created text object.</returns>
        public static TMPro.TextMeshPro WorldText
            (string text, Vector3 position, Quaternion rotation = default, float fontSize = 36, Color? color = null, GameObject parent = null)
        {
            // Create a new GameObject
            GameObject textObject = new GameObject("WorldText");

            // Optional: Set the object's parent
            if (parent != null)
            {
                textObject.transform.SetParent(parent.transform);
            }

            // Set the position and rotation
            textObject.transform.position = position;
            textObject.transform.rotation = rotation;

            // Add a TextMeshPro component
            TMPro.TextMeshPro textComponent = textObject.AddComponent<TMPro.TextMeshPro>();

            // Set the initial text
            textComponent.text = text;

            // Set the font size
            textComponent.fontSize = fontSize;

            // Set the color, if provided
            if (color.HasValue)
            {
                textComponent.color = color.Value;
            }

            // Return the TextMeshPro component
            return textComponent;
        }


        /// <summary>
        /// Creates a screen space UI text object using TextMeshProUGUI and returns the associated component.
        /// </summary>
        /// <param name="text">The text content to display.</param>
        /// <param name="position">The anchored position of the text within the UI layout.</param>
        /// <param name="fontSize">The font size of the text. Defaults to 36.</param>
        /// <param name="color">The color of the text. If null, the default TextMeshProUGUI color is used.</param>
        /// <param name="parent">The parent Transform for the UI text object. If null, the text object will be parented to a Canvas in the scene or a new Canvas will be created.</param>
        /// <returns>Returns the <see cref="TMPro.TextMeshProUGUI"/> component associated with the created UI text object.</returns>
        public static TMPro.TextMeshProUGUI UIText
            (string text, Vector2 position, float fontSize = 36, Color? color = null, Transform parent = null)
        {
            // Ensure a Canvas exists in the scene
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObject = new GameObject("Canvas");
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            // Create a new GameObject for the text
            GameObject textObject = new GameObject("UIText");

            // Set the object's parent
            textObject.transform.SetParent(parent != null ? parent : canvas.transform, false);

            // Set the position
            RectTransform rectTransform = textObject.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = position;

            // Add a TextMeshProUGUI component
            TMPro.TextMeshProUGUI textComponent = textObject.AddComponent<TMPro.TextMeshProUGUI>();

            // Set the initial text
            textComponent.text = text;

            // Set the font size
            textComponent.fontSize = fontSize;

            // Set the color, if provided
            if (color.HasValue)
            {
                textComponent.color = color.Value;
            }

            // Return the TextMeshProUGUI component
            return textComponent;
        }


        /// <summary>
        /// Creates a UI text element using TextMeshProUGUI and returns the associated component.
        /// </summary>
        /// <param name="text">The text to display in the UI element.</param>
        /// <param name="direction">The direction or alignment for the text element (e.g., left, right, center).</param>
        /// <param name="fontSize">The font size of the text. Defaults to 36.</param>
        /// <param name="color">The color of the text. If null, the default TextMeshProUGUI color is used.</param>
        /// <param name="parent">The parent Transform for the UI text element. If null, the element will have no parent.</param>
        /// <returns>Returns the <see cref="TMPro.TextMeshProUGUI"/> component associated with the created UI text element.</returns>
        public static TMPro.TextMeshProUGUI UIText
            (string text, EightDirection direction, float fontSize = 36, Color? color = null, Transform parent = null)
        {
            // Ensure a Canvas exists in the scene
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObject = new GameObject("Canvas");
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            // Create a new GameObject for the text
            GameObject textObject = new GameObject("UIText");

            // Set the object's parent
            textObject.transform.SetParent(parent != null ? parent : canvas.transform, false);

            // Add a RectTransform
            RectTransform rectTransform = textObject.AddComponent<RectTransform>();

            // Set anchor and position based on the direction
            switch (direction)
            {
                case EightDirection.TopLeft:
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(0, 1);
                    rectTransform.pivot = new Vector2(0, 1);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case EightDirection.TopCenter:
                    rectTransform.anchorMin = new Vector2(0.5f, 1);
                    rectTransform.anchorMax = new Vector2(0.5f, 1);
                    rectTransform.pivot = new Vector2(0.5f, 1);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case EightDirection.TopRight:
                    rectTransform.anchorMin = new Vector2(1, 1);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(1, 1);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case EightDirection.CenterLeft:
                    rectTransform.anchorMin = new Vector2(0, 0.5f);
                    rectTransform.anchorMax = new Vector2(0, 0.5f);
                    rectTransform.pivot = new Vector2(0, 0.5f);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case EightDirection.Center:
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case EightDirection.CenterRight:
                    rectTransform.anchorMin = new Vector2(1, 0.5f);
                    rectTransform.anchorMax = new Vector2(1, 0.5f);
                    rectTransform.pivot = new Vector2(1, 0.5f);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case EightDirection.BottomLeft:
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(0, 0);
                    rectTransform.pivot = new Vector2(0, 0);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case EightDirection.BottomCenter:
                    rectTransform.anchorMin = new Vector2(0.5f, 0);
                    rectTransform.anchorMax = new Vector2(0.5f, 0);
                    rectTransform.pivot = new Vector2(0.5f, 0);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case EightDirection.BottomRight:
                    rectTransform.anchorMin = new Vector2(1, 0);
                    rectTransform.anchorMax = new Vector2(1, 0);
                    rectTransform.pivot = new Vector2(1, 0);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
            }

            // Add a TextMeshProUGUI component
            TMPro.TextMeshProUGUI textComponent = textObject.AddComponent<TMPro.TextMeshProUGUI>();

            // Set the initial text
            textComponent.text = text;

            // Set the font size
            textComponent.fontSize = fontSize;

            // Set the color, if provided
            if (color.HasValue)
            {
                textComponent.color = color.Value;
            }

            // Return the TextMeshProUGUI component
            return textComponent;
        }
    }
}
