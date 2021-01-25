using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRegion : MonoBehaviour
{
    [SerializeField] int verticalRegionCount;
    [SerializeField] int horizontalRegionCount;
    [SerializeField] int regionWidth;
    [SerializeField] int regionHeight;

    public Vector2[,] gridCenters;

    // Start is called before the first frame update
    void Start()
    {
        GridInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetGridNum (Vector2 pos)
    {
        return 0;
    }

    private void GridInit()
    {
        gridCenters = new Vector2[verticalRegionCount, horizontalRegionCount];

        int width = regionWidth * horizontalRegionCount;
        int height = regionHeight * verticalRegionCount;

        float rightmostX = transform.position.x - width / 2f;
        float uppermostY = transform.position.y + height / 2f;

        for (int i = 0; i < verticalRegionCount; i++)
        {
            for (int j = 0; j < horizontalRegionCount; j++)
            {
                Vector2 rightupperMostOfCurr = new Vector2(rightmostX, uppermostY) + new Vector2(regionWidth * j, -regionHeight * i);
                gridCenters[i, j] = rightupperMostOfCurr - new Vector2(width / 2f, height / 2f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        gridCenters = new Vector2[verticalRegionCount, horizontalRegionCount];

        int width = regionWidth * horizontalRegionCount;
        int height = regionHeight * verticalRegionCount;

        float rightmostX = transform.position.x - width / 2f;
        float uppermostY = transform.position.y + height / 2f;

        for(int i = 0; i < verticalRegionCount; i++)
        {
            for(int j = 0; j < horizontalRegionCount; j++)
            {
                Vector2 rightupperMostOfCurr = new Vector2(rightmostX, uppermostY) + new Vector2(regionWidth * j, -regionHeight * i);
                Debug.DrawLine(rightupperMostOfCurr,
                    rightupperMostOfCurr - new Vector2(0f, regionHeight), Color.cyan);
                Debug.DrawLine(rightupperMostOfCurr,
                    rightupperMostOfCurr + new Vector2(regionWidth, 0f), Color.cyan);

                gridCenters[i, j] = rightupperMostOfCurr - new Vector2(width/2f, height/2f);
            }
        }
    }
}
