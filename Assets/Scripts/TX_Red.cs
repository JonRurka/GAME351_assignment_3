using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TX_Red : MonoBehaviour
{
    public bool dual_started = false;
    public bool was_hit = false;

    public AudioClip gun_Shot;

    public float shoot_duration = 1.5f;
    private float shoot_timer = 0;
    private bool did_shoot = false;

    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
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
    }

    public void do_shoot()
    {
        Debug.Log("Texas Red shot a bullet!");
        audio.PlayOneShot(gun_Shot);
        did_shoot = true;
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.cyan, 10000);
        if (Physics.Raycast(ray, out hit))
        {
            hit.collider.gameObject.BroadcastMessage("Shot");

        }
    }

    public void Shot()
    {
        Debug.Log("Texas Red has been shot!");
        was_hit = true;
    }
}
