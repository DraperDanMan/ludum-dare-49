using System;
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


    private Vector3 _startHeadEuler;
    private Vector3 _startBodyEuler;

    private void Start()
    {
        _startHeadEuler = head.eulerAngles;
        _startBodyEuler = body.eulerAngles;

        playerFacing.x = _startBodyEuler.y;
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.visible = !focus;
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }

    // Update is called once per frame
    private void Update()
    {

        //if (Locked || UIManager.SacToggle || UIManager.Instance._invToggle) return;
        if (Locked) return;
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
            var invTimeScale = 1 + (1 -Player.PlayerTimeScale.Value);
            _rigidbody.AddForce(body.up*_jumpForce*invTimeScale, ForceMode.Impulse);
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

        var timeScale = Player.PlayerTimeScale.Value;
        var timeAffectedMovement = playerPhysicsMovement * timeScale;
        var yVel = _rigidbody.velocity.y;
        timeAffectedMovement.y = timeScale > 1 ? yVel : yVel * timeScale; //only apply scale on slow down force

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

        _rigidbody.velocity = timeAffectedMovement;
        Player.Position = _rigidbody.position;
    }

    public void Reset()
    {
        head.eulerAngles = _startHeadEuler;
        body.eulerAngles = _startBodyEuler;

        playerFacing.x = _startBodyEuler.y;
    }
}