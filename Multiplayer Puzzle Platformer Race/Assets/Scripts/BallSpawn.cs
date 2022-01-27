using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawn : MonoBehaviour
{
    public string prefab;
    public Transform spawn;

    public float countdown;

    public bool upsideDown;
    public float force = 2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (countdown > -0.02)
        {
            countdown -= Time.deltaTime;
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject != null && collision.gameObject.tag == "Player" && countdown < 0)
        {
            countdown = 5f;

            GameObject ball = PhotonNetwork.Instantiate(prefab, spawn.position, Quaternion.identity);
            ball.GetComponent<Rigidbody2D>().gravityScale = upsideDown ? -1 : 1;
            ball.GetComponent<Rigidbody2D>().AddForce(Vector2.left * force);
        }
    }

}
