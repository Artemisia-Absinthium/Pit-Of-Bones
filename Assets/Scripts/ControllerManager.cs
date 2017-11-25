using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    internal int nbPlayerSet;

    //
    // Physical controller
    private bool[] controllerSet = new bool[4];

    internal string[] playerControllerName = new string[4];

    //
    // Other manager
    private GameManager gameMG;

    void Start()
    {
        gameMG = GetComponent<GameManager>();
    }

    public void Init()
    {
        //
        // Init values
        nbPlayerSet = 0;
        for (int nValue = 0; nValue < 4; ++nValue)
        {
            controllerSet[nValue] = false;
        }
    }

    public void CheckInputDefault()
    {
        //
        // Escape Key
        if (Input.GetKey("escape"))
        {
            if (gameMG.menuUI.activeInHierarchy == true)
            {
                gameMG.ButtonQuit();
            }
            else
            {
                gameMG.PauseTheGame();
            }
        }
    }

    public void CheckInputInit()
    {
        //
        // Check validation for every controllers
        if (Input.GetButtonDown("A_k") && !controllerSet[0])
        {
            controllerSet[0] = true;
            playerControllerName[nbPlayerSet] = "k";
            gameMG.menuPanels[nbPlayerSet].GetComponent<MenuPanelScript>().ShowSkinSelection("k");
            nbPlayerSet++;
        }
        else if (Input.GetButtonDown("A_1") && !controllerSet[1])
        {
            controllerSet[1] = true;
            playerControllerName[nbPlayerSet] = "1";
            gameMG.menuPanels[nbPlayerSet].GetComponent<MenuPanelScript>().ShowSkinSelection("1");
            nbPlayerSet++;
        }
        else if (Input.GetButtonDown("A_2") && !controllerSet[2])
        {
            controllerSet[2] = true;
            playerControllerName[nbPlayerSet] = "2";
            gameMG.menuPanels[nbPlayerSet].GetComponent<MenuPanelScript>().ShowSkinSelection("2");
            nbPlayerSet++;
        }
        else if (Input.GetButtonDown("A_3") && !controllerSet[3])
        {
            controllerSet[3] = true;
            playerControllerName[nbPlayerSet] = "3";
            gameMG.menuPanels[nbPlayerSet].GetComponent<MenuPanelScript>().ShowSkinSelection("3");
            nbPlayerSet++;
        }
    }

    void Update()
    {

    }
}
