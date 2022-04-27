using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class mayaSquid : MonoBehaviour, IControllable
{
    public Rigidbody squidForce;
    public float rotateSpeed = 2f;
    public float speed = 30;

    public bool hackyNonsense;
    public float charge;

    public Animator squidAnim;


    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //localVelocity = transform.InverseTransformDirection(sharkForce.velocity);
        Steer(Input.GetAxis("Horizontal"));
        //Accelerate(Input.GetAxis("Vertical"));
        Action2();
        //hackyNonsense = false;
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
        if (InputSystem.GetDevice<Keyboard>().eKey.isPressed)

        {
            squidAnim.SetBool("Swimming", false);

            hackyNonsense = true;
            if (hackyNonsense)
            {
                //charge = 0;
                charge += 16 * Time.deltaTime;
            }
        }
        else
        {
            
            
            //squidAnim.speed = charge/4;
            hackyNonsense = false;
            squidForce.AddForceAtPosition((charge*speed)*transform.TransformDirection(new Vector3(0,1,0)), squidForce.transform.position);
            if (charge >= 0f)
            {
                squidAnim.speed = charge / 4;
                squidAnim.SetBool("Swimming", true);
            }
            charge = 0;
        }
    }

    public void Action3()
    {
        throw new System.NotImplementedException();
    }
}
