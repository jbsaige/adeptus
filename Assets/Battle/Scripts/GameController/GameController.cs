using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    private int PlayerType;
    private int EnemyType;

    // use public prefab variables to assign player/enemy prefabs and then reference those below in the case switches to instantiate
    
    
	// Use this for initialization
	void Start () 
    {
        // During Debug / development phase only
        PlayerType = 0;
        EnemyType = 0;


        CreatePlayers();
	}

    // instantiates an object for the player and one for the enemy depending on the piece types passed from the world scene
    void CreatePlayers()
    {
        PlayerType = PlayerPrefs.GetInt("PlayerPiece");
        EnemyType = PlayerPrefs.GetInt("EnemyPiece");

        // create player object according to what piece was moved to start a battle
        switch (PlayerType)
        {
            case 1:
                // code to instantiate a player prefab object of the first type
            case 2:  
                // code to instantiate a player prefab object of the second type
            default:
                Debug.Log("Default Case");
                break;
        }

        // create enemy object according to what piece was moved to start the battle
        switch (EnemyType)
        {
            case 1:
                // code to instantiate enemy prefab of this type
            case 2:
                // code to instantiate enemy prefab of this type
            default:
                Debug.Log("Default Case");
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
            PlayerPrefs.SetInt("WhoWon", 0);
            PlayerPrefs.SetInt("PlayerPiece", PlayerType);
            Debug.Log("player won");
        }
        else
        {
            PlayerPrefs.SetInt("WhoWon", 1);
            PlayerPrefs.SetInt("EnemyPiece", EnemyType);
        }
        Application.LoadLevel("World");
        Debug.Log("Load Level World");
    }

}
