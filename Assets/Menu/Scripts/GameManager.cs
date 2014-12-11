using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Actor BattleP1, BattleP2;
    public Vector2 P1Coord, P2Coord;
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
        if (BattleWinner > 0)
        {
            Debug.Log("Battle Winner: " + BattleWinner + "\r\nCurrent Player:" + CurrentPlayer);
            if (BattleWinner == 1)
            {
                TileManger.allTiles[(int)P2Coord.x, (int)P2Coord.y].Actor.SetUp(Actor.ActorType.None, (int)P2Coord.x, (int)P2Coord.y, (int)WorldManager.ElementType.None, 0, WorldManager.None, worldManager);
            }
            else
            {
                TileManger.allTiles[(int)P1Coord.x, (int)P1Coord.y].Actor.SetUp(Actor.ActorType.None, (int)P1Coord.x, (int)P1Coord.y, (int)WorldManager.ElementType.None, 0, WorldManager.None, worldManager);
            }
            if (CurrentPlayer == BattleWinner)
            {
                Actor temp = TileManger.allTiles[(int)P1Coord.x, (int)P1Coord.y].Actor;
                TileManger.allTiles[(int)P1Coord.x, (int)P1Coord.y].Actor = TileManger.allTiles[(int)P2Coord.x, (int)P2Coord.y].Actor;
                TileManger.allTiles[(int)P2Coord.x, (int)P2Coord.y].Actor = temp;
            }
            BattleWinner = 0;
        }
        worldManager.SetGameManager(this);
        worldManager.renderMode = this.RenderMode;
        if (WorldIsLoaded)
        {
            worldManager.setAllTiles(TileManger.allTiles);
        }
        worldManager.Start_SetupGame();
        WorldIsLoaded = true;
        WorldManager = worldManager;
        WorldManager.changePlayer();
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

    public void ReturnFromBattle(int winner, int P1X, int P1Z, int P2X, int P2Z)
    {
        Debug.Log("Returning from battle");
        BattleWinner = winner;
        BattleP1.x = P1X;
        BattleP1.z = P1Z;
        BattleP2.x = P2X;
        BattleP2.z = P2Z;
        this.loadStoredMap();
    }

    public void LoadBattle(Actor P1, Actor P2, string BattleGroundName)
    {
        TileManger.allTiles = WorldManager.getAllTiles();
        BattleP1 = TileManger.allTiles[P1.x, P1.z].Actor;
        BattleP2 = TileManger.allTiles[P2.x, P2.z].Actor;
        //BattleP1.Player = P1.Player;
        //BattleP1.x = P1.x;
        //BattleP1.z = P1.z;
        //BattleP1.Element = P1.Element;
        //BattleP1.characterType = P1.characterType;
        //BattleP2.Player = P2.Player;
        //BattleP2.x = P2.x;
        //BattleP2.z = P2.z;
        //BattleP2.Element = P2.Element;
        //BattleP2.characterType = P2.characterType;
        P1Coord = new Vector2(BattleP1.x, BattleP1.z);
        P2Coord = new Vector2(BattleP2.x, BattleP2.z);
        Application.LoadLevel(BattleGroundName);
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
