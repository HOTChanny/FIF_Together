using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class FoxController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isAttacking = false;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpForce = 7f;

    [Header("Jump Sprites")]
    public Sprite jumpUpSprite;
    public Sprite jumpDownSprite;

    private Sprite defaultSprite;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Run.performed += ctx => isRunning = true;
        inputActions.Player.Run.canceled += ctx => isRunning = false;

        inputActions.Player.Jump.performed += ctx => Jump();
        inputActions.Player.Attack.performed += ctx => Attack();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        // ���� Flip
        if (moveInput.x != 0)
            spriteRenderer.flipX = moveInput.x < 0;

        // �ִϸ����� �Ķ���� ������Ʈ
        animator.SetFloat("moveSpeed", Mathf.Abs(moveInput.x));
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetFloat("verticalVelocity", rb.velocity.y);

        // ���� Sprite ó�� (�ִϸ��̼� ��� ���� Sprite ���)
        if (isJumping)
        {
            animator.enabled = false;
            if (rb.velocity.y >= 0)
                spriteRenderer.sprite = jumpUpSprite;
            else
                spriteRenderer.sprite = jumpDownSprite;
        }
        else if (!isAttacking && !animator.enabled)
        {
            // ���� �� Sprite ���� (�ִϸ����� �ٽ� Ȱ��ȭ)
            spriteRenderer.sprite = defaultSprite;
            animator.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        float speed = isRunning ? runSpeed : walkSpeed;
        rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);
    }

    private void Jump()
    {
        if (isJumping) return;

        isJumping = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;

        // ���� �߿� ������ �ִϸ����� Ȱ��ȭ
        if (!animator.enabled)
        {
            animator.enabled = true;
        }

        animator.SetTrigger("isAttacking");

        StartCoroutine(ResetAttackAfterTime(0.4f));
    }

    private IEnumerator ResetAttackAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground") && isJumping)
        {
            isJumping = false;
        }
    }
}