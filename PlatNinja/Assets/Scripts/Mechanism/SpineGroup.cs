using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineGroup : MonoBehaviour
{

    public Transform respawnPosition;
    public float HPSubtract;

    public void Coll(Collider2D collision)
    {
        object[] obj = new object[2];
        obj[0] = respawnPosition.position;
        obj[1] = HPSubtract;
        collision.SendMessage("Respawn", obj);
    }
}
