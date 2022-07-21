using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    private List<TabGroupButton> registeredButtons = new List<TabGroupButton>();

    private TabGroupButton activeButton;

    public void Register(TabGroupButton tabButton)
    {
        registeredButtons.Add(tabButton);

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
        if (activeButton != null)
        {
            activeButton.SetShowObject(false);
        }

        activeButton = tabButton;

        if (activeButton != null)
        {
            activeButton.SetShowObject(true);
        }
    }
}
