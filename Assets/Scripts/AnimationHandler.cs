using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public enum Direction { Left, Right }
    [SerializeField] float angleSensitivity;
    public Direction direction = Direction.Left;
    MovingEntity movingEntity;
    Rigidbody2D rigidbody2D;
    Animator anim;
    // Start is called before the first frame update
    Vector2 directionVec;
    void Start()
    {
        movingEntity = GetComponent<MovingEntity>();
        anim = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        SetDirection();
        ProcessMoveAnimation();
    }

    private void ProcessMoveAnimation()
    {
        
        if (direction == Direction.Left)
        {
            transform.localScale = new Vector3(-7f, 7f, 7f);
        }
        else if (direction == Direction.Right)
        {
            transform.localScale = new Vector3(7f, 7f, 7f);
        }

        anim.SetFloat("Speed", rigidbody2D.velocity.magnitude / movingEntity.maxSpeed);
    }

    private void SetDirection()
    {
        directionVec = GetComponent<Rigidbody2D>().velocity;

        if (direction == Direction.Left)
        {
            if (Vector2.Dot(Vector2.left, directionVec.normalized) < Mathf.Cos(angleSensitivity))
            {
                direction = Direction.Right;
            }
        }
        else
        {
            if (Vector2.Dot(Vector2.right, directionVec.normalized) < Mathf.Cos(angleSensitivity))
            {
                direction = Direction.Left;
            }
        }
    }

    public void AttackAnimation()
    {
        anim.SetTrigger("Attack");
    }
}
