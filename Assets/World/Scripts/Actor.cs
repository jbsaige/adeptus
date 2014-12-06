using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour
{
    public int Player, x, z;
    public GameManager.ElementType Element;
    public ActorType characterType;
    public enum ActorType { None, Adept, Demon, Monster, Castle };
    public GameObject Avatar;
    public GameManager Manager;

    public void Move(int newX, int newZ)
    {
        x = newX;
        z = newZ;

    }

    public void SetUp(ActorType type, int x, int z, int element, int player, GameObject avatar, GameManager manager)
    {
        this.Player = player;
        this.x = x;
        this.z = z;
        this.Element = (GameManager.ElementType)element;
        this.characterType = type;
        this.Avatar = avatar;
        this.Manager = manager;
    }


}
