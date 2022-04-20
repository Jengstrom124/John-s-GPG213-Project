using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class mayaSquid : MonoBehaviour, IControllable
{
    public Rigidbody squidForce;
    public float speed = 25;
    public float rotateSpeed = 2f;
    public float charge = 0;
    public float boostDistance = 5;
    private float chargeRate = 3;

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
        Debug.Log(charge);
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
        if (charge <= 0f)
        {
            charge *= + chargeRate;
            if (InputSystem.GetDevice<Keyboard>().eKey.wasReleasedThisFrame || charge >= 100f) ;
            {
                squidAnim.speed = (charge*boostDistance*speed)/2;
                squidAnim.Play("Swim");
                squidForce.AddForceAtPosition(
                    charge * boostDistance * speed * transform.TransformDirection(Vector3.forward), transform.position,
                    ForceMode.Impulse);
                charge *= - chargeRate;
                
            }
        }
    }

    public void Action3()
    {
        throw new System.NotImplementedException();
    }
}
