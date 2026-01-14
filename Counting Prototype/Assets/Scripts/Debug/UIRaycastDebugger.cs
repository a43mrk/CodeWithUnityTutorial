using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIRaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (!EventSystem.current) return;

        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        if (results.Count > 0)
        {
            Debug.Log("UI Hit Order:");
            foreach (var r in results)
                Debug.Log($"{r.gameObject.name} | Canvas: {r.module}");
        }
    }
}
