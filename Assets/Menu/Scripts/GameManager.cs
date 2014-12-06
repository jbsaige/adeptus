using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private WorldManager worldManager;

    public void Start()
    {
        DontDestroyOnLoad(this);
    }



}
