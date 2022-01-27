using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCheck : MonoBehaviour
{
    public AudioSource hitGround;

    private void Start()
    {
        hitGround = GetComponentInParent<PlayerController>().hitGround;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponentInParent<PlayerController>().isJumping)
        {
            hitGround.Play();
        }
    }
}
