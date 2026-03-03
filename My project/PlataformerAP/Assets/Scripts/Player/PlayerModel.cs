using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] float JumpForce = 5f;
    [SerializeField] float originalSpeed;
    [SerializeField] float smoothTime = 0.15f;
    [SerializeField] float rotationSpeed = 180f;

    private Vector3 currentVelocity;
    private Vector3 smoothVelocityRef;
    private Vector3 velocityRef;
    private float speed;

    private bool isGrounded = true;

    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody Myrigidbody;
    private PlayerView _view;
    private PlayerController _controller;
    Vector3 PostInicial;
    private PlayerLife playerLife;
    string sceneToLoad= "MenuNiveles";
    private void Start()
    {
        speed = originalSpeed;
        isGrounded = true;

        PostInicial = transform.position;
        playerLife = GetComponent<PlayerLife>();
    }

    private void Awake()
    {
        _controller = new PlayerController();

        _controller.OnMove += Move;
        _controller.OnJump += JumpLogic;
        _controller.OnAttack += AttackLogic;
        _view = new PlayerView().SetAnimator(_animator);
    }

    private void Update()
    {
        _controller.ArtificialUpdate();

        if (transform.position.y < PostInicial.y - 15f)
        {
            playerLife.HandleFall();
        }
    }

    private void JumpLogic(bool jump)
    {
        if (isGrounded && jump)
        {
            Myrigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            isGrounded = false;
            _view.changeAnimationTrigger("Jump");
        }
    }

    private void AttackLogic()
    {
        _view.changeAnimationTrigger("Attack");
    }

    private void OnDestroy()
    {
        _controller.OnMove -= Move;
        _controller.OnJump -= JumpLogic;
        _controller.OnAttack -= AttackLogic;
    }

    #region SetFunctions
    public bool SetInmune(bool value)
    {
        if (playerLife != null)
        {
            playerLife.SetInmune(value);
            return value;
        }
        return false;
    }

    public float SetSpeed(float Newspeed)
    {
        speed = Newspeed;
        return speed;
    }

    public float SetJumpForce(float NewForce)
    {
        JumpForce = NewForce;
        return JumpForce;
    }
    #endregion

    #region GetFunctions
    public float GetSpeed()
    {
        return speed;
    }

    public float GetJumpForce()
    {
        return JumpForce;
    }

    public int GetCurrentLives()
    {
        if (playerLife != null)
        {
            return playerLife.GetCurrentLives();
        }
        return 0;
    }
    #endregion

    #region Trigger&Collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _controller.setJumping(false);
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Finish>() != null)
        {
            other.gameObject.GetComponent<Finish>().ChangeScene(sceneToLoad);
        }
    }
    #endregion

    private void Move(float vertical, float horizontal)
    {
        if (Mathf.Abs(horizontal) > 0.01f)
        {
            float rotation = horizontal * rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, rotation, 0f);
        }

        Vector3 targetVelocity = transform.forward * vertical * speed;

        currentVelocity = Vector3.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref velocityRef,
            smoothTime
        );

        transform.position += currentVelocity * Time.deltaTime;

        bool isWalking = Mathf.Abs(vertical) > 0.1f;
        _view.changeAnimation("IsWalking", isWalking);
    }
}
