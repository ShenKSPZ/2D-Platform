using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSRSendMSG : MonoBehaviour
{

    public BossAI BAI;
    public SpriteRenderer SR;

    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    public void AttackStartPlay()
    {
        
    }

    public void BlueSoulEnable()
    {
        BAI.BlueSoulPostion();
    }


    public void AttackPlayFinish()
    {
        BAI.AttackAnimFinish();
    }

    public void HitBoxEnable()
    {
        BAI.HitBoxEnable();
    }

    public void HitBoxDisable()
    {
        BAI.HitBoxDisable();
    }

    public void InstantiateGreenSoul()
    {
        BAI.InstantiateGreenSoul();
    }
}
