using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    // Handling Variables
    public float rotationSpeed = 450;
    public float walkSpeed = 15;
    public float runSpeed = 8;
    public float turnSmoothing = 15f;
    public float speedDampTime = 0.1f;

    private float acceleration = 5;

    //Components
    public Transform handHold;
    private WeaponOne weaponOne;
    public WeaponOne[] weapons;
    public WeaponTwo weaponTwo;
    private CharacterController controller;
    private Camera cam;
    private Animator animator;

    // System Variables
    private Quaternion targetRotation;
    private Vector3 currentVelocityMod;

    // Other private variables not in use yet
    //private Animator anim;
    //private HashIDs hash;

    //void Awake()
    //{
    //    anim = GetComponent<Animator>();
    //    hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
    //    anim.SetLayerWeight(1, 1f);
    //}

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = Camera.main;
        // can put an switch statement here where it equips the weapon corresponding to the type of creature
        //  alternate is to make a script for each creature for it's weapons/defenses
        EquipWeapon(0);
    }


    //void FixedUpdate()
    //{
    //    float h = Input.GetAxis("Horizontal");
    //    float v = Input.GetAxis("Vertical");
    //    bool run = Input.GetButton("Run");

    //    MovementManagement(h, v, run);
    //}



    void Update()
    {
        ControlMouse();
        ControlWASD();

        //Weapon INPUT
        //added extra if layer for if a weapon isn't equipped, but this functionality not useful in this game because we won't be equpping various guns
        // most likley not
        if (weaponOne)
        {
            if (Input.GetButtonDown("Shoot"))
            {
                // calls old shoot method
                //weaponOne.Shoot();
                
                // new shoot method
                weaponOne.RangedAttack();
            }
            else if (Input.GetButton("Shoot"))
            {
                weaponOne.ShootContinuous();
            }
        }
        if (weaponTwo)
        {
            if (Input.GetButtonDown("Punch"))
            {
                weaponTwo.MeleeAttack();
            }
        }


        for (int i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown((i + 1) + "") || Input.GetKeyDown("[" + (i+1) + "]"))
            {
                EquipWeapon(i);
                break;
            }
        }
    }

    void EquipWeapon(int i)
    {
        if (weaponOne)
        {
            Destroy(weaponOne.gameObject);

        }

        weaponOne = Instantiate(weapons[i], handHold.position, handHold.rotation) as WeaponOne;
        weaponOne.transform.parent = handHold;
        animator.SetFloat("Weapon ID", weaponOne.weaponID);
    }

    void ControlMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.transform.position.y - transform.position.y));
        targetRotation = Quaternion.LookRotation(mousePos - new Vector3(transform.position.x,0,transform.position.z));
        transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, rotationSpeed * Time.deltaTime);

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        currentVelocityMod = Vector3.MoveTowards(currentVelocityMod, input, acceleration * Time.deltaTime);
        Vector3 motion = currentVelocityMod;
        motion *= (Mathf.Abs(input.x) == 1 && Mathf.Abs(input.z) == 1) ? .7f : 1;
        motion *= (Input.GetButton("Run")) ? runSpeed : walkSpeed;
        motion += Vector3.up * -8;

        controller.Move(motion * Time.deltaTime);

        animator.SetFloat("Speed",Mathf.Sqrt(motion.x * motion.x + motion.z * motion.z));
    }

    void ControlWASD()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (input != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(input);
            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, rotationSpeed * Time.deltaTime);
        }

        currentVelocityMod = Vector3.MoveTowards(currentVelocityMod, input, acceleration * Time.deltaTime);
        Vector3 motion = currentVelocityMod;
        motion *= (Mathf.Abs(input.x) == 1 && Mathf.Abs(input.z) == 1) ? .7f : 1;
        motion *= (Input.GetButton("Run")) ? runSpeed : walkSpeed;
        motion += Vector3.up * -800;

        controller.Move(motion * Time.deltaTime);
    }



    //void Rotating(float horizontal, float vertical)
    //{
    //    Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
    //    Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
    //    Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
    //    rigidbody.MoveRotation(newRotation);
    //}
}