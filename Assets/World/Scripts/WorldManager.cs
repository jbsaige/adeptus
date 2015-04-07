using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{

    public GameObject Tile, TileHighlight, PowerWell;
    public Canvas Canvas;
    public GameObject PanelLeft, PanelRight, PanelTop, PanelBottom, PanelTips, PanelSummonTop, PanelSummonBottom, TutorialPanel, TutorialImage, TutorialText;
    public GameObject InfoZone, InfoUnit, InfoP1Power, InfoP2Power, InfoTips;
    public GameObject ButtonCancel, ButtonZoomOut, ButtonArmageddon, ButtonMove, ButtonOSpell, ButtonDSpell, ButtonAdeptE, ButtonAdeptA, ButtonAdeptF, ButtonAdeptW;
    public GameObject ButtonSME, ButtonSMA, ButtonSMF, ButtonSMW, ButtonSEE, ButtonSEA, ButtonSEF, ButtonSEW;
    public GameObject Castle, Adept, Demon, Monster, None;
    public GameObject[] TextDisplays;
    public Material texEarth, texAir, texFire, texWater, texVoid, transMat;
    public Shader transShader;
    public GUISkin mySkin;
    public float xOffset, zOffset, cameraX, cameraY, cameraZ, cameraFOV, zoomMaxSteps, zoomFOV, zoomY;
    public int xSize, zSize, numPowerWells, powerSummonMonster, powerSummonElemental, powerArmageddon, powerOSpell, powerDSpell, powerMove;
    public enum RenderMode
    {
        Pattern,
        Random,
        StripeH,
        StripeV,
        FromStored
    };
    public RenderMode renderMode;
    public enum ElementType
    {
        Void,
        Earth,
        Air,
        Fire,
        Water
    };
    public enum GameMode
    {
        Default,
        PlaceSpawn,
        CastSpell,
        MoveSpawn,
        Armageddon
    };
    [HideInInspector]
    public Material[] mats;
    [HideInInspector]
    public GameObject Highlighting;
    [HideInInspector]
    public Tiles HighlightedTile;
    [HideInInspector]
    public bool isZoomed = false, Player1IsCPU = false, Player2IsCPU = false, isTutorial = true;
    [HideInInspector]
    public enum ZoomingMode
    {
        ZoomedIn,
        ZoomingIn,
        ZoomedOut,
        ZoomingOut
    };
    public ZoomingMode zoom = ZoomingMode.ZoomedOut;
    public enum SummoningMenuMode
    {
        SlidedIn,
        SlidingIn,
        SlidedOut,
        SlidingOut
    };
    public SummoningMenuMode summonMode = SummoningMenuMode.SlidedOut;
    [HideInInspector]
    private float zsX, zsY, zsZ, zsFOV, ztX, ztY, ztZ, ztFOV, zoomStep, zoomStart, xDiff, yDiff, zDiff, fovDiff;
    private float canvasWidth, canvasHeight, panelWidth, panelHeight, panelAnchorX, panelAnchorY;
    private int fontSize, pendingPowerExpendature;
    [HideInInspector]
    private Tiles selectedTile;
    private Actor IamSpawning;
    public Color[] PlayerColor;
    public Color[] ElementalColors;
    private bool showingTip = false, showingTutorial = false;
    private float hideTipWhen = 0f;
    private GameManager GameManager;

    // Use this for initialization
    void Start()
    {
        GameManager = FindObjectOfType<GameManager>();
        GameManager.FinishLoad(this);
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.GameManager = gameManager;
    }

    public void setAllTiles(Tiles[,] tiles)
    {
        GameManager.TileManger.allTiles = tiles;
    }

    public Tiles[,] getAllTiles()
    {
        return GameManager.TileManger.allTiles;
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
        if (GameManager.TileManger.allTiles == null)
        {
            GameManager.TileManger.allTiles = new Tiles[xSize, zSize];
        }
        mats = new Material[4] { texEarth, texAir, texFire, texWater };
        Highlighting = (GameObject)Instantiate(TileHighlight, new Vector3(0, -0.2f, 0), TileHighlight.transform.rotation);
        IamSpawning = new Actor();
        Camera.main.transform.position = new Vector3(cameraX, Camera.main.transform.position.y, cameraZ);
        //GameManager.placedAdepts = new bool[2, 4];
        //for (int a = 0; a < 2; a++)
        //{
        //    for (int b = 0; b < 4; b++)
        //    {
        //        GameManager.placedAdepts[a, b] = false;
        //    }
        //}
        recalculateCanvasSize();
        if (renderMode == RenderMode.FromStored)
        {
            regenerateMap();
        }
        else
        {
            generateMap();
            placePowerWells();
            Start_PlaceActors();
        }
        Start_PanelSetup();
        Start_ButtonSetup(true);
    }

    private void Start_PanelSetup()
    {
        PanelLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(-panelWidth, 0f);
        PanelRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelWidth, 0f);
        PanelTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, panelHeight);
        PanelBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -panelHeight);
        InfoP1Power.GetComponent<Text>().color *= PlayerColor[0];
        InfoP1Power.GetComponent<Text>().text = "Player 1 Power:\r\n" + GameManager.PlayerPower[0];
        InfoP2Power.GetComponent<Text>().color *= PlayerColor[1];
        InfoP2Power.GetComponent<Text>().text = "Player 2 Power:\r\n" + GameManager.PlayerPower[1];
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
    }

    private void regenerateMap()
    {
        GameObject tileHolder = new GameObject("TileHolder");
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                Tiles oldTile = GameManager.TileManger.allTiles[x, z];
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
                tileInteracter.SetElement((int)oldTile.Element - 1);
                tileInteracter.SetXandZ(x, z);
                tileInteracter.transform.parent = tileHolder.transform;
                tileInteracter.hasPowerWell = oldTile.hasPowerWell;
                GameManager.TileManger.allTiles[x, z] = tileInteracter;
                placeNewActor(x, z, oldTile.Actor.characterType, oldTile.Actor.Element, oldTile.Actor.Player);
                if (oldTile.hasPowerWell)
                {
                    GameObject powerWell = (GameObject)Instantiate(PowerWell, new Vector3(GameManager.TileManger.allTiles[x, z].transform.position.x, 0F, GameManager.TileManger.allTiles[x, z].transform.position.z), GameManager.TileManger.allTiles[x, z].transform.rotation);
                    powerWell.transform.parent = GameManager.TileManger.allTiles[x, z].transform;
                    powerWell.name = "PowerWell[" + x.ToString() + "," + z.ToString() + "]";
                }
            }
        }
    }

    public void generateMap()
    {
        if (GameManager != null)
        {
            GameManager.CurrentPlayer = 0;
        }
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
                GameManager.TileManger.allTiles[x, z] = tileInteracter;
                placeNewActor(x, z, Actor.ActorType.None, 0, 0);
            }
        }
    }

    private void placePowerWells()
    {
        if (renderMode != RenderMode.FromStored)
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
                GameManager.TileManger.allTiles[x, z].hasPowerWell = true;
                GameObject powerWell = (GameObject)Instantiate(PowerWell, new Vector3(GameManager.TileManger.allTiles[x, z].transform.position.x, 0F, GameManager.TileManger.allTiles[x, z].transform.position.z), GameManager.TileManger.allTiles[x, z].transform.rotation);
                powerWell.transform.parent = GameManager.TileManger.allTiles[x, z].transform;
                powerWell.name = "PowerWell[" + x.ToString() + "," + z.ToString() + "]";
            }
        }
        else
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    if (GameManager.TileManger.allTiles[x, z].hasPowerWell == true)
                    {
                        GameObject powerWell = (GameObject)Instantiate(PowerWell, new Vector3(GameManager.TileManger.allTiles[x, z].transform.position.x, 0F, GameManager.TileManger.allTiles[x, z].transform.position.z), GameManager.TileManger.allTiles[x, z].transform.rotation);
                        powerWell.transform.parent = GameManager.TileManger.allTiles[x, z].transform;
                        powerWell.name = "PowerWell[" + x.ToString() + "," + z.ToString() + "]";
                    }
                }
            }
        }
    }

    private void Start_PlaceActors()
    {
        if (renderMode == RenderMode.FromStored)
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    Actor actor = GameManager.TileManger.allTiles[x, z].Actor;
                    if (actor.characterType != Actor.ActorType.None)
                    {
                        placeNewActor(x, z, actor.characterType, actor.Element, actor.Player);
                    }
                }
            }
        }
        else
        {
            placeNewActor(3, 7, Actor.ActorType.Castle, ElementType.Void, 1);
            placeNewActor(19, 7, Actor.ActorType.Castle, ElementType.Void, 2);
        }
    }

    public void cancelAction()
    {
        GameManager.GameMode = GameMode.Default;
        Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.82f);
        ButtonCancel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
        pendingPowerExpendature = 0;
    }

    public void placeAdept(ElementType Type)
    {
        GameManager.GameMode = GameMode.PlaceSpawn;
        IamSpawning.characterType = Actor.ActorType.Adept;
        IamSpawning.Element = Type;
        setUpZoomOut();
    }

    public void placeNewActor(int x, int z, Actor.ActorType type, ElementType element, int player)
    {
        GameObject instantiatee = null;
        switch (type)
        {
            case Actor.ActorType.Adept:
                instantiatee = Adept;
                GameManager.placedAdepts[player - 1, ((int)element) - 1] = true;
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
        if (GameManager.TileManger.allTiles[x, z].transform.FindChild("None") != null)
        {
            DestroyImmediate(GameManager.TileManger.allTiles[x, z].transform.FindChild("None").gameObject);
        }

        GameObject CharacterObject = (GameObject)Instantiate(instantiatee, GameManager.TileManger.allTiles[x, z].transform.position, instantiatee.transform.rotation);
        CharacterObject.name = type.ToString();
        //Some objects have SkinnedMeshRenderer, some have MeshRenderer.
        SkinnedMeshRenderer[] skMeshes = CharacterObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (skMeshes != null && skMeshes.Length > 0)
        {
            foreach (SkinnedMeshRenderer mesh in skMeshes)
            {
                Material[] mats = mesh.materials;
                ColorActor(mats, player, (int)element);
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
                    ColorActor(mats, player, (int)element);
                }
            }
        }
        Actor CharacterScript = CharacterObject.AddComponent<Actor>();
        CharacterScript.SetUp(type, x, z, (int)element, player, CharacterObject, this);

        CharacterObject.transform.parent = GameManager.TileManger.allTiles[x, z].transform;
        GameManager.TileManger.allTiles[x, z].Actor = CharacterScript;
    }

    private void ColorActor(Material[] mats, int player, int element)
    {
        Color primerColor = new Color(1f, 1f, 1f);
        if (mats != null && mats.Length > 0)
        {
            foreach (Material mat in mats)
            {
                mat.color = primerColor * PlayerColor[player - 1] * ElementalColors[element];
            }
        }
        else
        {
        }
    }

    private void moveActor()
    {
        GameManager.GameMode = GameMode.MoveSpawn;
        setUpZoomOut();
    }

    public void moveActor(int oldX, int oldZ, int newX, int newZ)
    {
        Actor movingActor = GameManager.TileManger.allTiles[oldX, oldZ].GetComponentInChildren<Actor>();
        Actor emptyActor = GameManager.TileManger.allTiles[newX, newZ].GetComponentInChildren<Actor>();
        //Update the transform parents
        movingActor.transform.parent = GameManager.TileManger.allTiles[newX, newZ].transform;
        emptyActor.transform.parent = GameManager.TileManger.allTiles[oldX, oldZ].transform;
        //Update the Tiles.Actor information.
        GameManager.TileManger.allTiles[oldX, oldZ].Actor = emptyActor;
        GameManager.TileManger.allTiles[newX, newZ].Actor = movingActor;
        //Do the actual move.
        movingActor.Move(newX, newZ);
        movingActor.transform.position = GameManager.TileManger.allTiles[newX, newZ].transform.position;
    }

    public void destroyActor(Actor actor)
    {
        int x = actor.x;
        int z = actor.z;
        Destroy(actor);
        //Required to keep the consistancy.
        placeNewActor(x, z, Actor.ActorType.None, 0, 0);
    }

    public void destroyActorAt(int x, int z)
    {
        Actor actor = GameManager.TileManger.allTiles[x, z].GetComponentInChildren<Actor>();
        Destroy(actor);
        //Required to keep the consistancy.
        placeNewActor(x, z, Actor.ActorType.None, 0, 0);
    }

    public void triggerBattle(Actor P1Piece, Actor P2Piece, Tiles BattleTile)
    {
        GameManager.LoadBattle(P1Piece, P2Piece, BattleTile.Element.ToString());
    }

    private void adeptSummonAction(ElementType element, Actor.ActorType type)
    {
        IamSpawning.characterType = type;
        IamSpawning.Element = element;
        pendingPowerExpendature = -50;
        GameManager.GameMode = GameMode.PlaceSpawn;
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
        else if (renderMode == RenderMode.FromStored)
        {
            return (int)(GameManager.TileManger.allTiles[x, z].Element) - 1;
        }
        else
        {
            return Random.Range(0, mats.Length);
        }
    }

    public void zoomOnToTile(Tiles tile)
    {
        if (GameManager.GameMode == GameMode.Default)
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
                if (selectedTile.GetComponentInChildren<Actor>().Element != ElementType.Void)
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
                        if (selectedTile.GetComponentInChildren<Actor>().Player == GameManager.CurrentPlayer)
                        {
                            ButtonMove.GetComponentInChildren<Text>().text = "Move\r\n(" + powerMove.ToString() + "p)";
                            ButtonDSpell.GetComponentInChildren<Text>().text = "++ Spell\r\n(" + powerDSpell.ToString() + "p)";
                            ButtonOSpell.GetComponentInChildren<Text>().text = "-- Spell\r\n(" + powerOSpell.ToString() + "p)";
                            ButtonArmageddon.GetComponentInChildren<Text>().text = "Armageddon\r\n(" + powerArmageddon.ToString() + "p)";
                            summonMode = SummoningMenuMode.SlidingIn;
                            if (GameManager.PlayerPower[GameManager.CurrentPlayer - 1] >= powerMove)
                            {
                                ButtonMove.GetComponent<Button>().onClick.AddListener(() => moveActor());
                                ButtonMove.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonMove.GetComponent<Button>().interactable = false;
                            }
                            if (GameManager.PlayerPower[GameManager.CurrentPlayer - 1] >= powerOSpell)
                            {
                                //ButtonOSpell.GetComponent<Button>().onClick.AddListener(() => moveActor());
                                ButtonOSpell.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonOSpell.GetComponent<Button>().interactable = false;
                            }
                            if (GameManager.PlayerPower[GameManager.CurrentPlayer - 1] >= powerDSpell)
                            {
                                //ButtonDSpell.GetComponent<Button>().onClick.AddListener(() => moveActor());
                                ButtonDSpell.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonDSpell.GetComponent<Button>().interactable = false;
                            }
                            if (GameManager.PlayerPower[GameManager.CurrentPlayer - 1] >= powerArmageddon)
                            {
                                ButtonArmageddon.GetComponent<Button>().onClick.AddListener(() => triggerArmegddeon());
                                ButtonArmageddon.GetComponent<Button>().interactable = true;
                            }
                            else
                            {
                                ButtonArmageddon.GetComponent<Button>().interactable = false;
                            }
                            if (GameManager.PlayerPower[GameManager.CurrentPlayer - 1] >= powerSummonMonster)
                            {
                                ButtonSEE.GetComponent<Button>().interactable = true;
                                ButtonSEA.GetComponent<Button>().interactable = true;
                                ButtonSEF.GetComponent<Button>().interactable = true;
                                ButtonSEW.GetComponent<Button>().interactable = true;
                                ButtonSEE.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Earth, Actor.ActorType.Demon));
                                ButtonSEA.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Air, Actor.ActorType.Demon));
                                ButtonSEF.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Fire, Actor.ActorType.Demon));
                                ButtonSEW.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Water, Actor.ActorType.Demon));
                                ButtonSEE.GetComponentInChildren<Text>().text = "Summon Earth Elemental\r\n(" + powerSummonElemental.ToString() + "p)";
                                ButtonSEA.GetComponentInChildren<Text>().text = "Summon Air Elemental\r\n(" + powerSummonElemental.ToString() + "p)";
                                ButtonSEF.GetComponentInChildren<Text>().text = "Summon Fire Elemental\r\n(" + powerSummonElemental.ToString() + "p)";
                                ButtonSEW.GetComponentInChildren<Text>().text = "Summon Water Elemental\r\n(" + powerSummonElemental.ToString() + "p)";
                            }
                            else
                            {
                                ButtonSME.GetComponent<Button>().interactable = false;
                                ButtonSMA.GetComponent<Button>().interactable = false;
                                ButtonSMF.GetComponent<Button>().interactable = false;
                                ButtonSMW.GetComponent<Button>().interactable = false;
                            }
                            if (GameManager.PlayerPower[GameManager.CurrentPlayer - 1] >= powerSummonElemental)
                            {
                                ButtonSME.GetComponent<Button>().interactable = true;
                                ButtonSMA.GetComponent<Button>().interactable = true;
                                ButtonSMF.GetComponent<Button>().interactable = true;
                                ButtonSMW.GetComponent<Button>().interactable = true;
                                ButtonSME.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Earth, Actor.ActorType.Monster));
                                ButtonSMA.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Air, Actor.ActorType.Monster));
                                ButtonSMF.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Fire, Actor.ActorType.Monster));
                                ButtonSMW.GetComponent<Button>().onClick.AddListener(() => adeptSummonAction(ElementType.Water, Actor.ActorType.Monster));
                                ButtonSME.GetComponentInChildren<Text>().text = "Summon Earth Monster\r\n(" + powerSummonMonster.ToString() + "p)";
                                ButtonSMA.GetComponentInChildren<Text>().text = "Summon Air Monster\r\n(" + powerSummonMonster.ToString() + "p)";
                                ButtonSMF.GetComponentInChildren<Text>().text = "Summon Fire Monster\r\n(" + powerSummonMonster.ToString() + "p)";
                                ButtonSMW.GetComponentInChildren<Text>().text = "Summon Water Monster\r\n(" + powerSummonMonster.ToString() + "p)";
                            }
                            else
                            {
                                ButtonSEE.GetComponent<Button>().interactable = false;
                                ButtonSEA.GetComponent<Button>().interactable = false;
                                ButtonSEF.GetComponent<Button>().interactable = false;
                                ButtonSEW.GetComponent<Button>().interactable = false;
                            }
                            if (GameManager.PlayerPower[GameManager.CurrentPlayer - 1] >= powerArmageddon)
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
                        if (selectedTile.GetComponentInChildren<Actor>().Player == GameManager.CurrentPlayer)
                        {
                            ButtonMove.GetComponentInChildren<Text>().text = "Move\r\n(" + powerMove.ToString() + "p)";
                            if (GameManager.PlayerPower[GameManager.CurrentPlayer - 1] >= powerMove)
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
                        if (selectedTile.GetComponentInChildren<Actor>().Player == GameManager.CurrentPlayer)
                        {
                            ButtonMove.GetComponentInChildren<Text>().text = "Move\r\n(" + powerMove.ToString() + "p)";
                            if (GameManager.PlayerPower[GameManager.CurrentPlayer - 1] >= powerMove)
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
                        if (selectedTile.GetComponentInChildren<Actor>().Player == GameManager.CurrentPlayer)
                        {
                            if (GameManager.placedAdepts[GameManager.CurrentPlayer - 1, 0] == false)
                            {
                                ButtonAdeptE.GetComponentInChildren<Text>().text = "Spawn Earth Adept";
                                ButtonAdeptE.GetComponent<Button>().onClick.AddListener(() => placeAdept(ElementType.Earth));
                                ButtonAdeptE.GetComponent<Button>().interactable = true;
                            }
                            if (GameManager.placedAdepts[GameManager.CurrentPlayer - 1, 1] == false)
                            {
                                ButtonAdeptA.GetComponentInChildren<Text>().text = "Spawn Air Adept";
                                ButtonAdeptA.GetComponent<Button>().onClick.AddListener(() => placeAdept(ElementType.Air));
                                ButtonAdeptA.GetComponent<Button>().interactable = true;
                            }
                            if (GameManager.placedAdepts[GameManager.CurrentPlayer - 1, 2] == false)
                            {
                                ButtonAdeptF.GetComponentInChildren<Text>().text = "Spawn Fire Adept";
                                ButtonAdeptF.GetComponent<Button>().onClick.AddListener(() => placeAdept(ElementType.Fire));
                                ButtonAdeptF.GetComponent<Button>().interactable = true;
                            }
                            if (GameManager.placedAdepts[GameManager.CurrentPlayer - 1, 3] == false)
                            {
                                ButtonAdeptW.GetComponentInChildren<Text>().text = "Spawn Water Adept";
                                ButtonAdeptW.GetComponent<Button>().onClick.AddListener(() => placeAdept(ElementType.Water));
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
                ztY = zoomY;
                ztFOV = zoomFOV;
                zsX = Camera.main.transform.position.x;
                zsY = Camera.main.transform.position.y;
                zsZ = Camera.main.transform.position.z;
                zsFOV = Camera.main.fieldOfView;
                setUpZoom();
            }
        }
        else
        {
            if (tile.GetComponentInChildren<Actor>().characterType == Actor.ActorType.None)
            {
                //If the GameManager.GameMode is CastSpell we don't care.  Players cannot cast onto empty hexes.
                if (GameManager.GameMode == GameMode.PlaceSpawn)
                {
                    //This hex is empty.  Place the new thing here.
                    Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.82f);
                    placeNewActor(tile.x, tile.z, IamSpawning.characterType, IamSpawning.Element, GameManager.CurrentPlayer);
                    GameManager.GameMode = GameMode.Default;
                    changePlayer();
                }
                else if (GameManager.GameMode == GameMode.MoveSpawn)
                {
                    //This hex is empty.  Move to here.
                    Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.82f);
                    moveActor(selectedTile.x, selectedTile.z, tile.x, tile.z);
                    GameManager.GameMode = GameMode.Default;
                    changePlayer();
                }
            }
            else if (tile.GetComponentInChildren<Actor>().Player == GameManager.CurrentPlayer)
            {
                //The player clicked on their own piece.
                if (GameManager.GameMode == GameMode.CastSpell)
                {
                    //The player wants to cast a spell on their own piece.
                }
                else
                {
                    //Let's cancel the current mode and return to default.
                    Highlighting.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.82f);
                    GameManager.GameMode = GameMode.Default;
                }
            }
            else// if (tile.GetComponentInChildren<Actor>().Player != GameManager.CurrentPlayer)
            {//The above else if appears redundant, maybe use else.
                //The player clicked on an oppoent's piece.
                if (GameManager.GameMode == GameMode.CastSpell)
                {
                    //The player wants to cast a spell on an oppoent's piece.
                }
                else if (GameManager.GameMode == GameMode.MoveSpawn)
                {
                    //The player wants to battle.
                    //TODO: Trigger battle.
                    Actor P1 = selectedTile.GetComponentInChildren<Actor>();
                    Actor P2 = tile.GetComponentInChildren<Actor>();
                    triggerBattle(P1, P2, tile);
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
        ztY = cameraY;
        ztZ = cameraZ;
        ztFOV = cameraFOV;
        zsX = Camera.main.transform.position.x;
        zsY = Camera.main.transform.position.y;
        zsZ = Camera.main.transform.position.z;
        zsFOV = Camera.main.fieldOfView;
        setUpZoom();
        if (GameManager.GameMode == GameMode.Default)
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
        yDiff = zsY - ztY;
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
        InfoTips.GetComponent<Text>().fontSize = InfoTips.GetComponent<Text>().fontSize * 2;
        if (zoom == ZoomingMode.ZoomedOut)
        {
            PanelLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(-panelWidth, 0f);
            PanelRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(panelWidth, 0f);
            PanelTop.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, panelHeight);
            PanelBottom.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -panelHeight);
        }
    }

    public void setTutorialUp()
    {
        Vector3 TutorialOffset = new Vector3(0, 0, 3f);
        TutorialImage.GetComponent<Image>().color = new Color(192f, 192f, 192f, 255f);
        TutorialText.GetComponent<Text>().color = new Color(0f, 0f, 0f, 255f);

        if (GameManager.roundNumber == 0 && GameManager.GameMode == GameMode.Default)
        {
            showingTutorial = true;
            RectTransform TutRect = TutorialPanel.GetComponent<RectTransform>();
            TutRect.position = Camera.main.WorldToScreenPoint(GameManager.TileManger.allTiles[3, 7].transform.position + TutorialOffset);
            TutorialText.GetComponent<Text>().text = "Click your castle and pick an Adept to summon.";
        }
        else if (GameManager.roundNumber == 0 && GameManager.GameMode == GameMode.PlaceSpawn)
        {
            showingTutorial = true;
            Tiles tile = GameManager.TileManger.allTiles[0, 0];
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    tile = GameManager.TileManger.allTiles[Mathf.Abs(x), z];
                    if (tile.GetComponentInChildren<Actor>().characterType == Actor.ActorType.None && tile.hasPowerWell)
                    {
                        x = xSize;
                        z = zSize;
                    }
                }
            }

            RectTransform TutRect = TutorialPanel.GetComponent<RectTransform>();
            TutRect.position = Camera.main.WorldToScreenPoint(tile.transform.position + TutorialOffset);
            TutorialText.GetComponent<Text>().text = "Place your Adept on a Power Well.";
        }
        else if (GameManager.roundNumber >= 1 && GameManager.roundNumber <= 3)
        {
            showingTutorial = true;
            RectTransform TutRect = TutorialPanel.GetComponent<RectTransform>();
            TutRect.position = Camera.main.WorldToScreenPoint(GameManager.TileManger.allTiles[3, 7].transform.position + TutorialOffset);
            TutorialText.GetComponent<Text>().text = "Place the rest of your Adept on Power Wells.";
        }
        else if (GameManager.roundNumber == 4)
        {
            showingTutorial = true;
            Tiles tile = GameManager.TileManger.allTiles[0, 0];
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    tile = GameManager.TileManger.allTiles[Mathf.Abs(x), z];
                    if (tile.GetComponentInChildren<Actor>().characterType == Actor.ActorType.Adept && tile.GetComponentInChildren<Actor>().Player == 1)
                    {
                        x = xSize;
                        z = zSize;
                    }
                }
            }

            RectTransform TutRect = TutorialPanel.GetComponent<RectTransform>();
            TutRect.position = Camera.main.WorldToScreenPoint(tile.transform.position + TutorialOffset);
            TutorialText.GetComponent<Text>().text = "Summon a Demon or Monster with your Adept.";
        }
        else if (GameManager.roundNumber == 5)
        {
            showingTutorial = true;
            Tiles tile = GameManager.TileManger.allTiles[0, 0];
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    tile = GameManager.TileManger.allTiles[Mathf.Abs(x), z];
                    if ((tile.GetComponentInChildren<Actor>().characterType == Actor.ActorType.Demon || tile.GetComponentInChildren<Actor>().characterType == Actor.ActorType.Monster) && tile.GetComponentInChildren<Actor>().Player == 1)
                    {
                        x = xSize;
                        z = zSize;
                    }
                }
            }

            RectTransform TutRect = TutorialPanel.GetComponent<RectTransform>();
            TutRect.position = Camera.main.WorldToScreenPoint(tile.transform.position + TutorialOffset);
            TutorialText.GetComponent<Text>().text = "Attack, destroy the enemy, capture all wells.";
        }
        else if (GameManager.roundNumber == 6)
        {
            showingTutorial = true;
            Tiles tile = GameManager.TileManger.allTiles[0, 0];
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    tile = GameManager.TileManger.allTiles[Mathf.Abs(x), z];
                    if (tile.GetComponentInChildren<Actor>().characterType == Actor.ActorType.Adept && tile.GetComponentInChildren<Actor>().Player == 1)
                    {
                        x = xSize;
                        z = zSize;
                    }
                }
            }

            RectTransform TutRect = TutorialPanel.GetComponent<RectTransform>();
            TutRect.position = Camera.main.WorldToScreenPoint(tile.transform.position + TutorialOffset);
            TutorialText.GetComponent<Text>().text = "Adepts can also cast spells.";
        }
        else
        {
            TutorialImage.GetComponent<Image>().color = new Color(192f, 192f, 192f, 0f);
            TutorialText.GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
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
            if (GameManager.PlayerIsAI[GameManager.CurrentPlayer - 1])
            {
                GameManager.AIHelper.MakeDecision();
            }
            else if (GameManager.TutorialEnabled)
            {
                setTutorialUp();
            }
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
            if (showingTutorial == true)
            {
                showingTutorial = false;
                TutorialImage.GetComponent<Image>().color = new Color(192f, 192f, 192f, 0f);
                TutorialText.GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
            }
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
                Camera.main.transform.position = new Vector3(zsX - (xDiff * bezier), zsY - (yDiff * bezier), zsZ - (zDiff * bezier));
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
                }
            }
            else
            {
                if (zoom == ZoomingMode.ZoomingOut)
                {
                    zoom = ZoomingMode.ZoomedOut;
                    isZoomed = false;
                    if (GameManager.roundNumber == 0 && GameManager.GameMode == GameMode.PlaceSpawn && GameManager.TutorialEnabled)
                    {
                        setTutorialUp();
                    }
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
        GameManager.CurrentPlayer++;
        if (GameManager.CurrentPlayer == 3)
        {
            GameManager.roundNumber++;
            GameManager.CurrentPlayer = 1;
        }

        Debug.Log("Placed Adepts for Player " + GameManager.CurrentPlayer.ToString() + ": "
            + GameManager.placedAdepts[GameManager.CurrentPlayer - 1, 0].ToString()
            + GameManager.placedAdepts[GameManager.CurrentPlayer - 1, 1].ToString()
            + GameManager.placedAdepts[GameManager.CurrentPlayer - 1, 2].ToString()
            + GameManager.placedAdepts[GameManager.CurrentPlayer - 1, 3].ToString()
            );

        int CurrentPlayerNumberOfAdepts = 0;
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                if (GameManager.TileManger.allTiles[x, z].transform.Find("Adept") != null)
                {
                    GameObject adept = GameManager.TileManger.allTiles[x, z].transform.Find("Adept").gameObject;
                    if (adept.GetComponent<Actor>().Player == GameManager.CurrentPlayer)
                    {
                        CurrentPlayerNumberOfAdepts++;
                        if (GameManager.TileManger.allTiles[x, z].hasPowerWell)
                        {
                            GameManager.PlayerPower[GameManager.CurrentPlayer - 1] += 10 - pendingPowerExpendature;
                        }
                    }
                }
            }
        }

        Debug.Log("Player " + GameManager.CurrentPlayer + " has " + CurrentPlayerNumberOfAdepts + " Adepts on the field.");

        if (CurrentPlayerNumberOfAdepts == 0)
        {//The current player has no adepts on the field.  Check to see that they have adepts to place.
            for (int a = 0; a < 4; a++)
            {
                if (GameManager.placedAdepts[GameManager.CurrentPlayer - 1, a] == false)
                {//There is an unplaced adept.
                    CurrentPlayerNumberOfAdepts++;
                }
            }
            Debug.Log("Player " + GameManager.CurrentPlayer + " has " + CurrentPlayerNumberOfAdepts + " Adepts total.");
            if (CurrentPlayerNumberOfAdepts == 0)
            {
                GameManager.TriggerEndGame((GameManager.CurrentPlayer == 0) ? 2 : 1);
            }
        }

        InfoP1Power.GetComponent<Text>().text = "Player 1 Power:\r\n" + GameManager.PlayerPower[0];
        InfoP2Power.GetComponent<Text>().text = "Player 2 Power:\r\n" + GameManager.PlayerPower[1];
        showingTip = true;
        hideTipWhen = Time.time + zoomMaxSteps;
        //InfoTips.GetComponent<Text>().text = "Player " + GameManager.CurrentPlayer + "'s Turn!\r\nCurrent Power: " + GameManager.PlayerPower[GameManager.CurrentPlayer - 1].ToString();
        //Since this is a single player game now, let's change the verbage.
        if (GameManager.CurrentPlayer == 1)
        {
            InfoTips.GetComponent<Text>().text = "Your Turn!\r\nCurrent Power: " + GameManager.PlayerPower[0];
        }
        else
        {
            InfoTips.GetComponent<Text>().text = "Computer's Turn!\r\nCurrent Power: " + GameManager.PlayerPower[1];
        }
    }

    public void triggerArmegddeon()
    {
        GameManager.GameMode = GameMode.Armageddon;
        Actor P1Castle = GameManager.TileManger.allTiles[3, 7].Actor;
        Actor P2Castle = GameManager.TileManger.allTiles[19, 7].Actor;
        Tiles Stage = new Tiles();
        Stage.Element = ElementType.Void;
        triggerBattle(P1Castle, P2Castle, Stage);
    }

}
