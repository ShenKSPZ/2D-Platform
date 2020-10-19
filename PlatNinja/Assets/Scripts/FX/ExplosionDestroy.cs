using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDestroy : MonoBehaviour
{

    public Animator Anim;

    AnimatorStateInfo AS;

    // Start is called before the first frame update
    void Awake()
    {
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //AS = Anim.GetCurrentAnimatorStateInfo(0);
        //if(AS.normalizedTime >= 1.0f)
        //{
        //    Destroy(gameObject);
        //}
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
