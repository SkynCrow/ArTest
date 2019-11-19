using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    uint NetworkId { get; set; }
    string Name { get; set; }
}
