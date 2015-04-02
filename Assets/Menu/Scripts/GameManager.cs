using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public Actor BattleP1, BattleP2;
    [HideInInspector]
    public Vector2 P1Coord, P2Coord;
    [HideInInspector]
    public int[] PlayerPower;
    [HideInInspector]
    public bool[] PlayerIsAI;
    [HideInInspector]
    public WorldManager WorldManager;
    [HideInInspector]
    public MenuManager MenuManager;
    [HideInInspector]
    public TileManager TileManger;
    [HideInInspector]
    public LoadingManager LoadingManager;
    [HideInInspector]
    public WorldManager.RenderMode RenderMode;
    [HideInInspector]
    public WorldManager.GameMode GameMode;
    [HideInInspector]
    public GameController BattleManager;
    [HideInInspector]
    public AIHelper AIHelper;
    [HideInInspector]
    public bool WorldIsLoaded = false;
    [HideInInspector]
    public bool[,] placedAdepts;
    [HideInInspector]
    public string NextLevel;
    [HideInInspector]
    public int BattleWinner = 0, roundNumber = 0, CurrentPlayer = 0, GameWinner = 0;
    [HideInInspector]
    public bool TutorialEnabled = true;
    [HideInInspector]
    public EndGameManager EndGameManager;

    private bool firstWorldLoad = true;

    public void Start()
    {
        DontDestroyOnLoad(this);
        PlayerPower = new int[2] { 0, 0 };
        PlayerIsAI = new bool[2] { false, true };
        MenuManager = GameObject.FindObjectOfType<MenuManager>();
        MenuManager.SetGameManager(this);
        TileManger = gameObject.AddComponent<TileManager>();
        BattleP1 = gameObject.AddComponent<Actor>();
        BattleP2 = gameObject.AddComponent<Actor>();
        AIHelper = gameObject.AddComponent<AIHelper>();
        AIHelper.GameManager = this;
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

    private void OnLevelWasLoaded(int level)
    {

    }

    public void FinishLoad(WorldManager worldManager)
    {
        if (BattleWinner > 0)
        {
            Debug.Log("Battle Winner: " + BattleWinner + "\r\nCurrent Player:" + CurrentPlayer);
            if (BattleWinner == 1)
            {
                TileManger.allTiles[(int)P2Coord.x, (int)P2Coord.y].Actor.SetUp(Actor.ActorType.None, (int)P2Coord.x, (int)P2Coord.y, (int)WorldManager.ElementType.Void, 0, worldManager.None, worldManager);
            }
            else
            {
                TileManger.allTiles[(int)P1Coord.x, (int)P1Coord.y].Actor.SetUp(Actor.ActorType.None, (int)P1Coord.x, (int)P1Coord.y, (int)WorldManager.ElementType.Void, 0, worldManager.None, worldManager);
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
        Debug.Log("Checking Game Mode: " + GameMode.ToString());
        System.Threading.Thread.Sleep(50);
        if (GameMode == global::WorldManager.GameMode.Armageddon)
        {
            this.TriggerEndGame(BattleWinner);
        }
        else
        {
            this.loadStoredMap();
        }
    }

    public void TriggerEndGame(int Winner)
    {
        GameWinner = Winner;
        Application.LoadLevel("EndGame");
    }

    public void LoadBattle(Actor P1, Actor P2, string BattleGroundName)
    {
        TileManger.allTiles = WorldManager.getAllTiles();
        BattleP1 = P1;
        BattleP2 = P2;
        P1Coord = new Vector2(BattleP1.x, BattleP1.z);
        P2Coord = new Vector2(BattleP2.x, BattleP2.z);
        Application.LoadLevel(BattleGroundName);
    }

    public void SetBattleManager(GameController battleManager)
    {
        this.BattleManager = battleManager;
    }

    public void SetLoadingManager(LoadingManager loadingManager)
    {
        this.LoadingManager = loadingManager;
        LoadingManager.LoadNext(NextLevel);
    }

    public void SetEndGameManager(EndGameManager endGameManager)
    {
        Debug.Log("EndGameManager has been set.");
        this.EndGameManager = endGameManager;
        EndGameManager.SetWinner(GameWinner);
    }

    public void LoadLevel(string LevelName)
    {
        NextLevel = LevelName;
        Application.LoadLevel("Loading");
    }

}
