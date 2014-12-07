using UnityEngine;
using System.Collections;

public class TidyRoar : MonoBehaviour {

    //public GameObject explodePrefab;
    private bool _destroy;

    void Update()
    {
        if (_destroy)
        {
            Explode();
        }
        else
        {
            Invoke("Explode", 2);
        }
    }

    private void Explode()
    {
        Destroy(gameObject);
        //GameObject explode = Instantiate(explodePrefab, transform.position, transform.rotation) as GameObject;
        //Destroy(explode, 3);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.name.Contains("Ground"))
        {
            return;
        }
        _destroy = true;
    }
}
