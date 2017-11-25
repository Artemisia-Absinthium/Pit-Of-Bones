using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BoardManager : MonoBehaviour {

    public Tile[,] map;
    public TMXImporter tmx;
    public GameObject gameObjects;

    internal Vector2[] playerPos  = new Vector2[4];
    internal Vector2[] playerPosCollision = new Vector2[4];

    //
    // Item manager
    public uint currentNbRangedItem = 0;
    public uint maxNbRangedItem = 4;
    public List<GameObject> rangedItems;
    public List<Vector2> rangedItemsPos;
    
    public uint currentNbPotionItem = 0;
    public uint maxNbPotionItem = 2;
    public List<GameObject> potionItems;
    public List<Vector2> potionItemsPos;

    public uint currentNbTpItem = 0;
    public uint maxNbTpItem = 1;
    public List<GameObject> tpItems;
    public List<Vector2> tpItemsPos;
    //
    // Other manager
    private ControllerManager controllerMG;
    internal GameManager gameMG;
    private PlayerManager playerMG;

    void Start() {
        rangedItems = new List<GameObject>();
        potionItems = new List<GameObject>();
        tpItems     = new List<GameObject>();

        controllerMG = gameObject.GetComponent<ControllerManager>();
        Assert.IsNotNull(controllerMG);
        gameMG = gameObject.GetComponent<GameManager>();
        Assert.IsNotNull(gameMG);
        playerMG = gameObject.GetComponent<PlayerManager>();
        Assert.IsNotNull(playerMG);

        tmx.LoadTMX();

        map = new Tile[(int)tmx.size.x, (int)tmx.size.y];

        for (uint x = 0; x < (int)tmx.size.x; ++x)
        {
            for (uint y = 0; y < (int)tmx.size.y; ++y)
            {
                map[x, y] = new Tile();
                map[x, y].InitWithTMXData(x, y, tmx.heights[x, y], tmx.collisions[x, y]);
            }
        }
        foreach (Vector2 v in tmx.startLocation)
        {
            map[(int)v.x, (int)v.y].setStartLocation();
        }
    }

    void Update()
    {
        AddMissingItems();
    }

    public void init()
    {
        for (int nPlayer = 0; nPlayer < 4; nPlayer++)
        {
            playerPos[nPlayer] = new Vector2(-5000, -5000);
            playerPosCollision[nPlayer] = new Vector2(-5000, -5000);
        }
    }

    public PlayerScript getPlayerScript(int nPlayer)
    {
        return playerMG.players[nPlayer].GetComponent<PlayerScript>();
    }

    internal bool isTileNotWalkable(Vector2 currentPos, int x, int y)
    {
        if (x >= tmx.size.x || x < 0
            || y >= tmx.size.y || y < 0)

        {
            return true;
        }
        else if (map[x, y].isCollision)
        {
            return true;
        }
        else if (Mathf.Abs(map[(int)currentPos.x, (int)currentPos.y].height - map[x, y].height) > 1.0001)
        {
            return true;
        }
        else
        {
            for(int nPlayer = 0; nPlayer < 4; nPlayer++)
            {
                if((int)playerPosCollision[nPlayer].x == x
                    && (int)playerPosCollision[nPlayer].y == y)
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal bool isTileNotAccessible(Vector2 currentPos, int x, int y, int radius)
    {
        if (currentPos.x + x >= tmx.size.x || currentPos.x + x < 0
         || currentPos.y + y >= tmx.size.y || currentPos.y + y < 0)
        {
            return true;
        }
        else if (map[(int)currentPos.x + x, (int)currentPos.y + y].isCollision)
        {
            return true;
        }
        else if (Mathf.Abs(map[(int)currentPos.x, (int)currentPos.y].height - map[(int)currentPos.x + x, (int)currentPos.y + y].height) > radius)
        {
            return true;
        }

        return false;
    }

    void AddMissingItems()
    {
        if (currentNbRangedItem < maxNbRangedItem)
        {
            for (uint i = 0; i < maxNbRangedItem - currentNbRangedItem; ++i)
            {
                int x, y;
                do
                {
                    x = Random.Range(0, (int)tmx.size.x);
                    y = Random.Range(0, (int)tmx.size.y);
                } while (map[x, y].isCollision || map[x, y].isItemLocation);

                map[x, y].isItemLocation = true;
                ++currentNbRangedItem;
                string itemRessource = System.String.Concat ("Item", Random.Range(1, 5));
                GameObject item = Instantiate(Resources.Load(itemRessource) as GameObject);

                Vector2 posInMap    = map[x, y].position;
                Vector2 posInScreen = gameMG.computeScreenPosFromBoard(posInMap.x, posInMap.y);
                item.transform.position = posInScreen;
                item.transform.SetParent(gameObjects.transform, true);
                rangedItems.Add(item);
                rangedItemsPos.Add(new Vector2(x, y));
            }
        }

        if (currentNbPotionItem < maxNbPotionItem)
        {
            for (uint i = 0; i < maxNbPotionItem - currentNbPotionItem; ++i)
            {
                int x, y;
                do
                {
                    x = Random.Range(0, (int)tmx.size.x);
                    y = Random.Range(0, (int)tmx.size.y);
                } while (map[x, y].isCollision || map[x, y].isItemLocation);

                map[x, y].isItemLocation = true;
                ++currentNbPotionItem;
                GameObject item = Instantiate(Resources.Load("Item5") as GameObject);

                Vector2 posInMap    = map[x, y].position;
                Vector2 posInScreen = gameMG.computeScreenPosFromBoard(posInMap.x, posInMap.y);
                item.transform.position = posInScreen;
                item.transform.SetParent(gameObjects.transform, true);
                potionItems.Add(item);
                potionItemsPos.Add(new Vector2(x, y));
            }
        }

        if (currentNbTpItem < maxNbTpItem)
        {
            for (uint i = 0; i < maxNbTpItem - currentNbTpItem; ++i)
            {
                int x, y;
                do
                {
                    x = Random.Range(0, (int)tmx.size.x);
                    y = Random.Range(0, (int)tmx.size.y);
                } while (map[x, y].isCollision || map[x, y].isItemLocation);

                map[x, y].isItemLocation = true;
                ++currentNbTpItem;
                GameObject item = Instantiate(Resources.Load("Item6") as GameObject);

                Vector2 posInMap    = map[x, y].position;
                Vector2 posInScreen = gameMG.computeScreenPosFromBoard(posInMap.x, posInMap.y);
                item.transform.position = posInScreen;
                item.transform.SetParent(gameObjects.transform, true);
                tpItems.Add(item);
                tpItemsPos.Add(new Vector2(x, y));
            }
        }
    }
}
