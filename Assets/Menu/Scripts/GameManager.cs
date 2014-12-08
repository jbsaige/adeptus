using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Actor BattleP1, BattleP2;
    public int[] PlayerPower;

    private WorldManager WorldManager;
    private MenuManager MenuManager;
    private TileManager TileManger;
    private WorldManager.RenderMode RenderMode;
    private GameController BattleManager;
    private bool WorldIsLoaded = false;

    public void Start()
    {
        DontDestroyOnLoad(this);
        PlayerPower = new int[2];
        MenuManager = GameObject.FindObjectOfType<MenuManager>();
        MenuManager.SetGameManager(this);
        TileManger = new TileManager();
    }

    /// <summary>
    /// This relies on WorldManager calling this script back at FinishLoad.
    /// </summary>
    /// <param name="mode">A WorldManager.RenderMode type.</param>
    /// <returns></returns>
    private void LoadMap(global::WorldManager.RenderMode mode)
    {
        this.RenderMode = mode;
        Application.LoadLevel("World");
    }

    public void FinishLoad()
    {
        WorldManager = FindObjectOfType<WorldManager>();
        WorldManager.renderMode = this.RenderMode;
        WorldManager.setGameManager(this);
        if (WorldIsLoaded)
        {
            WorldManager.setAllTiles(TileManger.allTiles);
        }
        WorldManager.Start_SetupGame();
        WorldIsLoaded = true;
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
        this.loadStoredMap();
    }

    public void LoadBattle(string BattleGroundName)
    {
        TileManger.allTiles = WorldManager.getAllTiles();
        Application.LoadLevel(BattleGroundName);
    }

    public void SetBattleManager(GameController battleManager)
    {
        this.BattleManager = battleManager;
        //This is for debug only
        //BattleManager.FightOver(true);
    }

}
