using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour {

    private const int MAX_PLAYER = 4;

    //
    // Menu Management
    public GameObject menuUI;
    public GameObject menuUICredits;
    public GameObject menuUIRules;
    public GameObject menuUIMainMenu;
    public GameObject[] menuPanels = new GameObject[MAX_PLAYER];
    public GameObject help_text;

    //
    // Game UI
    public GameObject gameUI;
    public GameObject menuUIPause;
    public GameObject menuVictory;
    public GameObject[] UISelectionPlayer = new GameObject[MAX_PLAYER];

    //
    // Game objects
    public GameObject gameObjects;
    public GameObject board;
    public GameObject startCenter;
    public GameObject offsetCenter;

    //
    // Music

    //
    // Other manager
    public ControllerManager controllerMG;
    public PlayerManager playersMG;
    private BoardManager boardMG;

    //
    // Values
    internal Vector2 startScreenPos;
    internal Vector2 offset;

    // Use this for initialization
    void Start () {
        //
        // Menu initial state
        ButtonBackToMenu();

        controllerMG = gameObject.GetComponent<ControllerManager>();
        Assert.IsNotNull(controllerMG);
        playersMG = gameObject.GetComponent<PlayerManager>();
        Assert.IsNotNull(playersMG);
        boardMG = gameObject.GetComponent<BoardManager>();
        Assert.IsNotNull(boardMG);

        //
        // Compute Screen Value
        startScreenPos = startCenter.transform.position;
        offset = offsetCenter.transform.position - startCenter.transform.position;

        // LeanTween Example
        // LeanTween.moveX(keyboardObject, 1f, 1f).setEase(LeanTweenType.easeInBounce).setDelay(1f);
        help_text.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        //
        //Check inputs
        controllerMG.CheckInputDefault();

        if (menuUI.activeInHierarchy == true)
        {
            controllerMG.CheckInputInit();
        }
    }

    public Vector2 computeScreenPosFromBoard(float x, float y)
    {
        Vector2 res = new Vector2((-x + y) * offset.x, (x + y) * offset.y);
        res += startScreenPos;
        return res;
    }

    //
    // UI Buttons
    public void ButtonPlay()
    {
        if(controllerMG.nbPlayerSet >= 2)
        {
            menuUI.SetActive(false);
            gameUI.SetActive(true);
            menuUIPause.SetActive(false);
            menuVictory.SetActive(false);
            gameObjects.SetActive(true);

            playersMG.InitForGame();
        }
        else
        {
            help_text.SetActive(true);
        }
    }

    public void ButtonCredits()
    {
        help_text.SetActive(false);
        menuUICredits.SetActive(true);
        menuUIRules.SetActive(false);
        menuUIMainMenu.SetActive(false);
        menuUIPause.SetActive(false);
    }

    public void ButtonBackToMenu()
    {
        menuUI.SetActive(true);
        gameUI.SetActive(false);
        menuUICredits.SetActive(false);
        menuUIRules.SetActive(false);
        menuUIMainMenu.SetActive(true);
        gameObjects.SetActive(false);
        controllerMG.Init();
        help_text.SetActive(false);

        //
        // Initialisation of the panel values
        foreach (GameObject panel in menuPanels)
        {
            panel.GetComponent<MenuPanelScript>().Init();
        }
    }

    public void ButtonRules()
    {
        help_text.SetActive(false);
        menuUICredits.SetActive(false);
        menuUIRules.SetActive(true);
        menuUIMainMenu.SetActive(false);
        menuUIPause.SetActive(false);
    }

    public void ButtonBackToGame()
    {
        menuUIPause.SetActive(false);
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }

    //
    // Other actions
    public void PauseTheGame()
    {
        if(!menuVictory.activeSelf)
        {
            menuUIPause.SetActive(true);
        }
    }

    public void EndTheGame()
    {
        menuUIPause.SetActive(true);
    }

    public void victorySreen(int player)
    {
        menuVictory.SetActive(true);
        if (player == -1)
        {
            menuVictory.transform.Find("Menu_Victory_Text").GetComponent<TextMesh>().text = "Fight over!";
        }
        else
        {
            menuVictory.transform.Find("Menu_Victory_Text").GetComponent<TextMesh>().text = "Player " + player + " Won!";
        }
    }
}
