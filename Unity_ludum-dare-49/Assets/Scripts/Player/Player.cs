using UnityEngine;
using Utils;

[DefaultExecutionOrder(-1)]
public class Player : Entity, IDamagable
{
    public static Vector3 Position;

    public static FloatEffectorStack PlayerTimeScale;

    public CameraRecoil Recoil;
    public Camera Camera;

    public PlayerMovement Movement;

    public Weapon Weapon;

    public Transform Spawn;
    public float DeathHeight = -10;

    private float _timeScale = 1;

    protected override void Awake()
    {
        base.Awake();
        PlayerTimeScale = LocalTimeScale; //give a copy of our local time to the static field
    }

    private void Update()
    {
        _timeScale = PlayerTimeScale.Value;
        PlayerActionInput();
        HeightDieCheck();
    }

    private void PlayerActionInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot(true);
        }
        else if (Input.GetButton("Fire1"))
        {
            Shoot();
        }
    }

    public void Shoot(bool firstShot = false)
    {
        Weapon.Fire(firstShot);
    }


    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        GameManager.Instance.Reset();
        Transform respawn = Spawn; //temp respwan at same place
        transform.position = respawn.position;
        transform.rotation = respawn.rotation;

        //do respawn
        //Destroy(gameObject);
    }

    private void HeightDieCheck()
    {
        if (_rigidbody.position.y < DeathHeight)
        {
            Die();
        }
    }

}