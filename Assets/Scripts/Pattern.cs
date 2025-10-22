using System.Collections;
using UnityEngine;

public abstract class Pattern : ScriptableObject
{
    public float followingPatternDelay;
    public abstract IEnumerator Do();
}