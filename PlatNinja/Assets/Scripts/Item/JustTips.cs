using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustTips : MonoBehaviour
{

    public GameObject Tips;
    public MainController MC;
    public AVGManager AVG;
    public AVGData data;

    Collider2D coll;

    bool PlayerInside;
    // Start is called before the first frame update
    void Awake()
    {
        Tips.SetActive(false);
        AVG = FindObjectOfType<AVGManager>();
        MC = FindObjectOfType<MainController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (PlayerInside && Input.GetAxis("Interact") == 1)
        {
            AVG.StartAVG(data, MC.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            coll = collision;
            PlayerInside = true;
            Tips.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Tips.SetActive(false);
            PlayerInside = false;
        }
    }
}
