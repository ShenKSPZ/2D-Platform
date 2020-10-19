using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOVModify : MonoBehaviour
{
    [Header("相机 自动寻找")]
    public CameraManager CFC;
    public UIManager UI;
    [Header("Box")]
    public bool ChangeBox;
    public int AreaNum;
    public bool ChangeAreaTitle;
    public string AreaTitle;
    public float AreaTitleDuration;
    public float AreaTitleFadeTime;
    [Header("Damp")]
    bool Triggered3 = false;
    bool Triggered4 = false;

    void Awake()
    {
        CFC = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CameraManager>();
        UI = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UIManager>();
    }

    void FixedUpdate()
    {
        if(Triggered4)
        {
            CFC.AreaNum = AreaNum;
            Triggered4 = false;
        }

        if(Triggered3)
        {
            UI.ShowATitle(AreaTitle, AreaTitleDuration, AreaTitleFadeTime);
            Triggered3 = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (ChangeAreaTitle) Triggered3 = true;
            if (ChangeBox) Triggered4 = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (ChangeBox) Triggered4 = true;
        }
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
    }
}
