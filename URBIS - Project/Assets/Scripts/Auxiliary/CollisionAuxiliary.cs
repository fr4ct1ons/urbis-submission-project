using System;
using UnityEngine;

public class CollisionAuxiliary : MonoBehaviour
{
    public delegate void OnCollisionEvent(Collision other);
    public delegate void OnTriggerEvent(Collider other);

    public event OnTriggerEvent TriggerStay;
    public event OnTriggerEvent TriggerEnter;
    public event OnTriggerEvent TriggerExit;
    
    public event OnCollisionEvent CollisionStay;
    public event OnCollisionEvent CollisionEnter;
    public event OnCollisionEvent CollisionExit;

    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExit?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerStay?.Invoke(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        CollisionEnter?.Invoke(other);
    }

    private void OnCollisionExit(Collision other)
    {
        CollisionExit?.Invoke(other);
    }

    private void OnCollisionStay(Collision other)
    {
        CollisionStay?.Invoke(other);
    }
}
