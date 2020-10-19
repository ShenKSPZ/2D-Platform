using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//UIManager: Manage about all clicking and overall UI related event. 
public class UIManager : MonoBehaviour
{

    public AVGManager aVGManager;
    public UIPanel ActualUI;

    

    void Awake()
    {
        aVGManager = GetComponent<AVGManager>();
    }

    void Update()
    {
        

        //if(Input.GetKeyDown("1"))
        //{
        //    aVGManager.StartAVG();
        //}
    }

    public void ShowATitle(string Text, float Time, float FadeTime)
    {
        ActualUI.ShowAreaTitle(Text, Time, FadeTime);
    }

    public void ShowBScreen(float Time, float FadeTime)
    {
        ActualUI.ShowBlackScreen(Time, FadeTime);
    }
}
