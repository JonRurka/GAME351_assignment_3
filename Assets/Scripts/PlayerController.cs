using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerMode 
    {
        Cutscene,
        Dual,
        Game
    }

    public float impulseForce  = 170000.0f;
    public float impulseTorque = 3000.0f;
    public float vert_rotation_speed = 500;
    public Vector3 gun_ray_offset;


    public GameObject hero;
    public Camera player_camera;
    public Texture crosshair;

    Animator animController;
    Rigidbody rigidBody;

    PlayerMode current_mode;

    public float rotY = 0;

    // Start is called before the first frame update
    void Start()
    {
        // get references to the animation controller of hero
        // character and player's corresponding rigid body
        animController = hero.GetComponent<Animator> ();
        rigidBody      = GetComponent<Rigidbody>();
        current_mode = PlayerMode.Cutscene;
    }

    // Update is called once per frame
    void Update()
    {
        if (current_mode == PlayerMode.Game)
        {
            process_look();
            process_movement();
            process_shoot();
        }
        else if (current_mode == PlayerMode.Dual)
        {
            process_look();
            process_shoot();
        }
        else
        {
            //process_shoot();
        }
    }

    void process_look()
    {
        Vector3 input = new Vector3(
            0, 
            Mathf.Clamp(Input.GetAxis("Mouse X") + Input.GetAxis("Horizontal"), -1, 1), 
            -Input.GetAxis("Mouse Y")
        );

        if (input.magnitude > 0.001 && !animController.GetBool("Crouch"))
        {
            rigidBody.AddRelativeTorque(new Vector3(0, input.y * impulseTorque * Time.deltaTime, 0));
            

            rotY += input.z * vert_rotation_speed * Time.deltaTime;
            rotY = Mathf.Clamp(rotY, -80f, 80f);
            player_camera.transform.localRotation = Quaternion.Euler(rotY, 0, 0);

            animController.SetBool("Walk", current_mode == PlayerMode.Game);
        }

    }

    void process_movement()
    {
        // W/A/S/D input as a combined rotation and movement vector
        Vector3 input = new Vector3(0, 0, Input.GetAxis("Vertical"));

        // allow movement when input detected and not crouching
        if (input.magnitude > 0.001 && !animController.GetBool("Crouch"))
        {
            // rotations are about y axis
            //rigidBody.AddRelativeTorque(new Vector3(0, input.y * impulseTorque * Time.deltaTime, 0));
            // motion is forward/backward (about z axis)
            rigidBody.AddRelativeForce(new Vector3(0, 0, input.z * impulseForce * Time.deltaTime));

            animController.SetBool("Walk", true);
        }
        else
        {
            animController.SetBool("Walk", false);

            // crouching with 'C' key (only when not moving)
            if (Input.GetKey(KeyCode.C))
                animController.SetBool("Crouch", true);
            else
                animController.SetBool("Crouch", false);
        }
    }

    public bool did_hit = false;
    void process_shoot()
    {
        did_hit = false;
        Vector3 gun_ray_start = hero.transform.position + gun_ray_offset;
        Vector3 hit_point = gun_ray_start + hero.transform.forward * 20;
        
        RaycastHit hit;

        Ray cam_ray = new Ray(player_camera.transform.position, player_camera.transform.forward);
        Debug.DrawRay(cam_ray.origin, cam_ray.direction * 20, Color.green);
        if (Physics.Raycast(cam_ray, out hit))
        {
            hit_point = hit.point;
            did_hit = true;
        }
        Debug.DrawLine(gun_ray_start, hit_point, Color.blue);

        Vector3 gun_dir = (hit_point - gun_ray_start).normalized;
        Ray gun_ray = new Ray(gun_ray_start, gun_dir);

        Debug.DrawRay(gun_ray_start, gun_dir * 20, Color.red);
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (Physics.Raycast(gun_ray, out hit))
            {
                hit.collider.SendMessage("shot");
            }
        }
    }

    private void OnGUI()
    {
        if (current_mode == PlayerMode.Dual ||
            current_mode == PlayerMode.Game)
        {
            GUI.DrawTexture(new Rect(Screen.width / 2 - 25, Screen.height / 2 - 25, 50, 50), crosshair);
        }
    }

    public void SetMode(PlayerMode mode)
    {
        current_mode = mode;
    }
}
