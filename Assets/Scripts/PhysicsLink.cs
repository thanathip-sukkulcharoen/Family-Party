using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsLink : NetworkBehaviour
{
    public Rigidbody2D rigidbody2D;

    [SyncVar]
    public Vector2 velocity;
    [SyncVar]
    public Vector2 position;

    private void Update()
    {
        if (GetComponent<NetworkIdentity>().isServer)
        {
            position = rigidbody2D.position;
            velocity = rigidbody2D.velocity;
            rigidbody2D.position = position;
            rigidbody2D.velocity = velocity;
        }
        if (GetComponent<NetworkIdentity>().isClient)
        {
            rigidbody2D.position = position + velocity * (float)NetworkTime.rtt;
            rigidbody2D.velocity = velocity;
        }
    }
    public void ApplyForce(Vector2 force,ForceMode2D FMode)
    {
        rigidbody2D.AddForce(force, FMode);
        CmdApplyForce(force, FMode);
    }
    [Command] 
    public void CmdApplyForce(Vector2 force,ForceMode2D FMode)
    {
        rigidbody2D.AddForce(force, FMode);
    }
}
