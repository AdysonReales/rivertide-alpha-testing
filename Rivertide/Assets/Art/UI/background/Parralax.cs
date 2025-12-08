using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parralax : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect; 

    void Start()
    {
        startPos = transform.position.x;
    }

    void FixedUpdate()
    {
        // Calculate distance background move based on cam movement
        float distance = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
