using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Actor BattleP1, BattleP2;

    private WorldManager WorldManager;
    private MenuManager MenuManager;
    private TileManager TileManger;
    private WorldManager.RenderMode RenderMode;

    public void Start()
    {
        DontDestroyOnLoad(this);
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
        WorldManager.Start_SetupGame();
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

    }

}
