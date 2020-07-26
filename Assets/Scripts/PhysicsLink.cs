using UnityEngine;
using Mirror;

public class PhysicsLink : NetworkBehaviour
{
    public Rigidbody2D rb;

    //[SyncVar]//all the essental varibles of a rigidbody
    //public Vector2 Velocity;
    [SyncVar]
    public Vector2 Position;
    
    void Update()
    {
        if (GetComponent<NetworkIdentity>().isServer)//if we are the server update the varibles with our cubes rigidbody info
        {
            Position = rb.position;
            //Velocity = rb.velocity;
            rb.position = Position;
            //rb.velocity = Velocity;
        }
        if (GetComponent<NetworkIdentity>().isClient)//if we are a client update our rigidbody with the servers rigidbody info
        {
            rb.position = Position; //+ Velocity * (float)NetworkTime.rtt;//account for the lag and update our variblesr
            //rb.velocity = Velocity;
        }
    }
}