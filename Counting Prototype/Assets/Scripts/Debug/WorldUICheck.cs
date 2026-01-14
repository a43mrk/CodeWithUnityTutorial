
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldUICheck : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("World Space UI hit!");
    }
}
