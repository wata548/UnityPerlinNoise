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
    float[] increse;

    void MakeMap(float x, float y)
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
                    value[i, j] += MakeNoise((x + INTERVAL * i) * increse[k], (y + INTERVAL * j) * increse[k]);
                }
                value[i, j] /= REPEAT;

                if(Mathf.Abs(1 - (x + INTERVAL * i)) < 0.01)Debug.Log($"{x + INTERVAL * i}, {y + INTERVAL * j} : {value[i, j]}");
               //streamWriter.Write(value[i, j] + " ");
            }
            //streamWriter.Write("\n");
        }
        Debug.Log('*');
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
        increse = new float[20];
        for(int i = 0; i < 20; i++)
        {
            increse[i] = Mathf.Pow(AMP, i) / Mathf.Pow(FREQUENCY, i);
        }
    }

    public float x = 0, y = 0;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Horizontal")) {
            x += Input.GetAxisRaw("Horizontal") / 10;
            MakeMap(x,y);
        }
    }
}
