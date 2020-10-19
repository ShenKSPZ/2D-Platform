using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChangeColor : MonoBehaviour
{
    public SpriteRenderer SR;
    public Material mPuerWhite;
    public Material mDefault;
    public bool changeColor;
    public bool Changed = false;
    public float HitStopTime = 0.2f;

    // Start is called before the first frame update
    void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
        Changed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(changeColor && !Changed)
        {
            ChangeColorToRed();
            Changed = true;
        }
        else if(!changeColor)
        {
            ChangeBack();
            Changed = false;
        }
    }

    public void ChangeColorToRed()
    {
        SR.material = mPuerWhite;
        FindObjectOfType<HitStop>().Stop(HitStopTime);
    }

    public void ChangeBack()
    {
        SR.material = mDefault;
    }
}
