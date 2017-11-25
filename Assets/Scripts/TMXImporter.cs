using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class TMXImporter : MonoBehaviour {
    public string levelPath = "StreamingAssets/Level0.tmx";

    public bool[,] collisions;
    public int[,] heights;

    internal Vector2 size;
    public List<Vector2> startLocation;

    private bool isParsingALayer = false;
    private Vector2 offsetTile;
    private bool isOffsetSet;

    public void LoadTMX ()
    {
        ParseSize();
        InitArraySize();
        ParseStartLocation();
        ParseHeight();
        ParseCollision();
    }

    void ParseSize()
    {
        XmlTextReader reader = new XmlTextReader(levelPath);
        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    if (string.Compare(reader.Name, "layer") == 0)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            if (string.Compare(reader.Value, "Ground") == 0)
                            {
                                isParsingALayer = true;
                            }
                            else if (string.Compare(reader.Name, "width") == 0)
                            {
                                size += new Vector2(int.Parse(reader.Value), 0);
                            }
                            else if (string.Compare(reader.Name, "height") == 0)
                            {
                                size += new Vector2(0, int.Parse(reader.Value));
                            }
                        }
                    }
                    break;
                case XmlNodeType.Text:
                    if (isParsingALayer == true)
                    {
                        int x = 0, y = 0;
                        string[] lines = reader.Value.Split('\n');
                        foreach (string line in lines)
                        {
                            string[] tiles = line.Split(',');
                            if (tiles.Length <= 1)
                            {
                                continue;
                            }
                            int tileInt = 0;

                            ++y;
                            x = 0;
                            foreach (string tile in tiles)
                            {
                                ++x;
                                if (int.TryParse(tile, out tileInt))
                                {
                                    if (tileInt > 0)
                                    {
                                        if (!isOffsetSet)
                                        {
                                            offsetTile += new Vector2(x - 1 , y - 1);
                                            size -= offsetTile;
                                            isOffsetSet = true;
                                            isParsingALayer = false;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }

    void InitArraySize()
    {
        collisions = new bool[(int)size.x, (int)size.y];
        heights = new int[(int)size.x + 1, (int)size.y + 1];
    }


    void ParseStartLocation()
    {
        XmlTextReader reader = new XmlTextReader(levelPath);

        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    if (string.Compare(reader.Name, "layer") == 0)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            if (string.Compare(reader.Value, "Spawn") == 0)
                            {
                                isParsingALayer = true;
                            }
                        }
                    }
                    break;
                case XmlNodeType.Text:
                    if (isParsingALayer == true)
                    {
                        int x = 0, y = 0;
                        string[] lines = reader.Value.Split('\n');
                        foreach (string line in lines)
                        {
                            string[] tiles = line.Split(',');
                            if (tiles.Length <= 1)
                            {
                                continue;
                            }

                            int tileInt = 0;
                            ++y;
                            x = 0;
                            foreach (string tile in tiles)
                            {
                                ++x;
                                if (int.TryParse(tile, out tileInt))
                                {
                                    if (tileInt > 0)
                                    {
                                        startLocation.Add(new Vector2(size.x - (x - offsetTile.x), size.y - (y - offsetTile.y)));
                                    }
                                }
                            }
                        }
                        isParsingALayer = false;
                        return;
                    }
                    break;
            }
        }
    }

    void ParseHeight()
    {
        XmlTextReader reader = new XmlTextReader(levelPath);
        bool isGroundHalfLayer = false;
        int currentHeight = -1;

        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    if (string.Compare(reader.Name, "layer") == 0)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            if (string.Compare(reader.Value, "Ground") == 0)
                            {
                                isParsingALayer = false;
                                isGroundHalfLayer = false;

                            }
                            else if (string.Compare(reader.Value, "GroundHalf") == 0)
                            {
                                isParsingALayer = true;
                                isGroundHalfLayer = true;
                            }
                            else if (string.Compare(reader.Value, 0, "Ground", 0, 6) == 0)
                            {
                                isParsingALayer = true;
                                isGroundHalfLayer = false;
                                currentHeight++;
                            }
                        }
                    }
                    break;
                case XmlNodeType.Text:
                    if (isParsingALayer == true)
                    {
                        int x = 0, y = 0;
                        string[] lines = reader.Value.Split('\n');
                        foreach (string line in lines)
                        {
                            string[] tiles = line.Split(',');
                            if (tiles.Length <= 1)
                            {
                                continue;
                            }

                            int tileInt = 0;
                            ++y;
                            x = 0;
                            foreach (string tile in tiles)
                            {
                                ++x;
                                if (int.TryParse(tile, out tileInt))
                                {
                                    if (tileInt > 0)
                                    {
                                        if (isGroundHalfLayer)
                                        {
                                            heights[(int)size.x - (x - (int)offsetTile.x), (int)size.y - (y - (int)offsetTile.y)]--;
                                        }
                                        else
                                        {
                                            heights[(int)size.x - (x - (int)offsetTile.x) - currentHeight, (int)size.y - (y - (int)offsetTile.y) - currentHeight] += 2;
                                        }
                                    }
                                }
                            }
                        }
                        isParsingALayer = false;
                    }
                    break;
            }
        }
    }

    void ParseCollision()
    {
        XmlTextReader reader = new XmlTextReader(levelPath);

        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    if (string.Compare(reader.Name, "layer") == 0)
                    {
                        while (reader.MoveToNextAttribute())
                        {
                            if (string.Compare(reader.Value, "Collision") == 0)
                            {
                                isParsingALayer = true;
                            }
                        }
                    }
                    break;
                case XmlNodeType.Text:
                    if (isParsingALayer == true)
                    {
                        int x = 0, y = 0;
                        string[] lines = reader.Value.Split('\n');
                        foreach (string line in lines)
                        {
                            string[] tiles = line.Split(',');
                            if (tiles.Length <= 1)
                            {
                                continue;
                            }

                            int tileInt = 0;
                            ++y;
                            x = 0;
                            foreach (string tile in tiles)
                            {
                                ++x;
                                if (int.TryParse(tile, out tileInt))
                                {
                                    if (tileInt > 0)
                                    {
                                        collisions[(int)size.x - (x - (int)offsetTile.x), (int)size.y - (y - (int)offsetTile.y)] = true;
                                    }
                                }
                            }
                        }
                        isParsingALayer = false;
                        return;
                    }
                    break;
            }
        }
    }
}
