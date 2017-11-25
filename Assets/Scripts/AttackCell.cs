using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AttackCell : MonoBehaviour {

    public Vector2 radius = new Vector2(3, 3);
    public BoardManager boardMG;

    private List<GameObject> zone = new List<GameObject>();
    internal float timeAttackCellDisplayed;
    internal float delayAttackCell;

    void Start()
    {
    }

    void Update()
    {
        if (timeAttackCellDisplayed < 0.0001f)
        {
            // Skip
        }
        else if (Time.fixedTime >= timeAttackCellDisplayed + delayAttackCell)
        {
            foreach (GameObject tile in zone)
            {
                Destroy(tile);
            }
            zone.Clear();
        }
        else if (zone != null)
        {
            foreach (GameObject tile in zone)
            {
                tile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, (Mathf.Sin(Time.fixedTime * 5.0f) + 1) / 4 + 0.25f);
            }
        }
    }

    public List<Vector2> DisplayAttackCell(Vector2 currentPos, int width, int height, uint color, float delay)
    {
        List<Vector2> attackPos = new List<Vector2>();
        timeAttackCellDisplayed = Time.fixedTime;
        delayAttackCell = delay;

        radius = new Vector2(width, height);

        for (int x = - (width - 1); x < width; ++x)
        {
            for (int y = - (height - 1); y < height; ++y)
            {
                if (boardMG.isTileNotAccessible(currentPos, x, y, Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(width))))
                {
                    continue;
                }
                GameObject tile = Instantiate(Resources.Load("AttackCell1") as GameObject);

                Vector2 nextPosInMap = boardMG.map[x + (int)currentPos.x, y + (int)currentPos.y].position;
                Vector2 nextPosInScreen = boardMG.gameMG.computeScreenPosFromBoard(nextPosInMap.x, nextPosInMap.y);
                nextPosInScreen.y = nextPosInScreen.y + 0.27f;
                nextPosInScreen.x = nextPosInScreen.x + 0.05f;
                tile.transform.position = nextPosInScreen;
                zone.Add(tile);
                attackPos.Add(new Vector2((int)currentPos.x + x, (int)currentPos.y + y));
            }
        }

        return attackPos;
    }
}
