using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMeAfter : MonoBehaviour
{
    public float time;

    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
