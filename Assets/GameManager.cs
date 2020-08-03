using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    NetworkManagerLobby game;
    private void Awake()
    {
        game = FindObjectOfType<NetworkManagerLobby>();
    }
    void Update()
    {
               
    }
}
