using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    private List<TabGroupButton> registeredButtons = new List<TabGroupButton>();

    private TabGroupButton activeButton;

    public void Register(TabGroupButton tabButton)
    {
        registeredButtons.Add(tabButton);

        Debug.Log($"TabGroupButton registered: {tabButton}");

        if (activeButton == null)
        {
            SwitchTo(tabButton);
        }
        else
        {
            tabButton.SetShowObject(false);
        }
    }

    public void Unregister(TabGroupButton tabButton)
    {
        registeredButtons.Remove(tabButton);

        if (activeButton == tabButton)
        {
            if (registeredButtons.Count != 0)
            {
                SwitchTo(registeredButtons[0]);
            }
            else
            {
                SwitchTo(null);
            }
        }
    }

    public void SwitchTo(TabGroupButton tabButton)
    {
        if (activeButton == tabButton) return;

        if (activeButton != null)
        {
            activeButton.SetShowObject(false);
        }

        activeButton = tabButton;

        if (activeButton != null)
        {
            activeButton.SetShowObject(true);
        }

        var first = tabButton?.DisplayObject?.GetComponentInChildren<TabFirstSelectable>();

        var firstSelectable = first?.GetComponent<Selectable>();

        foreach (var btn in registeredButtons)
        {
            var btnSelectable = btn.GetComponent<Selectable>();

            if (btnSelectable == null) continue;

            var nav = btnSelectable.navigation;

            nav.selectOnDown = firstSelectable;

            btnSelectable.navigation = nav;
        }

        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(DeferSelect(firstSelectable));
        }
    }

    private IEnumerator DeferSelect(Selectable select)
    {
        // bro idk
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        select?.Select();
    }
}
