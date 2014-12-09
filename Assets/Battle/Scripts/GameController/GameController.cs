using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
    // The player pieces
    public GameObject playerAdept;
    public GameObject playerGiant;
    public GameObject playerKraken;
    public GameObject playerThunderbird;
    public GameObject playerSalamander;
    public GameObject playerBehemoth;
    public GameObject playerSiren;
    public GameObject playerDjinn;
    public GameObject playerFirebird;
    public GameObject playerJuggernaut;
    public GameObject playerWraith;
    public GameObject playerGorgon;
    public GameObject playerChimera;
    public GameObject playerCastle;
    
    // The enemy pieces
    public GameObject enemyAdept;
    public GameObject enemySalamander;
    public GameObject enemyFirebird;
    public GameObject enemyChimera;
    public GameObject enemyCastle;

    // Spawn Variables
    public Vector3 playerSpawnValues;
    public Vector3 enemySpawnValues;

    private Actor.ActorType PlayerType, EnemyType;
    private WorldManager.ElementType PlayerElement, EnemyElement;

    private GameManager GameManager;

    // use public prefab variables to assign player/enemy prefabs and then reference those below in the case switches to instantiate
    
    
	// Use this for initialization
	void Start () 
    {
        GameManager = FindObjectOfType<GameManager>();
        GameManager.SetBattleManager(this);
        CreatePlayers();
	}

    // instantiates an object for the player and one for the enemy depending on the piece types passed from the world scene
    void CreatePlayers()
    {
        PlayerType = GameManager.BattleP1.characterType;
        PlayerElement = GameManager.BattleP1.Element;
        EnemyType = GameManager.BattleP2.characterType;
        EnemyElement = GameManager.BattleP2.Element;

        switch (PlayerType)
        {
            case Actor.ActorType.None:
                //This should not happen.
                break;
            case Actor.ActorType.Adept:
                SpawnPlayerAdept();
                break;
            case Actor.ActorType.Demon:
                SpawnPlayerGorgon();
                break;
            case Actor.ActorType.Monster:
                SpawnPlayerSalamander();
                break;
            case Actor.ActorType.Castle:
                SpawnPlayerCastle();
                break;
            default:
                break;
        }

        switch (EnemyType)
        {
            case Actor.ActorType.None:
                //This should not happen.
                break;
            case Actor.ActorType.Adept:
                SpawnEnemyAdept();
                break;
            case Actor.ActorType.Demon:
                SpawnEnemyChimera();
                break;
            case Actor.ActorType.Monster:
                SpawnEnemySalamander();
                break;
            case Actor.ActorType.Castle:
                SpawnEnemyCastle();
                break;
            default:
                break;
        }
        

        
    }

    // Sends results of fight back to main world scene, loads main world scene
    // This public method should be called by the Die method (the override version in PlayerStats and Enemy)
    public void FightOver(bool PlayerWon)
    {
        // send back the piece type from whomever won
        // may need to send back a string like "player won" or "enemy won" instead or as well or maybe just another int that is set to 0 or 1.   0 means the player won.
        if (PlayerWon)
        {
            Debug.Log("player won");
            GameManager.ReturnFromBattle(1);
        }
        else
        {
            GameManager.ReturnFromBattle(2);
        }
    }


    // Spawn Methods
    void SpawnPlayerAdept()
    {
        Vector3 spawnPosition = new Vector3(playerSpawnValues.x, playerSpawnValues.y, playerSpawnValues.z);  //    Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(playerAdept, spawnPosition, spawnRotation);
    }

    void SpawnPlayerGorgon()
    {
        Vector3 spawnPosition = new Vector3(playerSpawnValues.x, playerSpawnValues.y, playerSpawnValues.z);  //    Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(playerGorgon, spawnPosition, spawnRotation);
    }

    void SpawnPlayerSalamander()
    {
        Vector3 spawnPosition = new Vector3(playerSpawnValues.x, playerSpawnValues.y, playerSpawnValues.z);  //    Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(playerSalamander, spawnPosition, spawnRotation);
    }

    void SpawnPlayerCastle()
    {
        Vector3 spawnPosition = new Vector3(playerSpawnValues.x, playerSpawnValues.y, playerSpawnValues.z);  //    Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(playerCastle, spawnPosition, spawnRotation);
    }

    void SpawnEnemyAdept()
    {
        Vector3 spawnPosition = new Vector3(enemySpawnValues.x, enemySpawnValues.y, enemySpawnValues.z);  //    Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(enemyAdept, spawnPosition, spawnRotation);
    }

    void SpawnEnemySalamander()
    {
        Vector3 spawnPosition = new Vector3(enemySpawnValues.x, enemySpawnValues.y, enemySpawnValues.z);  //    Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(enemySalamander, spawnPosition, spawnRotation);
    }

    void SpawnEnemyChimera()
    {
        Vector3 spawnPosition = new Vector3(enemySpawnValues.x, enemySpawnValues.y, enemySpawnValues.z);  //    Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(enemyChimera, spawnPosition, spawnRotation);
    }

    void SpawnEnemyCastle()
    {
        Vector3 spawnPosition = new Vector3(enemySpawnValues.x, enemySpawnValues.y, enemySpawnValues.z);  //    Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(enemyCastle, spawnPosition, spawnRotation);
    }

}
