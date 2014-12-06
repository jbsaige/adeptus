using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Canvas Canvas;
    public GameObject[] TextDisplays;
    public GameObject[] TextDisplaysLarge;
    public GameObject ButtonRandomMap, ButtonPatternMap;

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

    public void loadPatternMap()
    {

    }

    public void loadRandomMap()
    {

    }



}
