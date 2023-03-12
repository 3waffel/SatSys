using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicController : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;
    
    void Update()
    {
        Vector2 velocity = new Vector2();
        velocity.x = Input.GetKey(KeyCode.D) ? 1f : (Input.GetKey(KeyCode.A) ? -1f : 0f);
        velocity.y = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f);
        velocity *= speed;
        transform.position += new Vector3(velocity.x * Time.deltaTime, 0, velocity.y * Time.deltaTime);
    }
}
