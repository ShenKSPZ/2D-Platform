using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointGear : MonoBehaviour
{
    public GameObject Tips;
    public MainController MC;
    public CameraManager Cam;
    public AVGManager AVG;
    public AVGData SuccessData;
    public AVGData FailData;
    public int SavePointNumber;
    public bool FinishTalking = true;

    Collider2D coll;
    AVGManager.STATE LastState;
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
        if (PlayerInside && Input.GetButtonDown("Interact") && FinishTalking)
        {
            FinishTalking = false;
            MainController MC = FindObjectOfType<MainController>();
            Save save = new Save
            {
                HasSword = MC.hasSword,
                CanWallJump = MC.CanWallJump,
                SavePointNum = SavePointNumber,
                AreaNum = Cam.AreaNum};
            MC.currentHP = MC.MaxHP;
            MC.currentDashEnergy = MC.MaxDashEnergy;
            bool SaveGamesuccess = FindObjectOfType<SaveLoad>().SaveGame(save);
            if(SaveGamesuccess)
                AVG.StartAVG(SuccessData, gameObject);
            else
                AVG.StartAVG(FailData, gameObject);
        }

        AVGManager.STATE NewState = AVG.state;
        if (NewState != LastState && NewState != AVGManager.STATE.OFF)
        {
            MC.UnLock(false);
        }
        else if (NewState != LastState && NewState == AVGManager.STATE.OFF)
        {
            MC.UnLock(true);
        }
        LastState = NewState;

        if (PlayerInside && AVG.state == AVGManager.STATE.OFF)
        {
            Tips.SetActive(true);
        }
        else
        {
            Tips.SetActive(false);
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
