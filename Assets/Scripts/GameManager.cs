using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public GameObject Tile, TileHighlight, PowerWell;
    public Material texEarth, texAir, texFire, texWater;
    public GUISkin mySkin;
    public float xOffset, zOffset, cameraX, cameraZ, cameraFOV, zoomMaxSteps, zoomFOV;
    public int xSize, zSize, numPowerWells;
    public enum RenderMode { Pattern, Random, StripeH, StripeV };
    public RenderMode renderMode;

    [HideInInspector]
    public Material[] mats;
    [HideInInspector]
    public GameObject Highlighting;
    [HideInInspector]
    public string[] elements;
    [HideInInspector]
    public bool isZoomed = false;
    private enum ZoomingMode { ZoomedIn, ZoomingIn, ZoomedOut, ZoomingOut };
    private ZoomingMode zoom = ZoomingMode.ZoomedOut;
    /// <summary>
    /// These are the zoom start/target x, z, and pov.
    /// </summary>
    private float zsX, zsZ, zsFOV, ztX, ztZ, ztFOV, zoomStep, zoomStart, xDiff, zDiff, fovDiff;
    private Tiles selectedTile;
    private Tiles[,] allTiles;

    // Use this for initialization
    void Start()
    {
        allTiles = new Tiles[xSize, zSize];
        mats = new Material[4] { texEarth, texAir, texFire, texWater };
        elements = new string[4] { "Earth", "Air", "Fire", "Water" };
        Highlighting = (GameObject)Instantiate(TileHighlight, new Vector3(0, -0.2f, 0), TileHighlight.transform.rotation);
        Camera.main.transform.position = new Vector3(cameraX, Camera.main.transform.position.y, cameraZ);
        generateMap();
    }

    private void generateMap()
    {
        GameObject tileHolder = new GameObject("TileHolder");
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                float xPos, zPos;
                xPos = x * xOffset;
                zPos = z * zOffset;
                if (x % 2 == 1)
                {
                    zPos += zOffset * 0.5f;
                }
                //Create the new tile.
                GameObject newTile = (GameObject)Instantiate(Tile, new Vector3(xPos, 0, zPos), Tile.transform.rotation);
                //Enable interaction with the tile aspects of the tile.
                Tiles tileInteracter = newTile.GetComponent<Tiles>();
                tileInteracter.manager = this;
                int chosenElement = colorPicker(x, z);
                tileInteracter.SetElement(chosenElement);
                tileInteracter.SetXandZ(x, z);
                tileInteracter.transform.parent = tileHolder.transform;
                allTiles[x, z] = tileInteracter;
            }
        }
        placePowerWells();
    }

    private void placePowerWells()
    {
        int x, z;
        int[] powerWellX = new int[8] { 0, 2, 4, 6, xSize - 1, xSize - 3, xSize - 5, xSize - 7 };
        int[] powerWellY = new int[8] { 0, zSize - 7, 2, zSize - 5, 4, zSize - 3, 6, zSize - 1 };
        int[] powerWellZ = new int[8] { 0, zSize - 3, 4, zSize - 7, zSize - 1, 2, zSize - 5, 6 };
        if (renderMode == RenderMode.Pattern)
        {
            numPowerWells = 8;
        }
        GameObject powerWellHolder = new GameObject("Power Wells");
        for (int i = 0; i < numPowerWells; i++)
        {
            if (renderMode == RenderMode.Random)
            {
                x = (int)Random.Range(0, xSize - 1);
                z = (int)Random.Range(0, zSize - 1);
            }
            else
            {
                x = powerWellX[i];
                z = powerWellZ[i];
            }
            allTiles[x, z].hasPowerWell = true;
            GameObject powerWell = (GameObject)Instantiate(PowerWell, new Vector3(allTiles[x, z].transform.position.x, 0F, allTiles[x, z].transform.position.z), allTiles[x, z].transform.rotation);
            powerWell.transform.parent = powerWellHolder.transform;
            powerWell.name = "PowerWell[" + x.ToString() + "," + z.ToString() + "]";
        }
    }

    private int colorPicker(int x, int z)
    {
        if (renderMode == RenderMode.StripeH)
        {

            return (z % mats.Length);
        }
        else if (renderMode == RenderMode.StripeV)
        {
            return (x % mats.Length);
        }
        else if (renderMode == RenderMode.Pattern)
        {
            int value, distToE, distToN, distToW, distToS;
            distToE = xSize - x - 1;
            distToN = zSize - z - 1;
            distToS = z;
            distToW = x;
            if (distToE < distToN && distToE < distToS && distToE < distToW)
            {
                value = distToE;
            }
            else if (distToN < distToS && distToN < distToW)
            {
                value = distToN;
            }
            else if (distToS < distToW)
            {
                value = distToS;
            }
            else
            {
                value = distToW;
            }
            return (((int)value / 2) % mats.Length);
        }
        else
        {
            return Random.Range(0, mats.Length);
        }
    }

    public void zoomOnToTile(Tiles tile)
    {
        //Debug.Log("zoom called, isZoomed:" + isZoomed.ToString() + "; tile tag:" + tile.tag + "; tile pos:" + tile.transform.position.ToString());
        if (zoom == ZoomingMode.ZoomedOut)
        {
            selectedTile = tile;
            zoom = ZoomingMode.ZoomingIn;
            ztX = tile.transform.position.x;
            ztZ = tile.transform.position.z;
            ztFOV = zoomFOV;
            zsX = Camera.main.transform.position.x;
            zsZ = Camera.main.transform.position.z;
            zsFOV = Camera.main.fieldOfView;
            setUpZoom();
        }
        else if (zoom == ZoomingMode.ZoomedIn)
        {
            setUpZoomOut();
        }
    }

    private void setUpZoomOut()
    {
        zoom = ZoomingMode.ZoomingOut;
        ztX = cameraX;
        ztZ = cameraZ;
        ztFOV = cameraFOV;
        zsX = Camera.main.transform.position.x;
        zsZ = Camera.main.transform.position.z;
        zsFOV = Camera.main.fieldOfView;
        setUpZoom();
    }

    private void setUpZoom()
    {
        xDiff = zsX - ztX;
        zDiff = zsZ - ztZ;
        fovDiff = zsFOV - ztFOV;
        zoomStart = Time.time;
    }

    // Update is called every frame, if the MonoBehaviour is enabled (Since v1.0)
    void Update()
    {
        if (zoom == ZoomingMode.ZoomingIn || zoom == ZoomingMode.ZoomingOut)
        {
            if (Time.time - zoomStart < zoomMaxSteps)
            {
                float timeStep = (Time.time - zoomStart) / zoomMaxSteps;
                float bezier = BezierBlend(timeStep);
                Camera.main.fieldOfView = zsFOV - (fovDiff * bezier);
                Camera.main.transform.position = new Vector3(zsX - (xDiff * bezier), Camera.main.transform.position.y, zsZ - (zDiff * bezier));
            }
            else
            {
                if (zoom == ZoomingMode.ZoomingOut)
                {
                    zoom = ZoomingMode.ZoomedOut;
                }
                else
                {
                    zoom = ZoomingMode.ZoomedIn;
                }
            }

        }
    }

    private float BezierBlend(float t)
    {
        return (t * t) * (3.0f - 2.0f * t);
    }

    void OnGUI()
    {
        if (zoom == ZoomingMode.ZoomedIn)
        {
            mySkin.window.fontSize = Screen.width / 20;
            mySkin.label.fontSize = Screen.width / 26;
            mySkin.button.fontSize = Screen.width / 20;
            Rect windowArea = new Rect(Screen.width * 0.2F, Screen.height * 0.2F, Screen.width * 0.6F, Screen.height * 0.6F);
            GUILayout.Window(1, windowArea, drawInfoBars, "", mySkin.window);
            //GUILayout.Window(1, windowArea, drawInfoBars, "", mySkin.window);
        }
    }

    private void drawInfoBars(int windowID)
    {
        GUI.skin = mySkin;
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("This is a " + selectedTile.ElementName + " elemental area.");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Unit info goes here.");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Move"))
        {
            setUpZoomOut();
        }
        if (GUILayout.Button("Summon"))
        {
            setUpZoomOut();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Offensive Spell"))
        {
            setUpZoomOut();
        }
        if (GUILayout.Button("Defensive Spell"))
        {
            setUpZoomOut();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

}
