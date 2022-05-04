using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class mayaSquid : MonoBehaviour, IControllable
{
    private GameObject inkSplatterSpawned;
    public GameObject inkSplatterPrefab;
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
        if (InputSystem.GetDevice<Keyboard>().vKey.wasPressedThisFrame)
            Action();
        Action2();
        //hackyNonsense = false;
        //Debug.Log(charge);
    }


    


    public void Steer(float input)
    {
        //squidForce.AddForceAtPosition(input*rotateSpeed*transform.TransformDirection(Vector3.right), transform.position, 0);
        squidForce.AddRelativeTorque(new Vector3(0, input*rotateSpeed, 0));
    }

    public void Accelerate(float input)
    {
        
    }

    public void Reverse(float input)
    {
        
    }

    public void Action()
    {
        inkSplatterSpawned = Instantiate(inkSplatterPrefab, new Vector3(transform.position.x, 1.1f, transform.position.z), Quaternion.identity);
        GrowTheInk();
        ShrinkTheInk();
       // Debug.Log("Inked");
    }

    public void GrowTheInk()
    {
        inkSplatterSpawned.GetComponent<Transform>().DOScale(new Vector3(3, 0, 3), 5f);
        inkSplatterSpawned.GetComponent<Transform>().DORotate(new Vector3(0, 90, 0), 5f);
    }
    public void ShrinkTheInk()
    {
        inkSplatterSpawned.GetComponent<Transform>().DOScale(new Vector3(0, 0, 0), 25f);
    }



    public void Action2()
    {
        if (InputSystem.GetDevice<Keyboard>().eKey.isPressed)
        {
            
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
            squidForce.AddForceAtPosition((charge*speed)*transform.TransformDirection(new Vector3(0,0,1)), squidForce.transform.position);
            if (charge >= 1)
            {
                squidAnim.speed = charge / 5;
                squidAnim.SetTrigger("Swimming");
            }


            charge = 0;
        }
    }

    public void Action3()
    {
        
    }
}
