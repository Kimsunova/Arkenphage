using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Interactable interactFocus;

    public BoxCollider2D groundCheck;

    public GameObject inventory;

    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private float jumpPower;

    private bool jumpStart = false/*, doubleJumpStart = false*/, grounded = true;
    private float horizontal;

    //Animator animator;
    Rigidbody2D rb2D;
    SpriteRenderer sprite;

    // Use this for initialization
    void Start () {
        //animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Return) && grounded == true)
            jumpStart = true;

        if (Input.GetKeyDown(KeyCode.P) && interactFocus != null)
        {
            interactFocus.Interact();
        }
    }

    void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");

        if (horizontal < 0)
            sprite.flipX = true;
        else if (horizontal > 0)
            sprite.flipX = false;

        HandleMovement(horizontal);

        if (jumpStart == true)
        {
            jumpStart = false;
            rb2D.velocity = new Vector2(horizontal * playerSpeed * Time.deltaTime, 0f);
            Jump();
        }
    }

    void HandleMovement(float horizontal)
    {
        rb2D.velocity = new Vector2(horizontal * playerSpeed, rb2D.velocity.y);
    }

    void Jump()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
    }

    public void HitPlayer()
    {
        StartCoroutine(Damage_ColorChange());
    }

    IEnumerator Damage_ColorChange()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(2f);
        sprite.color = Color.white;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Interactable" && interactFocus == null)
        {
            interactFocus = collision.gameObject.GetComponent<Interactable>();
            interactFocus.OnFocused();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Interactable" && interactFocus != null)
        {
            interactFocus.OnDefocused();
            interactFocus = null;
        }
    }
}
