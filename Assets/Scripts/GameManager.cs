using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{

    public GameObject Tile, TileHighlight, PowerWell;
    public Canvas Canvas;
    public GameObject PanelLeft, PanelRight, PanelTop, PanelBottom;
    public GameObject InfoZone, InfoUnit, InfoP1Power, InfoP2Power;
    public GameObject ButtonCancel, ButtonZoomOut, ButtonSummon, ButtonMove, ButtonOSpell, ButtonDSpell, ButtonAdeptE, ButtonAdeptA, ButtonAdeptF, ButtonAdeptW;
    public GameObject Castle, Adept, Demon, Monster, None;
    public GameObject[] TextDisplays;
    public Material texEarth, texAir, texFire, texWater, texVoid;
    public GUISkin mySkin;
    public float xOffset, zOffset, cameraX, cameraZ, cameraFOV, zoomMaxSteps, zoomFOV;
    public int xSize, zSize, numPowerWells;
    public enum RenderMode { Pattern, Random, StripeH, StripeV };
    public RenderMode renderMode;
    public enum ElementType { None, Earth, Air, Fire, Water };
    public enum GameMode { Default, PlaceSpawn, CastSpell, MoveSpawn };
    [HideInInspector]
    public GameMode gameMode = GameMode.Default;
    [HideInInspector]
    public Material[] mats;
    [HideInInspector]
    public GameObject Highlighting;
    [HideInInspector]
    public Tiles HighlightedTile;
    [HideInInspector]
    public bool isZoomed = false, Player1IsCPU = false, Player2IsCPU = false;
    [HideInInspector]
    public int CurrentPlayer = 1;
    [HideInInspector]
    public int[] PlayerPower;
    public enum ZoomingMode { ZoomedIn, ZoomingIn, ZoomedOut, ZoomingOut };
    [HideInInspector]
    public ZoomingMode zoom = ZoomingMode.ZoomedOut;
    private float zsX, zsZ, zsFOV, ztX, ztZ, ztFOV, zoomStep, zoomStart, xDiff, zDiff, fovDiff;
    private float canvasWidth, canvasHeight, panelWidth, panelHeight, panelAnchorX, panelAnchorY;
    private int fontSize;
    private bool[,] placedAdepts;
    private Tiles selectedTile;
    private Tiles[,] allTiles;
    private Actor IamSpawning;
    public Color[] PlayerColor;
    public Color[] ElementalColors;

    // Use this for initialization
    void Start()
    {
        //Place level select UI here.
        Start_SetupGame();
    }

    public void Start_SetupGame()
    {
        allTiles = new Tiles[xSize, zSize];
        mats = new Material[4] { texEarth, texAir, texFire, texWater };
        Highlighting = (GameObject)Instantiate(TileHighlight, new Vector3(0, -0.2f, 0), TileHighlight.transform.rotation);
        IamSpawning = new Actor();
        Camera.main.transform.position = new Vector3(cameraX, Camera.main.transform.position.y, cameraZ);
        PlayerPower = new int[2];
        placedAdepts = new bool[2, 4];
        for (int a = 0; a < 2; a++)
        {
            for (int b = 0; b < 4; b++)
            {
                placedAdepts[a, b] = false;
            }
        }
        recalculateCanvasSize();
        generateMap();
        Start_PanelSetup();
        Start_ButtonSetup();
    }

    private void Start_PanelSetup()
    {
        PanelLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(-panelWidth, 0f);
        PanelRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelWidth, 0f);
        PanelTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, panelHeight);
        PanelBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -panelHeight);
        InfoP1Power.GetComponent<Text>().color *= PlayerColor[0];
        InfoP1Power.GetComponent<Text>().text = "Player 1 Power:\r\n" + PlayerPower[0];
        InfoP2Power.GetComponent<Text>().color *= PlayerColor[1];
        InfoP2Power.GetComponent<Text>().text = "Player 2 Power:\r\n" + PlayerPower[1];
    }

    private void Start_ButtonSetup()
    {
        Button[] ButtonsLeft = PanelLeft.GetComponentsInChildren<Button>();
        Button[] ButtonsRight = PanelRight.GetComponentsInChildren<Button>();
        Button[] Buttons = ButtonsLeft.Concat(ButtonsRight).ToArray();
        foreach (Button button in Buttons)
        {
            button.GetComponentInChildren<Text>().text = "";
            button.interactable = false;
            button.onClick.RemoveAllListeners();
        }
        //Set up Cancel button.  This is the only constant button.
        ButtonCancel.GetComponentInChildren<Text>().text = "Cancel";
        ButtonCancel.GetComponent<Button>().onClick.AddListener(() => setUpZoomOut());
        ButtonCancel.GetComponent<Button>().interactable = true;
        //Set up Zoom Out button.  This is the only constant button.  It's a clone of cancel.
        ButtonZoomOut.GetComponentInChildren<Text>().text = "Zoom Out";
        ButtonZoomOut.GetComponent<Button>().onClick.AddListener(() => setUpZoomOut());
        ButtonZoomOut.GetComponent<Button>().interactable = true;
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
                tileInteracter.Manager = this;
                int chosenElement = colorPicker(x, z);
                tileInteracter.SetElement(chosenElement);
                tileInteracter.SetXandZ(x, z);
                tileInteracter.transform.parent = tileHolder.transform;
                allTiles[x, z] = tileInteracter;
                placeNewActor(x, z, Actor.ActorType.None, 0, 0);
            }
        }
        placePowerWells();
        placeActors();
    }

    private void placePowerWells()
    {
        int x, z;
        int[] powerWellX = new int[8] { 0, 2, 4, 6, xSize - 1, xSize - 3, xSize - 5, xSize - 7 };
        int[] powerWellZ = new int[8] { 0, zSize - 3, 4, zSize - 7, zSize - 1, 2, zSize - 5, 6 };
        if (renderMode == RenderMode.Pattern)
        {
            numPowerWells = 8;
        }
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
            powerWell.transform.parent = allTiles[x, z].transform;
            powerWell.name = "PowerWell[" + x.ToString() + "," + z.ToString() + "]";
        }
    }

    private void placeActors()
    {
        //TODO: Put castles at 3,7 and 19,7.
        //TODO: Put adepts at 2,6-10 and 20,6-10.
        placeNewActor(3, 7, Actor.ActorType.Castle, ElementType.None, 1);
        //placeNewActor(2, 6, Actor.ActorType.Adept, ElementType.Earth, 1);
        //placeNewActor(2, 7, Actor.ActorType.Adept, ElementType.Air, 1);
        //placeNewActor(2, 8, Actor.ActorType.Adept, ElementType.Fire, 1);
        //placeNewActor(2, 9, Actor.ActorType.Adept, ElementType.Water, 1);

        placeNewActor(19, 7, Actor.ActorType.Castle, ElementType.None, 2);
        //placeNewActor(20, 6, Actor.ActorType.Adept, ElementType.Earth, 2);
        //placeNewActor(20, 7, Actor.ActorType.Adept, ElementType.Air, 2);
        //placeNewActor(20, 8, Actor.ActorType.Adept, ElementType.Fire, 2);
        //placeNewActor(20, 9, Actor.ActorType.Adept, ElementType.Water, 2);
    }

    public void cancelAction()
    {
        gameMode = GameMode.Default;
        Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.82f);
        ButtonCancel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
    }

    public void placeAdeptEarth()
    {
        gameMode = GameMode.PlaceSpawn;
        IamSpawning.characterType = Actor.ActorType.Adept;
        IamSpawning.Element = ElementType.Earth;
        setUpZoomOut();
    }

    public void placeAdeptAir()
    {
        gameMode = GameMode.PlaceSpawn;
        IamSpawning.characterType = Actor.ActorType.Adept;
        IamSpawning.Element = ElementType.Air;
        setUpZoomOut();
    }

    public void placeAdeptFire()
    {
        gameMode = GameMode.PlaceSpawn;
        IamSpawning.characterType = Actor.ActorType.Adept;
        IamSpawning.Element = ElementType.Fire;
        setUpZoomOut();
    }

    public void placeAdeptWater()
    {
        gameMode = GameMode.PlaceSpawn;
        IamSpawning.characterType = Actor.ActorType.Adept;
        IamSpawning.Element = ElementType.Water;
        setUpZoomOut();
    }

    private void placeNewActor(int x, int z, Actor.ActorType type, ElementType element, int player)
    {
        GameObject instantiatee = null;
        switch (type)
        {
            case Actor.ActorType.Adept:
                instantiatee = Adept;
                placedAdepts[player - 1, ((int)element) - 1] = true;
                break;
            case Actor.ActorType.Demon:
                instantiatee = Demon;
                break;
            case Actor.ActorType.Monster:
                instantiatee = Monster;
                break;
            case Actor.ActorType.Castle:
                instantiatee = Castle;
                break;
            case Actor.ActorType.None:
            default:
                instantiatee = None;
                break;
        }
        if (allTiles[x, z].transform.FindChild("None") != null)
        {
            DestroyImmediate(allTiles[x, z].transform.FindChild("None").gameObject);
        }

        GameObject CharacterObject = (GameObject)Instantiate(instantiatee, allTiles[x, z].transform.position, instantiatee.transform.rotation);
        CharacterObject.name = type.ToString();
        if (CharacterObject.GetComponentInChildren<MeshRenderer>() != null)
        {
            CharacterObject.GetComponentInChildren<MeshRenderer>().material.color = CharacterObject.GetComponentInChildren<MeshRenderer>().material.color * PlayerColor[player - 1] * ElementalColors[(int)element];
        }
        Actor CharacterScript = CharacterObject.AddComponent<Actor>();
        CharacterScript.SetUp(type, x, z, (int)element, player, CharacterObject, this);

        CharacterObject.transform.parent = allTiles[x, z].transform;

    }

    private void moveActor(int oldX, int oldZ, int newX, int newZ)
    {

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
        if (gameMode == GameMode.Default)
        {
            if (zoom == ZoomingMode.ZoomedOut)
            {
                selectedTile = tile;
                Start_ButtonSetup();
                InfoZone.GetComponent<Text>().text = selectedTile.Element.ToString() + " Zone";
                if (selectedTile.hasPowerWell)
                {
                    InfoZone.GetComponent<Text>().text += " with Magic Source";
                }
                string elementName = "";
                if (selectedTile.GetComponentInChildren<Actor>().Element != ElementType.None)
                {
                    elementName = selectedTile.GetComponentInChildren<Actor>().Element.ToString();
                }
                switch (selectedTile.GetComponentInChildren<Actor>().characterType)
                {
                    case Actor.ActorType.None:
                        InfoUnit.GetComponent<Text>().text = "This zone is empty";
                        break;
                    case Actor.ActorType.Adept:
                        InfoUnit.GetComponent<Text>().text = "Player " + selectedTile.GetComponentInChildren<Actor>().Player.ToString() + "'s " + elementName + " Adept";
                        break;
                    case Actor.ActorType.Demon:
                        InfoUnit.GetComponent<Text>().text = "Player " + selectedTile.GetComponentInChildren<Actor>().Player.ToString() + "'s " + elementName + " Demon";
                        break;
                    case Actor.ActorType.Monster:
                        InfoUnit.GetComponent<Text>().text = "Player " + selectedTile.GetComponentInChildren<Actor>().Player.ToString() + "'s " + elementName + " Monster";
                        break;
                    case Actor.ActorType.Castle:
                        InfoUnit.GetComponent<Text>().text = "Player " + selectedTile.GetComponentInChildren<Actor>().Player.ToString() + "'s Castle";
                        if (selectedTile.GetComponentInChildren<Actor>().Player == CurrentPlayer)
                        {
                            if (placedAdepts[CurrentPlayer - 1, 0] == false)
                            {
                                ButtonAdeptE.GetComponentInChildren<Text>().text = "Spawn Earth Adept";
                                ButtonAdeptE.GetComponent<Button>().onClick.AddListener(() => placeAdeptEarth());
                                ButtonAdeptE.GetComponent<Button>().interactable = true;
                            }
                            if (placedAdepts[CurrentPlayer - 1, 1] == false)
                            {
                                ButtonAdeptA.GetComponentInChildren<Text>().text = "Spawn Air Adept";
                                ButtonAdeptA.GetComponent<Button>().onClick.AddListener(() => placeAdeptAir());
                                ButtonAdeptA.GetComponent<Button>().interactable = true;
                            }
                            if (placedAdepts[CurrentPlayer - 1, 2] == false)
                            {
                                ButtonAdeptF.GetComponentInChildren<Text>().text = "Spawn Fire Adept";
                                ButtonAdeptF.GetComponent<Button>().onClick.AddListener(() => placeAdeptFire());
                                ButtonAdeptF.GetComponent<Button>().interactable = true;
                            }
                            if (placedAdepts[CurrentPlayer - 1, 3] == false)
                            {
                                ButtonAdeptW.GetComponentInChildren<Text>().text = "Spawn Water Adept";
                                ButtonAdeptW.GetComponent<Button>().onClick.AddListener(() => placeAdeptWater());
                                ButtonAdeptW.GetComponent<Button>().interactable = true;
                            }
                        }
                        break;
                    default:
                        InfoUnit.GetComponent<Text>().text = "Error";
                        break;
                }

                zoom = ZoomingMode.ZoomingIn;
                ztX = tile.transform.position.x;
                ztZ = tile.transform.position.z;
                ztFOV = zoomFOV;
                zsX = Camera.main.transform.position.x;
                zsZ = Camera.main.transform.position.z;
                zsFOV = Camera.main.fieldOfView;
                setUpZoom();
            }
        }
        else
        {
            Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.82f);
            placeNewActor(tile.x, tile.z, IamSpawning.characterType, IamSpawning.Element, CurrentPlayer);
            gameMode = GameMode.Default;
            changePlayer();
        }
    }

    public void setUpZoomOut()
    {
        zoom = ZoomingMode.ZoomingOut;
        ztX = cameraX;
        ztZ = cameraZ;
        ztFOV = cameraFOV;
        zsX = Camera.main.transform.position.x;
        zsZ = Camera.main.transform.position.z;
        zsFOV = Camera.main.fieldOfView;
        setUpZoom();
        if (gameMode == GameMode.Default)
        {
            Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.82f);
        }
        else
        {
            Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f, 0.82f);
            //Setup the Cancel button to do the right stuff.
            //ButtonCancel.GetComponentInChildren<Text>().text = "Cancel";
            //ButtonCancel.GetComponent<Button>().onClick.AddListener(() => cancelAction());
            //ButtonCancel.GetComponent<Button>().interactable = true;

        }
    }

    private void setUpZoom()
    {
        xDiff = zsX - ztX;
        zDiff = zsZ - ztZ;
        fovDiff = zsFOV - ztFOV;
        zoomStart = Time.time;
    }

    private void recalculateCanvasSize()
    {
        canvasWidth = Canvas.transform.position.x * 2;
        canvasHeight = Canvas.transform.position.y * 2;
        panelWidth = canvasWidth * 0.25F;
        panelHeight = canvasHeight * 0.25F;
        fontSize = (int)(canvasWidth * 0.02f);
        for (int i = 0; i < TextDisplays.Length; i++)
        {
            TextDisplays[i].GetComponent<Text>().fontSize = fontSize;
        }
    }

    // Update is called every frame, if the MonoBehaviour is enabled (Since v1.0)
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && zoom == ZoomingMode.ZoomedOut)
        {
            zoomOnToTile(HighlightedTile);
        }
        if (canvasWidth != Canvas.transform.position.x * 2 || canvasHeight != Canvas.transform.position.y * 2)
        {
            recalculateCanvasSize();
        }

        if (zoom == ZoomingMode.ZoomingIn || zoom == ZoomingMode.ZoomingOut)
        {
            if (Time.time - zoomStart < zoomMaxSteps)
            {
                float timeStep = (Time.time - zoomStart) / zoomMaxSteps;
                float bezier = BezierBlend(timeStep);
                Camera.main.fieldOfView = zsFOV - (fovDiff * bezier);
                Camera.main.transform.position = new Vector3(zsX - (xDiff * bezier), Camera.main.transform.position.y, zsZ - (zDiff * bezier));
                if (zoom == ZoomingMode.ZoomingIn)
                {
                    float x = (bezier * panelWidth) - panelWidth;
                    float y = (bezier * panelHeight) - panelHeight;
                    PanelLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0f);
                    PanelRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(-x, 0f);
                    PanelTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -y);
                    PanelBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, y);
                }
                else
                {
                    float x = -(bezier * panelWidth);
                    float y = -(bezier * panelHeight);
                    PanelLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0f);
                    PanelRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(-x, 0f);
                    PanelTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -y);
                    PanelBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, y);
                    //if (gameMode != GameMode.Default)
                    //{
                    //    ButtonCancel.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0f);
                    //}
                }
            }
            else
            {
                if (zoom == ZoomingMode.ZoomingOut)
                {
                    zoom = ZoomingMode.ZoomedOut;
                    isZoomed = false;
                }
                else
                {
                    zoom = ZoomingMode.ZoomedIn;
                    isZoomed = true;
                }
            }

        }
    }

    private float BezierBlend(float t)
    {
        return (t * t) * (3.0f - 2.0f * t);
    }

    public void changePlayer()
    {
        CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;

        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                if (allTiles[x, z].transform.Find("Adept") != null)
                {
                    GameObject adept = allTiles[x, z].transform.Find("Adept").gameObject;
                    if (allTiles[x, z].hasPowerWell && adept.GetComponent<Actor>().Player == CurrentPlayer)
                    {
                        PlayerPower[CurrentPlayer - 1] += 10;
                    }
                }
            }
        }

        InfoP1Power.GetComponent<Text>().text = "Player 1 Power:\r\n" + PlayerPower[0];
        InfoP2Power.GetComponent<Text>().text = "Player 2 Power:\r\n" + PlayerPower[1];
    }

}
