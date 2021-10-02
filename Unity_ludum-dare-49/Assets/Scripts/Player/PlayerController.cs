using UnityEngine;
using Utils;

public class PlayerController : SingletonBehaviour<PlayerController>, IDamagable
{
    public CameraRecoil Recoil;
    public Camera Camera;

    public PlayerMovement Movement;

    public Weapon Weapon;

    public int Health = 100;
    private int _maxHealth;

    protected override void Initialize()
    {
        _maxHealth = Health;
    }

    protected override void Shutdown()
    {

    }

    private void Update()
    {
        PlayerActionInput();
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
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Transform respawn = transform; //temp respwan at same place
        transform.position = respawn.position;
        transform.rotation = respawn.rotation;
        Health = _maxHealth;

        //do some stuff
        //Destroy(gameObject);
    }


}