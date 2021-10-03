using System;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-1)]
public class Player : Entity, IDamagable
{
    public static Vector3 Position;
    public static Camera Cam;

    public static FloatEffectorStack PlayerTimeScale;

    public CameraRecoil Recoil;
    public Camera Camera;

    public PlayerMovement Movement;

    public Weapon Weapon;

    public Transform Spawn;
    public float DeathHeight = -10;

    private float _timeScale = 1;

    [SerializeField] private AudioClip _shootSound;
    [SerializeField] private float _pitchVariance = 0.05f;

    protected void Awake()
    {
        Cam = Camera;
        PlayerTimeScale = LocalTimeScale; //give a copy of our local time to the static field
    }

    private void Update()
    {
        _timeScale = PlayerTimeScale.Value;
        PlayerActionInput();
        HeightDieCheck();
        Weapon.Data.CheckStage(GameManager.Kills);
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

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.Stop();
            GameManager.Instance.SetBestScore(0);
            GameManager.Instance.Reset();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.Stop();
            GameManager.Instance.Reset();
        }
    }

    public void Shoot(bool firstShot = false)
    {
        if (Weapon.Fire(firstShot))
        {
            var ac = PrefabManager.Instance.UnpoolAudioCue();
            float pitch = 1 + Random.Range(-_pitchVariance, _pitchVariance);
            ac.Play(Weapon.transform.position, _shootSound, 0.2f, pitch, true);
        }
    }


    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        GameManager.Instance.Stop();
        //do respawn
        //Destroy(gameObject);
    }

    public void Reset()
    {
        Transform respawn = Spawn; //temp respwan at same place
        transform.position = respawn.position;
        transform.rotation = respawn.rotation;
        LocalTimeScale.Clear();
        Weapon.Data.Reset();
    }

    private void HeightDieCheck()
    {
        if (_rigidbody.position.y < DeathHeight)
        {
            Die();
        }
    }

}