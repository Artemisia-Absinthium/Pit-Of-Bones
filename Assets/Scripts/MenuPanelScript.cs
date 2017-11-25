using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MenuPanelScript : MonoBehaviour {

    private GameObject panelButtons;
    private GameObject panelSkins;

    private string playerControllerName;

    public int currentSkin;

    public GameObject animatedObject;

    private float deltaAxis;

    private string[] skinNames = {
        "skeleton_occultist"
      , "skeleton_knight"
      , "skeleton_mage"
      , "skeleton_0"
    };

    void Start () {
        panelButtons = transform.Find("Panel_Buttons").gameObject;
        Assert.IsNotNull(panelButtons, "Panel Buttons null");

        panelSkins = transform.Find("Panel_Skins").gameObject;
        Assert.IsNotNull(panelSkins, "Panel Skins null");
        currentSkin = 0;

        deltaAxis = 0.0f;
    }

    void Update ()
    {
        deltaAxis += Time.deltaTime;
        if (panelSkins.activeSelf && deltaAxis > 0.4f)
        {
		    //
            // if skin active, check inputs
            if (Input.GetAxis("L_XAxis_" + playerControllerName) > 0)
            {
                currentSkin++;
                if(currentSkin > 3)
                {
                    currentSkin = 0;
                }
                animatedObject.GetComponent<Animator>().Play(skinNames[currentSkin]+"_walk_down_left");
                deltaAxis = 0.0f;
            }
            else if (Input.GetAxis("L_XAxis_" + playerControllerName) < 0)
            {
                currentSkin--;
                if (currentSkin < 0)
                {
                    currentSkin = 3;
                }
                animatedObject.GetComponent<Animator>().Play(skinNames[currentSkin] + "_walk_down_left");
                deltaAxis = 0.0f;
            }
        }
    }

    internal void Init()
    {
        //
        // Hide and show
        panelSkins.SetActive(false);
        panelButtons.SetActive(true);

        playerControllerName = "";
    }

    internal void ShowSkinSelection(string controllerName)
    {
        //
        // Hide and show
        panelSkins.SetActive(true);
        panelButtons.SetActive(false);
        playerControllerName = controllerName;
    }

}
