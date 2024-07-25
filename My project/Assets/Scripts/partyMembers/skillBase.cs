using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "skillScript", menuName = "partymember/create new skill")]
public class skillBase : ScriptableObject
{
    [SerializeField] string skillName;

    [TextArea]
    [SerializeField] string description;

    [TextArea]
    [SerializeField] string dialogue;

    [SerializeField] partymemberTrait type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int sp;

    public string Name => skillName;
    public string Description => description;
    public string Dialogue => dialogue;
    public partymemberTrait Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int Sp => sp;
}
