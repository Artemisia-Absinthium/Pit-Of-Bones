using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerUIScript : MonoBehaviour {

    public GameObject lifePrefab;
    internal GameObject lifeBarMiddle;
    internal GameObject lifeBarLeft;
    internal GameObject lifeBarRight;

    public GameObject staminaPrefab;
    internal GameObject staminaBarMiddle;
    internal GameObject staminaBarLeft;
    internal GameObject staminaBarRight;

    private float barSizeX;
    // Use this for initialization
    void Start () {

        lifeBarMiddle = lifePrefab.transform.Find("Full_Middle").gameObject;
        lifeBarLeft = lifePrefab.transform.Find("Full_Left").gameObject;
        lifeBarRight = lifePrefab.transform.Find("Full_Right").gameObject;

        staminaBarMiddle = staminaPrefab.transform.Find("Full_Middle").gameObject;
        staminaBarLeft = staminaPrefab.transform.Find("Full_Left").gameObject;
        staminaBarRight = staminaPrefab.transform.Find("Full_Right").gameObject;

        barSizeX = lifeBarMiddle.transform.localScale.x;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    internal void Init()
    {
    }

    internal void Updatelife(float purcentage)
    {
        if(purcentage < 0)
        {
            purcentage = 0;
        }
        else if (purcentage >= 1)
        {
            purcentage = 1;
            lifeBarLeft.SetActive(true);
            lifeBarRight.SetActive(true);
        }
        else
        {
            lifeBarLeft.SetActive(false);
            lifeBarRight.SetActive(false);
        }

        Vector3 scale = lifeBarMiddle.transform.localScale;
        scale.x = purcentage * barSizeX;
        lifeBarMiddle.transform.localScale = scale;
    }

    internal void UpdateStamina(float purcentage)
    {
        if (purcentage < 0)
        {
            purcentage = 0;
        }
        else if (purcentage >= 1)
        {
            purcentage = 1;
            staminaBarLeft.SetActive(true);
            staminaBarRight.SetActive(true);
        }
        else
        {
            staminaBarLeft.SetActive(false);
            staminaBarRight.SetActive(false);
        }

        Vector3 scale = staminaBarMiddle.transform.localScale;
        scale.x = purcentage * barSizeX;
        staminaBarMiddle.transform.localScale = scale;
    }

    internal void Die()
    {
    }
}
