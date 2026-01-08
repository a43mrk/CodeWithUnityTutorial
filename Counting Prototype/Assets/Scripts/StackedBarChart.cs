using UnityEngine;
using UnityEngine.UI;


public class StackedBarChart : MonoBehaviour
{
    [Header("Orientation")]
    [SerializeField] private ChartOrientation orientation = ChartOrientation.Vertical;

    [Header("References")]
    public RectTransform barContainer;
    public GameObject barPrefab;
    public GameObject segmentPrefab;

    [Header("Layout")]
    public float barWidth = 60f;
    public float barSpacing = 20f;
    public float chartHeight = 300f;

    [Header("Segment Spacing")]
    [SerializeField] private float segmentSpacing = 0f;

    [Header("Axes (Optional)")]
    [SerializeField] private RectTransform xAxis;
    [SerializeField] private RectTransform yAxis;
    [SerializeField] private float axisThickness = 2f;

    [SerializeField] private float xAxisExtraLength = 0f;
    [SerializeField] private float yAxisExtraLength = 0f;

    [Header("X Axis Index")]
    [SerializeField] private float xIndexHeight = 10f;
    [SerializeField] private float xIndexYOffset = -12f;


    private float calculatedChartWidth;
    private float calculatedChartHeight;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BuildChart(new[]
        {
            new StackedBarData
            {
                segments = new[]{ 30f, 20f, 50f},
                colors = new[] { Color.red, Color.yellow, Color.green },
                xIndexColors = new[] { Color.blue }
            },
            new StackedBarData
            {
                segments = new[] { 10f, 40f, 1170f},
                colors = new[] { Color.blue, Color.cyan, Color.magenta},
                xIndexColors = new[] { Color.blue }
            },
            new StackedBarData {
                segments = new[]{ 30f, 20f, 50f},
                colors = new[] { Color.red, Color.yellow, Color.green },
                xIndexColors = new[] { Color.blue }
            },
            new StackedBarData
            {
                segments = new[] { 10f, 40f, 70f},
                colors = new[] { Color.blue, Color.cyan, Color.magenta},
                xIndexColors = new[] { Color.blue }
            },
            new StackedBarData {
                segments = new[]{ 30f, 20f, 50f},
                colors = new[] { Color.red, Color.yellow, Color.green },
                xIndexColors = new[] { Color.blue }
            },
            new StackedBarData
            {
                segments = new[] { 10f, 40f, 70f},
                colors = new[] { Color.blue, Color.cyan, Color.magenta},
                xIndexColors = new[] { Color.blue }
            },
            new StackedBarData {
                segments = new[]{ 30f, 20f, 50f},
                colors = new[] { Color.red, Color.yellow, Color.green },
                xIndexColors = new[] { Color.blue }
            },
            new StackedBarData
            {
                segments = new[] { 10f, 40f, 70f},
                colors = new[] { Color.blue, Color.cyan, Color.magenta},
                xIndexColors = new[] { Color.blue }
            },
            new StackedBarData {
                segments = new[]{ 30f, 20f, 50f},
                colors = new[] { Color.red, Color.yellow, Color.green },
                xIndexColors = new[] { Color.blue }
            },
            new StackedBarData
            {
                segments = new[] { 10f, 40f, 70f},
                colors = new[] { Color.blue, Color.cyan, Color.magenta},
                xIndexColors = new[] { Color.blue }
            },
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildChart(StackedBarData[] data)
    {
        if(orientation == ChartOrientation.Vertical)
        {
            calculatedChartHeight = chartHeight;
            calculatedChartWidth = data.Length * barWidth + (data.Length - 1) * barSpacing;
        }
        else
        {
            calculatedChartWidth = chartHeight;
            calculatedChartHeight = data.Length * barWidth + (data.Length - 1) * barSpacing;
        }

        // ForceRectTransformSetup();
        SetupAxes();

        foreach(Transform child in barContainer)
            Destroy(child.gameObject);

        // used to normalize the rows
        float maxTotal = 0f;

        foreach(var barData in data)
        {
            float total = 0f; // sum
            foreach(float v in barData.segments)
                total += v;

            if (total > maxTotal)
                maxTotal = total;
        }

        for(int i = 0; i < data.Length; i++)
        {
            RectTransform bar = Instantiate(barPrefab, barContainer).GetComponent<RectTransform>();
            if(orientation == ChartOrientation.Vertical)
            {
                bar.sizeDelta = new Vector2(barWidth, chartHeight);
                bar.anchoredPosition = new Vector2(i * (barWidth + barSpacing), 0);
            }
            else
            {
                bar.sizeDelta = new Vector2(chartHeight, barWidth);
                bar.anchoredPosition = new Vector2(0, -i * (barWidth + barSpacing));
            }

            float indexPosition = i* (barWidth + barSpacing);
            CreateXIndex(barContainer, i, indexPosition, data[i]);


            float offset = 0f;

            for(int s= 0; s < data[i].segments.Length; s++)
            {
                float rawSize = (data[i].segments[s] / maxTotal) * chartHeight;
                float size = Mathf.Max(0f, Mathf.Round(rawSize));

                RectTransform segment = Instantiate(segmentPrefab, bar).GetComponent<RectTransform>();

                if(orientation == ChartOrientation.Vertical)
                {

                segment.anchorMin = new Vector2(0, 0);
                segment.anchorMax = new Vector2(1, 0);
                segment.pivot = new Vector2(0.5f, 0);
                segment.sizeDelta = new Vector2(0, size);
                segment.anchoredPosition = new Vector2(0, offset);
                }
                else
                {
                    segment.anchorMin = new Vector2(0,0);
                    segment.anchorMax = new Vector2(0, 1);
                    segment.pivot = new Vector2(0, 0.5f);

                    segment.sizeDelta = new Vector2(size, 0);
                    segment.anchoredPosition = new Vector2(offset, 0);
                }

                Debug.Log($"Segment Y:{segment.anchoredPosition.y} H:{segment.sizeDelta.y}");

                segment.GetComponent<Image>().color = data[i].colors[s];

                offset += size + segmentSpacing;
            }
        }
    }

    private void ForceRectTransformSetup()
    {
        // barContainer bottom anchor and pivot
        barContainer.anchorMin = new Vector2(0, 0);
        barContainer.anchorMax = new Vector2(1, 0);
        barContainer.pivot = new Vector2(0, 0);
        barContainer.anchoredPosition = Vector2.zero;
    }

    private void SetupAxes()
    {
        // x axis (bottom)
        if(xAxis != null)
        {
            xAxis.anchorMin = new Vector2(0,0);
            xAxis.anchorMax = new Vector2(0,0); // (0,0) = no strech, (1, 0) = strech on x
            xAxis.pivot = new Vector2(0,0);

            if (orientation == ChartOrientation.Vertical)
            {
                // Increase length by adding xAxisExtraLength to width
                // sizeDelta.x is width adjustment beyond anchors streching
                xAxis.sizeDelta = new Vector2(calculatedChartWidth + xAxisExtraLength, axisThickness);
                xAxis.anchoredPosition = Vector2.zero;

            }
            else
            {
                xAxis.sizeDelta = new Vector2(axisThickness, calculatedChartHeight + xAxisExtraLength + barWidth);
                xAxis.anchoredPosition = new Vector2(0, -calculatedChartHeight + barWidth );
            }
        }

        if(yAxis != null)
        {
            yAxis.anchorMin = new Vector2(0,0);
            yAxis.anchorMax = new Vector2(0,0);
            yAxis.pivot = new Vector2(0,0);

            if(orientation == ChartOrientation.Vertical)
            {
                // Increase length by adding yAxisExtraLength to height
                yAxis.sizeDelta = new Vector2(axisThickness, chartHeight + yAxisExtraLength);

                yAxis.anchoredPosition = Vector2.zero;
            }
            else
            {
                yAxis.sizeDelta = new Vector2(chartHeight + yAxisExtraLength + xIndexHeight, axisThickness);

                yAxis.anchoredPosition = new Vector2(-xIndexHeight, barWidth);
            }
        }
    }

    private void CreateXIndex(
        RectTransform parent,
        int index,
        float xPosition,
        StackedBarData data
    )
    {
        RectTransform indexRect = Instantiate(segmentPrefab, parent).GetComponent<RectTransform>();

        if(orientation == ChartOrientation.Vertical)
        {
            indexRect.anchorMin = new Vector2(0, 0);
            indexRect.anchorMax = new Vector2(0, 0);
            indexRect.pivot = new Vector2(0, 1);

            indexRect.sizeDelta = new Vector2(barWidth, xIndexHeight);
            indexRect.anchoredPosition = new Vector2(xPosition, xIndexYOffset);
        }
        else
        {
            // Left of y Axis
            indexRect.anchorMin = new Vector2(0, 0);
            indexRect.anchorMax = new Vector2(0, 0);
            indexRect.pivot = new Vector2(1, 0);

            indexRect.sizeDelta = new Vector2(xIndexHeight, barWidth);
            indexRect.anchoredPosition = new Vector2(xIndexYOffset, -xPosition);
        }

        Image img = indexRect.GetComponent<Image>();
        var text = indexRect.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        // Color handling
        if(data.xIndexColors != null && data.xIndexColors.Length > 0)
        {
            Color bg = data.xIndexColors[0];
            img.color = bg;

            if(text !=null)
            {
                if(data.xIndexColors.Length > 1)
                    text.color = data.xIndexColors[1];
                else
                    text.color = new Color(
                        1f - bg.r,
                        1f - bg.g,
                        1f - bg.b,
                        1f
                    );
            }
        }
        
        if(text !=null)
            text.text = index.ToString();
    }
}

public enum ChartOrientation
{
    Vertical,
    Horizontal
}

[System.Serializable]
public class StackedBarData
{
    public float[] segments;
    public Color[] colors;

    [Header("X Axis Index")]
    public Color[] xIndexColors; // [0] background, [1] text (optional)
}
