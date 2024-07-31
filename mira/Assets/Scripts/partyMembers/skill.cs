using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skill
{
    public skillBase Base { get; set; }
    public int SP { get; set; }
    public skill(skillBase sBase)
    {
        Base = sBase;
        SP = sBase.Sp;
    }


}
