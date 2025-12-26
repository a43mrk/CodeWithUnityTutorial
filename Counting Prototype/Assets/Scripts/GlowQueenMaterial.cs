using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GlowQueenMaterial : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color glowColor = Color.orange;

    [Range(0f, 20f)]
    public float intensity = 5f;

    private Material[] runtimeMaterials;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();

        // This creates a runtime instance (NOT the imported asset)
        runtimeMaterials = renderer.materials;

        EnableGlow();
    }

    private void EnableGlow()
    {
        var mat = runtimeMaterials.FirstOrDefault(a => a.name.Contains("Orange_Glass_Or_Acrylic"));

        mat.EnableKeyword("_EMISSION");

        Color finalColor = glowColor * intensity;
        mat.SetColor("_EmissionColor", finalColor);
    }

    // Update is called once per frame
    void Update()
    {
        float pulse = Mathf.Sin(Time.time * 4f) * 0.5f + 0.5f;
        var mat = runtimeMaterials.FirstOrDefault(a => a.name.Contains("Orange_Glass_Or_Acrylic"));
        Color finalColor = glowColor * intensity * pulse;
        mat.SetColor("_EmissionColor", finalColor);

    }
}
