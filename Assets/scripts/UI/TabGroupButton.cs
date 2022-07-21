using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupButton : MonoBehaviour
{
    public GameObject displayObject;

    private bool showObject = false;

    private TabGroup tabGroup;

    void OnEnable()
    {
        tabGroup = GetComponentInParent<TabGroup>();
        Debug.Assert(tabGroup != null);
        tabGroup.Register(this);
    }

    void OnDisable()
    {
        tabGroup.Unregister(this);
    }

    void Start()
    {
        displayObject.SetActive(showObject);
    }

    public void Click()
    {
        tabGroup.SwitchTo(this);
    }

    public void SetShowObject(bool shouldShowObject)
    {
        showObject = shouldShowObject;
        displayObject.SetActive(showObject);
    }
}
