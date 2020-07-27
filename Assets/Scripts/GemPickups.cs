using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemPickups : MonoBehaviour
{
    [SerializeField] AudioClip pickupsSFX;
    [SerializeField] int pointsForPickups = 200;
    Animator myAnimator;
    bool isPicked = false;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPicked)
        {
            PickedUp();
        }
    }

    private void PickedUp()
    {
        isPicked = true;
        FindObjectOfType<GameSession>().AddToScore(pointsForPickups);
        if (pickupsSFX != null)
        {
            AudioSource.PlayClipAtPoint(pickupsSFX, Camera.main.transform.position);
        }
        myAnimator.SetTrigger("Picked");
    }
}
