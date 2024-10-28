using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Rainbow : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public float colorChangeSpeed = 5f;
    public Gradient colorGradient;

    private float colorTime = 0f;

    void Start()
    {
        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (tmpText != null)
        {
            colorTime += Time.deltaTime * colorChangeSpeed;
            UpdateTextColors();
        }
    }

    void UpdateTextColors()
    {
        string text = tmpText.text;
        tmpText.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            Color charColor = colorGradient.Evaluate((i + colorTime) % 1f);

            int vertexIndex = charInfo.vertexIndex;

            tmpText.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[vertexIndex + 0] = charColor;
            tmpText.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[vertexIndex + 1] = charColor;
            tmpText.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[vertexIndex + 2] = charColor;
            tmpText.textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[vertexIndex + 3] = charColor;
        }

        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
