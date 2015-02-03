using UnityEngine;
using System.Collections;

public class AIHelper : MonoBehaviour
{

    public int myPlayerNum;
    public GameManager GameManager;
    public WorldManager WorldManager;

    private bool doIhaveMorePP, doIhaveMorePW;

    public void MakeDecision()
    {
        Debug.Log("The AI is making a decision.");
        myPlayerNum = GameManager.CurrentPlayer;
        if (WorldManager == null)
        {
            WorldManager = GameManager.WorldManager;
        }
        int adept = haveIplacedAllMyAdepts();
        if (adept > 0)
        {
            placeAnAdept(adept);
        }
        doIhaveMorePP = (myPlayerNum == whoHasMorePP());
        doIhaveMorePW = (myPlayerNum == whoHasMorePW());
    }

    private int whoHasMorePP()
    {
        return (GameManager.PlayerPower[0] > GameManager.PlayerPower[1]) ? 1 : 2;
    }

    private int whoHasMorePW()
    {
        int p1Wells = 0, p2Wells = 0;
        for (int x = 0; x < GameManager.WorldManager.xSize; x++)
        {
            for (int z = 0; z < GameManager.WorldManager.zSize; z++)
            {
                if (GameManager.TileManger.allTiles[x, z].hasPowerWell)
                {
                    if (GameManager.TileManger.allTiles[x, z].Actor.Player == 1)
                    {
                        p1Wells++;
                    }
                    else if (GameManager.TileManger.allTiles[x, z].Actor.Player == 2)
                    {
                        p2Wells++;
                    }
                }
            }
        }
        return (p1Wells > p2Wells) ? 1 : 2;
    }

    private int haveIplacedAllMyAdepts()
    {
        for (int i = 0; i < 4; i++)
        {
            if (!WorldManager.placedAdepts[myPlayerNum - 1, i])
            {
                return i + 1;
            }
        }
        return 0;
    }

    private void placeAnAdept(int element)
    {
        Tiles tileWithOpenPW = findOpenPW();
        if (tileWithOpenPW == null)
        {

        }
        else
        {
            WorldManager.placeNewActor(tileWithOpenPW.x, tileWithOpenPW.z, Actor.ActorType.Adept, (WorldManager.ElementType)element, myPlayerNum);
        }
        WorldManager.changePlayer();
    }

    private Tiles findOpenPW()
    {
        int start = 0, stop = 0;
        if (myPlayerNum == 1)
        {
            stop = WorldManager.xSize;
        }
        else
        {
            start = -WorldManager.xSize + 1;
        }
        for (int x = start; x < stop; x++)
        {
            for (int z = 0; z < WorldManager.zSize; z++)
            {
                Tiles tile = GameManager.TileManger.allTiles[Mathf.Abs(x), z];
                if (tile.GetComponentInChildren<Actor>().characterType == Actor.ActorType.None && tile.hasPowerWell)
                {
                    return tile;
                }
            }
        }
        return null;
    }
}
