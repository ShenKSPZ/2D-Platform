using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBoxColliderTrigger : MonoBehaviour
{

    public string[] TriggerEnter;
    public string[] CollisionEnter;

    void OnTriggerEnter2D(Collider2D other)
    {
        foreach(var msg in TriggerEnter)
        {
            SendMessageUpwards(msg, other);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        foreach(var msg in TriggerEnter)
        {
            SendMessageUpwards(msg, other);
        }
    }
}
