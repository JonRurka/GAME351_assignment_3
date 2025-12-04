using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 200;

    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = transform.TransformVector(new Vector3(0, 0, speed));
    }

    // Update is called once per frame
    void Update()
    {
        //body.transform.Translate(0, 0, speed *  Time.deltaTime);
        Debug.DrawRay(body.transform.position, body.transform.forward, Color.black, 100000);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogFormat("Hit: {0}", other.gameObject.name);
        other.gameObject.SendMessageUpwards("Shot", SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
