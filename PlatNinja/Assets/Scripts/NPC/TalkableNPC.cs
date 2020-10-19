using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkableNPC : MonoBehaviour
{

    public List<AVGData> data = new List<AVGData>();
    public GameObject Tips;
    public MainController MC;

    AVGManager AVG;

    bool PlayerInside = false;
    public bool FinishTalking = true;
    int TalkTimes = 0;
    AVGManager.STATE LastState;

    private void Awake()
    {
        AVG = FindObjectOfType<AVGManager>();
        MC = FindObjectOfType<MainController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerInside && Input.GetButtonDown("Interact") && AVG.state == AVGManager.STATE.OFF && FinishTalking)
        {
            AVG.StartAVG(data[TalkTimes], gameObject);
            FinishTalking = false;
            if(TalkTimes < data.Count - 1)
                TalkTimes++;
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInside = false;
        }
    }
}
