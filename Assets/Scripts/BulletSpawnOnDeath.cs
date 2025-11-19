using System;
using UnityEngine;

public class BulletSpawnOnDeath : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var value1 = other.gameObject.GetComponent<SpriteRenderer>().sprite.vertices;
        var value2 = other.gameObject.GetComponent<SpriteRenderer>().sprite.uv;
        Debug.Log(value1.Length);
    }
}
