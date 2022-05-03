using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{

    public int currentTabID;
    public MenuTabScript[] menuTabs;

    public void OpenNewTab(int tabToOpen)
    {
        //only refresh if the tab will be changing
        if (currentTabID != tabToOpen)
        {
            foreach (GameObject gO in menuTabs[currentTabID].myPickups)
            {
                gO.SetActive(false);
            }

            currentTabID = tabToOpen;

            foreach (GameObject gO in menuTabs[currentTabID].myPickups)
            {
                gO.SetActive(true);
            }
        }
    }
}
