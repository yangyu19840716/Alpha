using UnityEngine;

class RandomModule
{
    static public float Rand(float min = 0.0f, float max = 1.0f)
    {
        return Random.Range(min, max);
    }
}