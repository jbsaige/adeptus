using UnityEngine;
using System.Collections;
using BreadcrumbAi;

[System.Serializable]
public class EnemySounds
{
    public AudioClip audio_hit_1, audio_hit_2, audio_dead_1, audio_dead_2, audio_melee_attack_1, audio_melee_attack_2;
}

public class Enemy : Entity 
{
    public EnemySounds audioClips;
    public enum EnemyType { Melee, Ranged, Special };
    public EnemyType enemyType;

    // used for xp system
    //public float expOnDeath;
    //private PlayerStats player;

    public GameObject bloodPrefab;
    public GameObject specialPrefab;
    private Transform player;
    private Ai ai;
    private AudioSource audioSource;
    private GameController gameController;

    //Weapon Components
    public Transform handHold;
    private WeaponOne weaponOne;
    public WeaponOne[] weapons;
    public Rigidbody rangedProjectilePrefab;
    private Animator animator;
    private bool _animAttack;

    // Attack Rates
    private float rangedAttackNext = 0.0f;
    private float rangedAttackRate = 2.0f;
    private float meleeAttackNext = 0.0f;
    private float meleeAttackRate = 1.0f;

    // Health Bar
    private Enemy thisEnemy;
    private float maxHealth;
    private GameGUI gui;
    [HideInInspector]public bool _isHit, _isHitBySpit;


    void Start()
    {
        // this player object used to add xp to the player for levelling up
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        ai = GetComponent<Ai>();
        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GameGUI>();
        thisEnemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        maxHealth = thisEnemy.health;
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go)
        {
            player = go.transform;
        }
        EquipWeapon(0);
    }

    void Update()
    {
        EnemyHealthManager();
    }

    void FixedUpdate()
    {
        Attack();
    }

    private void Attack()
    {
        if (player)
        {
            if (ai.lifeState == Ai.LIFE_STATE.IsAlive)
            {
                if (enemyType != EnemyType.Ranged)
                {
                    if (ai.attackState == Ai.ATTACK_STATE.CanAttackPlayer && Time.time > meleeAttackNext)
                    {
                        meleeAttackNext = Time.time + meleeAttackRate;
                        float rand = Random.value;
                        if (rand <= 0.4f)
                        {
                            audioSource.clip = audioClips.audio_melee_attack_1;
                        }
                        else
                        {
                            audioSource.clip = audioClips.audio_melee_attack_2;
                        }
                        audioSource.PlayOneShot(audioSource.clip);
                        player.GetComponent<DemoPlayerControls>()._isHit = true;
                        player.GetComponent<DemoPlayerControls>().Bleed(transform.rotation);
                        _animAttack = true;
                    }
                    else
                    {
                        _animAttack = false;
                    }
                }
                else
                {
                    if (ai.attackState == Ai.ATTACK_STATE.CanAttackPlayer && Time.time > rangedAttackNext)
                    {
                        rangedAttackNext = Time.time + rangedAttackRate;
                        Rigidbody spit = Instantiate(rangedProjectilePrefab, transform.position + transform.forward + transform.up, transform.rotation) as Rigidbody;
                        spit.AddForce(transform.forward * 500);
                        _animAttack = true;
                    }
                    else
                    {
                        _animAttack = false;
                    }
                }
            }
        }
    }

    void EquipWeapon(int i)
    {
        if (weaponOne)
        {
            Destroy(weaponOne.gameObject);

        }

        weaponOne = Instantiate(weapons[i], handHold.position, handHold.rotation) as WeaponOne;
        weaponOne.transform.parent = handHold;
        animator.SetFloat("Weapon ID", weaponOne.weaponID);
    }

    private void EnemyHealthManager()
    {
        if (_isHit)
        {
            thisEnemy.TakeDamage(1);
            _isHit = false;
        }

        if (_isHitBySpit)
        {
            thisEnemy.TakeDamage(3);
            _isHitBySpit = false;
        }
        gui.SetEnemyHealth(thisEnemy.health / maxHealth, thisEnemy.health);
    }

    public override void Die()
    {
        //player.AddExperience(expOnDeath);
        thisEnemy.health = 0;
        _isHit = false;
        _isHitBySpit = false;
        EnemyHealthManager();
        gameController.FightOver(true);
        base.Die();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name.Contains("FireSpit"))
        {
            _isHitBySpit = true;
        }
        
    }
}
