using UnityEngine;
using System.Collections;

public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerMover : MonoBehaviour 
{
    public float speed;
    public float rotationSpeed = 450;
    public float turnSmoothing = 15f;
    public float speedDampTime = 0.1f;
    public Boundary boundary;

    private Quaternion targetRotation;

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        GetComponent<Rigidbody>().velocity = movement * speed;

        if (moveHorizontal != 0f || moveVertical != 0f)
        {
            Rotating(moveHorizontal, moveVertical);
            GetComponent<Rigidbody>().position = new Vector3
            (
                Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
                0.0f,
                Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
            );
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    void Rotating(float horizontal, float vertical)
    {
        Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(GetComponent<Rigidbody>().rotation, targetRotation, turnSmoothing * Time.deltaTime);
        GetComponent<Rigidbody>().MoveRotation(newRotation);
    }
	
}
