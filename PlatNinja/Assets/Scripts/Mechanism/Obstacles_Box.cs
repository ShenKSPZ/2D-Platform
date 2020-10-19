using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Obstacles_Box : MonoBehaviour
{

    public SpriteRenderer SR;
    public Material mPuerWhite;
    public Material mDefault;
    public GameObject Explosion;
    public GameObject BreakPart;
    public GameObject Trail;

    bool ChangeColor;

    // Start is called before the first frame update
    void Awake()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (ChangeColor)
        {
            SR.material = mPuerWhite;
            Instantiate(Explosion, transform.position + new Vector3(0, 1.23f, 0), transform.rotation);
            GameObject[] Break = new GameObject[5];
            for(int i = 0; i < Break.Length; i++)
            {
                float a = i - 2;
                Break[i] = Instantiate(BreakPart, transform.position, transform.rotation);
                Break[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(a * 50, 400));
            }
            FindObjectOfType<HitStop>().Stop(0.2f);
            Destroy(gameObject);
        }
    }

    public void GetHurt(object[] obj)//被攻击判断 int TypeOfHurt, float Damage, float InvincibleTiming, Vector2 Forcedir
    {
        ChangeColor = true;
    }
}
