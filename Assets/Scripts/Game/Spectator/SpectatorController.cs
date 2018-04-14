using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorController : MonoBehaviour {

    private float speed = 5.0F;

    void Update()
    {
        if (this.gameObject != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed += 10.0F;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                speed = 10.0F;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
            }
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
            }
        }
    }
}
