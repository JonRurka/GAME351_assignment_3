using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : MonoBehaviour
{
    public bool is_dead;

    public AudioClip[] taunts;

    Animator animController;
    AudioSource audio;
    public float taunt_timer;

    // Start is called before the first frame update
    void Start()
    {
        animController = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        taunt_timer = Random.Range(1.0f, 30.0f);
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

    }

    public void Shot()
    {
        Debug.LogFormat("{0} was shot!", gameObject.name);
        animController.SetBool("Dead", true);
        is_dead = true;
    }
}
