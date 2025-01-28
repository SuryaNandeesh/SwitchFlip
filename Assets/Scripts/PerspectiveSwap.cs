using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveSwap : MonoBehaviour
{
    public Camera camera2D;
    public Camera camera3D;
    public float flipDuration = 1f; //time to transition
    public Input flipKey;

    private bool is2D = true; //2d = true, 3d = false
    private Rigidbody rb;
    private Vector3 ogScale; //scale of player for 2d

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ogScale = transform.localScale;

        // Start in 2d
        //yet to implement
    }

    // Update is called once per frame
    void Update()
    {
        //flip input key
        //if(Input.GetKeyDown(flipKey))
    }
}
