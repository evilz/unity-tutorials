using UnityEngine;
using System;
using System.Collections;

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
                if (neighboursX >= 0 && neighboursX < width && neighboursY >= 0 && neighboursY < height)
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

    void OnDrawGizmos()
    {
        if (map == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gizmos.color = map[x, y] == 1 ? Color.black : Color.white;
                Vector3 pos = new Vector3(-width / 2 + x + .5f, -height / 2 + y + .5f);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }


}
