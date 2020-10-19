using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterBossRegion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            SendMessageUpwards("StartFollow");
            Destroy(gameObject);
        }
    }
}
