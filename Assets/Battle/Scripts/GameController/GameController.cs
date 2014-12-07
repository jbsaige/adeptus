using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

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
                break;
            case Actor.ActorType.Demon:
                break;
            case Actor.ActorType.Monster:
                break;
            case Actor.ActorType.Castle:
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
                break;
            case Actor.ActorType.Demon:
                break;
            case Actor.ActorType.Monster:
                break;
            case Actor.ActorType.Castle:
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

}
