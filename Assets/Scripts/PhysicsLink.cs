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

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        // only simulate body on server
        rigidbody2D.simulated = true;
    }
    private void Update()
    {
        if (GetComponent<NetworkIdentity>().isServer) // if we are server update the varibles with our rigidbody info
        {
            // Update other client player with our server info
            position = rigidbody2D.position;
            velocity = rigidbody2D.velocity;
            // Change client player with our info
            rigidbody2D.position = position;
            rigidbody2D.velocity = velocity;
        }
        if (GetComponent<NetworkIdentity>().isClient) //if we are a client update our rigidbody with the servers rigidbody info
        {
            rigidbody2D.position = position + velocity * (float)NetworkTime.rtt;
            rigidbody2D.velocity = velocity;
        }
    }
    public void Move(Vector2 newVector)
    {
        rigidbody2D.velocity = newVector;
        CmdMove(newVector);
    }

    [Command]
    public void CmdMove(Vector2 newVector)
    {
        rigidbody2D.velocity = newVector;
    }

    public void AddVector(Vector2 newVector)
    {
        rigidbody2D.velocity += newVector;
        CmdAddVector(newVector);
    }

    [Command]
    public void CmdAddVector(Vector2 newVector)
    {
        rigidbody2D.velocity += newVector;
    }

    /*
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
    */
}
