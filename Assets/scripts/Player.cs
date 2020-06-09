using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpSpeed;
    [Range(0.1f, 0.5f)]
    [SerializeField] float dashTime; 

    Collider2D myCollider;
    //BoxCollider2D myFeet;
    Rigidbody2D myBody;
    Animator myAnimator;
    Animator dasher;

    private Vector2 boostSpeed = new Vector2(50, 0);
    private bool canBoost = true;
    private float boostCooldown = 2f;

    float startTime = 0f;
    float direction = 1f;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<CapsuleCollider2D>();
        myBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        //myFeet = GetComponent<BoxCollider2D>();
        dasher = transform.GetChild(0).GetComponent<Animator>();
        Debug.Log(dasher.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        Jump();
        Fire();
        Dash();


    }

    private void Fire()
    {
        if(Input.GetKey(KeyCode.Z))
        {
            myAnimator.SetLayerWeight(1, 1);
        }
        else
            myAnimator.SetLayerWeight(1, 0);
    }

    private void Run()
    {
        float h = Input.GetAxis("Horizontal");
        bool isMoving = (Mathf.Abs(h) > 0);
        myBody.velocity = new Vector2(h * speed, myBody.velocity.y);

        myAnimator.SetBool("running", isMoving);
        
        if (isMoving)
        {
            direction = Mathf.Sign(h);
            transform.localScale = new Vector2(direction, 1);
        }
            
    }

    private void Jump()
    {
        bool inGround = isGrounded();

        if(inGround)
        {
            if (Input.GetButtonDown("Jump"))
            {
                myBody.velocity += new Vector2(0, jumpSpeed);
                myAnimator.SetTrigger("takeof");
            }
            myAnimator.SetBool("jumping", false);
        }
        else
            myAnimator.SetBool("jumping", true);

    }

    private bool isGrounded()
    {
        //return myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"));
        RaycastHit2D ray = Physics2D.Raycast(myCollider.bounds.center, Vector2.down, myCollider.bounds.extents.y + 0.2f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(myCollider.bounds.center, Vector2.down * (myCollider.bounds.extents.y + 0.2f), Color.green);
        return (ray.collider != null);

    }


    private void Dash()
    {
        if(Input.GetKeyDown(KeyCode.X) && isGrounded())
        {
            startTime = Time.time;
            Debug.Log("start time: " + startTime);
            Debug.Log("Hold until: " + (startTime + dashTime));
            dasher.SetTrigger("dashing");
        }
        if(Input.GetKey(KeyCode.X) && isGrounded())
        {
            if(startTime + dashTime >= Time.time)
            {
                myAnimator.SetBool("dashing", true);
                myBody.velocity += new Vector2(speed * 1.5f * direction, 0);
            }
            else
                myAnimator.SetBool("dashing", false);
        }
        else
            myAnimator.SetBool("dashing", false);
    }

    IEnumerator Boost(float boostDur) 
    {
        float time = 0; 
        canBoost = false; //set canBoost to false so that we can't keep boosting while boosting

        while (boostDur > time)
        {
            time += Time.deltaTime; //Increase our "time" variable by the amount of time that it has been since the last update
            myBody.velocity = boostSpeed; //set our rigidbody velocity to a custom velocity every frame, so that we get a steady boost direction like in Megaman
            yield return 0; //go to next frame
        }
        canBoost = true; //set back to true so that we can boost again.
    }

}
