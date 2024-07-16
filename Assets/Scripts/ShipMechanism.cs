using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ShipMechanism
{
        Ship ship {get; set;}
        Rigidbody shipRb{get; set;}
        ShipPart part{get; set;}
}
