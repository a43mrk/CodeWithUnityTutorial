using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    public Camera camP1;
    public Camera camP2;
    // public string layerP1 = "P1";
    // public string layerP2 = "P2";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camP1.rect = new Rect(0f, 0f, 0.5f, 1f);
        camP2.rect = new Rect(0.5f, 0f, 0.5f, 1f);

        // camP1.cullingMask = LayerMask.GetMask(layerP1);
        // camP2.cullingMask = LayerMask.GetMask(layerP2);

        // Ensure only one AudioListener
        var listeners = FindObjectsOfType<AudioListener>();

        for(int i = 1; i < listeners.Length; i++)
            listeners[i].enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
