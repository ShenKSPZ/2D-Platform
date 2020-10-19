using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoistionModify : MonoBehaviour
{

    public Transform From;
    public Transform To;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            From.position = To.position;
        }
    }
}
