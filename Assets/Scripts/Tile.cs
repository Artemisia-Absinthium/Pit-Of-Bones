using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public bool isCollision;
    public bool isStartLocation;
    public int height;
    public bool isItemLocation;
    public Vector2 position;

    void CalculatePosition(uint x, uint y)
    {
        position = new Vector2(x + height / 2.0f, y + height / 2.0f);
    }

    public void InitWithTMXData(uint x, uint y, int height, bool collision)
    {
        this.height = height;
        this.isCollision = collision;

        CalculatePosition(x, y);
    }

    public void setStartLocation()
    {
        isStartLocation = true;
    }
}
