using UnityEngine;
using System.Collections;

public class PlayerStats : Entity 
{

    //This stuff is all for a levelling system, not to be implemented in Adeptus
    private int level;
    private float currentLevelExperience;
    private float experienceToLevel;

    //Health System
    [HideInInspector]
    public bool _isHit, _pickedUpHealth, _hasMaxHealth, _isHitBySpit, _isHitByMagic, _isHitByRock, _isHitByWave, _isHitByBolt, _isHitByRoar, _isHitBySong, _isHitByFireBurst, _isHitByDrain, _isHitByGaze, _isHitByFireball, _isHitByAcidSpit;
    public GameObject bloodPoolPrefab, fightOver;

    private PlayerStats thisPlayer;
    private PlayerController thisPlayerController;
    private float maxHealth;
    private GameGUI gui;
    private GameController gameController;
    private Enemy enemy;
    

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GameGUI>();
        thisPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        thisPlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        maxHealth = thisPlayer.health;


        // for xp system:
        //LevelUp();
    }

    void Update()
    {
        HealthManager();
    }

    // part of a level system that's not in use
    public void AddExperience(float exp)
    {
        currentLevelExperience += exp;
        if (currentLevelExperience >= experienceToLevel)
        {
            currentLevelExperience -= experienceToLevel;
            LevelUp();
        }

        Debug.Log("EXP: " + currentLevelExperience + "  Level: " + level);

        gui.SetPlayerExperience(currentLevelExperience / experienceToLevel, level);
    }

    // part of a levelling up system, not currently used
    private void LevelUp()
    {
        level++;
        experienceToLevel = level * 50 + Mathf.Pow(level * 2, 2);

        AddExperience(0);
    }

    private void HealthManager()
    {
        if (_isHit)
        {
            thisPlayer.health -= 1;
            _isHit = false;
        }

        if (_isHitBySpit)
        {
            thisPlayer.TakeDamage(2);
            _isHitBySpit = false;
        }

        if (_isHitByRock)
        {
            thisPlayer.TakeDamage(4);
            //thisEnemy.TakeDamage(1);
            _isHitByRock = false;
        }

        if (_isHitByWave)
        {
            thisPlayer.TakeDamage(3);
            //thisEnemy.TakeDamage(1);
            _isHitByWave = false;
        }

        if (_isHitByBolt)
        {
            thisPlayer.TakeDamage(2);
            _isHitByBolt = false;
        }

        if (_isHitByRoar)
        {
            thisPlayer.TakeDamage(1);
            _isHitByRoar = false;
        }

        if (_isHitBySong)
        {
            thisPlayer.TakeDamage(1);
            _isHitBySong = false;
        }

        if (_isHitByFireBurst)
        {
            thisPlayer.TakeDamage(2);
            _isHitByFireBurst = false;
        }

        if (_isHitByDrain)
        {
            if(enemy.name.Contains("EnemyWraith"))
            {
                thisPlayer.TakeDamage(2);
                enemy.health++;
                enemy.health++;
                _isHitByDrain = false;
            }
            else
            {
                _isHitByDrain = false;
            }
        }

        if (_isHitByGaze)
        {
            thisPlayer.TakeDamage(1);
            thisPlayerController.walkSpeed--;
            if (thisPlayerController.walkSpeed <= 0)
            {
                thisPlayer.TakeDamage(thisPlayer.maxHealth);
            }
            _isHitByGaze = false;
        }

        if (_isHitByFireball)
        {
            thisPlayer.TakeDamage(2);
            _isHitByFireball = false;
        }

        if (_isHitByAcidSpit)
        {
            thisPlayer.TakeDamage(2);
            _isHitByAcidSpit = false;
        }

        if (_isHitByMagic)
        {
            // change when implement weapon system for enemies to:
            // thisPlayer.TakeDamage(enemyWeaponOne.damage);
            thisPlayer.TakeDamage(4);
            _isHitByMagic = false;
        }

        gui.SetPlayerHealth(thisPlayer.health / maxHealth, thisPlayer.health);
    }

    public override void Die()
    {
        Debug.Log("Player Death was Triggered");
        thisPlayer.health = 0;
        _isHitBySpit = false;
        _isHit = false;
        _isHitByRock = false;
        _isHitByWave = false;
        _isHitByBolt = false;
        _isHitByRoar = false;
        _isHitBySong = false;
        _isHitByFireBurst = false;
        _isHitByDrain = false;
        _isHitByGaze = false;
        _isHitByFireball = false;
        _isHitByAcidSpit = false;
        _isHitByMagic = false;
        HealthManager();
        base.Die();
        //    Instantiate(bloodPoolPrefab, transform.position, Quaternion.identity);
        //    Instantiate(fightOver);
        gameController.FightOver(false);
        
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name.Contains("Spit"))
        {
            _isHitBySpit = true;
        }

        if (col.collider.name.Contains("FireSpit"))
        {
            _isHitBySpit = true;
        }

        if (col.collider.name.Contains("Rock"))
        {
            _isHitByRock = true;
        }

        if (col.collider.name.Contains("KrakenWave"))
        {
            _isHitByWave = true;
        }

        if (col.collider.name.Contains("ThunderBolt Projectile"))
        {
            _isHitByBolt = true;
        }

        if (col.collider.name.Contains("Roar"))
        {
            _isHitByRoar = true;
        }

        if (col.collider.name.Contains("Gaze"))
        {
            _isHitByGaze = true;
        }

        if (col.collider.name.Contains("Fireball"))
        {
            _isHitByFireball = true;
        }

        if (col.collider.name.Contains("AcidSpit"))
        {
            _isHitByAcidSpit = true;
        }

        if (col.collider.name.Contains("AdeptShot"))
        {
            _isHitByMagic = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Roar")
        {
            _isHitByRoar = true;
        }

        if (other.tag == "Song")
        {
            _isHitBySong = true;
        }

        if (other.tag == "FireBurst")
        {
            _isHitByFireBurst = true;
        }

        if (other.tag == "Drain")
        {
            _isHitByDrain = true;
        }
    }
	
}
