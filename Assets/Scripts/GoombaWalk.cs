using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaWalk : MonoBehaviour
{
    public float speed = 4f;      // The speed in m/s
    public int dir = 1;
    private Animator animator;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // This is a Property: A method that acts like a field
    public Vector3 pos
    {                                                     // a
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }

    void Update()
    {
        Move();
        //if (pos.x > Screen.width - 40 || pos.x < 40)
        //{                      // a
        //    // We're off the bottom, so destroy this GameObject            // b
        //    this.gameObject.transform.Rotate(0, 180, 0);
        //    this.dir *= -1;// b
        //}
        if (pos.x > 25F || pos.x < -25)
        {
            //Vector3 tempPos = pos;
            //tempPos.x = -25;
            //pos = tempPos;
            this.gameObject.transform.Rotate(0, 180, 0);
            this.dir *= -1;// b
        }
    }
    public virtual void Move()
    {                                             // b
        Vector3 tempPos = pos;
        tempPos.x += dir * speed * Time.deltaTime;
        pos = tempPos;
    }
}
