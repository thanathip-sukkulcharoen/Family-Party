using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryPickups : MonoBehaviour
{
    [SerializeField] AudioClip coinsPickupsSFX;
    [SerializeField] int pointsForCoinPickups = 100;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FindObjectOfType<GameSession>().AddToScore(pointsForCoinPickups);
        if (coinsPickupsSFX != null)
            AudioSource.PlayClipAtPoint(coinsPickupsSFX, Camera.main.transform.position);
        Destroy(this.gameObject);
    }
}
