using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    MovingEntity movingEntity;
    new Rigidbody2D rigidbody2D;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        movingEntity = GetComponent<MovingEntity>();
        anim = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rigidbody2D.velocity.x < 0)
        {
            transform.localScale = new Vector3(-7f, 7f, 7f);
        }
        else if(rigidbody2D.velocity.x > 0)
        {
            transform.localScale = new Vector3(7f, 7f, 7f);
        }


        anim.SetFloat("Speed", rigidbody2D.velocity.magnitude / movingEntity.maxSpeed);
    }

    public void AttackAnimation()
    {
        anim.SetTrigger("Attack");
    }
}
