using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class mayaSquid : MonoBehaviour, IControllable
{
    public Rigidbody squidForce;
    public float rotateSpeed = 2f;

    public bool hackyNonsense;
    public float charge;

    public Animator squidAnim;


    // Start is called before the first frame update
    void Start()
    {
        squidAnim.keepAnimatorControllerStateOnDisable = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //localVelocity = transform.InverseTransformDirection(sharkForce.velocity);
        Steer(Input.GetAxis("Horizontal"));
        //Accelerate(Input.GetAxis("Vertical"));
        Action2();
        hackyNonsense = false;
        //Debug.Log(charge);
    }


    


    public void Steer(float input)
    {
        //squidForce.AddForceAtPosition(input*rotateSpeed*transform.TransformDirection(Vector3.right), transform.position, 0);
        squidForce.AddRelativeTorque(new Vector3(0, 0, input*rotateSpeed));
    }

    public void Accelerate(float input)
    {
        throw new System.NotImplementedException();
    }

    public void Reverse(float input)
    {
        throw new System.NotImplementedException();
    }

    public void Action()
    {
        throw new System.NotImplementedException();
    }

    public void Action2()
    {
        hackyNonsense = true;
        if (hackyNonsense)
        {
            charge *= (Time.deltaTime * 5);
            if (!hackyNonsense)
            {
                squidForce.AddForceAtPosition(Vector3.forward * charge, transform.position);
            }
        }
        else
            charge = 0f;
    }

    public void Action3()
    {
        throw new System.NotImplementedException();
    }
}
