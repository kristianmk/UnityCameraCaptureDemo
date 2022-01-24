// Written by K. M. Knausgård 2022-01-24

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereWalker : MonoBehaviour
{
    
    private int ii = 0;

    private CameraCapture cameraCapture;


    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCameraGameObject = GameObject.Find("Main Camera");

        if (mainCameraGameObject is null)
        {
            Debug.Log("Could not fined Main Camera in scene. Stopping.");
            return;
        }

        cameraCapture = mainCameraGameObject.GetComponent<CameraCapture>();

        // Could set position programmatically using:
        // transform.position = new Vector3(0, 0, 0);
    }


    // Update is called once per frame
    void Update()
    {
        if (cameraCapture != null && ii++ < cameraCapture.numberOfIterations)
        {
            transform.position += new Vector3(1, 0, 0); // Time.deltaTime
        }
    }

}