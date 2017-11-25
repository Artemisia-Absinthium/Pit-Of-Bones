using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    //
    // editables values
    public int maxLife;
    public int maxStamina;
    public int closeAttackStamina;
    public int rangeAttackStamina;
    public int closeAttackDamage;
    public int rangeAttackDamage;
    public int rangeAttackRange;
    public float rangeAttackDelay;
    public float timeHitAnimation;
    public float blinkAnimationTime;
    public int staminaRegenPerS;
    public AttackCell attackZone;
    public GameObject currentGameUI;

    //
    // Music
    public AudioSource hitSoundSource;
    public AudioSource attackSoundSource;
    public AudioSource teleportSoundSource;
    public AudioSource lootSoundSource;
    public AudioSource dieSoundSource;
    public AudioSource touchedSoundSource;

    //
    // private Values
    internal string playersSkinName = "skeleton_occulist";
    internal string controllerName = "1";
    private Animator playerAnimator;
    private float currentLife;
    private float currentStamina;
    private bool hitAnimation;
    private float deltaTimeHitAnimation;
    private float deltaBlinkAnimation;
    private float posX;
    private float posY;
    private Vector2 offsetedCasePos;
    private BoardManager boardMG;
    private float magicOffset = 0.43f;
    private GameObject walkableZone;
    private int playerNumber;
    private bool isRangeAttackEnabled;

    //
    // Enums
    private enum EPlayerState
    {
        state_idle,
        state_walk,
        state_close_attack,
        state_range_attack,
        state_dying
    }

    private EPlayerState curentState = EPlayerState.state_walk;

    enum EOrientation
    {
        up_left,
        up_right,
        down_left,
        down_right,
    }

    private EOrientation curentOrientation = EOrientation.up_left;

    //
    // Datas
    internal string[] orientationNames =
        {"_up_left",
        "_up_right",
        "_down_left",
        "_down_right" };

    internal string[] stateNames =
        {"_idle",
        "_walk",
        "_close_attack",
        "_range_attack",
        "_dying"};

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    public void Init(string ctrlName, string skinName, float X, float Y, BoardManager board, int playerNb)
    {
        controllerName = ctrlName;
        playersSkinName = skinName;
        curentState = EPlayerState.state_idle;
        curentOrientation = EOrientation.up_left;
        currentLife = maxLife;
        currentStamina = maxStamina;
        hitAnimation = false;
        deltaTimeHitAnimation = 0.0f;
        deltaBlinkAnimation = 0.0f;

        boardMG = board;
        //
        // Put in start position
        posX = X;
        posY = Y;
        playerNumber = playerNb;
        boardMG.playerPos[playerNumber] = new Vector2(X, Y);
        boardMG.playerPosCollision[playerNumber] = new Vector2(X, Y);
        offsetedCasePos = boardMG.map[(int)X, (int)Y].position;

        Vector2 screenPos = boardMG.gameMG.computeScreenPosFromBoard(offsetedCasePos.x, offsetedCasePos.y);

        Vector3 pos = transform.position;
        pos.x = screenPos.x;
        pos.y = screenPos.y + magicOffset;
        transform.position = pos;

        switchAnim();

        currentGameUI.SetActive(true);
        currentGameUI.GetComponent<playerUIScript>().Init();
    }

    void Update()
    {
        switch (curentState)
        {
            case EPlayerState.state_idle:
                CheckInputs();
                break;
            case EPlayerState.state_dying:
                break;
            case EPlayerState.state_walk:
                break;
            case EPlayerState.state_range_attack:
                break;
            case EPlayerState.state_close_attack:
                break;
        }

        //
        // Update hit animation
        if (hitAnimation && curentState != EPlayerState.state_dying)
        {
            deltaTimeHitAnimation += Time.deltaTime;
            deltaBlinkAnimation += Time.deltaTime;
            if (deltaTimeHitAnimation > timeHitAnimation)
            {
                deltaTimeHitAnimation = 0.0f;
                hitAnimation = false;
                //
                // Change color
                Color color = transform.GetComponent<SpriteRenderer>().color;
                color.g = 255;
                color.b = 255;
                transform.GetComponent<SpriteRenderer>().color = color;
            }
            else if (deltaBlinkAnimation > blinkAnimationTime)
            {
                deltaBlinkAnimation = 0.0f;
                //
                // Change color
                Color color = transform.GetComponent<SpriteRenderer>().color;
                color.g = (color.g == 255) ? 0 : 255;
                color.b = (color.b == 255) ? 0 : 255;
                transform.GetComponent<SpriteRenderer>().color = color;
            }
        }

        //
        // Update UI
        if (curentState != EPlayerState.state_dying)
        {
            currentStamina += staminaRegenPerS * Time.deltaTime;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
            currentGameUI.GetComponent<playerUIScript>().Updatelife(currentLife / (float)maxLife);
            currentGameUI.GetComponent<playerUIScript>().UpdateStamina(currentStamina / (float)maxStamina);
        }

        //
        // Update item w00ting
        retrieveWoot();
    }

    void CheckInputs()
    {
        //
        // orientation inputs
        if(String.Compare(controllerName, "k") == 0)
        {
            if (Input.GetAxis("L_YAxis_" + controllerName) < 0)
            {
                curentOrientation = EOrientation.up_right;
                switchAnim();
            }
            else if (Input.GetAxis("L_YAxis_" + controllerName) > 0)
            {
                curentOrientation = EOrientation.down_left;
                switchAnim();
            }
            else if (Input.GetAxis("L_XAxis_" + controllerName) > 0)
            {
                curentOrientation = EOrientation.down_right;
                switchAnim();
            }
            else if (Input.GetAxis("L_XAxis_" + controllerName) < 0)
            {
                curentOrientation = EOrientation.up_left;
                switchAnim();
            }
        }
        else
        {
            if (Input.GetAxis("L_YAxis_" + controllerName) < 0 && Input.GetAxis("L_XAxis_" + controllerName) > 0)
            {
                curentOrientation = EOrientation.up_right;
                switchAnim();
            }
            else if (Input.GetAxis("L_YAxis_" + controllerName) > 0 && Input.GetAxis("L_XAxis_" + controllerName) < 0)
            {
                curentOrientation = EOrientation.down_left;
                switchAnim();
            }
            else if (Input.GetAxis("L_XAxis_" + controllerName) > 0 && Input.GetAxis("L_YAxis_" + controllerName) > 0)
            {
                curentOrientation = EOrientation.down_right;
                switchAnim();
            }
            else if (Input.GetAxis("L_XAxis_" + controllerName) < 0 && Input.GetAxis("L_YAxis_" + controllerName) < 0)
            {
                curentOrientation = EOrientation.up_left;
                switchAnim();
            }
        }


        //
        // actions inputs
        if (Input.GetButtonDown("A_" + controllerName) && currentStamina > closeAttackStamina)
        {
            curentState = EPlayerState.state_close_attack;
            switchAnim();
            StartCoroutine(WaitForAnimEnd(0.5f));
            float attX = 0, attY = 0;
            switch(curentOrientation)
            {
                case EOrientation.up_right:
                    attX = posX; attY = posY + 1;
                    break;
                case EOrientation.down_left:
                    attX = posX; attY = posY-1;
                    break;
                case EOrientation.down_right:
                    attX = posX-1; attY = posY;
                    break;
                case EOrientation.up_left:
                    attX = posX+1; attY = posY;
                    break;
            }
            StartCoroutine(CheckPlayerCloseHit(0.25f, attX, attY));
            currentStamina -= closeAttackStamina;
        }
        else if (Input.GetButtonDown("X_" + controllerName))
        {
            int tmpPosY = (int)posY, tmpPosX = (int)posX;
            switch (curentOrientation)
            {
                case EOrientation.down_left:
                    tmpPosY--;
                    break;
                case EOrientation.down_right:
                    tmpPosX--;
                    break;
                case EOrientation.up_left:
                    tmpPosX++;
                    break;
                case EOrientation.up_right:
                    tmpPosY++;
                    break;
            }
            tryMoveTo((int)tmpPosX, (int)tmpPosY);
            switchAnim();
        }
        else if (Input.GetButtonDown("Y_" + controllerName))
        {
            if (isRangeAttackEnabled && Time.fixedTime >= attackZone.timeAttackCellDisplayed + attackZone.delayAttackCell)
            {
                isRangeAttackEnabled = false;

                curentState = EPlayerState.state_range_attack;
                switchAnim();
                currentStamina -= rangeAttackStamina;

                List<Vector2> atkPos = attackZone.DisplayAttackCell(new Vector2(posX, posY), 3, 3, 0, rangeAttackDelay);
                foreach (Vector2 pos in atkPos)
                {
                    StartCoroutine(CheckPlayerRangeHit(rangeAttackDelay, pos.x, pos.y));
                }
                StartCoroutine(WaitForAnimEnd(rangeAttackDelay));
            }
        }
        drawNextPosition();
    }

    private IEnumerator CheckPlayerCloseHit(float time, float x, float y)
    {
        attackSoundSource.Play();
        yield return new WaitForSeconds(time);
        for (int nPlayer = 0; nPlayer < 4; nPlayer++)
        {
            if (nPlayer != playerNumber 
                && boardMG.playerPos[nPlayer].x == x 
                && boardMG.playerPos[nPlayer].y == y)
            {
                boardMG.getPlayerScript(nPlayer).getHit(closeAttackDamage);
            }
        }
    }

    private IEnumerator CheckPlayerRangeHit(float time, float x, float y)
    {
        yield return new WaitForSeconds(time);
        for (int nPlayer = 0; nPlayer < 4; nPlayer++)
        {
            if (nPlayer != playerNumber 
                && boardMG.playerPos[nPlayer].x == x 
                && boardMG.playerPos[nPlayer].y == y)
            {
                boardMG.getPlayerScript(nPlayer).getHit(rangeAttackDamage);
            }
        }
        hitSoundSource.Play();
    }

    IEnumerator WaitForAnimEnd(float time)
    {
        yield return new WaitForSeconds(time);
        backToIdle();
    }

    void backToIdle()
    {
        curentState = EPlayerState.state_idle;
        switchAnim();
    }

    public void getHit(int damage)
    {
        if(!hitAnimation)
        {
            //
            // update values
            currentLife -= damage;
            touchedSoundSource.Play();
            if (currentLife <= 0) // start dead animation
            {
                curentState = EPlayerState.state_dying;
                currentGameUI.GetComponent<playerUIScript>().Updatelife(0);
                switchAnim();
                StartCoroutine(WaitForKillEvent(0.5f));
                dieSoundSource.Play();
            }
            else // start hit animation
            {
                hitAnimation = true;
            }
        }
    }

    IEnumerator WaitForKillEvent(float time)
    {
        yield return new WaitForSeconds(time);
        boardMG.gameMG.playersMG.playerKilled(playerNumber);
    }

    public void tryMoveTo(int x, int y)
    {
        if (boardMG.isTileNotWalkable(new Vector2(posX, posY), x, y))
        {
            return;
        }
        curentState = EPlayerState.state_walk;

        posX = x;
        posY = y;
        offsetedCasePos = boardMG.map[x, y].position;

        Vector2 screenPos = boardMG.gameMG.computeScreenPosFromBoard(offsetedCasePos.x, offsetedCasePos.y);

        Vector3 pos = transform.position;
        pos.x = screenPos.x;
        pos.y = screenPos.y + magicOffset;
        float timeMove = 0.50f;
        LeanTween.move(gameObject, pos, timeMove).setEase(LeanTweenType.linear).setOnComplete(backToIdle);
        StartCoroutine(WaitForUpdatePos(timeMove/2.0f));
        boardMG.playerPosCollision[playerNumber] = new Vector2(posX, posY);
    }

    public void teleportTo(int x, int y)
    {

        teleportSoundSource.Play();
        posX = x;
        posY = y;
        offsetedCasePos = boardMG.map[x, y].position;

        Vector2 screenPos = boardMG.gameMG.computeScreenPosFromBoard(offsetedCasePos.x, offsetedCasePos.y);

        Vector3 pos = transform.position;
        pos.x = screenPos.x;
        pos.y = screenPos.y + magicOffset;
        float timeMove = 0.50f;
        LeanTween.move(gameObject, pos, timeMove).setEase(LeanTweenType.linear).setOnComplete(backToIdle);
        StartCoroutine(WaitForUpdatePos(timeMove/2.0f));
        boardMG.playerPosCollision[playerNumber] = new Vector2(posX, posY);
    }

    IEnumerator WaitForUpdatePos(float time)
    {
        yield return new WaitForSeconds(time);
        uptadePos();
    }

    void uptadePos()
    {
        boardMG.playerPos[playerNumber] = new Vector2(posX, posY);
    }

    public void drawNextPosition()
    {
        if (walkableZone != null)
        {
            Destroy(walkableZone);
        }
        float nextPosX = posX, nextPosY = posY;

        switch (curentOrientation)
        {
            case EOrientation.down_left:
                nextPosY--;
                break;
            case EOrientation.down_right:
                nextPosX--;
                break;
            case EOrientation.up_left:
                nextPosX++;
                break;
            case EOrientation.up_right:
                nextPosY++;
                break;
        }

        if (boardMG.isTileNotWalkable(new Vector2(posX, posY), (int)nextPosX, (int)nextPosY))
        {
            return;
        }

        walkableZone = Instantiate(Resources.Load("WalkableZone1") as GameObject);

        Vector2 nextPosInMap    = boardMG.map[(int)nextPosX, (int)nextPosY].position;
        Vector2 nextPosInScreen = boardMG.gameMG.computeScreenPosFromBoard(nextPosInMap.x, nextPosInMap.y);
        walkableZone.transform.position = nextPosInScreen;
    }

    void switchAnim()
    {
        string animName = playersSkinName + stateNames[(int)curentState] + orientationNames[(int)curentOrientation];
        playerAnimator.Play(animName);
    }

    void retrieveWoot()
    {
        if (boardMG.map[(int)posX, (int)posY].isItemLocation)
        {
            for (int i = 0; i < boardMG.currentNbRangedItem; ++i)
            {
                if ((int)boardMG.rangedItemsPos[i].x == (int)posX &&
                    (int)boardMG.rangedItemsPos[i].y == (int)posY)
                {
                    Item woot = boardMG.rangedItems[i].GetComponent<Item>();
                    rangeAttackRange = woot.range;
                    rangeAttackDelay = woot.castTime;
                    rangeAttackDamage = woot.damage;

                    Destroy(boardMG.rangedItems[i]);
                    boardMG.rangedItemsPos.RemoveAt(i);
                    boardMG.rangedItems.RemoveAt(i);

                    boardMG.map[(int)posX, (int)posY].isItemLocation = false;
                    boardMG.currentNbRangedItem--;
                    lootSoundSource.Play();
                    isRangeAttackEnabled = true;

                    return;
                }
            }

            for (int i = 0; i < boardMG.currentNbPotionItem; ++i)
            {
                if ((int)boardMG.potionItemsPos[i].x == (int)posX &&
                    (int)boardMG.potionItemsPos[i].y == (int)posY)
                {
                    Item woot = boardMG.potionItems[i].GetComponent<Item>();
                    currentLife = (currentLife + woot.healPoint) > maxLife ? maxLife : currentLife + woot.healPoint;

                    Destroy(boardMG.potionItems[i]);
                    boardMG.potionItemsPos.RemoveAt(i);
                    boardMG.potionItems.RemoveAt(i);

                    boardMG.map[(int)posX, (int)posY].isItemLocation = false;
                    boardMG.currentNbPotionItem--;
                    lootSoundSource.Play();
                    return;
                }
            }

            for (int i = 0; i < boardMG.currentNbTpItem; ++i)
            {
                if ((int)boardMG.tpItemsPos[i].x == (int)posX &&
                    (int)boardMG.tpItemsPos[i].y == (int)posY)
                {
                    Destroy(boardMG.tpItems[i]);
                    boardMG.tpItemsPos.RemoveAt(i);
                    boardMG.tpItems.RemoveAt(i);

                    boardMG.map[(int)posX, (int)posY].isItemLocation = false;
                    boardMG.currentNbTpItem--;

                    int x, y;
                    do
                    {
                        x = Random.Range(0, (int)boardMG.tmx.size.x);
                        y = Random.Range(0, (int)boardMG.tmx.size.y);
                    } while (boardMG.map[x, y].isCollision || boardMG.map[x, y].isItemLocation);

                    teleportTo(x, y);

                    return;
                }
            }
        }
    }
}
