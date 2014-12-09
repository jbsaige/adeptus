using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Actor BattleP1, BattleP2;
    public int[] PlayerPower;
    public int CurrentPlayer = 1;

    private WorldManager WorldManager;
    private MenuManager MenuManager;
    private TileManager TileManger;
    private LoadingManager LoadingManager;
    private WorldManager.RenderMode RenderMode;
    private GameController BattleManager;
    private bool WorldIsLoaded = false;
    private string NextLevel;
    private int BattleWinner = 0;


    public void Start()
    {
        DontDestroyOnLoad(this);
        PlayerPower = new int[2];
        MenuManager = GameObject.FindObjectOfType<MenuManager>();
        MenuManager.SetGameManager(this);
        TileManger = gameObject.AddComponent<TileManager>();
        BattleP1 = gameObject.AddComponent<Actor>();
        BattleP2 = gameObject.AddComponent<Actor>();
    }

    /// <summary>
    /// This relies on WorldManager calling this script back at FinishLoad.
    /// </summary>
    /// <param name="mode">A WorldManager.RenderMode type.</param>
    /// <returns></returns>
    private void LoadMap(global::WorldManager.RenderMode mode)
    {
        this.RenderMode = mode;
        this.LoadLevel("World");
    }

    public void FinishLoad(WorldManager worldManager)
    {
        worldManager.SetGameManager(this);
        worldManager.renderMode = this.RenderMode;
        if (WorldIsLoaded)
        {
            worldManager.setAllTiles(TileManger.allTiles);
        }
        worldManager.Start_SetupGame();
        if (BattleWinner > 3)
        {
            Debug.Log("Battle Winner: " + BattleWinner + "\r\nCurrent Player:" + CurrentPlayer);
            if (BattleWinner == 1)
            {
                worldManager.destroyActorAt(this.BattleP2.x, this.BattleP2.z);
                if (CurrentPlayer == BattleWinner)
                {
                    worldManager.moveActor(this.BattleP1.x, this.BattleP1.z, this.BattleP2.x, this.BattleP2.z);
                }
            }
            else
            {
                worldManager.destroyActorAt(this.BattleP1.x, this.BattleP1.z);
                if (CurrentPlayer == BattleWinner)
                {
                    worldManager.moveActor(this.BattleP2.x, this.BattleP2.z, this.BattleP1.x, this.BattleP1.z);
                }
            }
            BattleWinner = 0;
        }
        WorldIsLoaded = true;
        WorldManager = worldManager;
    }

    public void loadRandomMap()
    {
        this.LoadMap(global::WorldManager.RenderMode.Random);
    }

    public void loadPatternMap()
    {
        this.LoadMap(global::WorldManager.RenderMode.Pattern);
    }

    public void loadStoredMap()
    {
        this.LoadMap(global::WorldManager.RenderMode.FromStored);
    }

    public void storeTiles(Tiles[,] tileData)
    {
        TileManger.allTiles = tileData;
    }

    public Tiles getStoredTile(int x, int z)
    {
        return TileManger.allTiles[x, z];
    }

    public Tiles[,] getStoredTiles()
    {
        return TileManger.allTiles;
    }

    public void ReturnFromBattle(int winner)
    {
        Debug.Log("Returning from battle");
        BattleWinner = winner;
        this.loadStoredMap();
    }

    public void LoadBattle(Actor P1, Actor P2, string BattleGroundName)
    {
        TileManger.allTiles = WorldManager.getAllTiles();
        BattleP1 = TileManger.allTiles[P1.x, P1.z].Actor;
        BattleP2 = TileManger.allTiles[P2.x, P2.z].Actor;
        BattleP1.Player = P1.Player;
        BattleP1.x = P1.x;
        BattleP1.z = P1.z;
        BattleP1.Element = P1.Element;
        BattleP1.characterType = P1.characterType;
        BattleP2.Player = P2.Player;
        BattleP2.x = P2.x;
        BattleP2.z = P2.z;
        BattleP2.Element = P2.Element;
        BattleP2.characterType = P2.characterType; Application.LoadLevel(BattleGroundName);
    }

    public void SetBattleManager(GameController battleManager)
    {
        this.BattleManager = battleManager;
        //This is for debug only
        //BattleManager.FightOver(true);
    }

    public void SetLoadingManager(LoadingManager loadingManager)
    {
        this.LoadingManager = loadingManager;
        LoadingManager.LoadNext(NextLevel);
    }

    public void LoadLevel(string LevelName)
    {
        NextLevel = LevelName;
        Application.LoadLevel("Loading");
    }

}
