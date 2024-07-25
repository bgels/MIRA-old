using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skillScript : MonoBehaviour
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] partymemberTrait type;  
}
