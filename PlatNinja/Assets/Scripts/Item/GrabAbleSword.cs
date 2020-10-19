using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAbleSword : MonoBehaviour
{

    public GameObject Tips;
    public MainController MC;
    public AVGManager AVG;
    public AVGData data;
    public GameObject justTips;
    public CameraManager Cam;
    Collider2D coll;

    bool PlayerInside;

    void Awake()
    {
        Tips.SetActive(false);
        AVG = FindObjectOfType<AVGManager>();
        MC = FindObjectOfType<MainController>();
        Cam = FindObjectOfType<CameraManager>();
    }

    private void Update()
    {
        if (PlayerInside && Input.GetAxis("Interact") == 1)
        {
            MainController MC = FindObjectOfType<MainController>();
            Save save = new Save
            {
                SavePointNum = 2,
                HasSword = true,
                CanWallJump = false,
                AreaNum = Cam.AreaNum
            };
            MC.hasSword = true;
            MC.currentHP = MC.MaxHP;
            FindObjectOfType<SaveLoad>().SaveGame(save);
            AVG.StartAVG(data, MC.gameObject);
            Destroy(justTips);
            Destroy(transform.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
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
