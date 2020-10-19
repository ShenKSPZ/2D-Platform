using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spine : MonoBehaviour
{
    public Vector2 Dir;
    Vector2 ActualDir;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ActualDir = collision.GetComponent<MainController>().ActualDir;
            if(Dir.x == 0 && ActualDir.x == 0 && Dir.y != ActualDir.y)
            {
                SendMessageUpwards("Coll", collision);
            }
            else if (Dir.y == 0 && ActualDir.y == 0 && Dir.x != ActualDir.x)
            {
                SendMessageUpwards("Coll", collision);
            }
            else if(ActualDir.x != Dir.x || ActualDir.y != Dir.y)
            {
                SendMessageUpwards("Coll", collision);
            }
            else
            {
                Debug.Log("NoThisOne");
            }
        }
    }
}
