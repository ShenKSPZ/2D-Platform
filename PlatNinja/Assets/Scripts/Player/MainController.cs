using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainController : MonoBehaviour
{
    [Header("帧速度")]
    public Vector2 FrameSpeed;
    public Vector2 ActualDir;

    [Header("触地判定")]
    public LayerMask GroundLayer;
    public bool OnGround;

    [Header("触墙判定")]
    public Vector2 BottomOffset;
    public Vector3 BottomSize;
    public Vector2 RightOffset;
    public Vector2 LeftOffset;
    public Vector3 Size;
    public float WallJumpSpeedX = 2f;
    public float WallJumpSpeedY = 2f;
    public float WallFallSpeed = 2f;
    public bool WallJumped = false;
    public float WallJumpTime = 0.2f;
    public float WallLerp = 5f;
    public bool CanWallJump;
    public bool CanOnWall;
    public bool OnWall;
    public bool OnLeftWall;
    public bool OnRightWall;
    public bool GroundTouched;

    [Header("引力")]
    public float FallMultiplier = 2.5f;
    public float LowJumpMultiplier = 2f;
    public float MaxFallSpeed;
    bool BetterJumpingEnable = true;
    Rigidbody2D Rig;

    [Header("跳跃")]
    public float JumpingSpeed = 1f;
    bool IsJumping = false;
    bool CanJump = true;

    [Header("移动")]
    public float MovingSpeed = 50f;
    public float AccelerateSpeed = 0.1f;
    public float DecelerateSpeed = 0.06f;
    public bool Flip;
    public Vector2 dirOffest;
    Vector2 dir;
    public bool CanMove = true;
    float DampVelocity1;

    [Header("冲刺")]
    public float DashSpeed = 20f;
    public float DashXMultiplier = 1.2f;
    public float DashDrag = 10f;
    public float DragDuration = 0.8f;
    public float DashEnergy = 30f;
    public float MaxDashEnergy = 90f;
    public float currentDashEnergy = 0f;
    public float DashEnergyRecoveryTime = 15f; //PerOneSecond
    public Slider EnergyBar;
    bool WasDashed = false;
    bool IsDashing = false;
    bool DashEnergyRecoverying = false;
    bool CanDash = true;
    public bool CanEnergyRecovery = true;

    [Header("攻击")]
    public float AttackingMoveSpeed;
    public LayerMask EnemyHurtBox;
    public bool hasSword;
    public bool CanAttack;
    public Vector2 hitoffset;
    public Vector2 hitReactionForce;
    bool AttackingButton;
    bool HeavyAttackingButton;
    bool LastAttack;
    bool LastHeavyAttack;
    List<Collider2D> TotalHitColl = new List<Collider2D>();

    [Header("受击")]
    public Vector2 HitBoxPosition;
    public float HitBoxRadius;
    float HitBoxDamage;
    int HitBoxHurtType;
    float HitBoxInvincibleTime;

    [Header("属性状态")]
    public bool Locked;
    public float MaxHP = 5f;
    public float currentHP = 5f;
    public bool IsHurting;
    public Vector2 TouchHurtForce;
    public Vector2 LightHurtForce;
    public Vector2 HeavyHurtForce;
    public bool HitBoxEnable;
    public HeartManager HM;
    bool HeavyHurting;
    bool HurtRecoveryDetection;
    int HurtType = 0; //0Touch 1Light 2Heavy

    [Header("无敌帧")]
    public bool Invincible;
    public float InvincibleTime;

    [Header("动画控制")]
    public Transform Character;
    [HideInInspector]
    public Animator Anim;
    [HideInInspector]
    public SpriteRenderer SR;

    [Header("高空落地")]
    public float BigFallTime;
    public float BigFallRecoveryTime;
    float FallTime;

    [Header("机关")]
    public bool Ejecting;

    [Header("粒子效果")]
    public ParticleSystem JumpParticle;
    public ParticleSystem WallJumpParticle;
    public ParticleSystem SlideParticle;
    public ParticleSystem BigFallPartical;
    public ParticleSystem DashPartical;
    public ParticleSystem WalkPartical;

    [Header("FX")]
    public GameObject SlashFX1;
    public GameObject SlashFX2;
    public GameObject PlayerExplosion;

    [Header("AudioManager")]
    AudioManager AM;

    private int Xbox_One_Controller = 0;

    private int PS4_Controller = 0;

    void Awake()
    {
        Rig = GetComponent<Rigidbody2D>();
        Anim = Character.GetComponent<Animator>();
        SR = Character.GetComponent<SpriteRenderer>();
        AM = FindObjectOfType<AudioManager>();
        SlideParticle.Stop();
        DashPartical.Stop();
        WalkPartical.Stop();
        currentDashEnergy = MaxDashEnergy;
    }

    private void Update()
    {
        string[] names = Input.GetJoystickNames();

        for (int x = 0; x < names.Length; x++)
        {
            if (names[x].Length == 19)
            {
                PS4_Controller = 1;
                Xbox_One_Controller = 0;
            }
            else if (names[x].Length == 33)
            {
                PS4_Controller = 0;
                Xbox_One_Controller = 1;
            }
            else
            {
                PS4_Controller = 0;
                Xbox_One_Controller = 0;
            }
        }

        if (!Flip)
        {
            HitBoxPosition = new Vector2(transform.position.x, transform.position.y) + hitoffset;
        }
        else
        {
            HitBoxPosition = new Vector2(transform.position.x, transform.position.y) + new Vector2(-hitoffset.x, hitoffset.y);
        }

        if (hasSword)
        {
            Anim.SetLayerWeight(1, 1f);
        }
        else
        {
            Anim.SetLayerWeight(1, 0f);
        }
    }

    void FixedUpdate()
    {
        //每帧的碰撞判定
        OnGround = IsOnGround();
        OnLeftWall = IsOnLeftWall();
        OnRightWall = IsOnRightWall();
        OnWall = OnLeftWall || OnRightWall;

        //取得帧速度，单位：Unity单位距离/帧
        FrameSpeed = Rig.velocity;

        ActualDir = GetActualDir(Rig.velocity);

        //跳跃
        if (Input.GetAxis("Jump") > 0 && OnGround && IsJumping == false && CanJump)
        {
            Rig.velocity = new Vector2(Rig.velocity.x, JumpingSpeed);

            IsJumping = true;

            Anim.SetTrigger("Jump");

            JumpParticle.Play();
        }
        if(!OnGround)
        {
            Anim.SetBool("OnGround",false);
            WalkPartical.Stop();
        }

        //弹墙跳
        if (OnWall && !OnGround && Input.GetAxis("Jump") > 0 && WallJumped == false && IsJumping == false && CanWallJump)
        {
            WallJumped = true;
            IsJumping = true;
            if (OnLeftWall && OnRightWall)
            {
                //被加载中间或者是头顶墙会触发这里的代码，一般什么都不做
            }
            else if (OnLeftWall)
            {
                Rig.velocity = new Vector2(WallJumpSpeedX, WallJumpSpeedY);
            }
            else if (OnRightWall)
            {
                Rig.velocity = new Vector2(WallJumpSpeedX * -1, WallJumpSpeedY);
            }
        }

        //引力调整器
        if (BetterJumpingEnable == true)
        {
            if (Rig.velocity.y < 0)
            {
                Rig.velocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (Rig.velocity.y > 0 && Input.GetAxis("Jump") != 1)
            {
                Rig.velocity += Vector2.up * Physics2D.gravity.y * (LowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        //最大下落速度限制
        if(Rig.velocity.y <= MaxFallSpeed)
        {
            Rig.velocity = new Vector2(Rig.velocity.x, MaxFallSpeed);
            Rig.gravityScale = 0;
        }
        else if(!IsDashing)
        {
            Rig.gravityScale = 1;
        }

        //判断不同的手柄
        if(Xbox_One_Controller == 1)
        {
            //获取朝向
            if (Input.GetAxis("Horizontal") > dirOffest.x || Input.GetAxis("XBOXHorizontal") > dirOffest.x)
            {
                dir.x = 1;
            }
            else if (Input.GetAxis("Horizontal") < -dirOffest.x || Input.GetAxis("XBOXHorizontal") < -dirOffest.x)
            {
                dir.x = -1;
            }
            else
            {
                dir.x = 0;
            }

            if (Input.GetAxis("Vertical") > dirOffest.y || Input.GetAxis("XBOXVertical") > dirOffest.y)
            {
                dir.y = 1;
            }
            else if (Input.GetAxis("Vertical") < -dirOffest.y || Input.GetAxis("XBOXVertical") < -dirOffest.y)
            {
                dir.y = -1;
            }
            else
            {
                dir.y = 0;
            }

            //左右移动
            if (CanMove)
            {
                if ((Input.GetAxis("Horizontal") > 0 || Input.GetAxis("XBOXHorizontal") > 0) && !OnRightWall)
                {
                    if (!WallJumped)
                    {
                        if (Rig.velocity.x < MovingSpeed * 50 * Time.fixedDeltaTime)
                        {
                            Rig.velocity = new Vector2(Mathf.SmoothDamp(Rig.velocity.x, MovingSpeed * 50 * Time.fixedDeltaTime, ref DampVelocity1, AccelerateSpeed), Rig.velocity.y);
                        }
                        Flip = false;
                        SR.flipX = false;
                    }
                    else
                    {
                        Rig.velocity = Vector2.Lerp(Rig.velocity, new Vector2(MovingSpeed * 50 * Time.fixedDeltaTime, Rig.velocity.y), WallLerp * Time.fixedDeltaTime);
                        Flip = false;
                        SR.flipX = false;
                    }
                    Anim.SetFloat("Run", 1);
                }
                else if ((Input.GetAxis("Horizontal") < 0 || Input.GetAxis("XBOXHorizontal") < 0) && !OnLeftWall)
                {
                    if (!WallJumped)
                    {
                        if (Rig.velocity.x > MovingSpeed * -50 * Time.fixedDeltaTime)
                        {
                            Rig.velocity = new Vector2(Mathf.SmoothDamp(Rig.velocity.x, MovingSpeed * -50 * Time.fixedDeltaTime, ref DampVelocity1, AccelerateSpeed), Rig.velocity.y);
                        }
                        Flip = true;
                        SR.flipX = true;
                    }
                    else
                    {
                        Rig.velocity = Vector2.Lerp(Rig.velocity, new Vector2(MovingSpeed * -50 * Time.fixedDeltaTime, Rig.velocity.y), WallLerp * Time.fixedDeltaTime);
                        Flip = true;
                        SR.flipX = true;
                    }
                    Anim.SetFloat("Run", 1);
                }
            }

            if (((Input.GetAxis("Horizontal") == 0 && Input.GetAxis("XBOXHorizontal") == 0) && !WallJumped && !IsDashing) || !CanMove && !IsDashing)
            {
                Rig.velocity = new Vector2(Mathf.SmoothDamp(Rig.velocity.x, 0, ref DampVelocity1, DecelerateSpeed), Rig.velocity.y);
                Anim.SetFloat("Run", 0);
            }
        }
        else if(PS4_Controller == 1)
        {
            if (Input.GetAxis("Horizontal") > dirOffest.x || Input.GetAxis("PS4Horizontal") > dirOffest.x)
            {
                dir.x = 1;
            }
            else if (Input.GetAxis("Horizontal") < -dirOffest.x || Input.GetAxis("PS4Horizontal") < -dirOffest.x)
            {
                dir.x = -1;
            }
            else
            {
                dir.x = 0;
            }

            if (Input.GetAxis("Vertical") > dirOffest.y || Input.GetAxis("PS4Vertical") > dirOffest.y)
            {
                dir.y = 1;
            }
            else if (Input.GetAxis("Vertical") < -dirOffest.y || Input.GetAxis("PS4Vertical") < -dirOffest.y)
            {
                dir.y = -1;
            }
            else
            {
                dir.y = 0;
            }

            //左右移动
            if (CanMove)
            {
                if ((Input.GetAxis("Horizontal") > 0 || Input.GetAxis("PS4Horizontal") > 0) && !OnRightWall)
                {
                    if (!WallJumped)
                    {
                        if (Rig.velocity.x < MovingSpeed * 50 * Time.fixedDeltaTime)
                        {
                            Rig.velocity = new Vector2(Mathf.SmoothDamp(Rig.velocity.x, MovingSpeed * 50 * Time.fixedDeltaTime, ref DampVelocity1, AccelerateSpeed), Rig.velocity.y);
                        }
                        Flip = false;
                        SR.flipX = false;
                    }
                    else
                    {
                        Rig.velocity = Vector2.Lerp(Rig.velocity, new Vector2(MovingSpeed * 50 * Time.fixedDeltaTime, Rig.velocity.y), WallLerp * Time.fixedDeltaTime);
                        Flip = false;
                        SR.flipX = false;
                    }
                    Anim.SetFloat("Run", 1);
                }
                else if ((Input.GetAxis("Horizontal") < 0 || Input.GetAxis("PS4Horizontal") < 0) && !OnLeftWall)
                {
                    if (!WallJumped)
                    {
                        if (Rig.velocity.x > MovingSpeed * -50 * Time.fixedDeltaTime)
                        {
                            Rig.velocity = new Vector2(Mathf.SmoothDamp(Rig.velocity.x, MovingSpeed * -50 * Time.fixedDeltaTime, ref DampVelocity1, AccelerateSpeed), Rig.velocity.y);
                        }
                        Flip = true;
                        SR.flipX = true;
                    }
                    else
                    {
                        Rig.velocity = Vector2.Lerp(Rig.velocity, new Vector2(MovingSpeed * -50 * Time.fixedDeltaTime, Rig.velocity.y), WallLerp * Time.fixedDeltaTime);
                        Flip = true;
                        SR.flipX = true;
                    }
                    Anim.SetFloat("Run", 1);
                }
            }

            if (((Input.GetAxis("Horizontal") == 0 && Input.GetAxis("PS4Horizontal") == 0) && !WallJumped && !IsDashing) || !CanMove && !IsDashing)
            {
                Rig.velocity = new Vector2(Mathf.SmoothDamp(Rig.velocity.x, 0, ref DampVelocity1, DecelerateSpeed), Rig.velocity.y);
                Anim.SetFloat("Run", 0);
            }
        }
        else
        {
            if (Input.GetAxis("Horizontal") > dirOffest.x)
            {
                dir.x = 1;
            }
            else if (Input.GetAxis("Horizontal") < -dirOffest.x)
            {
                dir.x = -1;
            }
            else
            {
                dir.x = 0;
            }

            if (Input.GetAxis("Vertical") > dirOffest.y)
            {
                dir.y = 1;
            }
            else if (Input.GetAxis("Vertical") < -dirOffest.y)
            {
                dir.y = -1;
            }
            else
            {
                dir.y = 0;
            }

            //左右移动
            if (CanMove)
            {
                if (Input.GetAxis("Horizontal") > 0 && !OnRightWall)
                {
                    if (!WallJumped)
                    {
                        if (Rig.velocity.x < MovingSpeed * 50 * Time.fixedDeltaTime)
                        {
                            Rig.velocity = new Vector2(Mathf.SmoothDamp(Rig.velocity.x, MovingSpeed * 50 * Time.fixedDeltaTime, ref DampVelocity1, AccelerateSpeed), Rig.velocity.y);
                        }
                        Flip = false;
                        SR.flipX = false;
                    }
                    else
                    {
                        Rig.velocity = Vector2.Lerp(Rig.velocity, new Vector2(MovingSpeed * 50 * Time.fixedDeltaTime, Rig.velocity.y), WallLerp * Time.fixedDeltaTime);
                        Flip = false;
                        SR.flipX = false;
                    }
                    Anim.SetFloat("Run", 1);
                }
                else if (Input.GetAxis("Horizontal") < 0 && !OnLeftWall)
                {
                    if (!WallJumped)
                    {
                        if (Rig.velocity.x > MovingSpeed * -50 * Time.fixedDeltaTime)
                        {
                            Rig.velocity = new Vector2(Mathf.SmoothDamp(Rig.velocity.x, MovingSpeed * -50 * Time.fixedDeltaTime, ref DampVelocity1, AccelerateSpeed), Rig.velocity.y);
                        }
                        Flip = true;
                        SR.flipX = true;
                    }
                    else
                    {
                        Rig.velocity = Vector2.Lerp(Rig.velocity, new Vector2(MovingSpeed * -50 * Time.fixedDeltaTime, Rig.velocity.y), WallLerp * Time.fixedDeltaTime);
                        Flip = true;
                        SR.flipX = true;
                    }
                    Anim.SetFloat("Run", 1);
                }
            }

            if ((Input.GetAxis("Horizontal") == 0 && !WallJumped && !IsDashing) || !CanMove && !IsDashing)
            {
                Rig.velocity = new Vector2(Mathf.SmoothDamp(Rig.velocity.x, 0, ref DampVelocity1, DecelerateSpeed), Rig.velocity.y);
                Anim.SetFloat("Run", 0);
            }
        }
        

        if(dir == Vector2.zero)
        {
            if(OnGround)
            {
                if (Flip == false)
                {
                    dir.x = 1;
                }
                else
                {
                    dir.x = -1;
                }
            }
            else
            {
                dir.y = 1;
            }
            
        }

        //是否有武器判定
        if(hasSword && !Locked)
        {
            CanAttack = true;
        }

        //轻攻击按键输入判定
        bool NewAttack = false;
        if (Input.GetAxis("Attack") == 0) NewAttack = false;
        if (Input.GetAxis("Attack") != 0 && CanAttack) NewAttack = true;
        if (NewAttack != LastAttack && NewAttack == true)
        {
            AttackingButton = true;
        }
        else
        {
            AttackingButton = false;
        }
        LastAttack = NewAttack;

        //重攻击按键输入判定
        //bool NewHeavyAttack = false;
        //if (Input.GetAxis("HeavyAttack") == 0) NewHeavyAttack = false;
        //if (Input.GetAxis("HeavyAttack") != 0) NewHeavyAttack = true;
        //if (NewHeavyAttack != LastHeavyAttack && NewHeavyAttack == true)
        //{
        //    HeavyAttackingButton = true;
        //}
        //else
        //{
        //    HeavyAttackingButton = false;
        //}
        //LastHeavyAttack = NewHeavyAttack;

        //攻击动画播放
        if (AttackingButton && !IsDashing && !IsHurting && hasSword)
        {
            //AttackDir= 0Right 1Down 2Up
            if(dir.y == 1)Anim.SetFloat("AttackDir", 2);
            if(dir.y == -1 && !OnGround)Anim.SetFloat("AttackDir", 1);
            if(dir.y == 0)Anim.SetFloat("AttackDir", 0);
            Anim.SetTrigger("Attacking");
        }

        //if(HeavyAttackingButton && !IsDashing && !IsHurting)
        //{
        //    Anim.SetTrigger("HeavyAttacking");
        //}

        //冲刺
        if (Input.GetAxis("Dash") > 0 && WasDashed == false && CanDash)
        {
            WasDashed = true;
            Rig.velocity = Vector2.zero;
            Vector2 DashDir = Vector2.zero;
            DashDir = new Vector2(dir.x * DashXMultiplier, dir.y);
            AM.PlayDash();
            if (dir.y == 1)
            {
                FallTime = 0;
            }

            Camera.main.transform.DOShakePosition(.2f, .3f, 10, 90, false, true);
            FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

            Rig.velocity += DashDir.normalized * DashSpeed;
            StartCoroutine(DashWait());
        }

        if(CanEnergyRecovery && !DashEnergyRecoverying)
        {
            StartCoroutine(DashEnergyRecovery());
        }
        EnergyBar.value = currentDashEnergy / MaxDashEnergy;

        //蹭墙
        if (OnWall && !OnGround && Rig.velocity.y <= 0 && dir.y != -1 && CanOnWall)
        {
            if (OnLeftWall)
            {
                Rig.velocity = new Vector2(Rig.velocity.x, -WallFallSpeed * Time.fixedDeltaTime * 50);
                Flip = false;
                SR.flipX = false;
            }
            else
            {
                Rig.velocity = new Vector2(Rig.velocity.x, -WallFallSpeed * Time.fixedDeltaTime * 50);
                Flip = true;
                SR.flipX = true;
            }
            SlideParticle.transform.parent.localPosition = new Vector3(ParticleSide(), -0.45f, 0);
            Anim.SetBool("OnWall", true);
            FallTime = 0;
            if (!SlideParticle.isPlaying)
            {
                SlideParticle.Play();
            }
        }
        else if (OnWall && !OnGround && Rig.velocity.y <= 0 && dir.y == -1)
        {
            if (!SlideParticle.isPlaying)
            {
                SlideParticle.Play();
            }
        }
        else 
        {
            Anim.SetBool("OnWall", false);
            SlideParticle.Stop();
        }

        //下落动画处理
        if(Rig.velocity.y <= 0)
        {
            Anim.SetBool("Fall", true);
        }
        else
        {
            Anim.SetBool("Fall", false);
        }

        //触地重置
        if (OnGround && Input.GetAxis("Jump") != 1)
        {
            IsJumping = false;
        }
        if (OnGround && Input.GetAxis("Dash") != 1)
        {
            WasDashed = false;
        }

        if(OnGround && !GroundTouched)
        {
            if (FallTime >= BigFallTime)
            {
                BigTouchGround();
                StartCoroutine(Recovery(BigFallRecoveryTime));
            }
            else
            {
                TouchGround();
            }
        }
        else if(!OnGround)
        {
            GroundTouched = false;
        }

        if (OnGround && !GroundTouched)
        {
            GroundTouched = true;
        }

        if (OnGround)
        {
            WallJumped = false;
            Anim.SetBool("OnGround", true);
            FallTime = 0;
            if (HeavyHurting && HurtRecoveryDetection)
            {
                Anim.SetTrigger("Recover");
                HeavyHurting = false;
            }
        }
        //完全滞空判定
        if (!OnGround && Rig.velocity.y <= 0)
        {
            FallTime += Time.fixedDeltaTime;
        }

        //蹭墙重置
        if(OnWall && Input.GetAxis("Jump") != 1)
        {
            WallJumped = false;
            IsJumping = false;
        }
        
        //攻击判定框
        if(HitBoxEnable)
        {
            Collider2D[] Coll;
            Coll = Physics2D.OverlapCircleAll(HitBoxPosition, HitBoxRadius, EnemyHurtBox);
            Vector2 ForceDir;
            if (Coll.Length != 0)
            {
                foreach (var other in Coll)
                {
                    if(!CompareCollider(TotalHitColl, other))
                    {
                        TotalHitColl.Add(other);
                        Camera.main.transform.DOShakePosition(.2f, .6f, 10, 90, false, true);
                        float a = Random.Range(0, 2);
                        AM.PlayHitSomeThing();
                        if (a == 0)
                        {
                            Instantiate(SlashFX1, other.transform.position, other.transform.rotation);
                        }
                        else
                        {
                            Instantiate(SlashFX2, other.transform.position, other.transform.rotation);
                        }
                        if(Flip)
                        {
                            Rig.velocity = hitReactionForce;
                        }
                        else
                        {
                            Rig.velocity = new Vector2(hitReactionForce.x * -1, hitReactionForce.y);
                        }
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
    }

    //特效处理
    //碰地
    void TouchGround() {
        PlayPartical(JumpParticle);
        PlayPartical(WalkPartical);
        Camera.main.transform.DOShakePosition(.05f, .05f, 1, 90, false, true);
        AM.PlayTouchedGround();
    }

    //高空落地
    void BigTouchGround()
    {
        PlayPartical(BigFallPartical);
        PlayPartical(WalkPartical);
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
        Camera.main.transform.DOShakePosition(.3f, .7f, 10, 90, false, true);
        AM.PlayBigFallTouchedGround();
    }

    //播放粒子特效的方法
    void PlayPartical(ParticleSystem partical)
    {
        if (!partical.isPlaying)
        {
            partical.Play();
        }
        else
        {
            partical.Stop();
            partical.Play();
        }
    }

    //返回墙面粒子位置
    float ParticleSide()
    {
        return OnLeftWall ? -0.45f : 0.45f;
    }

    //碰撞检测
    public bool IsOnGround()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + BottomOffset, BottomSize, 0f, GroundLayer);
    }

    public bool IsOnRightWall()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + RightOffset, Size, 0f,GroundLayer);
    }

    public bool IsOnLeftWall()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + LeftOffset, Size, 0f, GroundLayer);
    }

    //调整空气阻力
    public void RigidbodyDrag(float x)
    {
        Rig.drag = x;
    }

    //攻击动画，由动画控制器呼叫
    public void StartAttack(object[] obj)//开始播放攻击动画 0 Vector2 position, 1 float radius, 2 float Damage, 3 int HurtType, 4 float InvincibleTime
    {
        CanJump = false;
        CanDash = false;
        HitBoxEnable = true;
        hitoffset = (Vector2)obj[0];
        HitBoxRadius = (float)obj[1];
        HitBoxDamage = (float)obj[2];
        HitBoxHurtType = (int)obj[3];
        HitBoxInvincibleTime = (float)obj[4];
        AM.PlaySwordSlash();
    }

    //攻击判定结束，由动画控制器呼叫
    public void HitBoxDisable()
    {
        HitBoxEnable = false;
    }

    //从攻击状态恢复
    public void AttackIdle()
    {
        CanJump = true;
        CanDash = true;
    }

    //受伤判定
    public void HurtBoxEnter(Collider2D other)//触碰敌人判断
    {
        if(other.CompareTag("Enemy") && !Invincible)
        {
            HurtType = 0;//0Touch 1Light 2Heavy
            Anim.SetTrigger("Hurt");
            currentHP --;
            float a = Random.Range(0, 2);
            if (a == 0)
            {
                Instantiate(SlashFX1, Character.transform.position, Character.transform.rotation);
            }
            else
            {
                Instantiate(SlashFX2, Character.transform.position, Character.transform.rotation);
            }
            if(currentHP <= 0)
            {
                StartCoroutine(IEDeath());
            }
        }
    }

    //被攻击判断 int TypeOfHurt, float Damage, float InvincibleTiming, Vector2 Forcedir
    public void GetHurt(object[] obj)
    {
        if(!Invincible)
        {
            HurtType = (int)obj[0];
            Vector2 ForceDir = (Vector2)obj[3];
            if(HurtType == 1)//0Touch 1Light 2Heavy
            {
                Anim.SetTrigger("Hurt");
                currentHP -= (float)obj[1];
                float a = Random.Range(0, 2);
                if (a == 0)
                {
                    Instantiate(SlashFX1, Character.transform.position, Character.transform.rotation);
                }
                else
                {
                    Instantiate(SlashFX2, Character.transform.position, Character.transform.rotation);
                }
                Rig.velocity = LightHurtForce * ForceDir;
            }

            if (currentHP <= 0)
            {
                StartCoroutine(IEDeath());
            }
        }
    }

    //受击动画开始播放
    public void AnimHurtStartPlay()
    {
        DampVelocity1 = 0;
        HurtLock(true);
        Camera.main.transform.DOShakePosition(.5f, .5f, 10, 90, false, true);
        //if (HurtType == 0)
        //{
        //    if (Flip == false) Rig.AddForce(new Vector2(TouchHurtForce.x * -1, TouchHurtForce.y));
        //    else Rig.AddForce(TouchHurtForce);
        //}

        if (HurtType == 0)
        {
            if (Flip == false) Rig.velocity = new Vector2(TouchHurtForce.x * -1, TouchHurtForce.y);
            else Rig.velocity = TouchHurtForce;
        }
    }

    //从受击状态恢复
    public void HurtClear()
    {
        HurtLock(false);
    }

    void HurtLock(bool enabled)
    {
        CanMove = !enabled;
        CanJump = !enabled;
        CanDash = !enabled;
        CanEnergyRecovery = !enabled;
        CanWallJump = !enabled;
        BetterJumpingEnable = !enabled;
        Invincible = enabled;
        IsHurting = enabled;
    }

    //从被击飞状态站起来
    public void HurtRecoveryDetect()
    {
        HurtRecoveryDetection = true;
    }

    //检查目前动画控制器播放的是什么动画
    public bool CheckStateName(string StateName, string LayerName = "Base Layer")
    {
        return Anim.GetCurrentAnimatorStateInfo(Anim.GetLayerIndex(LayerName)).IsName(StateName);
    }

    //检查目前播放动画的tap
    public bool CheckStateTag(string StateTag, string LayerName = "Base Layer")
    {
        return Anim.GetCurrentAnimatorStateInfo(Anim.GetLayerIndex(LayerName)).IsTag(StateTag);
    }

    //比较other是否属于List中的一员
    public bool CompareCollider(List<Collider2D> List, Collider2D other)
    {
        foreach(var c in List)
        {
            if (other == c)
                return true;
        }
        return false;
    }

    //弹射，由弹射跳板呼叫
    public void Eject(Vector2 Direction)
    {
        Rig.velocity = new Vector2(Direction.x * 30, Direction.y * 17);

        if (Direction.x != 0)
            StartCoroutine(EjectXWait());
        else
            StartCoroutine(EjectYWait());

        IsJumping = true;

        Anim.SetTrigger("Jump");

        JumpParticle.Play();

        
        FallTime = 0;
    }
    
    //碰钉子重生，由钉子呼叫
    public void Respawn(object[] obj)//0RespawnPosition 1HPSubtract
    {
        if(!Invincible || IsDashing)
        {
            Invincible = true;
            currentHP -= (float)obj[1];
            IsDashing = false;
        }
        if(currentHP <= 0)
        {
            StartCoroutine(IEDeath());
        }
        else
        {
            Vector2 pp = (Vector3)obj[0];
            StartCoroutine(IERespawn(pp));
        }
    }

    //额外线程 带有X轴的弹射
    IEnumerator EjectXWait()
    {
        DOVirtual.Float(DashDrag, 0f, DragDuration, RigidbodyDrag);
        PlayPartical(DashPartical);
        BetterJumpingEnable = false;
        CanMove = false;
        WallJumped = true;
        IsDashing = true;
        yield return new WaitForSeconds(.3f);
        DashPartical.Stop();
        BetterJumpingEnable = true;
        CanMove = true;
        WallJumped = false;
        IsDashing = false;
    }

    //额外线程 不带有X轴的弹射
    IEnumerator EjectYWait()
    {
        DOVirtual.Float(.3f, 0f, DragDuration, RigidbodyDrag);
        PlayPartical(DashPartical);
        yield return new WaitForSeconds(.3f);
        DashPartical.Stop();
    }

    //额外线程 能量恢复
    IEnumerator DashEnergyRecovery()
    {
        DashEnergyRecoverying = true;
        if (currentDashEnergy < MaxDashEnergy)
        {
            if (IsDashing)
            {
                yield return new WaitForSeconds(1.5f);
            }
            else if (currentDashEnergy == 0)
            {
                yield return new WaitForSeconds(1.5f);
                currentDashEnergy += DashEnergyRecoveryTime * Time.fixedDeltaTime;
            }
            else
            {
                currentDashEnergy += DashEnergyRecoveryTime * Time.fixedDeltaTime;
            }
        }
        DashEnergyRecoverying = false;
    }

    //额外线程 冲刺时禁止其他动作并增加空气阻力
    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());
        if (dir.y >= 0 || dir.x != 0)
        {
            DOVirtual.Float(DashDrag, 0f, DragDuration, RigidbodyDrag);
        }

        PlayPartical(DashPartical);
        BetterJumpingEnable = false;
        CanMove = false;
        Rig.gravityScale = 0;
        WallJumped = true;
        IsDashing = true;

        if (currentDashEnergy > DashEnergy)
        {
            currentDashEnergy -= DashEnergy;
            Invincible = true;
            FindObjectOfType<GhostTrail>().ShowGhost();
        }
        else if (currentDashEnergy <= DashEnergy && currentDashEnergy > 0)
        {
            currentDashEnergy = 0;
            Invincible = true;
            FindObjectOfType<GhostTrail>().ShowGhost();
        }

        yield return new WaitForSeconds(.3f);

        DashPartical.Stop();
        Rig.gravityScale = 1;
        BetterJumpingEnable = true;
        CanMove = true;
        WallJumped = false;
        IsDashing = false;
        Invincible = false;
    }

    //在地面上冲刺
    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (OnGround && Input.GetAxis("Dash") != 1)
            WasDashed = false;
    }

    //从高空下落后的硬直时间
    IEnumerator Recovery(float time)
    {
        CanMove = false;
        CanDash = false;
        CanJump = false;
        CanAttack = false;
        Anim.SetBool("BigFall", true);
        yield return new WaitForSeconds(time);
        CanMove = true;
        CanDash = true;
        CanAttack = true;
        CanJump = true;
        Anim.SetBool("BigFall", false);
    }

    //额外线程 重生
    IEnumerator IERespawn(Vector2 pp)
    {
        FallTime = 0;
        AM.PlayExplosion();
        SR.enabled = false;
        UnLock(false);
        Instantiate(PlayerExplosion, Character.position, Character.rotation);
        yield return new WaitForSeconds(0.47f);
        FindObjectOfType<HitStop>().Stop(0.2f);
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
        Camera.main.transform.DOShakePosition(.2f, .5f, 10, 90, false, true);
        yield return new WaitForSeconds(0.7f);
        FindObjectOfType<UIManager>().ShowBScreen(1.2f, 0.2f);
        yield return new WaitForSeconds(0.32f);
        FallTime = 0;
        Character.position = pp;
        SR.enabled = true;
        yield return new WaitForSeconds(1.7f);
        UnLock(true);
        FallTime = 0;
        Invincible = false;
    }

    //额外线程 死亡
    IEnumerator IEDeath()
    {
        Invincible = true;
        FallTime = 0;
        AM.PlayExplosion();
        SR.enabled = false;
        UnLock(false);
        Instantiate(PlayerExplosion, Character.position, Character.rotation);
        yield return new WaitForSecondsRealtime(0.47f);
        FindObjectOfType<HitStop>().Stop(0.2f);
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
        Camera.main.transform.DOShakePosition(.2f, .5f, 10, 90, false, true);
        yield return new WaitForSecondsRealtime(0.7f);
        FindObjectOfType<UIManager>().ShowBScreen(1.2f, 0.2f);
        yield return new WaitForSecondsRealtime(0.32f);
        FindObjectOfType<GameManager>().ReloadScene();
    }

    //锁住角色的所有移动
    public void UnLock(bool enabled)
    {
        CanMove = enabled;
        CanAttack = enabled;
        Locked = !enabled;
        if (!enabled)
            Rig.constraints = RigidbodyConstraints2D.FreezeAll;
        else
        {
            Rig.constraints = RigidbodyConstraints2D.None;
            Rig.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    //获取角色的真实朝向 重载1
    Vector2 GetActualDir(float x, float y)//
    {
        float AX = 0, AY = 0;
        if(x > y)
        {
            if(x >= 0.05)
            {
                AX = 1;
            }
            else if(x < -0.05)
            {
                AX = -1;
            }
            else
            {
                AX = 0;
            }
        }
        else if(x < y)
        {
            if (y >= 0.05)
            {
                AY = 1;
            }
            else if (y < -0.05)
            {
                AY = -1;
            }
            else
            {
                AY = 0;
            }
        }
        else if(x == y)
        {
            if (y >= 0.05)
            {
                AX = 1;
                AY = 1;
            }
            else if (y < -0.05)
            {
                AX = -1;
                AY = -1;
            }
            else
            {
                AX = 0;
                AY = 0;
            }
        }
        return new Vector2(AX, AY);
    }

    //获取真实朝向 重载2
    Vector2 GetActualDir(Vector2 velocity)
    {
        float x = velocity.x;
        float y = velocity.y;
        float AX = 0, AY = 0;
        if (x > y)
        {
            if (x >= 0.05)
            {
                AX = 1;
            }
            else if (x < -0.05)
            {
                AX = -1;
            }
            else
            {
                AX = 0;
            }
        }
        else if (x < y)
        {
            if (y >= 0.05)
            {
                AY = 1;
            }
            else if (y < -0.05)
            {
                AY = -1;
            }
            else
            {
                AY = 0;
            }
        }
        else if (x == y)
        {
            if (y >= 0.05)
            {
                AX = 1;
                AY = 1;
            }
            else if (y < -0.05)
            {
                AX = -1;
                AY = -1;
            }
            else
            {
                AX = 0;
                AY = 0;
            }
        }
        return new Vector2(AX, AY);
    }

    public void LeftFoot()
    {
        AM.PlayLeft();
    }

    public void RightFoot()
    {
        AM.PlayRight();
    }

    void OnDrawGizmos()//绘制Debug辅助图形
    {
        Gizmos.color = Color.red;

        //var positions = new Vector2[] { BottomOffset, RightOffset, LeftOffset };
        
        Gizmos.DrawWireCube((Vector2)transform.position + BottomOffset, BottomSize);
        Gizmos.DrawWireCube((Vector2)transform.position + RightOffset, Size);
        Gizmos.DrawWireCube((Vector2)transform.position + LeftOffset, Size);
        
        Gizmos.DrawWireSphere((Vector2)HitBoxPosition, HitBoxRadius);
    }
}