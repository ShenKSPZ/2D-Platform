using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossAI : MonoBehaviour
{
    [Header("Transform")]
    public Transform Player;
    public Transform Enemy;

    [Header("Follow")]
    public float FollowingDistance;
    public bool StartFollowing;
    public float FollowSpeed;
    public float FollowingReac;
    public bool FlipSR;
    float tempReac;

    [Header("Attack")]
    public float stageOneWaitTime = 3f;
    public float stageTwoWaitTime = 1f;
    public bool Attacking;
    public Vector2 HitBoxPosition;
    public float HitBoxRadius;
    public LayerMask PlayerHurtBox;
    public int HitBoxHurtType = 1;
    public float HitBoxDamage = 1;
    public float HitBoxInvincibleTime = 0;
    public Vector2 hitoffset;
    public bool WannaCloseAttack;
    public float CloseAttackRange;
    List<Collider2D> TotalHitColl = new List<Collider2D>();
    bool HitBoxEnabled;

    [Header("Hurt")]
    public bool Invincible = false;
    public Material PuerWhite;
    public Material Default;

    [Header("属性")]
    public float StageOneMaxHP = 3500;
    public float StageTwoMaxHP = 5000;
    public float currentHP = 3500;
    public int currentStage = 0;

    [Header("AudioManager")]
    public AudioManager AM;


    [Header("InstantiatePoint")]
    public GameObject GreenSoul;
    public Transform GreenSoulPoint;
    public Transform GreenSoulPointFlip;
    public GameObject BlueSoul;
    public Transform BlueSoulPoint;
    public Transform BlueSoulPointFlip;

    bool isHurting;
    bool JustStartFollow;
    bool LastFollowState;
    bool Flip = false;
    public bool Dying = false;
    public bool transferingStage;


    Rigidbody2D Rig;
    Animator Anim;
    SpriteRenderer SR;

    // Start is called before the first frame update
    void Awake()
    {
        Player = FindObjectOfType<MainController>().transform;
        Anim = Enemy.GetComponent<Animator>();
        AM = FindObjectOfType<AudioManager>();
        SR = Enemy.GetComponent<SpriteRenderer>();
        Rig = Enemy.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float tempdis = (Player.position - Enemy.position).magnitude;

        if (StartFollowing != LastFollowState && StartFollowing == true)
        {
            JustStartFollow = true;
        }
        else
        {
            JustStartFollow = false;
        }
        LastFollowState = StartFollowing;

        if (JustStartFollow)
        {
            currentStage++;
            StartCoroutine(StageOneStart());
        }

        if(StartFollowing && !Attacking && !isHurting && !transferingStage)
        {
            Attacking = true;
            float a = Random.Range(0, 3);//0等待 1弹幕雨 2激光

            Debug.Log(a);

            if(Player.position.x < Enemy.position.x)
            {
                Flip = true;
                if (!FlipSR)
                    SR.flipX = true;
                else
                    SR.flipX = false;
            }
            else
            {
                Flip = false;
                if (!FlipSR)
                    SR.flipX = false;
                else
                    SR.flipX = true;
            }

            if (a == 0)
            {
                Anim.SetFloat("Run", 0f);
                if (currentStage == 1)
                {
                    Attacking = false;
                }
                else
                {
                    if(!BlueSoul.activeSelf)
                    {
                        Anim.SetTrigger("BarrageAttack");
                    }
                    else
                    {
                        Attacking = false;
                    }
                }
            }
            else if(a == 1)
            {
                WannaCloseAttack = true;
            }
            else if(a == 2)
            {
                Anim.SetFloat("Run", 0f);
                Anim.SetTrigger("GreenSoulAttack");
            }
        }

        if (WannaCloseAttack)
        {
            if (tempdis < CloseAttackRange)
            {
                WannaCloseAttack = false;
                Anim.SetFloat("Run", 0f);
                AM.PlaySwordSlash();
                Anim.SetTrigger("SwordAttack");
            }
            else
            {
                Follow(Player);
            }
        }

        if (!SR.flipX)
        {
            HitBoxPosition = new Vector2(Enemy.transform.position.x, Enemy.transform.position.y) + hitoffset;
        }
        else
        {
            HitBoxPosition = new Vector2(Enemy.transform.position.x, Enemy.transform.position.y) + new Vector2(-hitoffset.x, hitoffset.y);
        }

        if (HitBoxEnabled)
        {
            Collider2D[] Coll;
            Coll = Physics2D.OverlapCircleAll(HitBoxPosition, HitBoxRadius, PlayerHurtBox);
            Vector2 ForceDir;
            if (Coll.Length != 0)
            {
                foreach (var other in Coll)
                {
                    if (!CompareCollider(TotalHitColl, other))
                    {
                        TotalHitColl.Add(other);
                        float a = Random.Range(0, 2);
                        AM.PlayHitSomeThing();
                        if (other.transform.position.x < transform.position.x)
                        {
                            ForceDir = new Vector2(-1, 1);
                        }
                        else if (other.transform.position.x > transform.position.x)
                        {
                            ForceDir = new Vector2(1, 1);
                        }
                        else
                        {
                            ForceDir = new Vector2(1, 1);
                        }
                        object[] obj = new object[4];
                        obj[0] = HitBoxHurtType;
                        obj[1] = HitBoxDamage;
                        obj[2] = HitBoxInvincibleTime;
                        obj[3] = ForceDir;
                        other.gameObject.SendMessageUpwards("GetHurt", obj);
                    }
                    else
                    {
                        
                    }
                }
            }
            else
            {
                
            }

        }
        else
        {
            TotalHitColl.Clear();
        }

        if(currentHP <= 0)
        {
            if(currentStage == 1)
            {
                Invincible = true;
                transferingStage = true;
                currentStage++;
                currentHP = StageTwoMaxHP;
                StartCoroutine(StageTwoStart());
            }
            else if(currentStage == 2 && !Dying)
            {
                Dying = true;
                transferingStage = true;
                StartCoroutine(IEDeath());
            }
        }

    }

    private void FixedUpdate()
    {
        
    }

    public void GetHurt(object[] obj)//被攻击判断 int TypeOfHurt, float Damage, float InvincibleTiming, Vector2 Forcedir
    {
        if(!Invincible)
        {
            currentHP -= (float)obj[1];
            StartCoroutine(IEHurt());
        }

    }

    public void StartFollow()
    {
        StartFollowing = true;
    }

    public void Follow(Transform target)
    {
        if (target.position.x < Enemy.position.x && tempReac < FollowingReac && !Flip)
        {
            if (JustStartFollow)
            {
                tempReac = FollowingReac;
            }
            else
                tempReac += Time.fixedDeltaTime;
            Anim.SetFloat("Run", 0f);
        }
        else if (target.position.x > Enemy.position.x && tempReac < FollowingReac && Flip)
        {
            if (JustStartFollow)
                tempReac = FollowingReac;
            else
                tempReac += Time.fixedDeltaTime;
            Anim.SetFloat("Run", 0f);
        }
        else if (target.position.x < Enemy.position.x && tempReac >= FollowingReac && !Flip)
        {
            Enemy.position = Vector2.MoveTowards(Enemy.position, new Vector2(target.position.x, Enemy.position.y), FollowSpeed * Time.fixedDeltaTime);
            Anim.SetFloat("Run", 1f);
            Flip = true;
            if (!FlipSR)
                SR.flipX = true;
            else
                SR.flipX = false;
            tempReac = 0;
        }
        else if (target.position.x > Enemy.position.x && tempReac >= FollowingReac && Flip)
        {
            Enemy.position = Vector2.MoveTowards(Enemy.position, new Vector2(target.position.x, Enemy.position.y), FollowSpeed * Time.fixedDeltaTime);
            Anim.SetFloat("Run", 1f);
            Flip = false;
            if (!FlipSR)
                SR.flipX = false;
            else
                SR.flipX = true;
            tempReac = 0;
        }
        else if (target.position.x < Enemy.position.x && Flip)
        {
            if (JustStartFollow)
                tempReac = FollowingReac;
            if (!FlipSR)
                SR.flipX = true;
            else
                SR.flipX = false;
            Enemy.position = Vector2.MoveTowards(Enemy.position, new Vector2(target.position.x, Enemy.position.y), FollowSpeed * Time.fixedDeltaTime);
            Anim.SetFloat("Run", 1f);
            tempReac = 0;
        }
        else if (target.position.x > Enemy.position.x && !Flip)
        {
            if (JustStartFollow)
                tempReac = FollowingReac;
            if (!FlipSR)
                SR.flipX = false;
            else
                SR.flipX = true;
            Enemy.position = Vector2.MoveTowards(Enemy.position, new Vector2(target.position.x, Enemy.position.y), FollowSpeed * Time.fixedDeltaTime);
            Anim.SetFloat("Run", 1f);
            tempReac = 0;
        }
    }

    IEnumerator IEHurt()
    {
        Invincible = true;
        SR.material = PuerWhite;
        yield return new WaitForSeconds(.2f);
        SR.material = Default;
        if(!transferingStage)
            Invincible = false;
    }

    IEnumerator StageOneStart()
    {
        AM.StopMusic();
        AM.PlayBossStageBridge();
        yield return new WaitForSeconds(3f);
        AM.PlayBossStageOne();
    }

    IEnumerator StageTwoStart()
    {
        AM.StopMusic();
        Anim.SetTrigger("TransferStage");
        yield return new WaitForSeconds(2.8f);
        AM.PlayBossStageBridge();
        yield return new WaitForSeconds(3f);
        AM.PlayBossStageTwo();
        currentHP = StageTwoMaxHP;
        Attacking = false;
        Invincible = false;
        transferingStage = false;
    }

    IEnumerator IEDeath()
    {
        Anim.SetTrigger("Death");
        BlueSoul.SetActive(false);
        AM.StopMusic();
        AM.PlayVictory();
        Camera.main.transform.DOShakePosition(20f, .08f, 10, 90, false, true);
        FindObjectOfType<MainController>().UnLock(false);
        FindObjectOfType<MainController>().Invincible = true;
        yield return new WaitForSecondsRealtime(4f);
        UIPanel UI = FindObjectOfType<UIPanel>();
        UI.ShowBlackScreen(1000f, 0.5f);
        yield return new WaitForSecondsRealtime(2.5f);
        UI.ShowAreaTitle("Congratulations! This is the end of the Demo", 3f, .2f);
        yield return new WaitForSecondsRealtime(5f);
        UI.ShowAreaTitle("Program / Level Design / UI / Art / Dialog" + System.Environment.NewLine + "Lawrence Peng", 4f, .2f);
        yield return new WaitForSecondsRealtime(6f);
        UI.ShowAreaTitle("Music" + System.Environment.NewLine + "iLoner", 4f, .2f);
        AM.PlayBGMusic();
        yield return new WaitForSecondsRealtime(6f);
        UI.ShowAreaTitle("Audio FX" + System.Environment.NewLine + "aigei.com", 4f, .2f);
        yield return new WaitForSecondsRealtime(6f);
        UI.ShowAreaTitle("Character Design" + System.Environment.NewLine + "Nate Kling", 4f, .2f);
        yield return new WaitForSecondsRealtime(6f);
        UI.ShowAreaTitle("Test" + System.Environment.NewLine + "Puer   BlinkTP   Mikazuk1 Augus", 5f, .2f);
        yield return new WaitForSecondsRealtime(7f);
        UI.ShowAreaTitle("Thanks for Playing", 6f, .2f);
        yield return new WaitForSecondsRealtime(11f);
        Debug.Log("Quit");
        Application.Quit();
    }

    public void BlueSoulPostion()
    {
        BlueSoul.SetActive(true);
        if (!Flip)
        {
            BlueSoul.transform.position = BlueSoulPoint.position;
        }
        else
        {
            BlueSoul.transform.position = BlueSoulPointFlip.position;
        }
        
    }

    public void AttackAnimFinish()
    {
        StartCoroutine(AnimFinish());
    }

    IEnumerator AnimFinish()
    {
        if (currentStage == 1)
        {
            yield return new WaitForSeconds(stageOneWaitTime);
            Attacking = false;
        }
        else
        {
            yield return new WaitForSeconds(stageTwoWaitTime);
            Attacking = false;
        }
    }

    public void HitBoxEnable()
    {
        HitBoxEnabled = true;
    }

    public void HitBoxDisable()
    {
        HitBoxEnabled = false;
    }

    public void InstantiateGreenSoul()
    {
        if(Flip)
        {
            Instantiate(GreenSoul, GreenSoulPointFlip.position, Quaternion.Euler(0, 0, 180));
        }
        else
        {
            Instantiate(GreenSoul, GreenSoulPoint.position, GreenSoulPoint.rotation);
        }
    }

    public bool CompareCollider(List<Collider2D> List, Collider2D other)
    {
        foreach (var c in List)
        {
            if (other == c)
                return true;
        }
        return false;
    }

    void OnDrawGizmos()//绘制Debug辅助图形
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)HitBoxPosition, HitBoxRadius);
    }
}
