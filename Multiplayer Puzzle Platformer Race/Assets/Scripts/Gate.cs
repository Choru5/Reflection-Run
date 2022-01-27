using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{

    public BoxCollider2D block;

    public Camera mainCam;

    private Vector3 velocity = Vector3.zero;

    public float smoothTime = 0.5f;

    public bool usedOnce;

    // Start is called before the first frame update
    void Start()
    {
        block.enabled = false;
        usedOnce = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !usedOnce)
        {
            block.enabled = true;

            Vector3 targetPos = new Vector3(mainCam.transform.position.x + 18.4f, 0f, mainCam.transform.position.z);

            mainCam.transform.position = targetPos;

            usedOnce = true;
        }
       
    }
}
