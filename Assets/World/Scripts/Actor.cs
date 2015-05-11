using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour
{
    public int Player = 0, x = 0, z = 0, HP = 0;
    public WorldManager.ElementType Element = WorldManager.ElementType.Void;
    public ActorType characterType = ActorType.None;
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
