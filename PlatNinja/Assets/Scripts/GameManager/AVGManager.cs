using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Manage about all dialog event
public class AVGManager : MonoBehaviour
{

    public UIPanel UIPanel;
    public AVGData data;
    public float TypingSpeed;
    public GameObject TalkingObject;
    public AudioManager AM;

    [SerializeField]
    private int CurrentLine;

    string tartgetstring;
    float timervalue;

    int LastTimerValue = 0;

    public enum STATE
    {
        OFF,
        TYPING,
        PAUSED
    }

    public STATE state;

    bool justEnter;

    void Awake()
    {
        state = STATE.OFF;
        justEnter = true;
        AM = GetComponent<AudioManager>();
    }

    void Update()
    {
        switch (state)
        {
            case STATE.OFF:
                if (justEnter)
                {
                    Debug.Log("Init");
                    Init();
                    justEnter = false;
                    if(TalkingObject != null)
                    {
                        StartCoroutine(Finish());
                    }
                        
                }
                break;
            case STATE.TYPING:
                if (justEnter)
                {
                    ShowUI();
                    HideTips();
                    LoadContent(data.contents[CurrentLine].DialogText);
                    justEnter = false;
                    timervalue = 0;
                }
                CheckTypingFinished();
                UpdateContentString();
                break;
            case STATE.PAUSED:
                if (justEnter)
                {
                    ShowTips();
                    justEnter = false;
                }
                break;
            default:
                break;
        }

        if (Input.GetButtonDown("Interact"))
        {
            if (state == STATE.OFF)
            {

            }
            else
            {
                UserClicked();
            }
        }
    }

    IEnumerator Finish()
    {
        yield return new WaitForSeconds(0.1f);
        if (TalkingObject.TryGetComponent(out TalkableNPC TN))
        {
            TN.FinishTalking = true;
        }
        if (TalkingObject.TryGetComponent(out SavePointGear SPG))
        {
            SPG.FinishTalking = true;
        }
    }

    void UpdateContentString()
    {
        timervalue += Time.deltaTime * TypingSpeed;
        string tempString = tartgetstring.Substring(0, Mathf.Min((int)Mathf.Floor(timervalue), tartgetstring.Length));
        UIPanel.SetContentText(tempString);
        if((int)Mathf.Floor(timervalue) != LastTimerValue)
        {
            AM.PlayVoice();
            LastTimerValue = (int)Mathf.Floor(timervalue);
        }
    }

    public void StartAVG(AVGData adata, GameObject NPC)
    {
        data = adata;
        GoToState(STATE.TYPING);
        TalkingObject = NPC;
        UIPanel.worldPos = NPC;
    }

    public void UserClicked()
    {
        switch (state)
        {
            case STATE.OFF:
                //GoToState(STATE.TYPING);
                break;
            case STATE.TYPING:
                if (CurrentLine == 0 && timervalue >= 2)
                {
                    timervalue = tartgetstring.Length;
                }
                else if (CurrentLine != 0)
                {
                    timervalue = tartgetstring.Length;
                }
                break;
            case STATE.PAUSED:
                NextLine();
                if (CurrentLine >= data.contents.Count)
                {
                    Debug.Log("OFF" + CurrentLine.ToString());
                    GoToState(STATE.OFF);
                }
                else
                {
                    Debug.Log("TYPING" + CurrentLine.ToString());
                    GoToState(STATE.TYPING);
                }
                break;
            default:
                break;
        }
    }

    private void CheckTypingFinished()
    {
        if (state == STATE.TYPING)
        {
            if ((int)Mathf.Floor(timervalue) >= tartgetstring.Length)
            {
                GoToState(STATE.PAUSED);
            }
        }
    }

    private void GoToState(STATE next)
    {
        state = next;
        justEnter = true;
    }

    void Init()
    {
        HideUI();
        HideTips();
        CurrentLine = 0;
        UIPanel.SetContentText("");
    }

    void ShowUI()
    {
        UIPanel.ShowAll(true);
    }

    void HideUI()
    {
        UIPanel.ShowAll(false);
    }

    void ShowTips()
    {
        UIPanel.ShowTips(true);
    }

    void HideTips()
    {
        UIPanel.ShowTips(false);
    }

    void NextLine()
    {
        CurrentLine++;
    }

    void LoadContent(string Text)
    {
        tartgetstring = Text;
    }
}
