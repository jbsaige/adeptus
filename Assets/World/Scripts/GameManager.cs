using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject Tile, TileHighlight, PowerWell;
    public Canvas Canvas;
    public GameObject PanelLeft, PanelRight, PanelTop, PanelBottom, PanelTips, PanelSummonTop, PanelSummonBottom;
    public GameObject InfoZone, InfoUnit, InfoP1Power, InfoP2Power, InfoTips;
    public GameObject ButtonCancel, ButtonZoomOut, ButtonArmageddon, ButtonMove, ButtonOSpell, ButtonDSpell, ButtonAdeptE, ButtonAdeptA, ButtonAdeptF, ButtonAdeptW;
    public GameObject ButtonSME, ButtonSMA, ButtonSMF, ButtonSMW, ButtonSEE, ButtonSEA, ButtonSEF, ButtonSEW;
    public GameObject Castle, Adept, Demon, Monster, None;
    public GameObject[] TextDisplays;
    public Material texEarth, texAir, texFire, texWater, texVoid;
    public GUISkin mySkin;
    public float xOffset, zOffset, cameraX, cameraZ, cameraFOV, zoomMaxSteps, zoomFOV;
    public int xSize, zSize, numPowerWells, powerSummonMonster, powerSummonElemental, powerArmageddon, powerOSpell, powerDSpell, powerMove;
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
    public bool isZoomed = false, Player1IsCPU = false, Player2IsCPU = false, isTutorial = true;
    [HideInInspector]
    public int CurrentPlayer = 1;
    [HideInInspector]
    public int[] PlayerPower;
    public enum ZoomingMode { ZoomedIn, ZoomingIn, ZoomedOut, ZoomingOut };
    public ZoomingMode zoom = ZoomingMode.ZoomedOut;
    public enum SummoningMenuMode { SlidedIn, SlidingIn, SlidedOut, SlidingOut };
    public SummoningMenuMode summonMode = SummoningMenuMode.SlidedOut;
    [HideInInspector]
    private float zsX, zsZ, zsFOV, ztX, ztZ, ztFOV, zoomStep, zoomStart, xDiff, zDiff, fovDiff;
    private float canvasWidth, canvasHeight, panelWidth, panelHeight, panelAnchorX, panelAnchorY;
    private int fontSize, pendingPowerExpendature;
    private bool[,] placedAdepts;
    private Tiles selectedTile;
    private Tiles[,] allTiles;
    private Actor IamSpawning;
    public Color[] PlayerColor;
    public Color[] ElementalColors;
    private bool showingTip = false;
    private float hideTipWhen = 0f;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        //Place level select UI here.
        Start_SetupGame();
    }

    public void Start_SetupGame()
    {
        if (powerArmageddon < 1)
        {
            powerArmageddon = 1000;
        }
        if (powerSummonElemental < 1)
        {
            powerSummonElemental = 100;
        }
        if (powerSummonMonster < 1)
        {
            powerSummonMonster = 50;
        }
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
        Start_ButtonSetup(true);
    }

    private void Start_PanelSetup()
    {
        PanelLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(-panelWidth, 0f);
        PanelRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelWidth, 0f);
        PanelTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, panelHeight);
        PanelSummonTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, panelHeight * 2);
        PanelBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -panelHeight);
        PanelSummonBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -panelHeight * 2);
        InfoP1Power.GetComponent<Text>().color *= PlayerColor[0];
        InfoP1Power.GetComponent<Text>().text = "Player 1 Power:\r\n" + PlayerPower[0];
        InfoP2Power.GetComponent<Text>().color *= PlayerColor[1];
        InfoP2Power.GetComponent<Text>().text = "Player 2 Power:\r\n" + PlayerPower[1];
    }

    private void Start_ButtonSetup()
    {
        Start_ButtonSetup(false);
    }

    private void Start_ButtonSetup(bool RunFirstTime)
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
        if (RunFirstTime)
        {
            ButtonSME.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Earth, Actor.ActorType.Monster));
            ButtonSMA.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Air, Actor.ActorType.Monster));
            ButtonSMF.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Fire, Actor.ActorType.Monster));
            ButtonSMW.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Water, Actor.ActorType.Monster));
            ButtonSEE.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Earth, Actor.ActorType.Demon));
            ButtonSEA.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Air, Actor.ActorType.Demon));
            ButtonSEF.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Fire, Actor.ActorType.Demon));
            ButtonSEW.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Water, Actor.ActorType.Demon));

        }
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
        Start_PlaceActors();
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

    private void Start_PlaceActors()
    {
        placeNewActor(3, 7, Actor.ActorType.Castle, ElementType.None, 1);
        placeNewActor(19, 7, Actor.ActorType.Castle, ElementType.None, 2);
    }

    public void cancelAction()
    {
        gameMode = GameMode.Default;
        Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.82f);
        ButtonCancel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
        pendingPowerExpendature = 0;
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
        Color primerColor = new Color(1f, 1f, 1f);
        Debug.Log("Trying to spawn " + element.ToString() + type.ToString() + " at " + x.ToString() + "," + z.ToString() + " for player " + player.ToString());
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
        Debug.Log("Setting Colors");
        SkinnedMeshRenderer[] skMeshes = CharacterObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (skMeshes != null && skMeshes.Length > 0)
        {
            foreach (SkinnedMeshRenderer mesh in skMeshes)
            {
                Material[] mats = mesh.materials;
                if (mats != null && mats.Length > 0)
                {
                    foreach (Material mat in mats)
                    {
                        mat.color = primerColor * PlayerColor[player - 1] * ElementalColors[(int)element];
                        Debug.Log("Color modified");
                    }
                }
                else
                {
                    Debug.Log("Material not found.");
                }
            }
        }
        else
        {
            MeshRenderer[] meshes = CharacterObject.GetComponentsInChildren<MeshRenderer>();
            if (meshes != null && meshes.Length > 0)
            {
                foreach (MeshRenderer mesh in meshes)
                {
                    Material[] mats = mesh.materials;
                    if (mats != null && mats.Length > 0)
                    {
                        foreach (Material mat in mats)
                        {
                            mat.color = primerColor * PlayerColor[player - 1] * ElementalColors[(int)element];
                            Debug.Log("Color modified");
                        }
                    }
                    else
                    {
                        Debug.Log("Material not found.");
                    }
                }
            }
            else
            {
                Debug.Log("MeshRender not found.");
            }
        }
        Actor CharacterScript = CharacterObject.AddComponent<Actor>();
        CharacterScript.SetUp(type, x, z, (int)element, player, CharacterObject, this);

        CharacterObject.transform.parent = allTiles[x, z].transform;

    }

    private void moveActor()
    {
        gameMode = GameMode.MoveSpawn;
        setUpZoomOut();
    }

    private void moveActor(int oldX, int oldZ, int newX, int newZ)
    {
        Actor movingActor = allTiles[oldX, oldZ].GetComponentInChildren<Actor>();
        Actor emptyActor = allTiles[newX, newZ].GetComponentInChildren<Actor>();
        movingActor.transform.parent = allTiles[newX, newZ].transform;
        emptyActor.transform.parent = allTiles[oldX, oldZ].transform;
        movingActor.Move(newX, newZ);
        movingActor.transform.position = allTiles[newX, newZ].transform.position;
    }

    private void triggerBattle(Actor P1Piece, Actor P2Piece)
    {
        //TODO: Trigger the real battle.
        //showingTip = true;
        //hideTipWhen = Time.time + 1f;
        //InfoTips.GetComponent<Text>().text = "Player " + CurrentPlayer + "'s" + P1Piece.Element.ToString() + " " + P1Piece.characterType.ToString() + " is attacking!\r\nOppoent's " + P2Piece.Element.ToString() + " " + P2Piece.characterType.ToString() + " is defending!\r\nGood Luck!";
        Application.LoadLevel("Sandbox");
    }

    private void adeptSummonAction(ElementType element, Actor.ActorType type)
    {
        IamSpawning.characterType = type;
        IamSpawning.Element = element;
        pendingPowerExpendature = -50;
        gameMode = GameMode.PlaceSpawn;
        setUpZoomOut();
    }

    private void finishAdeptSummonAction()
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
                Start_ButtonSetup(false);
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
                        if (selectedTile.GetComponentInChildren<Actor>().Player == CurrentPlayer)
                        {
                            ButtonMove.GetComponentInChildren<Text>().text = "Move\r\n(" + powerMove.ToString() + "p)";
                            ButtonDSpell.GetComponentInChildren<Text>().text = "++ Spell\r\n(" + powerDSpell.ToString() + "p)";
                            ButtonOSpell.GetComponentInChildren<Text>().text = "-- Spell\r\n(" + powerOSpell.ToString() + "p)";
                            ButtonArmageddon.GetComponentInChildren<Text>().text = "Armageddon\r\n(" + powerArmageddon.ToString() + "p)";
                            summonMode = SummoningMenuMode.SlidingIn;
                            if (PlayerPower[CurrentPlayer - 1] >= powerMove)
                            {
                                ButtonMove.GetComponent<Button>().onClick.AddListener(() => moveActor());
                                ButtonMove.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonMove.GetComponent<Button>().interactable = false;
                            }
                            if (PlayerPower[CurrentPlayer - 1] >= powerOSpell)
                            {
                                //ButtonOSpell.GetComponent<Button>().onClick.AddListener(() => moveActor());
                                ButtonOSpell.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonOSpell.GetComponent<Button>().interactable = false;
                            }
                            if (PlayerPower[CurrentPlayer - 1] >= powerDSpell)
                            {
                                //ButtonDSpell.GetComponent<Button>().onClick.AddListener(() => moveActor());
                                ButtonDSpell.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonDSpell.GetComponent<Button>().interactable = false;
                            }
                            if (PlayerPower[CurrentPlayer - 1] >= powerSummonMonster)
                            {
                                ButtonSEE.GetComponent<Button>().interactable = true;
                                ButtonSEA.GetComponent<Button>().interactable = true;
                                ButtonSEF.GetComponent<Button>().interactable = true;
                                ButtonSEW.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonSME.GetComponent<Button>().interactable = false;
                                ButtonSMA.GetComponent<Button>().interactable = false;
                                ButtonSMF.GetComponent<Button>().interactable = false;
                                ButtonSMW.GetComponent<Button>().interactable = false;
                            }
                            if (PlayerPower[CurrentPlayer - 1] >= powerSummonElemental)
                            {
                                ButtonSME.GetComponent<Button>().interactable = true;
                                ButtonSMA.GetComponent<Button>().interactable = true;
                                ButtonSMF.GetComponent<Button>().interactable = true;
                                ButtonSMW.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonSEE.GetComponent<Button>().interactable = false;
                                ButtonSEA.GetComponent<Button>().interactable = false;
                                ButtonSEF.GetComponent<Button>().interactable = false;
                                ButtonSEW.GetComponent<Button>().interactable = false;
                            }
                            if (PlayerPower[CurrentPlayer - 1] >= powerArmageddon)
                            {
                                //ButtonArmageddon.GetComponent<Button>().onClick.AddListener(() => moveActor());
                                ButtonArmageddon.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonArmageddon.GetComponent<Button>().interactable = false;
                            }
                        }
                        break;
                    case Actor.ActorType.Demon:
                        InfoUnit.GetComponent<Text>().text = "Player " + selectedTile.GetComponentInChildren<Actor>().Player.ToString() + "'s " + elementName + " Demon";
                        if (selectedTile.GetComponentInChildren<Actor>().Player == CurrentPlayer)
                        {
                            ButtonMove.GetComponentInChildren<Text>().text = "Move\r\n(" + powerMove.ToString() + "p)";
                            if (PlayerPower[CurrentPlayer - 1] >= powerMove)
                            {
                                ButtonMove.GetComponent<Button>().onClick.AddListener(() => moveActor());
                                ButtonMove.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonMove.GetComponent<Button>().interactable = false;
                            }
                        }
                        break;
                    case Actor.ActorType.Monster:
                        InfoUnit.GetComponent<Text>().text = "Player " + selectedTile.GetComponentInChildren<Actor>().Player.ToString() + "'s " + elementName + " Monster";
                        if (selectedTile.GetComponentInChildren<Actor>().Player == CurrentPlayer)
                        {
                            ButtonMove.GetComponentInChildren<Text>().text = "Move\r\n(" + powerMove.ToString() + "p)";
                            if (PlayerPower[CurrentPlayer - 1] >= powerMove)
                            {
                                ButtonMove.GetComponent<Button>().onClick.AddListener(() => moveActor());
                                ButtonMove.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonMove.GetComponent<Button>().interactable = false;
                            }
                        }
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
            if (tile.GetComponentInChildren<Actor>().characterType == Actor.ActorType.None)
            {
                //If the gameMode is CastSpell we don't care.  Players cannot cast onto empty hexes.
                if (gameMode == GameMode.PlaceSpawn)
                {
                    //This hex is empty.  Place the new thing here.
                    Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.82f);
                    placeNewActor(tile.x, tile.z, IamSpawning.characterType, IamSpawning.Element, CurrentPlayer);
                    gameMode = GameMode.Default;
                    changePlayer();
                }
                else if (gameMode == GameMode.MoveSpawn)
                {
                    //This hex is empty.  Move to here.
                    Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.82f);
                    moveActor(selectedTile.x, selectedTile.z, tile.x, tile.z);
                    gameMode = GameMode.Default;
                    changePlayer();
                }
            }
            else if (tile.GetComponentInChildren<Actor>().Player == CurrentPlayer)
            {
                //The player clicked on their own piece.
                if (gameMode == GameMode.CastSpell)
                {
                    //The player wants to cast a spell on their own piece.
                }
                else
                {
                    //Let's cancel the current mode and return to default.
                    Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.82f);
                    gameMode = GameMode.Default;
                }
            }
            else// if (tile.GetComponentInChildren<Actor>().Player != CurrentPlayer)
            {//The above else if appears redundant, maybe use else.
                //The player clicked on an oppoent's piece.
                if (gameMode == GameMode.CastSpell)
                {
                    //The player wants to cast a spell on an oppoent's piece.
                }
                else if (gameMode == GameMode.MoveSpawn)
                {
                    //The player wants to battle.
                    //TODO: Trigger battle.
                    Debug.Log("Trigger Battle Here!");
                    triggerBattle(selectedTile.GetComponentInChildren<Actor>(), tile.GetComponentInChildren<Actor>());
                }
            }
        }
    }

    public void setUpZoomOut()
    {
        if (summonMode == SummoningMenuMode.SlidedIn)
        {
            summonMode = SummoningMenuMode.SlidingOut;
        }
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
        fontSize = (int)(canvasWidth * 0.025f);
        for (int i = 0; i < TextDisplays.Length; i++)
        {
            TextDisplays[i].GetComponent<Text>().fontSize = fontSize;
        }
        if (zoom == ZoomingMode.ZoomedOut)
        {
            PanelLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(-panelWidth, 0f);
            PanelRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelWidth, 0f);
            PanelTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, panelHeight);
            PanelBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -panelHeight);
        }
        if (summonMode == SummoningMenuMode.SlidedOut)
        {
            PanelSummonTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, panelHeight * 2);
            PanelSummonBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -panelHeight * 2);
        }
    }

    // Update is called every frame, if the MonoBehaviour is enabled (Since v1.0)
    void Update()
    {
        if (showingTip == true && Time.time > hideTipWhen)
        {
            PanelTips.GetComponent<Image>().color = new Color(192f, 192f, 192f, 0f);
            InfoTips.GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
            showingTip = false;
        }
        else if (showingTip == true)
        {
            float alphaBlend = 255f;
            float timeRemaning = hideTipWhen - Time.time;
            if (timeRemaning > zoomMaxSteps - 0.25f)
            {
                alphaBlend = BezierBlend((0.25f + ((zoomMaxSteps - 0.25f) - timeRemaning)) * 4f);
            }
            else if (timeRemaning < 0.25F)
            {
                alphaBlend = BezierBlend((timeRemaning) * 4f);
            }
            PanelTips.GetComponent<Image>().color = new Color(192f, 192f, 192f, alphaBlend);
            InfoTips.GetComponent<Text>().color = new Color(0f, 0f, 0f, alphaBlend);
        }
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
                float x, y, y2;
                if (zoom == ZoomingMode.ZoomingIn)
                {
                    x = (bezier * panelWidth) - panelWidth;
                    y = (bezier * panelHeight) - panelHeight;
                    y2 = (bezier * panelHeight * 2) - panelHeight * 2;
                }
                else
                {
                    x = -(bezier * panelWidth);
                    y = -(bezier * panelHeight);
                    y2 = -(bezier * panelHeight * 2);
                }
                PanelLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0f);
                PanelRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(-x, 0f);
                PanelTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -y);
                PanelBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, y);
                if (summonMode == SummoningMenuMode.SlidingIn || summonMode == SummoningMenuMode.SlidingOut)
                {
                    PanelSummonTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -y2);
                    PanelSummonBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, y2);
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

                if (summonMode == SummoningMenuMode.SlidingIn)
                {
                    summonMode = SummoningMenuMode.SlidedIn;
                }
                else
                {
                    summonMode = SummoningMenuMode.SlidedOut;
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
                        PlayerPower[CurrentPlayer - 1] += 10 - pendingPowerExpendature;
                    }
                }
            }
        }

        InfoP1Power.GetComponent<Text>().text = "Player 1 Power:\r\n" + PlayerPower[0];
        InfoP2Power.GetComponent<Text>().text = "Player 2 Power:\r\n" + PlayerPower[1];
        showingTip = true;
        hideTipWhen = Time.time + zoomMaxSteps;
        InfoTips.GetComponent<Text>().text = "Player " + CurrentPlayer + "'s Turn!\r\nCurrent Power: " + PlayerPower[CurrentPlayer - 1].ToString();
        //PanelTips.GetComponent<Image>().color = new Color(192f, 192f, 192f, 0f);
        //InfoTips.GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
    }

}
