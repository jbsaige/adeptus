using UnityEngine;
using System.Collections;

public class Tiles : MonoBehaviour
{
    public WorldManager Manager;
    public int x, z;
    public WorldManager.ElementType Element;
    public bool hasPowerWell = false;
    public Actor Actor;
    private bool iAmHighLighted;
    WorldManager.ElementType otherElement;

    public void SetElement(int element)
    {
        this.Element = (WorldManager.ElementType)element + 1;
        this.GetComponent<MeshRenderer>().material = Manager.mats[element];
        //this.GetComponent<MeshRenderer>().material.Lerp(Manager.mats[element], Manager.transMat, 0.01f);
        //this.GetComponent<MeshRenderer>().material.shader = Manager.transShader;
        if (Element == WorldManager.ElementType.Air)
        {
            otherElement = WorldManager.ElementType.Fire;
        }
        else if (Element == WorldManager.ElementType.Fire)
        {
            otherElement = WorldManager.ElementType.Water;
        }
        else if (Element == WorldManager.ElementType.Water)
        {
            otherElement = WorldManager.ElementType.Earth;
        }
        else if (Element == WorldManager.ElementType.Earth)
        {
            otherElement = WorldManager.ElementType.Air;
        }
        else
        {
            otherElement = WorldManager.ElementType.Void;
        }
    }

    public void SetXandZ(int x, int z)
    {
        this.x = x;
        this.z = z;
        this.name = "Tile[" + x.ToString() + "," + z.ToString() + "]";
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (this.GetComponent<Collider>().Raycast(ray, out hitInfo, Mathf.Infinity) && this.Manager.zoom == WorldManager.ZoomingMode.ZoomedOut)
        {
            this.Manager.Highlighting.transform.position = new Vector3(this.transform.position.x, -0.2f, this.transform.position.z);
            this.Manager.HighlightedTile = this;
        }
    }

}
