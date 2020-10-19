using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject Tips;
    public Sprite Unopen;
    public Sprite Open;
    public SpriteRenderer SR;
    public 

    bool Opened;

    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
        Tips.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetAxis("Interact") == 1)
        {
            if(!Opened)
            {
                StartCoroutine(OpenDoor());
            }
        }
        else if (collision.CompareTag("Player"))
        {
            Tips.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Tips.SetActive(false);
        }
    }

    IEnumerator OpenDoor()
    {
        Opened = true;
        SR.sprite = Open;
        yield return new WaitForSeconds(.5f);
        SR.sprite = Unopen;
        Opened = false;
    }

}
