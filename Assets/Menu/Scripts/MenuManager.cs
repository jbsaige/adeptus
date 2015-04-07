using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Canvas Canvas;
    public GameObject[] TextDisplays;
    public GameObject[] TextDisplaysLarge;
    public GameObject ButtonRandomMap, ButtonPatternMap, TutorialEnabled, P1Human, P2Human;

    private GameManager GameManager;
    private float canvasWidth, canvasHeight;
    private int fontSize;

    public void Update()
    {
        if (canvasWidth != Canvas.transform.position.x * 2 || canvasHeight != Canvas.transform.position.y * 2)
        {
            recalculateCanvas();
        }
    }

    public void Start()
    {
        ButtonPatternMap.GetComponent<Button>().onClick.AddListener(() => loadPatternMap());
        ButtonRandomMap.GetComponent<Button>().onClick.AddListener(() => loadRandomMap());
    }

    private void recalculateCanvas()
    {
        canvasWidth = Canvas.transform.position.x * 2;
        canvasHeight = Canvas.transform.position.y * 2;
        fontSize = (int)(canvasWidth * 0.05f);
        for (int i = 0; i < TextDisplays.Length; i++)
        {
            TextDisplays[i].GetComponent<Text>().fontSize = fontSize;
        }
        for (int i = 0; i < TextDisplaysLarge.Length; i++)
        {
            TextDisplaysLarge[i].GetComponent<Text>().fontSize = (int)(fontSize * 1.5f);
        }
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.GameManager = gameManager;
    }

    public void loadPatternMap()
    {
        SetSettings();
        GameManager.loadPatternMap();
    }

    public void loadRandomMap()
    {
        SetSettings();
        GameManager.loadRandomMap();
    }

    public void SetSettings()
    {
        Debug.Log("Setting TutorialEnabled to " + TutorialEnabled.GetComponent<Toggle>().isOn);
        GameManager.TutorialEnabled = TutorialEnabled.GetComponent<Toggle>().isOn;
        Debug.Log("Setting PlayerIsAI[0] to " + P1Human.GetComponent<Toggle>().isOn);
        GameManager.PlayerIsAI[0] = P1Human.GetComponent<Toggle>().isOn;
        Debug.Log("Setting PlayerIsAI[1] to " + P2Human.GetComponent<Toggle>().isOn);
        GameManager.PlayerIsAI[1] = P2Human.GetComponent<Toggle>().isOn;
    }


}
