using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerManager : MonoBehaviour {

    private const int MAX_PLAYER = 4;
    public GameObject[] players = new GameObject[MAX_PLAYER];
    public Vector2[] playerStartPos = new Vector2[MAX_PLAYER];
    private bool[] playerLeft = new bool[MAX_PLAYER];
    private int leftPlayer;

    private string[] skinNames = {
        "skeleton_occultist"
      , "skeleton_knight"
      , "skeleton_mage"
      , "skeleton_0"
    };

    //
    // Other manager
    private ControllerManager controllerMG;
    private GameManager gameMG;
    public BoardManager boardMG;

    void Start () {
        controllerMG = gameObject.GetComponent<ControllerManager>();
        Assert.IsNotNull(controllerMG);
        gameMG = gameObject.GetComponent<GameManager>();
        Assert.IsNotNull(gameMG);
        boardMG = gameObject.GetComponent<BoardManager>();
        Assert.IsNotNull(boardMG);
    }
	
    public void InitForGame () {

        boardMG.init();

        for (int nPlayer = 0; nPlayer < MAX_PLAYER; ++nPlayer)
        {
            players[nPlayer].GetComponent<PlayerScript>().currentGameUI.SetActive(false);
            players[nPlayer].SetActive(false);
            playerLeft[nPlayer] = true;
        }
        //
        // Get number of player and put them in the right position
        int nbPlayer = controllerMG.nbPlayerSet;
        leftPlayer = nbPlayer;

        for (int nPlayer = 0; nPlayer < nbPlayer; ++nPlayer)
        {
            players[nPlayer].SetActive(true);
            Vector2 v = boardMG.tmx.startLocation[nPlayer];
            PlayerScript script = players[nPlayer].GetComponent<PlayerScript>();
            script.Init(controllerMG.playerControllerName[nPlayer], skinNames[gameMG.UISelectionPlayer[nPlayer].GetComponent<MenuPanelScript>().currentSkin], v.x, v.y, boardMG, nPlayer);
        }
    }

    public void playerKilled(int nPlayer)
    {
        playerLeft[nPlayer] = false;
        leftPlayer--;
        bool error = true;
        if (leftPlayer <= 1)
        {
            for (int nPlayerTab = 0; nPlayer < controllerMG.nbPlayerSet; ++nPlayer)
            {
                if(playerLeft[nPlayerTab] == true)
                {
                    gameMG.victorySreen(nPlayerTab + 1);
                    error = false;
                    break;
                }
            }
            if(error)
            {
                gameMG.victorySreen(-1);
            }
        }
    }

    void Update () {
	}
}
