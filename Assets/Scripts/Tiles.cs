using UnityEngine;
using System.Collections;

public class Tiles : MonoBehaviour
{
    public GameManager manager;
    public int Element, x, z;
    public string ElementName;
    public bool hasPowerWell = false;
    private bool iAmHighLighted;

    // Use this for initialization
    void Start()
    {
    }

    public void SetElement(int element)
    {
        this.Element = element;
        this.GetComponent<MeshRenderer>().material = manager.mats[element];
        this.ElementName = manager.elements[element];
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

        if (collider.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            manager.Highlighting.transform.position = new Vector3(transform.position.x, -0.2f, transform.position.z);
            iAmHighLighted = true;
        }
        else
        {
            iAmHighLighted = false;
        }

    }

    // OnMouseUp is called when the user has released the mouse button (Since v1.0)
    void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(0) && iAmHighLighted)
        {
            manager.zoomOnToTile(this);
        }

    }


}
