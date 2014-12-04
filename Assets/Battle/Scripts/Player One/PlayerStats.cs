using UnityEngine;
using System.Collections;

public class PlayerStats : Entity 
{

    //This stuff is all for a levelling system, not to be implemented in Adeptus
    private int level;
    private float currentLevelExperience;
    private float experienceToLevel;

    //Health System
    [HideInInspector]public bool _isHit, _pickedUpHealth, _hasMaxHealth, _isHitBySpit;
    public GameObject bloodPoolPrefab, fightOver;

    private PlayerStats thisPlayer;
    private float maxHealth;
    private GameGUI gui;
    private GameController gameController;
    

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GameGUI>();
        thisPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
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

        gui.SetPlayerHealth(thisPlayer.health / maxHealth, thisPlayer.health);
    }

    public override void Die()
    {
        thisPlayer.health = 0;
        _isHitBySpit = false;
        _isHit = false;
        HealthManager();
        //    Instantiate(bloodPoolPrefab, transform.position, Quaternion.identity);
        //    Instantiate(fightOver);
        gameController.FightOver(false);
        base.Die();
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name.Contains("Spit"))
        {
            _isHitBySpit = true;
        }
    }
	
}
