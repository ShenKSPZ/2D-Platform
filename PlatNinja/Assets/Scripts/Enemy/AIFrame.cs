using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFrame : MonoBehaviour
{

    [Header("本体")]
    public Transform Enemy;
    public Transform Eyes;
    public bool FlipSR = false;
    [Header("AI调节器")]
    public bool KeepFollowing = true;
    public bool CanJump = false;
    public bool CanPatrol = true;
    [Header("移动")]
    public float FollowSpeed = 2f;
    public float PatrolSpeed = 1f;
    public bool Flip = true;
    public bool CanMove;
    public Vector2 FrameSpeed;
    float MoveSpeed;
    Vector3 LastPoint;
    [Header("引力")]
    Rigidbody2D Rig;
    [Header("反应速度")]
    public float FollowingReac = 1f;
    public float PatrolReac = 0.5f;
    public float tempReac = 0f;
    [Header("非持续跟随")]
    public float CombatRange;
    public float CombatMoveSpeed;
    [Header("巡逻 0在左")]
    public Transform[] CheckPoints;
    public int CurrentPoint = 0;
    public bool RightRoute = false;
    [Header("跟随")]
    public float CloseEnough = 4f;
    public float FollowingDistance = 15f;
    public float MaxFollowDistance = 30f;
    public float MaxYDistance = 5f;
    public bool StartFollowing;
    public LayerMask ObstaclesLayer;
    [Header("地面判定")]
    public Vector2 BottomOffset;
    public Vector2 BottomSize;
    public LayerMask GroundLayer;
    [Header("攻击")]
    public LayerMask PlayerHurtBox;
    public bool HitBoxEnable = false;
    public Vector2 HitBoxPosition;
    public float HitBoxRadius;
    float HitBoxDamage;
    float HitBoxInvincibleTime;
    int HitBoxHurtType;
    [Header("动画控制")]
    public bool HaveRun = false;
    Animator Anim;
    [Header("属性状态")]
    public float MaxHP = 300f;
    public float currentHP = 300f;
    public bool IsHurting;
    public Vector2 LightHurtForce;
    public Vector2 HeavyHurtForce;
    bool HeavyHurting;
    bool HurtRecoveryDetection;
    int HurtType = 0; //0Touch 1Light 2Heavy
    [Header("无敌帧")]
    public bool Invincible;
    float InvincibleTime;
    [Header("FX")]
    public GameObject Explosion;
    public GameObject ExclamatoryMark;
    public AudioManager AM;
    

    SpriteRenderer SR;
    Transform Player;

    bool JustStartFollow;
    bool LastFollowState;

    // Start is called before the first frame update
    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Rig = Enemy.GetComponent<Rigidbody2D>();
        SR = Enemy.GetComponent<SpriteRenderer>();
        Anim = Enemy.GetComponent<Animator>();
        AM = FindObjectOfType<AudioManager>();
        MoveSpeed = PatrolSpeed;
        LastPoint = Enemy.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //获取每帧状态
        FrameSpeed = new Vector2(Mathf.Abs(LastPoint.x - Enemy.position.x), Mathf.Abs(LastPoint.y - Enemy.position.y));
        LastPoint = Enemy.position;
        bool OnGround = IsOnGround();

        
        //巡逻
        if (!StartFollowing && CanPatrol && CanMove)
        {
            if (!RightRoute)
            {
                Follow(CheckPoints[CurrentPoint]);
                if (Enemy.position.x <= CheckPoints[CurrentPoint].position.x && CurrentPoint != 0)
                {
                    CurrentPoint--;
                }
                else if (Enemy.position.x <= CheckPoints[CurrentPoint].position.x && CurrentPoint == 0)
                {
                    RightRoute = true;
                    CurrentPoint++;
                }
            }
            else
            {
                Follow(CheckPoints[CurrentPoint]);
                if (Enemy.position.x >= CheckPoints[CurrentPoint].position.x && CurrentPoint != CheckPoints.Length - 1)
                {
                    CurrentPoint++;
                }
                else if (Enemy.position.x >= CheckPoints[CurrentPoint].position.x && CurrentPoint == CheckPoints.Length - 1)
                {
                    RightRoute = false;
                    CurrentPoint--;
                }
            }
        }
        //简易跟随
        float tempdis = Player.transform.position.x - Enemy.position.x;
        float tempYdis = Player.transform.position.y - Enemy.position.y;
        bool HaveObstacle = Physics2D.Linecast(Eyes.position, Player.position, ObstaclesLayer);
        if (CanMove && KeepFollowing)
        {
            if (Mathf.Abs(tempdis) <= FollowingDistance && Mathf.Abs(tempYdis) < MaxYDistance && !StartFollowing && !HaveObstacle)//发现玩家
            {
                if (!Flip && tempdis > 0)
                {
                    StartFollowing = true;
                    Follow(Player);
                }
                else if (Flip && tempdis < 0)
                {
                    StartFollowing = true;
                    Follow(Player);
                }
                else if(tempdis < Mathf.Abs(CloseEnough))
                {
                    StartFollowing = true;
                    Follow(Player);
                    if(tempdis < 0)
                    {
                        Flip = true;
                        if (FlipSR)
                            SR.flipX = false;
                        else
                            SR.flipX = true;
                    }
                    else
                    {
                        Flip = false;
                        if (FlipSR)
                            SR.flipX = true;
                        else
                            SR.flipX = false;
                    }
                }
            }
            else if (StartFollowing && Mathf.Abs(tempdis) <= MaxFollowDistance && Mathf.Abs(tempYdis) < MaxYDistance && !HaveObstacle)//持续跟随
            {
                Follow(Player);
            }
            else//脱离跟随
            {
                StartFollowing = false;
            }
        }
        else if (CanMove && !KeepFollowing && Mathf.Abs(tempdis) <= CombatRange)//如果属于非持续跟随AI且处于战斗距离
        {
            if (!Flip && Player.position.x >= Enemy.position.x)//朝右
            {
                int a = UnityEngine.Random.Range(0, 2);
                if (a == 0)
                {
                    Attack();
                }
                else
                {
                    Guard();
                }
            }
            else if (Flip && Player.position.x <= Enemy.position.x)//朝左
            {
                int a = UnityEngine.Random.Range(0, 2);
                if (a == 0)
                {
                    Attack();
                }
                else
                {
                    Guard();
                }
            }

        }
        else if (CanMove && !KeepFollowing && Mathf.Abs(tempdis) > CombatRange)//如果属于非持续跟随AI且不处于战斗距离
        {
            if (Mathf.Abs(tempdis) <= FollowingDistance && Mathf.Abs(tempYdis) < MaxYDistance && !StartFollowing && !HaveObstacle)//发现玩家
            {
                if (!Flip && tempdis > 0)
                {
                    StartFollowing = true;
                    Follow(Player);
                }
                else if (Flip && tempdis < 0)
                {
                    StartFollowing = true;
                    Follow(Player);
                }
            }
            else if (StartFollowing && Mathf.Abs(tempdis) <= MaxFollowDistance && Mathf.Abs(tempYdis) < MaxYDistance && !HaveObstacle)//持续跟随
            {
                Follow(Player);
            }
            else//脱离跟随
            {
                StartFollowing = false;
            }
        }

        if (StartFollowing != LastFollowState && StartFollowing == true)
        {
            JustStartFollow = true;
        }
        else
        {
            JustStartFollow = false;
        }
        LastFollowState = StartFollowing;

        if (StartFollowing)
        {
            MoveSpeed = FollowSpeed;
        }
        else MoveSpeed = PatrolSpeed;

        //TODO动画控制
        if (OnGround)
        {
            if (FrameSpeed.x < 0.003f)
            {
                Anim.SetFloat("Run", 0f);
            }
            else if (FrameSpeed.x >= 0.003f && FrameSpeed.x <= PatrolSpeed * Time.fixedDeltaTime)
            {
                Anim.SetFloat("Run", 1f);
            }
            else if (FrameSpeed.x >= 0.003f && HaveRun)
            {
                Anim.SetFloat("Run", 2f);
            }
            else if (FrameSpeed.x >= 0.003f && !HaveRun)
            {
                Anim.SetFloat("Run", 1f);
            }
        }

        //攻击碰撞开始判定
        if (HitBoxEnable)
        {
            Collider2D[] Coll;
            Coll = Physics2D.OverlapCircleAll(HitBoxPosition, HitBoxRadius, PlayerHurtBox);
            Vector2 ForceDir;
            foreach (var other in Coll)
            {
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
                    ForceDir = new Vector2(0, 1);
                }
                object[] obj = new object[4];
                obj[0] = HitBoxHurtType;
                obj[1] = HitBoxDamage;
                obj[2] = HitBoxInvincibleTime;
                obj[3] = ForceDir;
                other.gameObject.SendMessageUpwards("GetHurt", obj);
            }
        }
        //TODOAI调节器

        //TODO死亡
    }

    //TODO受伤
    public void GetHurt(object[] obj)//被攻击判断 int TypeOfHurt, float Damage, float InvincibleTiming, Vector2 Forcedir
    {
        HurtType = (int)obj[0];
        if (HurtType == 1)//0Touch 1Light 2Heavy
        {
            Anim.SetTrigger("Hurt");
            currentHP -= (float)obj[1];
            InvincibleTime = (float)obj[2];
            Rig.velocity = LightHurtForce * (Vector2)obj[3];
        }
        else if (HurtType == 2)
        {
            Anim.SetTrigger("HeavyHurt");
            HeavyHurting = true;
            HurtRecoveryDetection = false;
            currentHP -= (float)obj[1];
            InvincibleTime = (float)obj[2];
            Rig.AddForce(HeavyHurtForce * (Vector2)obj[3]);
        }
    }

    public void AnimHurtStartPlay()//受击动画开始播放
    {
        CanMove = false;
        IsHurting = true;
        Invincible = true;
        if(currentHP <= 0)
        {
            StartCoroutine(Death());
        }
    }

    public void HurtClear()//从受击状态恢复
    {
        CanMove = true;
        IsHurting = false;
        Invincible = false;
    }

    public void HurtRecoveryDetect()//从被击飞状态站起来
    {
        HurtRecoveryDetection = true;
    }

    public void Follow(Transform target)
    {
        if (target.position.x < Enemy.position.x && tempReac < FollowingReac && StartFollowing && !Flip)
        {
            if (JustStartFollow)
            {
                Debug.Log("JUST");
                tempReac = FollowingReac;
                Instantiate(ExclamatoryMark, Enemy.position + new Vector3(0, 1.2f, 0), Enemy.rotation, Enemy);
            }
            else
                tempReac += Time.fixedDeltaTime;
        }
        else if (target.position.x > Enemy.position.x && tempReac < FollowingReac && StartFollowing && Flip)
        {
            if (JustStartFollow)
            {
                Debug.Log("JUST");
                tempReac = FollowingReac;
                Instantiate(ExclamatoryMark, Enemy.position + new Vector3(0, 1.2f, 0), Enemy.rotation, Enemy);
            }
            else
                tempReac += Time.fixedDeltaTime;
        }
        else if (target.position.x < Enemy.position.x && tempReac >= FollowingReac && StartFollowing && !Flip)
        {
            Enemy.position = Vector2.MoveTowards(Enemy.position, new Vector2(target.position.x, Enemy.position.y), MoveSpeed * Time.fixedDeltaTime);
            Flip = true;
            if (!FlipSR)
                SR.flipX = true;
            else
                SR.flipX = false;
            tempReac = 0;
        }
        else if (target.position.x > Enemy.position.x && tempReac >= FollowingReac && StartFollowing && Flip)
        {
            Enemy.position = Vector2.MoveTowards(Enemy.position, new Vector2(target.position.x, Enemy.position.y), MoveSpeed * Time.fixedDeltaTime);
            Flip = false;
            if (!FlipSR)
                SR.flipX = false;
            else
                SR.flipX = true;
            tempReac = 0;
        }
        else if (target.position.x < Enemy.position.x && tempReac < PatrolReac && !StartFollowing && !Flip)
        {
            tempReac += Time.fixedDeltaTime;
        }
        else if (target.position.x > Enemy.position.x && tempReac < PatrolReac && !StartFollowing && Flip)
        {
            tempReac += Time.fixedDeltaTime;
        }
        else if (target.position.x < Enemy.position.x && tempReac >= PatrolReac && !StartFollowing && !Flip)
        {
            Enemy.position = Vector2.MoveTowards(Enemy.position, new Vector2(target.position.x, Enemy.position.y), MoveSpeed * Time.fixedDeltaTime);
            Flip = true;
            if (!FlipSR)
                SR.flipX = true;
            else
                SR.flipX = false;
            tempReac = 0;
        }
        else if (target.position.x > Enemy.position.x && tempReac >= PatrolReac && !StartFollowing && Flip)
        {
            Enemy.position = Vector2.MoveTowards(Enemy.position, new Vector2(target.position.x, Enemy.position.y), MoveSpeed * Time.fixedDeltaTime);
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
            {
                Debug.Log("JUST");
                tempReac = FollowingReac;
                Instantiate(ExclamatoryMark, Enemy.position + new Vector3(0, 1.2f, 0), Enemy.rotation, Enemy);
            }
            Flip = true;
            if (!FlipSR)
                SR.flipX = true;
            else
                SR.flipX = false;
            Enemy.position = Vector2.MoveTowards(Enemy.position, new Vector2(target.position.x, Enemy.position.y), MoveSpeed * Time.fixedDeltaTime);
            tempReac = 0;
        }
        else if (target.position.x > Enemy.position.x && !Flip)
        {
            if (JustStartFollow)
            {
                Debug.Log("JUST");
                tempReac = FollowingReac;
                Instantiate(ExclamatoryMark, Enemy.position + new Vector3(0, 1.2f, 0), Enemy.rotation, Enemy);
            }
            Flip = false;
            if (!FlipSR)
                SR.flipX = false;
            else
                SR.flipX = true;
            Enemy.position = Vector2.MoveTowards(Enemy.position, new Vector2(target.position.x, Enemy.position.y), MoveSpeed * Time.fixedDeltaTime);
            tempReac = 0;
        }
    }

    public void Attack()
    {
        Anim.SetTrigger("Attacking");
    }

    //TODO防御代码
    public void Guard()
    {
        Debug.Log("hahahahahhhaahha");
    }

    //开始播放攻击动画
    public void StartAttack(object[] obj)//Vector2 position, float radius, float Damage, int HurtType, float InvincibleTime
    {
        Anim.SetLayerWeight(Anim.GetLayerIndex("Attack"), 1f);
        HitBoxEnable = true;
        if (!Flip)
        {
            HitBoxPosition = new Vector2(Enemy.position.x, Enemy.position.y) + (Vector2)obj[0];
        }
        else
        {
            HitBoxPosition = new Vector2(Enemy.position.x, Enemy.position.y) - (Vector2)obj[0];
        }
        HitBoxRadius = (float)obj[1];
        HitBoxDamage = (float)obj[2];
        HitBoxHurtType = (int)obj[3];
        HitBoxInvincibleTime = (float)obj[4];
    }

    //攻击判定结束
    public void HitBoxDisable()
    {
        HitBoxEnable = false;
    }

    //攻击动作恢复
    public void AttackIdle()
    {
        Anim.SetLayerWeight(Anim.GetLayerIndex("Attack"), 0f);
    }

    public bool IsOnGround()
    {
        return Physics2D.OverlapBox((Vector2)Enemy.position + BottomOffset, BottomSize, 0f, GroundLayer);
    }

    IEnumerator Death()
    {
        SR.enabled = false;
        Instantiate(Explosion, Enemy.position, Enemy.rotation);
        AM.PlayExplosion();
        Destroy(gameObject);
        yield return new WaitForSeconds(0.47f);
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(Enemy.transform.position));
        Camera.main.transform.DOShakePosition(.2f, .5f, 10, 90, false, true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { BottomOffset };

        Gizmos.DrawWireCube((Vector2)Enemy.position + BottomOffset, BottomSize);
        Gizmos.DrawWireSphere((Vector2)HitBoxPosition, HitBoxRadius);
    }
}
