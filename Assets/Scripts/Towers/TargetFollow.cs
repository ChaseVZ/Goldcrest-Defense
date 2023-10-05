using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollow : MonoBehaviour
{
    private bool track = false;
    //private bool inside = false;
    private float dist;
    private Renderer render;
    private Vector3 mortarPosition;
    private Vector3 mousePos;
    private float range;
    
    [SerializeField] private Camera mainCamera;
    void Start()
    {
        range = gameObject.GetComponentInParent<MortarTower>().GetRange();
        mortarPosition =  gameObject.GetComponentInParent<MortarTower>().GetPosition();
        render = GetComponent<Renderer>();
        mainCamera = Camera.main;
        render.material.color = Color.red;
    }

    private void Update()
    {  
        if (track && Input.GetMouseButtonDown(0))
        {
            track = false;
            if (GameManager.instance.tutorialMode && !BuildManager.instance.usedMortarTarget) { BuildManager.instance.usedMortarTarget = true; }
        }
        else if (Input.GetMouseButtonDown(0))  
        //else if (inside && Input.GetMouseButtonDown(0))  
        {
            track = true;
        }

        if (track)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                mousePos = new Vector3(raycastHit.point.x, 0, raycastHit.point.z);
                dist = Vector3.Distance(mortarPosition, mousePos);
                if (range > dist)
                {
                    transform.position = mousePos;
                }
                else { track = false; }
            }
        }
    }

    private void OnMouseEnter()
    {
        //inside = true;
        render.material.color = Color.green;
    }

    private void OnMouseExit()
    {
        //inside = false;
        render.material.color = Color.red;
    }

    
}
