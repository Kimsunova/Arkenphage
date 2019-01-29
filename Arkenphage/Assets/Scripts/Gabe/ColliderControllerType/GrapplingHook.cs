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
    [SerializeField] private float timeAfterGrappleFinishesToMaintainMomentumAndPreventPlayerInput = .2f;
    [SerializeField] LayerMask layerMaskToReceiveGrapple;

    [SerializeField] LineRenderer ropeRenderer;
    [SerializeField] float swingSpeed;
    [SerializeField] float distanceBelowJointWhereSwingingIsPermitted;
    [SerializeField] float jumpFromGrappleStrength = 5f;


    // Use this for initialization
    void Start()
    {
        grappleJoint = GetComponent<DistanceJoint2D>();
        grappleJoint.enabled = false;
        player = GetComponent<Player>();
        playerFeetCollider = GetComponent<CircleCollider2D>();

        ropeRenderer = GetComponent<LineRenderer>();
        ropeRenderer.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {

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

            ropeRenderer.enabled = true;
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPosition(0, transform.position);
            ropeRenderer.SetPosition(1, new Vector3(grappleJoint.connectedAnchor.x, grappleJoint.connectedAnchor.y, 0));

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
                player.GetComponent<Rigidbody2D>().velocity += new Vector2(horizontalInput * swingSpeed, 0);
            }


            if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                ropeRenderer.enabled = false;
                grappleJoint.enabled = false;
                Vector2 jumpVelocityToAdd = new Vector2(0, jumpFromGrappleStrength);
                player.GetComponent<Rigidbody2D>().velocity += jumpVelocityToAdd;
                //player.currentState = PlayerState.falling;
                StartCoroutine(WaitAfterGrappleToMaintainMomentum());//should just jump straight up and off of rope with above state change, or maintain momentum from jump like below?
                //playerAnimator.SetBool("Jumping", true);//could have a jump from rope animation later
            }
            

            

        }

        if (CrossPlatformInputManager.GetButtonUp("Fire2") && grappleJoint.enabled)
        {
            ropeRenderer.enabled = false;
            grappleJoint.enabled = false;
            StartCoroutine(WaitAfterGrappleToMaintainMomentum());
        }
    }

    private IEnumerator WaitAfterGrappleToMaintainMomentum()
    {
        yield return new WaitForSeconds(timeAfterGrappleFinishesToMaintainMomentumAndPreventPlayerInput);

        if (!grappleJoint.enabled) //this is needed again becuase if you immediately go back to a grapple before this coroutine is over then it sets it to grapple above and then to walk here when this coroutine is over but you are grappling again
        {
            player.currentState = PlayerState.walk;//perhaps should be idle or fall? 
        }
    }
}
