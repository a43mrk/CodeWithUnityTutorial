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

    [Header("Point Light Settings")]
    public Light pointLight;
    [Range(0f, 60f)]
    public float lightIntensity = 2f;
    [Range(0.1f, 15f)]
    public float lightRange = 1f;

    private Material runtimeMaterial;
    private bool hadEmission;
    private Color originalEmissionColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();

        // This creates a runtime instance (NOT the imported asset)
        runtimeMaterial = renderer.materials.FirstOrDefault(a => a.name.Contains("Glass"));

        // Cache original emission state
        if(runtimeMaterial.HasProperty("_EmissionColor"))
        {
            originalEmissionColor = runtimeMaterial.GetColor("EmissionColor");
            hadEmission = runtimeMaterial.IsKeywordEnabled("_EMISSION");
        }


        if(pointLight != null)
        {
            pointLight.enabled = false;
            pointLight.intensity = lightIntensity;
            pointLight.range = lightRange;
            pointLight.color = glowColor;
            pointLight.shadows = LightShadows.None;
        }
    }

    void Start()
    {
    }

    public void EnableGlow()
    {
        if (!runtimeMaterial.HasProperty("_EmissionColor"))
            return;

        runtimeMaterial.EnableKeyword("_EMISSION");

        Color finalColor = glowColor * intensity;
        runtimeMaterial.SetColor("_EmissionColor", finalColor);

        SetPointLight(true);
    }

    private void SetPointLight(bool v)
    {
        if (pointLight != null)
        {
            pointLight.color = glowColor;
            pointLight.intensity = lightIntensity;
            pointLight.range = lightRange;
            pointLight.enabled = v;
        }
    }

    public void PulsatingGlow()
    {
        if(!runtimeMaterial.HasProperty("_EmissionColor"))
            return;

        float pulse = Mathf.Sin(Time.time * 4f) * 0.5f + 0.5f;
        Color finalColor = glowColor * intensity * pulse;
        runtimeMaterial.SetColor("_EmissionColor", finalColor);

    }

    public void DisableGlow()
    {
        if(!runtimeMaterial.HasProperty("_EmissionColor"))
            return;

        if(hadEmission)
        {
            // Restore original emission
            runtimeMaterial.SetColor("_EmissionColor", originalEmissionColor);
        }
        else
        {
            // Fully disable emission
            runtimeMaterial.SetColor("_EmissionColor", Color.black);
            runtimeMaterial.DisableKeyword("_EMISSION");
        }


        if(pointLight != null)
        {
            pointLight.enabled = false;
        }
    }
}
