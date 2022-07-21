using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<Selectable>()?.Select();
    }
}
