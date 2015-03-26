using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndGameManager : MonoBehaviour
{

    public Canvas Canvas;
    public GameObject[] TextDisplays;
    public GameObject[] TextDisplaysLarge;
    public GameObject ButtonMenu, ButtonQuit, TextStatus;
    public GameManager GameManager;

    private float canvasWidth, canvasHeight;
    private int fontSize;

    void Start()
    {
        recalculateCanvas();
        GameManager = FindObjectOfType<GameManager>();
        GameManager.SetEndGameManager(this);
        ButtonMenu.GetComponent<Button>().onClick.AddListener(() => menuButtonClick());
        ButtonQuit.GetComponent<Button>().onClick.AddListener(() => quitButtonClick());
    }

    public void SetWinner(int Winner)
    {
        TextStatus.GetComponent<Text>().text = "Player " + Winner.ToString() + " wins!";
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

    public void menuButtonClick()
    {
        Application.LoadLevel("Menu");
    }

    public void quitButtonClick()
    {
        Application.Quit();
    }

}
