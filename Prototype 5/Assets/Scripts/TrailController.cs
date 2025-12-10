using System;
using Unity.VisualScripting;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    public Camera mainCam;
    public TrailRenderer trail;
    public Transform hitcollider;

    private bool isDragging = false;

    void Start()
    {
        if(!mainCam)
            mainCam = Camera.main;


        trail.emitting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartDrag(Input.mousePosition);
        
        if(Input.GetMouseButton(0))
            UpdateDrag(Input.mousePosition);

        if(Input.GetMouseButtonUp(0))
            EndDrag();
        
        // Touch Input
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if(t.phase == TouchPhase.Began)
                StartDrag(t.position);
            
            if(t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
                UpdateDrag(t.position);

            if(t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                EndDrag();
        }
    }


    private void UpdateDrag(Vector3 position)
    {
        if(!isDragging) return;

        MoveToScreen(position);
    }

    private void EndDrag()
    {
        isDragging = false;
        trail.emitting = false;
    }

    private void StartDrag(Vector3 screenPos)
    {
        isDragging = true;
        trail.Clear();
        trail.emitting = true;

        MoveToScreen(screenPos);
    }

    private void MoveToScreen(Vector3 screenPos)
    {
        Ray r = mainCam.ScreenPointToRay(screenPos);


        // You can project on a plane or move freely in world space
        // Option A: Raycast to ground (recommended)
        if(Physics.Raycast(r, out RaycastHit hit, Mathf.Infinity))
        {
            transform.position = hit.point;
        }
        else
        {
            // fallback: simply project in front of camera
            transform.position = r.GetPoint(10f);
        }

        // Move the collider to the same position
        if(hitcollider != null)
        {
            hitcollider.position = transform.position;
        }
    }
}
