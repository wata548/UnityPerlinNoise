using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class MakePerlinNoise : MonoBehaviour
{
    Texture2D map;
    [SerializeField] int seed = 234;
    const float INTERVAL = 0.1f;
    [SerializeField] float AMP = 0.5f;
    [SerializeField] float FREQUENCY = 2;
    [SerializeField] int REPEAT = 3;

    void MakeMap()
    {

        //StreamWriter streamWriter = new StreamWriter("Assets/f.txt");
        TerrainData terrainData = GetComponent<Terrain>().terrainData;

        var length = terrainData.heightmapResolution;

        float[,] value = terrainData.GetHeights(0, 0, length, length);

        float maxValue = 100;

        for(int i = 0; i < length; i++)
        {
            for(int j = 0; j < length; j++)
            {
                value[i, j] = 0;

                for(int k = 0; k < REPEAT; k++)
                {
                    float power = Mathf.Pow(AMP, k) / Mathf.Pow(FREQUENCY, k);
                    value[i, j] += MakeNoise(INTERVAL * i * power, INTERVAL * j * power);
                }
                value[i, j] /= REPEAT;
               //streamWriter.Write(value[i, j] + " ");
               // maxValue = maxValue < value[i, j] ? maxValue : value[i,j];
            }
            //streamWriter.Write("\n");
        }
        Debug.Log(maxValue);
        terrainData.SetHeights(0, 0, value);
    }

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

        /*if(x == 0 && y == 0)
        {
            Debug.Log($"gridX {gridX}");
            Debug.Log($"gridY {gridY}");
            Debug.Log($"leftUp {leftUp}");
            Debug.Log($"leftDown {leftDown}");
            Debug.Log($"rightUp {rightUp}");
            Debug.Log($"rightDown {rightDown}");
            Debug.Log($"intervalX {intervalX}");
            Debug.Log($"intervalY {intervalY}");
            Debug.Log($"interpolationLeft {interpolationLeft}");
            Debug.Log($"interpolationRight {interpolationRight}");
            Debug.Log($"result {result}");
        }*/
        return (result + 1)/ 2;

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

    

    void Awake()
    {
        //File.Create("Assets/f.txt");
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) {
            MakeMap();
        }
    }
}
