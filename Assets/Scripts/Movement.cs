using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    float x = 0;
    // Update is called once per frame
    void Update()
    {
        x += 1 * Time.deltaTime; //60fps - Time.deltaTime = 1/60
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * 5 * Time.deltaTime);


        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.Translate(Vector3.down * -5 * Time.deltaTime);
        }
    }
}
