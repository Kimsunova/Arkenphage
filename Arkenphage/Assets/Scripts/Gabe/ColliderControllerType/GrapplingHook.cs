using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GrapplingHook : MonoBehaviour
{

    DistanceJoint2D grappleJoint;
    Vector3 targetPosition;
    RaycastHit2D hit;
    [SerializeField] private float maxDistance = 100f;
    //GameObject playerObject;
    Player player;
    CircleCollider2D playerFeetCollider;
    CapsuleCollider2D playerBodyCollider;

    [SerializeField] private float timeAfterGrappleFinishesToMaintainMomentumAndPreventPlayerInput = .2f;
    [SerializeField] LayerMask layerMaskToReceiveGrapple;

    [SerializeField] LineRenderer ropeRenderer;
    [SerializeField] float swingSpeed;
    [SerializeField] float distanceBelowJointWhereSwingingIsPermitted;
    [SerializeField] float jumpFromGrappleStrength = 5f;

    [SerializeField] float hookShotRetractionSpeed = 25f;
    [SerializeField] float distanceToHookShotDisconnect = 1f;

    [SerializeField] float upOverDropdownPlatformJumpStrength = 30f;
    [SerializeField] PhysicsMaterial2D slipperyFeet;

    [SerializeField] float speedBounceOffWall = 3f;

    [SerializeField] bool isInHookshotMode = false;

    private bool IsDropdownTarget = false;

    Rigidbody2D playerRigidBody;


    // Use this for initialization
    void Start()
    {
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleJoint.enabled = false;
        player = GetComponent<Player>();
        playerFeetCollider = GetComponent<CircleCollider2D>();
        playerBodyCollider = GetComponent<CapsuleCollider2D>();


        ropeRenderer = GetComponent<LineRenderer>();
        ropeRenderer.enabled = false;

        playerRigidBody = player.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {



        if (Input.GetKeyDown(KeyCode.E))//not cross platform compatible
        {
            isInHookshotMode = !isInHookshotMode;
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire2"))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//this is not cross platform compatible? would have to do something like aim in direction of right stick instead

            hit = Physics2D.Raycast(this.transform.position, targetPosition - transform.position, maxDistance, layerMaskToReceiveGrapple);//could do layermask in final parameter, but not all platforms have same layer maks? make unique grapple receiving layermaks that is otherwise 
                                                                                                                                          //the same as ground, or maybe drop through ground? could be an invisible thing that just goes wherever you want to add grapple reception to a platform
            Rigidbody2D hitRb = null;

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    hitRb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                }
            }



            if (hit.collider != null && hitRb != null)
            {
                var exponent = hit.collider.gameObject.layer;
                var comparisonNumber = Mathf.Pow(2, exponent);

                if (comparisonNumber == LayerMask.GetMask("DropThroughGround"))
                {
                    IsDropdownTarget = true;
                }
                else
                {
                    IsDropdownTarget = false;
                }

                var distanceToSubtract = 0f;

                if (playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("DropThroughGround")))
                {
                    distanceToSubtract = 2f;//so pulls you off ledge if on ground to begin swing, but cannot be used to press repeatedly to climb upwards
                }

                grappleJoint.enabled = true;
                grappleJoint.connectedBody = hitRb;
                grappleJoint.distance = Vector2.Distance(transform.position, hit.point) - distanceToSubtract;
                grappleJoint.connectedAnchor = hit.point;

                player.currentState = PlayerState.grappling;//without setting it to this player will stop midswing because it will be receiving run input and at 0
            }
        }

        if (grappleJoint.enabled)
        {
            var a = player.currentState;

            playerFeetCollider.sharedMaterial = slipperyFeet;

            if (playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("DropThroughGround")))
            {
                grappleJoint.distance -= Time.deltaTime * 30f;
                playerRigidBody.gravityScale = -5;
            }
            else
            {
                playerRigidBody.gravityScale = 1;
            }


            if (playerBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || playerBodyCollider.IsTouchingLayers(LayerMask.GetMask("DropThroughGround")) && !isInHookshotMode)//needs to not be in hookshot mode because don't want to bounch off of platforms you are going through?
            {//or could be not dropthrough ground or not hookshot mode, as in it just can't be both
                var direction = 1;

                if(playerRigidBody.velocity.x < 0)
                {
                    direction = -1;
                }

                playerRigidBody.velocity += new Vector2(direction * speedBounceOffWall, 0);
            }


            ropeRenderer.enabled = true;
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPosition(0, transform.position);
            ropeRenderer.SetPosition(1, new Vector3(grappleJoint.connectedAnchor.x, grappleJoint.connectedAnchor.y, 0));


            if (!isInHookshotMode)
            {
                if (CrossPlatformInputManager.GetAxisRaw("Vertical") > 0)
                {
                    grappleJoint.distance -= Time.deltaTime * 5f;
                }
                if (CrossPlatformInputManager.GetAxisRaw("Vertical") < 0)
                {
                    grappleJoint.distance += Time.deltaTime * 5f;
                }


                //float angleToJoint = Vector2.SignedAngle(grappleJoint.connectedAnchor, new Vector2(this.transform.position.x, this.transform.position.y));
                //print(angleToJoint);
                float distanceBelowAnchor = this.transform.position.y - grappleJoint.connectedAnchor.y;
                if (distanceBelowAnchor < -distanceBelowJointWhereSwingingIsPermitted)//only allow momentum swinging when below anchor point
                {
                    float horizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");//not raw cuz want the build up on the swing
                    playerRigidBody.velocity += new Vector2(horizontalInput * swingSpeed, 0);
                }
            }
            else
            {
                if (grappleJoint.distance > distanceToHookShotDisconnect)
                {
                    grappleJoint.distance -= Time.deltaTime * hookShotRetractionSpeed;
                }
                else
                {
                    if (IsDropdownTarget)
                    {
                        Vector2 direction = grappleJoint.connectedAnchor - new Vector2(this.transform.position.x, this.transform.position.y);
                        direction.Normalize();
                        Vector2 addedJumpThrough = direction * upOverDropdownPlatformJumpStrength;
                        playerRigidBody.velocity += addedJumpThrough;

                    }
                    ropeRenderer.enabled = false;
                    grappleJoint.enabled = false;
                    playerFeetCollider.sharedMaterial = null;
                    playerRigidBody.gravityScale = 1;

                    StartCoroutine(WaitAfterGrappleToMaintainMomentum(isInHookshotMode));
                }
            }


            if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                playerFeetCollider.sharedMaterial = null;
                ropeRenderer.enabled = false;
                grappleJoint.enabled = false;
                playerRigidBody.gravityScale = 1;

                //add jump if jump off, although should be in direction of travel rather than straight up?
                //Vector2 jumpVelocityToAdd = new Vector2(0, jumpFromGrappleStrength);
                //playerRigidBody.velocity += jumpVelocityToAdd;
                //so could instead be:
                Vector2 direction = grappleJoint.connectedAnchor - new Vector2(this.transform.position.x, this.transform.position.y);
                direction.Normalize();
                if(direction.y < 0)
                {
                    direction.y = -direction.y;
                }
                Vector2 jumpOffGrappleVelocity = direction * jumpFromGrappleStrength;//should I add something extra to the y direction here?
                playerRigidBody.velocity += jumpOffGrappleVelocity;
                //end alternative jump

                //player.currentState = PlayerState.falling;
                StartCoroutine(WaitAfterGrappleToMaintainMomentum(isInHookshotMode));//should just jump straight up and off of rope with above state change, or maintain momentum from jump like below?
                //playerAnimator.SetBool("Jumping", true);//could have a jump from rope animation later
            }




        }
        else
        {
            if (playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("DropThroughGround")))
            {
                player.currentState = PlayerState.idle;//this is to stop falling once you have disabled the rope and hit the ground
            }
        }

        if (CrossPlatformInputManager.GetButtonUp("Fire2") && grappleJoint.enabled)
        {
            playerFeetCollider.sharedMaterial = null;
            ropeRenderer.enabled = false;
            grappleJoint.enabled = false;
            playerRigidBody.gravityScale = 1;
            StartCoroutine(WaitAfterGrappleToMaintainMomentum(isInHookshotMode));
        }
    }

    private IEnumerator WaitAfterGrappleToMaintainMomentum(bool isInHookShotMode)
    {

        var waitTime = timeAfterGrappleFinishesToMaintainMomentumAndPreventPlayerInput;
        if (isInHookshotMode)
        {
            waitTime = .15f;//if in hook shot need control right after disconnnect more likely, rather than needing momentum right after disconnect
        }

        yield return new WaitForSeconds(waitTime);

        if (!grappleJoint.enabled) //this is needed again becuase if you immediately go back to a grapple before this coroutine is over then it sets it to grapple above and then to walk here when this coroutine is over but you are grappling again
        {
            player.currentState = PlayerState.falling;//perhaps should be idle or walk? 
        }
    }
}
