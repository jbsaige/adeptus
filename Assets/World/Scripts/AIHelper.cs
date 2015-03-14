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
        Debug.Log("The AI is checking for unplaced Adepts.");
        int adept = haveIplacedAllMyAdepts();
        if (adept > 0)
        {
            Debug.Log("The AI is placing an adept.");
            placeAnAdept(adept);
            return;
        }
        doIhaveMorePP = (myPlayerNum == whoHasMorePP());
        doIhaveMorePW = (myPlayerNum == whoHasMorePW());
        Debug.Log("The AI is looking for open PowerWells.");
        Tiles openPW = findOpenPW();
        if (openPW != null)
        {
            Debug.Log("The AI is placing a spawnling on a PowerWell.");
            placeSpawnling(openPW);
            return;
        }
        Debug.Log("The AI is looking for an uncommited spawn.");
        Actor spawnling = findUncommitteedSpawnling();
        if (spawnling != null)
        {
            Debug.Log("The AI is going to attack!");
            Tiles enemyPW = findEnemyPW();
            if (enemyPW != null)
            {
                Debug.Log("The AI attacking a PowerWell.");
                Actor P1 = (myPlayerNum == 1) ? spawnling : enemyPW.GetComponentInChildren<Actor>();
                Actor P2 = (myPlayerNum == 2) ? spawnling : enemyPW.GetComponentInChildren<Actor>();
                WorldManager.triggerBattle(P1, P2, enemyPW);
                return;
            }
            Tiles enemyAdept = findEnemyAdept();
            if (enemyAdept != null)
            {
                Debug.Log("The AI is attacking an Adept.");
                Actor P1 = (myPlayerNum == 1) ? spawnling : enemyAdept.GetComponentInChildren<Actor>();
                Actor P2 = (myPlayerNum == 2) ? spawnling : enemyAdept.GetComponentInChildren<Actor>();
                WorldManager.triggerBattle(P1, P2, enemyAdept);
                return;
            }
            Debug.Log("The player has no Adepts or PowerWells?");
        }
        else
        {
            Tiles spawnLocation = findOpenTile();
            Debug.Log("The AI is placing a spawnling on an empty tile.");
            placeSpawnling(spawnLocation);
            return;
        }
        Debug.Log("The AI was stumpped!");
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
            if (!GameManager.placedAdepts[myPlayerNum - 1, i])
            {
                return i + 1;
            }
        }
        return 0;
    }

    private int adeptsAlive()
    {
        int adepts = 0;
        for (int x = 0; x < GameManager.WorldManager.xSize; x++)
        {
            for (int z = 0; z < GameManager.WorldManager.zSize; z++)
            {
                if (GameManager.TileManger.allTiles[x, z].Actor.characterType == Actor.ActorType.Adept)
                {
                    adepts++;
                }
            }
        }
        return adepts;
    }

    private Actor findUncommitteedSpawnling()
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
                Actor actor = tile.GetComponentInChildren<Actor>();
                if ((actor.characterType == Actor.ActorType.Demon || actor.characterType == Actor.ActorType.Monster) && !tile.hasPowerWell && actor.Player == myPlayerNum)
                {
                    Debug.Log(x + ", " + z + " Type: " + actor.characterType.ToString() + ", has PW: " + tile.hasPowerWell.ToString());
                    return actor;
                }
            }
        }
        Debug.Log("Could not find uncommitted spawnling.");
        return null;

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

    private void placeSpawnling(Tiles destinationTile)
    {
        WorldManager.ElementType element = destinationTile.Element;
        Actor.ActorType type = (Random.Range(0, 1) == 1) ? Actor.ActorType.Demon : Actor.ActorType.Monster;
        WorldManager.placeNewActor(destinationTile.x, destinationTile.z, type, element, myPlayerNum);
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
                    Debug.Log(x + ", " + z + " Type: " + tile.GetComponentInChildren<Actor>().characterType.ToString() + ", has PW: " + tile.hasPowerWell.ToString());
                    return tile;
                }
            }
        }
        Debug.Log("Could not find open PW.");
        return null;
    }

    private Tiles findEnemyPW()
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
                Actor actor = tile.GetComponentInChildren<Actor>();
                if (actor.characterType == Actor.ActorType.Adept && tile.hasPowerWell && actor.Player != myPlayerNum)
                {
                    Debug.Log(x + ", " + z + " Type: " + actor.characterType.ToString() + ", has PW: " + tile.hasPowerWell.ToString());
                    return tile;
                }
            }
        }
        Debug.Log("Could not find open PW.");
        return null;
    }

    private Tiles findEnemyAdept()
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
                Actor actor = tile.GetComponentInChildren<Actor>();
                if (actor.characterType == Actor.ActorType.Adept && actor.Player != myPlayerNum)
                {
                    Debug.Log(x + ", " + z + " Type: " + actor.characterType.ToString() + ", has PW: " + tile.hasPowerWell.ToString());
                    return tile;
                }
            }
        }
        Debug.Log("Could not find open PW.");
        return null;
    }

    private Tiles findOpenTile()
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
                Actor actor = tile.GetComponentInChildren<Actor>();
                if (actor.characterType == Actor.ActorType.None)
                {
                    Debug.Log(x + ", " + z + " Type: " + actor.characterType.ToString() + ", has PW: " + tile.hasPowerWell.ToString());
                    return tile;
                }
            }
        }
        Debug.Log("Could not find open tile.");
        return null;
    }
}
