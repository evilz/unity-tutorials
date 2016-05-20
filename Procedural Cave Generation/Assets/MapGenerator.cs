using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;

    [Range(0, 100)]
    public int randomFillPercent;

    int[,] map;

    public string seed;
    public bool useRandomSeed;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
    }

     

    private void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        ProcessMap();

        int borderSize = 1;
        int[,] borderedMap = new int[width+borderSize*2,height+borderSize*2];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize
                    && y >= borderSize && y < height + borderSize)
                {
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                }
                else
                {
                    borderedMap[x, y] = 1;
                }
            }
        }

        MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
        meshGenerator.GenerateMesh(borderedMap, 1);
    }

    void ProcessMap()
    {
        List<List<Coord>> WallRegions = GetRegions(1);
        int wallThresholdSize = 50;
        foreach (var wallRegion in WallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (var tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }


        List<List<Coord>> RoomRegions = GetRegions(0);
        int roomThresholdSize = 50;
        foreach (var roomRegion in RoomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (var tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY] = 1;
                }
            }
        }


    }



    List<List<MapGenerator.Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);
                    foreach (var tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width,height];
        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX,startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX +1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX ))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x,y));
                        }

                    }
                }
            }
            
        }
        return tiles;
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.unscaledTime.ToString();
        }

        var random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    map[x, y] = 1;
                else
                {
                    map[x, y] = random.Next(0, 100) < randomFillPercent
                        ? 1
                        : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighboursTiles = GetSurroundingWallCount(x, y);

                if (neighboursTiles > 4)
                {
                    map[x, y] = 1;
                }
                else if (neighboursTiles < 4 )
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighboursX = gridX - 1; neighboursX <= gridX + 1; neighboursX++)
        {  for (int neighboursY = gridY - 1; neighboursY <= gridY + 1; neighboursY++)
            {
                if (IsInMapRange(neighboursX,neighboursY))
                {
                    if (neighboursX != gridX || neighboursY != gridY)
                    {
                        wallCount += map[neighboursX, neighboursY];
                    }
                }
                else
                {
                    wallCount++;
                }

            }
        }
        return wallCount;
    }

    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int tileX, int tileY) : this()
        {
            this.tileX = tileX;
            this.tileY = tileY;
        }
    }

    //void OnDrawGizmos()
    //{
    //    if (map == null) return;

    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            Gizmos.color = map[x, y] == 1 ? Color.black : Color.white;
    //            Vector3 pos = new Vector3(-width / 2 + x + .5f, -height / 2 + y + .5f);
    //            Gizmos.DrawCube(pos, Vector3.one);
    //        }
    //    }
    //}


}
