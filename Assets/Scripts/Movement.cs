using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    float x = 0;

    // Update is called once per frame
    void Update()
    {
        // Increment x over time as an example (though this variable isn't used in this script)
        x += 1 * Time.deltaTime;

        // Move down continuously while 'S' key is held
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * 5 * Time.deltaTime);
        }

        // Move up continuously while 'W' key is held
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.up * 5 * Time.deltaTime);
        }

        // Move left continuously while 'A' key is held
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * 5 * Time.deltaTime);
        }

        // Move right continuously while 'D' key is held
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * 5 * Time.deltaTime);
        }
    }
}

