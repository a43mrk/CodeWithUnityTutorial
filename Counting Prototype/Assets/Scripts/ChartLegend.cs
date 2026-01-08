using System;
using UnityEngine;
using UnityEngine.UI;

public class ChartLegend : MonoBehaviour
{
    [Header("References")]
    public RectTransform legendContainer;
    public GameObject segmentPrefab;

    [Header("Layout")]
    [SerializeField] private float itemHeight = 16f;
    [SerializeField] private float itemSpacing = 6f;
    [SerializeField] private float squareSize = 12f;
    [SerializeField] private float textSpacing = 6f;

    public void BuildLegend(ChartGroupLegendItem[] items)
    {
        Clear();

        float yOffset = 0f;

        for(int i = 0; i < items.Length; i++)
        {
            RectTransform item = Instantiate(segmentPrefab, legendContainer).GetComponent<RectTransform>();

            item.anchorMin = new Vector2(1, 1);
            item.anchorMax = new Vector2(1, 1);
            item.pivot = new Vector2(1, 1);

            item.sizeDelta = new Vector2(legendContainer.rect.width, itemHeight);
            item.anchoredPosition = new Vector2(0, yOffset);

            Image bg = item.GetComponent<Image>();
            bg.color = Color.clear;

            // colored square
            RectTransform square = new GameObject("Square", typeof(RectTransform), typeof(Image))
                .GetComponent<RectTransform>();

            square.SetParent(item, false);
            square.anchorMin = new Vector2(0, 0.5f);
            square.anchorMax = new Vector2(0, 0.5f);
            square.pivot = new Vector2(0, 0.5f);

            square.sizeDelta = new Vector2(squareSize, squareSize);
            square.anchoredPosition = Vector2.zero;

            square.GetComponent<Image>().color = items[i].color;

            // label
            var text = item.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if(text != null)
            {
                text.text = items[i].label;
                text.alignment = TMPro.TextAlignmentOptions.Left;

                RectTransform textRect = text.GetComponent<RectTransform>();

                textRect.anchorMin = new Vector2(0, 0);
                textRect.anchorMax = new Vector2(1, 1);
                textRect.offsetMin = new Vector2(squareSize + textSpacing, 0);
                textRect.offsetMax = Vector2.zero;

                textRect.anchoredPosition = new Vector2(squareSize + textSpacing, 0);
            }

            yOffset += itemHeight + itemSpacing;

        }

    }

    private void Clear()
    {
        foreach(Transform child in legendContainer)
            Destroy(child.gameObject);
    }
}