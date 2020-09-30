using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuilding : MonoBehaviour
{
    protected virtual void Awake()
    {
        CameraMovement.OnMouseClick += MouseClicked;
    }

    private void MouseClicked(RaycastHit hitobject)
    {
        if (hitobject.collider.gameObject == gameObject)
        {
            OnSelection();
            Highlight();
        }
    }

    protected virtual void Highlight()
    {
        
    }
    
    protected virtual void OnSelection()
    {
        
    }

    protected virtual void Start()
    {
        Vector3[]  adjacents = new Vector3[4]; //frontHouse, backHouse, rightHouse, leftHouse;
        
        adjacents[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z + CityManager.Instance.DistanceBetweenHouses.z);
        adjacents[1]= new Vector3(transform.position.x, transform.position.y, transform.position.z - CityManager.Instance.DistanceBetweenHouses.z);
        adjacents[2] = new Vector3(transform.position.x + CityManager.Instance.DistanceBetweenHouses.x, transform.position.y, transform.position.z);
        adjacents[3]= new Vector3(transform.position.x - CityManager.Instance.DistanceBetweenHouses.x, transform.position.y, transform.position.z);

        RaycastHit[] results;
        
        foreach (Vector3 adjacent in adjacents)
        {
            results = Physics.SphereCastAll(adjacent, 1.5f, adjacent + Vector3.up);
            bool isAdjacentValid = true;
            foreach (RaycastHit result in results)
            {
                if (result.collider.TryGetComponent(out BaseBuilding house))
                {
                    isAdjacentValid = false;
                    break;
                }
            }

            if (isAdjacentValid)
            {
                CityManager.Instance.TrackEmptyTile(adjacent);
            }
        }
    }

    public virtual void Deselect()
    {
        
    }
}
