using UnityEngine;
using System.Collections;

public class Tiles : MonoBehaviour
{
    public GameManager Manager;
    public int x, z;
    public GameManager.ElementType Element;
    public bool hasPowerWell = false;
    private bool iAmHighLighted;

    public void SetElement(int element)
    {
        this.Element = (GameManager.ElementType)element+1;
        this.GetComponent<MeshRenderer>().material = Manager.mats[element];
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

        if (this.collider.Raycast(ray, out hitInfo, Mathf.Infinity) && this.Manager.zoom == GameManager.ZoomingMode.ZoomedOut)
        {
            this.Manager.Highlighting.transform.position = new Vector3(this.transform.position.x, -0.2f, this.transform.position.z);
            this.Manager.HighlightedTile = this;
        }
    }

}
