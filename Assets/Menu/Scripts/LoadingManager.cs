using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{

    public Canvas Canvas;
    public GameObject[] TextDisplays;
    public GameObject[] TextDisplaysLarge;
    public GameManager GameManager;

    private float canvasWidth, canvasHeight;
    private int fontSize;

    void Start()
    {
        recalculateCanvas();
        GameManager = FindObjectOfType<GameManager>();
        GameManager.SetLoadingManager(this);
    }

    public void LoadNext(string NextLevel)
    {
        Application.LoadLevel(NextLevel);
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

}
