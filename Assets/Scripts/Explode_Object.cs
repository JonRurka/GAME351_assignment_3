using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode_Object : MonoBehaviour
{
    public GameObject explosion_prefab;
    public GameObject debry;
    public AudioClip explode;

    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shot()
    {
        Debug.Log("BOOM!!!!");
        audio.PlayOneShot(explode);
        Instantiate(explosion_prefab, transform.position, Quaternion.identity);
        Instantiate(debry, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
