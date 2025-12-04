using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRandomDir : MonoBehaviour
{
    public float spread_angle = 30;

    public float rand_angle = 0;
    public float random_dist = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //random_dist = Random.value;
        //rand_angle = Random.Range(0, 360f);


        float dist = random_dist * Mathf.Sin(Mathf.Deg2Rad * spread_angle);

        float x = dist * Mathf.Cos(Mathf.Deg2Rad * rand_angle);
        float y = dist * Mathf.Sin(Mathf.Deg2Rad * rand_angle);

        Vector3 rand_dir_local = new Vector3(x, y, 1);
        Vector3 rand_dir = transform.TransformDirection(rand_dir_local);

        Debug.DrawRay(transform.position, rand_dir_local, Color.blue);
        //Debug.DrawRay(transform.position, rand_dir);
    }
}
