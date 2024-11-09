using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ShipMechanism // the active ship mechanism.
{
        FloatingShip parentShip {get; set;}
        Rigidbody shipRb{get; set;}
        ShipPart part{get; set;}
}
