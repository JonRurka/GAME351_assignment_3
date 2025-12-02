using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TX_Red : MonoBehaviour
{
    public bool dual_started = false;
    public bool was_hit = false;

    public AudioClip gun_Shot;
    public GameObject bullet_prefab;

    public float shoot_duration = 2.5f;
    private float shoot_timer = 0;
    private bool did_shoot = false;

    private AudioSource audio;
    Animator animController;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        animController = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dual_started)
        {
            if (!was_hit)
            {
                shoot_timer -= Time.deltaTime;
                if (shoot_timer <= 0)
                {
                    do_shoot();
                }
            }
        }
    }

    public void TriggerStart()
    {
        dual_started = true;
        shoot_timer = shoot_duration + Random.Range(-0.25f, 0.25f);
        Debug.Log("Texas Red start.");
    }

    public void do_shoot()
    {
        if (did_shoot)
        {
            return;
        }
        Debug.Log("Texas Red shot a bullet!");
        audio.PlayOneShot(gun_Shot);
        did_shoot = true;
        RaycastHit hit;
        Ray ray = new Ray(transform.position + new Vector3(0, 1, 0), Vector3.forward);
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.cyan, 10000);
        GameObject bullet_inst = Instantiate(bullet_prefab, ray.origin, Quaternion.identity);
        bullet_inst.transform.forward = ray.direction;
        bullet_inst.transform.Translate(0, 0, 1);
        /*if (Physics.Raycast(ray, out hit))
        {
            hit.collider.gameObject.SendMessageUpwards("Shot", SendMessageOptions.DontRequireReceiver);

        }*/
    }

    public void Shot()
    {
        Debug.Log("Texas Red has been shot!");
        was_hit = true;
        animController.SetBool("Dead", true);
        GameModeController.Instance.BeginBanditFight();
    }
}
