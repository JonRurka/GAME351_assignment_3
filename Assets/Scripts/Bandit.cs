using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : MonoBehaviour
{
    public bool is_dead;

    public AudioClip[] taunts;
    public AudioClip dead;

    public AudioClip gun_Shot;
    public GameObject bullet_prefab;

    public float min_spread = 10f;
    public float max_spread = 30f;

    Animator animController;
    AudioSource audio;
    public float taunt_timer;
    public float shoot_timer;

    // Start is called before the first frame update
    void Start()
    {
        animController = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        taunt_timer = Random.Range(1.0f, 45.0f);
        shoot_timer = Random.Range(3.0f, 10.0f);
        GameModeController.Instance.RegisterBandit();
    }

    // Update is called once per frame
    void Update()
    {
        if (is_dead)
        {
            return;
        }
        if (!(GameModeController.Instance.current_state == GameModeController.GameState.FightingBandits || 
              GameModeController.Instance.current_state == GameModeController.GameState.Relaxing))
        {
            return;
        }

        taunt_timer -= Time.deltaTime;
        if (taunt_timer < 0)
        {
            AudioClip clip = taunts[Random.Range(0, taunts.Length - 1)];
            audio.PlayOneShot(clip);
            taunt_timer = Random.Range(5.0f, 60.0f);
            Debug.LogFormat("{0} used taunt {1}", name, clip.name);
        }

        Vector3 playerLoc = GameModeController.Instance.player.transform.position;

        Vector3 look_loc = new Vector3(playerLoc.x, transform.position.y, playerLoc.z);
        transform.LookAt(look_loc, Vector3.up);

        
        shoot_timer -= Time.deltaTime;
        if (shoot_timer < 0)
        {

            float angle = Random.Range(0, 2 * Mathf.PI);
            float dist = Mathf.Sin(Mathf.Deg2Rad * Random.Range(min_spread, max_spread));

            float x = dist * Mathf.Cos(angle);
            float y = dist * Mathf.Sin(angle);

            Vector3 rand_dir_local = new Vector3(x, y, 1);
            Vector3 rand_dir = transform.TransformDirection(rand_dir_local);

            Ray ray = new Ray(transform.position + new Vector3(0, 1.2f, 0), rand_dir);

            Debug.DrawRay(ray.origin, ray.direction * 20, Color.cyan, 10000);
            audio.PlayOneShot(gun_Shot);
            GameObject bullet_inst = Instantiate(bullet_prefab, ray.origin, Quaternion.identity);
            bullet_inst.transform.forward = ray.direction;
            bullet_inst.transform.Translate(0, 0, 1);

            shoot_timer = Random.Range(3.0f, 10.0f);
        }

    }

    public void Shot()
    {
        Debug.LogFormat("{0} was shot!", gameObject.name);
        audio.PlayOneShot(dead);
        animController.SetBool("Dead", true);
        GetComponent<CapsuleCollider>().enabled = false;
        is_dead = true;
        GameModeController.Instance.killedBandit();
    }
}
