using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerMode 
    {
        Cutscene,
        Dual,
        Game,
        Dead
    }

    public float impulseForce  = 170000.0f;
    public float impulseTorque = 3000.0f;
    public float vert_rotation_speed = 500;
    public Vector3 gun_ray_offset;
    public float KickForce = 10;


    public GameObject hero;
    public Camera player_camera;
    public Texture crosshair;

    public GameObject bullet_prefab;
    public AudioClip gun_shot;

    Animator animController;
    Rigidbody rigidBody;
    AudioSource audio;

    public PlayerMode current_mode;

    public float rotY = 0;
    public bool cursor_is_locked = true;

    private Vector3 last_pos;
    private Vector3 cur_pos;
    public Vector3 velocity;

    private float shoot_timer = 0;

    private string[] kicks = {
        "front_kick 0",
        "defensive_side_kick",
        "back_kick"
    };

    // Start is called before the first frame update
    void Start()
    {
        // get references to the animation controller of hero
        // character and player's corresponding rigid body
        animController = hero.GetComponent<Animator> ();
        rigidBody      = GetComponent<Rigidbody>();
        audio = hero.GetComponent<AudioSource>();
        current_mode = PlayerMode.Cutscene;
        last_pos = transform.position;

        animController.SetBool("Walk", true);
        animController.speed = 0.5f;

        LockCursur(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (current_mode == PlayerMode.Game)
        {
            process_look();
            process_movement();
            process_attack();
        }
        else if (current_mode == PlayerMode.Dual)
        {
            process_look();
            process_attack();
        }
        else if (current_mode == PlayerMode.Cutscene)
        {
            
        }

        if (Input.GetKey(KeyCode.Escape) && cursor_is_locked)
        {
            LockCursur(false);
        }
        if (Input.GetMouseButtonDown(0) && !cursor_is_locked)
        {
            LockCursur(true);
        }

        cur_pos = transform.position;
        //velocity = (cur_pos - last_pos) / Time.deltaTime;
        velocity = rigidBody.velocity;
        last_pos = cur_pos;
    }

    void process_look()
    {
        if (!cursor_is_locked)
            return;

        Vector3 input = new Vector3(
            0, 
            Mathf.Clamp(Input.GetAxis("Mouse X") + Input.GetAxis("Horizontal"), -1, 1), 
            -Input.GetAxis("Mouse Y")
        );

        if (input.magnitude > 0.001 && !animController.GetBool("Crouch"))
        {
            rigidBody.AddRelativeTorque(new Vector3(0, input.y * impulseTorque * Time.deltaTime, 0));
            

            rotY += input.z * vert_rotation_speed * Time.deltaTime;
            rotY = Mathf.Clamp(rotY, -50f, 50f);
            player_camera.transform.localRotation = Quaternion.Euler(rotY, 0, 0);

            animController.SetBool("Walk", current_mode == PlayerMode.Game);
            animController.speed = 1.0f;

            
        }

    }

    void process_movement()
    {
        if (!cursor_is_locked)
            return;

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
            animController.speed = 1.0f;
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
    void process_attack()
    {
        if (!cursor_is_locked)
            return;

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
        shoot_timer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.F) && shoot_timer < 0)
        {
            Debug.Log("Player shot a bullet!");
            audio.PlayOneShot(gun_shot);
            /*if (Physics.Raycast(gun_ray, out hit))
            {
                Debug.LogFormat("Player shot: {0}", hit.collider.gameObject.name);
                //hit.collider.gameObject.SendMessageUpwards("Shot", SendMessageOptions.DontRequireReceiver);
            }*/
            GameObject bullet_inst = Instantiate(bullet_prefab, gun_ray.origin, Quaternion.identity);
            bullet_inst.transform.forward = gun_ray.direction;
            bullet_inst.transform.Translate(0, 0, 1);
            GameModeController.Instance.StartBanditFight();
            shoot_timer = 1.0f;
        }

        if(Input.GetKeyDown(KeyCode.Space) && shoot_timer < 0)
        {
            Debug.Log("Kicking...");
            //animController.SetBool("Kick", true);
            animController.Play(kicks[Random.Range(0, 100) % 3]);
            Invoke("ResetKick", 0.9f);
            GameModeController.Instance.StartBanditFight();
            shoot_timer = 1.0f;

            Ray kick_ray = new Ray(hero.transform.position + new Vector3(0, 0.3f, 0), hero.transform.forward);
            Debug.DrawRay(kick_ray.origin, kick_ray.direction, Color.green, 10000);
            if (Physics.Raycast(kick_ray, out hit, 1.0f))
            {
                Debug.LogFormat("Kicked {0}", hit.collider.gameObject.name);
                if (hit.rigidbody != null)
                {
                    Debug.LogFormat("Kicked with force {0}", hit.collider.gameObject.name);
                    hit.rigidbody.AddExplosionForce(KickForce, hit.point, 1.0f);
                }
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

    public void Shot()
    {
        Debug.Log("Player was shot!");
        if (current_mode == PlayerMode.Dual)
        {
            // Dead
            current_mode = PlayerMode.Dead;
            GameModeController.Instance.PlayerDied();
            animController.SetBool("Dead", true);
        }
        else if (current_mode == PlayerMode.Game)
        {
            // Reduce lives, or maybe just dead.
        }
    }

    public void SetMode(PlayerMode mode)
    {
        current_mode = mode;
    }

    public void PlayerFinishWalk()
    {
        animController.SetBool("Walk", false);
        animController.speed = 1.0f;
    }

    void LockCursur(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        cursor_is_locked = locked;
    }

    void ResetKick()
    {
        Debug.Log("Resetting kick");
        //animController.SetBool("Kick", false);
        animController.Play("Idle");
    }
}
