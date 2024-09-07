using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Horizontal"))
        {
            player.velocity = new Vector3(Mathf.Atan(Input.GetAxis("Horizontal") * Time.deltaTime * 10) * 5000, player.velocity.y - 0.5f, player.velocity.z);

        }
        if (Input.GetButton("Vertical"))
        {
            player.velocity = new Vector3(player.velocity.x, player.velocity.y - 0.5f, Mathf.Atan(Input.GetAxis("Vertical") * Time.deltaTime * 10) * 5000);
        }
        if (Input.GetKey(KeyCode.R)) this.transform.Rotate(Vector3.up);
    }
}
