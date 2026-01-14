using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class SlumpPlot : MonoBehaviour
{
    [Header("References")]
    public RectTransform plotArea;
    public Font labelFont;
    public Material lineMaterial;

    [Header("Display Mode")]
    [Tooltip("Parallel: All days overlaid on same X-axis. Sequential: Days connected in timeline.")]
    public DataDisplayMode displayMode = DataDisplayMode.Sequential;

    [Header("Line Rendering")]
    public float lineThickness = 2.5f;
    public bool enableGlow = true;

    [Header("Trend Thickness")]
    public float uptrendThicknessMultiplier = 1.0f;
    public float downtrendThicknessMultiplier = 1.0f;

    [Tooltip("Additive UI material used ONLY for glow duplication")]
    public Material glowMaterial;

    [Range(0f, 1f)]
    public float glowAlpha = 0.25f;

    public float glowThicknessMultiplier = 3.0f;


    [Header("Trend Colors")]
    public Color upTrendColor = new Color(0.2f, 1f, 0.2f);
    public Color downTrendColor = new Color(1f, 0.25f, 0.25f);

    [Header("Grid Colors")]
    public Color gridColor = new Color(1, 1, 1, 0.08f);
    public Color axisColor = new Color(1, 1, 1, 0.3f);

    [Header("Y Range")]
    public float yMin = -2500f;
    public float yMax = 4500f;

    [Header("Appearance")]
    public float lineWidth = 2.5f;
    public float gridLineWidth = 1f;
    public float pointRadius = 4f;
    public float missionPointRadius = 6f;

    [Header("Day Separators")]
    public Color daySeparatorColor = new Color(1, 1, 1, 0.25f);
    public float daySeparatorWidth = 2f;
    public float dayLabelOffsetY = 24f;

    [Header("Data")]
    public SlumpDayData[] dataSets;

    [Header("Overlay Display")]
    public OverlayDisplayMode overlayMode = OverlayDisplayMode.Missions;

    [Header("Pivot Markers")]
    public float pivotCircleRadius = 6f;
    public Color pivotUpColor = Color.green;
    public Color pivotDownColor = Color.red;

    [Header("Pivot Value Label")]
    public Font pivotLabelFont;
    public int pivotLabelFontSize = 12;
    public Color pivotLabelColor = Color.white;
    public Vector2 pivotLabelOffset = new Vector2(0f, 14f);

    [Header("UI Sprites")]
    public Sprite circleSprite;

    [Header("Bonus Markers")]
    public int bonusLabelFontSize = 12;
    public Color bonusTextColor = Color.black;
    
    [Header("Consecutive Bonus Marker")]
    public Sprite bonusSprite;
    public Vector2 bonusSpriteSize = new Vector2(18f, 18f);
    public Color bonusSpriteColor = Color.white;
    public Vector2 bonusLabelOffset = new Vector2(0f, -16f);

    [Header("Mission Icons")]
    public Sprite starSprite;
    public float starSize = 12f;
    public float starSpacing = 6f;

    RectTransform gridRoot;
    RectTransform plotRoot;

    void Awake()
    {
        plotArea = plotArea ? plotArea : GetComponent<RectTransform>();
        plotArea.pivot = Vector2.zero;

        gridRoot = CreateLayer("Grid");
        plotRoot = CreateLayer("Plot");

        if (dataSets == null || dataSets.Length == 0)
            GenerateSampleData();
    }

    void Start()
    {
        DrawGridAndAxes();
        DrawDaySeparators();
        DrawAllDataSets();
    }

    #region Day Separators

    void DrawDaySeparators()
    {
        if (dataSets == null || dataSets.Length < 3)
            return;

        // Only show separators in Sequential mode
        if (displayMode != DataDisplayMode.Sequential)
            return;

        int samplesPerDay = dataSets[0].points.Length;
        int totalSamples = samplesPerDay * dataSets.Length;

        DrawDaySeparator(samplesPerDay, "2 days ago", totalSamples);
        DrawDaySeparator(samplesPerDay * 2, "yesterday", totalSamples);
        DrawDaySeparator(samplesPerDay * 3, "today", totalSamples);
    }

    void DrawDaySeparator(int sampleIndex, string label, int totalSamples)
    {
        float x = MapX(sampleIndex, totalSamples);

        // Vertical line
        CreateLine(
            gridRoot,
            new Vector2(x, 0),
            new Vector2(x, plotArea.rect.height),
            daySeparatorColor,
            daySeparatorWidth
        );

        // Label
        CreateDayLabel(new Vector2(x, plotArea.rect.height), label);
    }

    void CreateDayLabel(Vector2 topPos, string text)
    {
        GameObject g = new GameObject("DayLabel", typeof(RectTransform), typeof(Text));
        g.transform.SetParent(plotArea, false);

        Text t = g.GetComponent<Text>();
        t.font = labelFont;
        t.fontSize = 14;
        t.color = Color.white;
        t.alignment = TextAnchor.LowerCenter;
        t.text = text;
        t.raycastTarget = false;

        RectTransform rt = g.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 0);
        rt.anchoredPosition = topPos + Vector2.up * dayLabelOffsetY;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.sizeDelta = new Vector2(120, 30);
    }

    #endregion

    #region Mission Markers
    void CreateMissionMarker(Vector2 pos, MissionCompletedType type)
    {
        switch (type)
        {
            case MissionCompletedType.EventMission:
                CreateSquareMarker(pos, GetMissionColor(type));
                break;

            case MissionCompletedType.OneStarMission:
                CreateStars(pos, 1, GetMissionColor(type));
                break;

            case MissionCompletedType.TwoStarMission:
                CreateStars(pos, 2, GetMissionColor(type));
                break;

            case MissionCompletedType.ThreeStarMission:
                CreateStars(pos, 3, GetMissionColor(type));
                break;
        }
    }

    void CreateSquareMarker(Vector2 pos, Color col)
    {
        GameObject g = new GameObject("EventMission", typeof(RectTransform), typeof(Image));
        g.transform.SetParent(plotRoot, false);

        Image img = g.GetComponent<Image>();
        img.color = col;

        RectTransform rt = g.GetComponent<RectTransform>();
        rt.sizeDelta = Vector2.one * missionPointRadius * 2f;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.anchoredPosition = pos;
    }
    
    void CreateStars(Vector2 pos, int count, Color col)
    {
        float totalWidth = (count - 1) * starSpacing;

        for (int i = 0; i < count; i++)
        {
            float offsetX = i * starSpacing - totalWidth * 0.5f;
            CreateSingleStar(pos + new Vector2(offsetX, 0), col);
        }
    }

    void CreateSingleStar(Vector2 pos, Color col)
    {
        GameObject g = new GameObject("Star", typeof(RectTransform), typeof(Image));
        g.transform.SetParent(plotRoot, false);

        Image img = g.GetComponent<Image>();
        img.sprite = starSprite;
        img.color = col;
        img.preserveAspect = true;

        RectTransform rt = g.GetComponent<RectTransform>();
        rt.sizeDelta = Vector2.one * starSize;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.anchoredPosition = pos;
    }

    Color GetMissionColor(MissionCompletedType type)
    {
        switch(type)
        {
            case MissionCompletedType.EventMission: return new Color(0.6f, 0.2f, 1f);
            case MissionCompletedType.OneStarMission: return Color.white;
            case MissionCompletedType.TwoStarMission: return Color.yellow;
            case MissionCompletedType.ThreeStarMission: return Color.cyan;
            default: return Color.gray;
        }
    }
    #endregion

    #region Coordinate Mapping

    float MapX(int localIndex, int count)
    {
        return (localIndex / (float)(count - 1)) * plotArea.rect.width;
    }

    float MapXSequential(int globalIndex, int totalCount)
    {
        return (globalIndex / (float)(totalCount - 1)) * plotArea.rect.width;
    }

    float MapY(float value)
    {
        return Mathf.InverseLerp(yMin, yMax, value) * plotArea.rect.height;
    }

    #endregion

    #region Grid

    void DrawGridAndAxes()
    {
        float range = Mathf.Max(Mathf.Abs(yMin), Mathf.Abs(yMax));
        float step = CalculateMajorStep(range);

        for (float v = -range; v <= range; v += step)
        {
            bool isZero = Mathf.Approximately(v, 0);
            DrawHorizontalGrid(v, isZero ? axisColor : gridColor, isZero);
            CreateYLabel(v);
        }

        // Y Axis
        CreateLine(
            gridRoot,
            Vector2.zero,
            new Vector2(0, plotArea.rect.height),
            axisColor,
            gridLineWidth * 2
        );
    }

    float CalculateMajorStep(float range)
    {
        float raw = range / 5f;
        float p = Mathf.Pow(10, Mathf.Floor(Mathf.Log10(raw)));
        float n = raw / p;
        return (n < 2) ? 1 * p : (n < 5 ? 2 * p : 5 * p);
    }

    void DrawHorizontalGrid(float value, Color col, bool thick)
    {
        float y = MapY(value);
        CreateLine(
            gridRoot,
            new Vector2(0, y),
            new Vector2(plotArea.rect.width, y),
            col,
            thick ? gridLineWidth * 2 : gridLineWidth
        );
    }

    #endregion

    #region Plot
    
    private void DrawAllDataSets()
    {
        if (displayMode == DataDisplayMode.Parallel)
            DrawParallel();
        else
            DrawSequential();
    }

    void DrawParallel()
    {
        // Each day rendered on the same X-axis range (0 to width)
        foreach(var day in dataSets)
            DrawSingleDataSetParallel(day);
    }

    void DrawSequential()
    {
        // All days connected as one continuous timeline
        int samplesPerDay = dataSets[0].points.Length;
        int totalSamples = samplesPerDay * dataSets.Length;
        int globalIndex = 0;

        foreach(var day in dataSets)
        {
            DrawSingleDataSetSequential(day, ref globalIndex, totalSamples);
        }
    }

    void DrawSingleDataSetParallel(SlumpDayData day)
    {
        int n = day.points.Length;

        for (int i = 0; i < n - 1; i++)
        {
            var point = day.points[i];
            var previousNet = i > 0 ? day.points[i - 1].NetValue : 0;
            var nextNet = day.points[i + 1].NetValue;
            var netValue = point.NetValue;
            var isUptrend = (nextNet - netValue) >= 0;

            Color c = isUptrend ? upTrendColor : downTrendColor;

            Vector2 a = new Vector2(MapX(i, n), MapY(netValue));
            Vector2 b = new Vector2(MapX(i + 1, n), MapY(nextNet));

            CreateSegment(a, b, c, isUptrend);

            Vector2 currentPos = new Vector2(MapX(i, n), MapY(point.NetValue));
            DrawOverlayMarkers(day, i, previousNet, netValue, nextNet, currentPos);
        }
    }

    void DrawSingleDataSetSequential(SlumpDayData day, ref int globalIndex, int totalCount)
    {
        int n = day.points.Length;

        for (int i = 0; i < n - 1; i++)
        {
            var point = day.points[i];
            var previousNet = i > 0 ? day.points[i - 1].NetValue : 0;
            var nextNet = day.points[i + 1].NetValue;
            var netValue = point.NetValue;
            var isUptrend = (nextNet - netValue) >= 0;

            Color c = isUptrend ? upTrendColor : downTrendColor;

            Vector2 a = new Vector2(MapXSequential(globalIndex, totalCount), MapY(netValue));
            Vector2 b = new Vector2(MapXSequential(globalIndex + 1, totalCount), MapY(nextNet));

            CreateSegment(a, b, c, isUptrend);

            Vector2 currentPos = new Vector2(MapXSequential(globalIndex, totalCount), MapY(point.NetValue));
            DrawOverlayMarkers(day, i, previousNet, netValue, nextNet, currentPos);

            globalIndex++;
        }
        
        globalIndex++; // Move to next day's starting position
    }

    void DrawOverlayMarkers(SlumpDayData day, int localIndex, int previousNet, int netValue, int nextNet, Vector2 currentPos)
    {
        var point = day.points[localIndex];

        switch (overlayMode)
        {
            case OverlayDisplayMode.Missions:
                if (point.missionCompletedType.HasValue)
                    CreateMissionMarker(currentPos, point.missionCompletedType.Value);
                break;

            case OverlayDisplayMode.TrendPivots:
                if(localIndex > 0 && localIndex < day.points.Length - 1)
                    TryCreateTrendPivot(previousNet, netValue, nextNet, currentPos);
                break;

            case OverlayDisplayMode.ConsecutiveBonus:
                if (point.consecutiveBonusAmount.HasValue)
                    CreateBonusMarker(currentPos, point.consecutiveBonusAmount.Value);
                break;
        }
    }

    void CreateGlow(RectTransform baseSegment, float length, Color baseColor)
    {
        GameObject glow = new GameObject("Glow", typeof(RectTransform), typeof(Image));
        glow.transform.SetParent(plotRoot, false);

        Image glowImg = glow.GetComponent<Image>();
        glowImg.material = glowMaterial;
        glowImg.color = new Color(baseColor.r, baseColor.g, baseColor.b, glowAlpha);
        glowImg.raycastTarget = false;

        RectTransform glowRt = glow.GetComponent<RectTransform>();
        glowRt.pivot = baseSegment.pivot;
        glowRt.anchorMin = baseSegment.anchorMin;
        glowRt.anchorMax = baseSegment.anchorMax;
        glowRt.anchoredPosition = baseSegment.anchoredPosition;
        glowRt.localRotation = baseSegment.localRotation;
        glowRt.sizeDelta = new Vector2(length, lineThickness * glowThicknessMultiplier);
    }

    void CreateSegment(Vector3 a, Vector3 b, Color col, bool isUptrend)
    {
        GameObject go = new GameObject("Segment", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(plotRoot, false);

        Image img = go.GetComponent<Image>();
        img.material = null;
        img.color = col;
        img.raycastTarget = false;

        RectTransform rt = go.GetComponent<RectTransform>();
        Vector3 diff = b - a;
        float length = diff.magnitude;

        float thickness = isUptrend
            ? lineThickness * uptrendThicknessMultiplier
            : lineThickness * downtrendThicknessMultiplier;

        rt.sizeDelta = new Vector2(length, thickness);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = new Vector2(0f, 0.5f);
        rt.anchoredPosition = a;

        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        rt.localRotation = Quaternion.Euler(0, 0, angle);

        if (enableGlow && isUptrend && glowMaterial != null)
        {
            CreateGlow(rt, length, col);
        }
    }

    void TryCreateTrendPivot(int prev, int current, int next, Vector2 pos)
    {
        int slopeBefore = current - prev;
        int slopeAfter = next - current;

        if (slopeBefore == 0 || slopeAfter == 0)
            return;

        if (Mathf.Sign(slopeBefore) != Mathf.Sign(slopeAfter))
        {
            bool isUpturn = slopeAfter > 0;
            CreatePivotCircle(pos, isUpturn, current);
        }
    }

    void CreatePivotCircle(Vector2 pos, bool isUpturn, int value)
    {
        GameObject root = new GameObject("TrendPivot", typeof(RectTransform));
        root.transform.SetParent(plotRoot, false);

        RectTransform rootRt = root.GetComponent<RectTransform>();
        rootRt.anchoredPosition = pos;
        rootRt.anchorMin = Vector2.zero;
        rootRt.anchorMax = Vector2.zero;

        // Circle
        GameObject circle = new GameObject("Circle", typeof(RectTransform), typeof(Image));
        circle.transform.SetParent(root.transform, false);

        Image img = circle.GetComponent<Image>();
        img.material = null;
        img.sprite = circleSprite;
        img.type = Image.Type.Simple;
        img.color = isUpturn ? pivotUpColor : pivotDownColor;
        img.raycastTarget = false;

        RectTransform crt = circle.GetComponent<RectTransform>();
        crt.sizeDelta = Vector2.one * pivotCircleRadius * 2f;

        // Label
        GameObject label = new GameObject("ValueLabel", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(root.transform, false);

        Text txt = label.GetComponent<Text>();
        txt.text = value.ToString();
        txt.font = pivotLabelFont;
        txt.fontSize = pivotLabelFontSize;
        txt.color = pivotLabelColor;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.raycastTarget = false;

        RectTransform lrt = label.GetComponent<RectTransform>();
        lrt.anchoredPosition = pivotLabelOffset;
        lrt.sizeDelta = new Vector2(60f, 20f);
    }

    void CreateBonusMarker(Vector2 pos, int bonus)
    {
        GameObject root = new GameObject("Bonus", typeof(RectTransform));
        root.transform.SetParent(plotRoot, false);

        RectTransform rt = root.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;

        // Sprite Icon
        GameObject icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        icon.transform.SetParent(root.transform, false);

        Image img = icon.GetComponent<Image>();
        img.material = null;
        img.color = bonusSpriteColor;
        img.sprite = bonusSprite;
        img.type = Image.Type.Simple;

        RectTransform irt = icon.GetComponent<RectTransform>();
        irt.sizeDelta = bonusSpriteSize;

        // Label
        GameObject label = new GameObject("Label", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(root.transform, false);

        Text txt = label.GetComponent<Text>();
        txt.text = bonus.ToString();
        txt.font = labelFont;
        txt.fontSize = bonusLabelFontSize;
        txt.color = bonusTextColor;
        txt.alignment = TextAnchor.MiddleCenter;

        RectTransform lrt = label.GetComponent<RectTransform>();
        lrt.anchoredPosition = bonusLabelOffset;
        lrt.anchorMin = new Vector2(0,0);
        lrt.anchorMax = new Vector2(1,1);
        lrt.pivot = new Vector2(0.5f, 0.5f);
        lrt.sizeDelta = irt.sizeDelta;
    }

    #endregion

    #region UI Primitives

    RectTransform CreateLayer(string name)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(plotArea, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.pivot = Vector2.zero;
        rt.sizeDelta = plotArea.rect.size;
        return rt;
    }

    void CreateLine(RectTransform parent, Vector2 a, Vector2 b, Color col, float width, Material lineMaterial = null)
    {
        Vector2 d = b - a;
        float len = d.magnitude;
        float ang = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;

        GameObject go = new GameObject("Line", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        Image img = go.GetComponent<Image>();
        img.color = col;
        img.material = lineMaterial;
        img.raycastTarget = false;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = new Vector2(len, width);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.anchoredPosition = a;
        rt.localRotation = Quaternion.Euler(0, 0, ang);
    }

    void CreateYLabel(float v)
    {
        GameObject g = new GameObject("YLabel", typeof(RectTransform), typeof(Text));
        g.transform.SetParent(gridRoot, false);

        Text t = g.GetComponent<Text>();
        t.font = labelFont;
        t.fontSize = 12;
        t.color = Color.white;
        t.text = v.ToString("N0");
        t.raycastTarget = false;

        RectTransform rt = g.GetComponent<RectTransform>();
        rt.pivot = new Vector2(1, 0.5f);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.anchoredPosition = new Vector2(-6, MapY(v));
        rt.sizeDelta = new Vector2(80, 20);
    }

    #endregion

    #region Sample Data Generator
    
    void GenerateSampleData()
    {
        const int samplesPerDay = 24;
        dataSets = new SlumpDayData[3];

        DateTime today = DateTime.Today;

        for (int day = 0; day < 3; day++)
        {
            var dayData = new SlumpDayData
            {
                dayLabel = day == 0 ? "2 days ago"
                        : day == 1 ? "yesterday"
                        : "today",
                lineColor = day == 0 ? new Color(0.6f, 0.6f, 0.6f)
                        : day == 1 ? new Color(0.3f, 0.6f, 1f)
                        : new Color(0.2f, 1f, 0.2f),
                points = new SlumpDataPoint[samplesPerDay]
            };

            int baseValue = -500 + day * 300;
            System.Random rng = new System.Random(100 + day);

            for (int i = 0; i < samplesPerDay; i++)
            {
                baseValue += rng.Next(-120, 180);

                int balls = rng.Next(80, 160);
                int payout = balls + baseValue;

                dayData.points[i] = new SlumpDataPoint
                {
                    dateTime = today.AddDays(day - 2).AddHours(i),
                    amountOfBallsShoot = balls,
                    amountOfPayouts = payout,
                    missionCompletedType = GenerateMission(i)
                };

                if (i == 8)  dayData.points[i].consecutiveBonusAmount = 50;
                if (i == 14) dayData.points[i].consecutiveBonusAmount = 120;
                if (i == 19) dayData.points[i].consecutiveBonusAmount = 300;
            }

            dataSets[day] = dayData;
        }
    }

    MissionCompletedType? GenerateMission(int index)
    {
        if (index == 5)  return MissionCompletedType.OneStarMission;
        if (index == 11) return MissionCompletedType.TwoStarMission;
        if (index == 17) return MissionCompletedType.ThreeStarMission;
        if (index == 21) return MissionCompletedType.EventMission;

        return null;
    }

    #endregion
}

public enum DataDisplayMode
{
    Parallel,    // All days overlaid on same X-axis
    Sequential   // Days connected as continuous timeline
}

[Serializable]
public class SlumpDayData
{
    public string dayLabel;
    public Color lineColor;
    public SlumpDataPoint[] points;
}

public class SlumpDataPoint
{
    public DateTime dateTime;
    public int amountOfPayouts;
    public int amountOfBallsShoot;
    public MissionCompletedType? missionCompletedType;
    public int? consecutiveBonusAmount;
    public int NetValue => amountOfPayouts - amountOfBallsShoot;
}

public enum MissionCompletedType
{
    EventMission,
    OneStarMission,
    TwoStarMission,
    ThreeStarMission
}

public enum OverlayDisplayMode
{
    None,
    Missions,
    TrendPivots,
    ConsecutiveBonus
}