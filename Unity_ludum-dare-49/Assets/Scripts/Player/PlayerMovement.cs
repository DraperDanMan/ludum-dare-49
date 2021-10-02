using UnityEngine;
using Utils;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float mouseSpeed = 1f;
    public Vector3 playerFacing = Vector3.zero; //euler angle aim dir.

    private Vector3 targetMovement = Vector3.zero; //players input based movement
    private Vector3 playerPhysicsMovement = Vector3.zero; //players physics momentum.

    private Vector3 _smoothMoveVelocity;

    [SerializeField]
    private float _groundFriction = 0.1f;
    [SerializeField]
    private float _airFriction = 0.01f;
    [SerializeField]
    private float _playerAccel = 10f;
    [SerializeField]
    private float _airAccel = 10f;
    [SerializeField]
    private float _jumpForce = 1f;
    [SerializeField]
    private float _landSlowDownFactor = 0.5f;
    [SerializeField]
    private float _maxSpeed = 2f;

    [SerializeField]
    private Transform body;
    [SerializeField]
    public Transform head;

    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private LayerMask _groundLayers;

    [SerializeField]
    private float _footstepSpeed = 2f; //footsteps per meter
    private float _footstepTimer;

    private bool _rightStep;

    private bool _grounded = false;
    private bool _doGrounded = false;

    private bool _hasDoubleJumpped = false;

    public Vector3 Velocity { get { return playerPhysicsMovement; } }

    public float StrafeSpeed { get { return transform.InverseTransformDirection(playerPhysicsMovement).x; } }

    public float FowardSpeed { get { return transform.InverseTransformDirection(playerPhysicsMovement).z; } }

    public float MaxSpeed => _maxSpeed;

    public bool Locked = false;


    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerFacing.x = body.eulerAngles.y;
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.visible = !focus;
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }

    // Update is called once per frame
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //if (Locked || UIManager.SacToggle || UIManager.Instance._invToggle) return;

        UpdateLookDirection();
        PlayerMovementInput();
    }

    private void FixedUpdate()
    {
        if (Locked) return;
        float timeScale = Time.fixedDeltaTime;
        UpdatePlayerMovement(timeScale);
    }

    private void UpdateLookDirection()
    {
        float xMovement = Input.GetAxis("Mouse X") * mouseSpeed;
        float yMovement = Input.GetAxis("Mouse Y") * mouseSpeed;

        playerFacing.x += xMovement;
        playerFacing.y -= yMovement;

        Vector3 headEuler = head.eulerAngles;
        Vector3 bodyEuler = body.eulerAngles;

        playerFacing.y = Mathf.Clamp(playerFacing.y, -89f, 89f); //don't allow the player too look too far up the go over their head.
        headEuler.x = playerFacing.y;
        bodyEuler.y = playerFacing.x;

        head.eulerAngles = headEuler;
        body.eulerAngles = bodyEuler;
    }

    private void PlayerMovementInput()
    {
        targetMovement = body.right * Input.GetAxis("Horizontal") + body.forward * Input.GetAxis("Vertical");
        targetMovement = targetMovement.normalized * _maxSpeed;

        if (_grounded && Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(body.up*_jumpForce, ForceMode.Impulse);
            _doGrounded = false;
            _grounded = false;
        }
    }

    private float _lastGroundedTime = 0;
    private float _lastGroundedSoundCooldown = 0.1f;

    private void UpdatePlayerMovement(float deltaTime)
    {
        playerPhysicsMovement = Vector3.SmoothDamp (playerPhysicsMovement, targetMovement,
                                ref _smoothMoveVelocity, 1/_playerAccel,Mathf.Infinity,deltaTime);
        playerPhysicsMovement.y = _rigidbody.velocity.y;

        Ray ray = new Ray (transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.SphereCast(ray,0.3f, out hit, 1 + .1f, _groundLayers))
        {
            _grounded = true;
        }
        else
        {
            _grounded = false;
        }

        _rigidbody.velocity = playerPhysicsMovement * Player.PlayerTimeScale.Value;
    }
}