using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAbleWallJump : MonoBehaviour
{
    public GameObject Tips;
    public MainController MC;
    public AVGManager AVG;
    public AVGData data;
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
                SavePointNum = 3,
                HasSword = true,
                CanWallJump = true,
                AreaNum = Cam.AreaNum
            };
            MC.CanWallJump = true;
            MC.currentHP = MC.MaxHP;
            FindObjectOfType<SaveLoad>().SaveGame(save);
            AVG.StartAVG(data, MC.gameObject);
            Destroy(transform.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
