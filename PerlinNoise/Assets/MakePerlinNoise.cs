using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class MakePerlinNoise : MonoBehaviour
{
    public float x = 0, y = 0;
    int length;

    TerrainData map;
    [SerializeField] int seed = 234;
    const float INTERVAL = 0.1f;
    [SerializeField] float AMP = 2;
    [SerializeField] float FREQUENCY = 2;
    [SerializeField] int REPEAT = 3;
    float[] increse;
    float[] decrese;

    public List<List<float>> mapBoard = new();
    void MakeMap(float x = 0, float y = 0)
    {

        //StreamWriter streamWriter = new StreamWriter("Assets/f.txt");
        length = map.heightmapResolution;

        float[,] value = map.GetHeights(0, 0, length, length);

        for(int i = 0; i < length; i++)
        {
            mapBoard.Add(new List<float>());
            for(int j = 0; j < length; j++)
            {
                value[i, j] = 0;

                for(int k = 0; k < REPEAT; k++)
                {
                    value[i, j] += MakeNoise(INTERVAL * i * increse[k], INTERVAL * j * increse[k]) / decrese[k];
                }
                //value[i, j] /= REPEAT;

                mapBoard[i].Add(value[i, j]);
               //streamWriter.Write(value[i, j] + " ");
            }
            //streamWriter.Write("\n");
            this.x = x + INTERVAL * i;
            this.y = this.x;
        }
        map.SetHeights(0, 0, value);
    }


    void Map(float x = 0, float y = 0)
    {
        float[,] value = map.GetHeights(0, 0, length, length);

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {

                for (int k = 0; k < REPEAT; k++)
                {
                    value[i, j] += 
                        Mathf.PerlinNoise((x + INTERVAL * i) * increse[k], (y + INTERVAL * j) * increse[k]) / decrese[i];
                }
                value[i, j] /= REPEAT;
            }
        }
        map.SetHeights(0, 0, value);
    }


    void Awake()
    {
        map = GetComponent<Terrain>().terrainData;
        increse = new float[20];
        decrese = new float[20];
        increse[0] = 1;
        decrese[0] = 1;
        for(int i = 1; i < 20; i++)
        {
            increse[i] = Mathf.Pow(AMP, i);
            decrese[i] = Mathf.Pow(FREQUENCY, i);
        }
        MakeMap();
    }

    

    // Update is called once per frame
    /*void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            mapBoard.Remove(mapBoard[0]);
            List<float> temp = new();
            for(int i = 0; i < length; i++)
            {
                x += INTERVAL;
                temp.Add(0);
                for(int j = 0; j < REPEAT; j++)
                {
                    temp[i] += MakeNoise(x + INTERVAL * increse[j], INTERVAL * i * increse[j]);
                }
            }
        }
    }*/









    float MakeNoise(float x, float y)
    {

        int gridX = (int)x;
        int gridY = (int)y;

        float leftUp = DotProduct(gridX, gridY, x, y);
        float leftDown = DotProduct(gridX + 1, gridY, x, y);

        float rightUp = DotProduct(gridX, gridY + 1, x, y);
        float rightDown = DotProduct(gridX + 1, gridY + 1, x, y);

        float intervalX = SmoothInterpolation(x - gridX);
        float intervalY = SmoothInterpolation(y - gridY);

        float interpolationLeft = LinearInterpolation(leftUp, leftDown, intervalX);
        float interpolationRight = LinearInterpolation(rightUp, rightDown, intervalX);

        float result = LinearInterpolation(interpolationLeft, interpolationRight, intervalY);

        return (result + 1) / 2;

        float DotProduct(int gridX, int gridY, float x, float y)
        {
            UnityEngine.Random.InitState(seed * (gridX * 12343 ^ gridY * 92));
            float degree = UnityEngine.Random.Range(0, 2 * Mathf.PI);

            float deltaX = x - gridX;
            float deltaY = y - gridY;

            if (deltaY == 0 && deltaX == 0) deltaX = 0.01f;

            return deltaX * Mathf.Sin(degree) + deltaY * Mathf.Cos(degree);
        }

        float SmoothInterpolation(float x) { return x * x * (3 - 2 * x); }
        float LinearInterpolation(float x1, float x2, float t) { return (1 - t) * x1 + t * x2; }
    }
}
