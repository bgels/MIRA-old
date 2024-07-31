using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Condition : MonoBehaviour
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    public Action<partymember> onStart { get; set; } 
    public Func<partymember, bool> OnBeforeMove { get; set; } //returning
    public Action<partymember> OnAfterTurn { get; set; } //setting
}
