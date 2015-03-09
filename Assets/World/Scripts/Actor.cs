using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour
{
    public int Player, x, z, HP;
    public WorldManager.ElementType Element;
    public ActorType characterType;
    public enum ActorType { None, Adept, Demon, Monster, Castle };
    public GameObject Avatar;
    public WorldManager Manager;

    public void Move(int newX, int newZ)
    {
        x = newX;
        z = newZ;
    }

    public void SetUp(ActorType type, int x, int z, int element, int player, GameObject avatar, WorldManager manager)
    {
        this.Player = player;
        this.x = x;
        this.z = z;
        this.Element = (WorldManager.ElementType)element;
        this.characterType = type;
        this.Avatar = avatar;
        this.Manager = manager;
    }


}
