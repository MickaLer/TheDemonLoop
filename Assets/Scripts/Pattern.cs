using System.Collections;
using UnityEngine;

public abstract class Pattern : ScriptableObject
{
    public abstract IEnumerator Do();
}