using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] public float gameSpeed = 2f;
    [SerializeField] public float boundaryX = 5f; // Set a boundary to flip direction
    private bool movingLeft = true; // Track movement direction

    // Start is called before the first frame update
    void Start()
    {
        // Optionally, you can initialize position or other logic here
    }

    // Update is called once per frame
    void Update()
    {
        // If the object has passed the boundary on either side, reverse the direction
        if (transform.position.x <= -boundaryX || transform.position.x >= boundaryX)
        {
            movingLeft = !movingLeft; // Reverse direction
        }

        // Move the object in the current direction
        float moveDirection = movingLeft ? -1f : 1f;
        transform.Translate(moveDirection * gameSpeed * Time.deltaTime, 0, 0);
    }
}
