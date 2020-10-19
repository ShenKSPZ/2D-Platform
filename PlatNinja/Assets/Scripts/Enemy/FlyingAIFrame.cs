using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pathfinding;

public class FlyingAIFrame : MonoBehaviour
{

    [Header("本体")]
    public Transform Enemy;
    [Header("移动")]
    public float FollowSpeed = 200f;
    public bool Flip = true;
    public bool CanMove;
    Vector3 LastPoint;
    [Header("引力")]
    Rigidbody2D Rig;
    [Header("跟随")]
    public float FollowingDistance = 15f;
    public float MaxFollowDistance = 30f;
    public float nextWaypointDistance = 3f;
    public bool StartFollowing = false;
    [Header("属性状态")]
    public float MaxHP = 300f;
    public float currentHP = 300f;
    public bool IsHurting;
    public Vector2 LightHurtForce;
    public Vector2 HeavyHurtForce;
    int HurtType = 0; //0Touch 1Light 2Heavy
    [Header("无敌帧")]
    public bool Invincible;
    float InvincibleTime;
    [Header("FX")]
    public GameObject Explosion;
    public GameObject ExclamatoryMark;
    public AudioManager AM;

    Animator Anim;
    Seeker seeker;
    SpriteRenderer SR;
    public Transform Player;
    Path path;

    Vector2 P;
    Vector2 E;

    public Vector2 LastPlayerPosition;

    int currentWaypoint = 0;
    float tempdis;
    bool reachedEndOfPath;
    bool JustStartFollow;
    bool LastFollowState;

    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Rig = Enemy.GetComponent<Rigidbody2D>();
        Anim = Enemy.GetComponent<Animator>();
        SR = Enemy.GetComponent<SpriteRenderer>();
        seeker = GetComponent<Seeker>();
        AM = FindObjectOfType<AudioManager>();
        JustStartFollow = true;
        LastPlayerPosition = P = E = Vector2.zero;
    }

    private void Start()
    {
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void Update()
    {
        P = Player.position;
        E = Enemy.position;
        tempdis = (P - E).magnitude;
        if(tempdis <= FollowingDistance && !StartFollowing)
        {
            if(JustStartFollow)
            {
                Instantiate(ExclamatoryMark, Enemy.position + new Vector3(0, 1.2f, 0), Enemy.rotation, Enemy);
            }
            JustStartFollow = false;
            StartFollowing = true;
        }
        else if(StartFollowing && tempdis >= MaxFollowDistance)
        {
            StartFollowing = false;
            JustStartFollow = true;
        }
    }

    void UpdatePath()
    {
        if (P != LastPlayerPosition && seeker.IsDone() && tempdis <= MaxFollowDistance)
        {
            seeker.StartPath(Rig.position, Player.position, OnPathDelegate);
            //currentWaypoint = 0;
            //reachedEndOfPath = false;
            LastPlayerPosition = P;
        }
    }

    private void FixedUpdate()
    {

        if (!StartFollowing)
            return;

        if (!CanMove)
            return;

        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Enemy.position = Vector2.MoveTowards(Enemy.position, path.vectorPath[currentWaypoint], FollowSpeed * Time.fixedDeltaTime);

        if(P.x < E.x)
        {
            SR.flipX = false;
        }
        else
        {
            SR.flipX = true;
        }

        float distance = Vector2.Distance(Rig.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void OnPathDelegate(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void GetHurt(object[] obj)//被攻击判断 int TypeOfHurt, float Damage, float InvincibleTiming, Vector2 Forcedir
    {
        HurtType = (int)obj[0];
        Anim.SetTrigger("Hurt");
        currentHP -= (float)obj[1];
        InvincibleTime = (float)obj[2];
        Rig.velocity = LightHurtForce * (Vector2)obj[3];

        //Rig.AddForce(LightHurtForce * (Vector2)obj[3]);
    }

    public void AnimHurtStartPlay()//受击动画开始播放
    {
        CanMove = false;
        IsHurting = true;
        Invincible = true;
        if (currentHP <= 0)
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
}
