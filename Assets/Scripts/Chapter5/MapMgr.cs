using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : MonoBehaviour
{
    
    public Map map = new Map(true);

    [SerializeField] Block blockPrefab;
    [SerializeField] int blockVerticalCount;
    [SerializeField] int blockHorizontalCount;
    [SerializeField] float blockSizeX;
    [SerializeField] float blockSizeY;
 
    public List<List<Block>> blocks = new List<List<Block>>();

    private void Start()
    {
        Vector3 ScreenCenterWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        ScreenCenterWorldPos.z = 0f;
        Vector3 leftBottomMost = ScreenCenterWorldPos + new Vector3(
                -blockSizeX * blockHorizontalCount / 2f + blockSizeX / 2f,
                -blockSizeY * blockVerticalCount / 2f + blockSizeY / 2f,
                0f
            );

        MakeNodes(leftBottomMost);
        MakeEdges();
    }

    private void MakeNodes(Vector3 leftBottomMost)
    {
        for (int i = 0; i < blockVerticalCount; i++)
        {
            blocks.Add(new List<Block>());
            for (int j = 0; j < blockHorizontalCount; j++)
            {
                Vector3 pos = leftBottomMost + new Vector3(blockSizeX * j, blockSizeY * i, 0f);
                Block block = Instantiate(blockPrefab, pos, Quaternion.identity);
                blocks[i].Add(block);
                map.AddNode(block);
            }
        }
    }

    private void MakeEdges()
    {
        // right up left down
        int[] x = { 1, 0, -1, 0 };
        int[] y = { 0, 1, 0, -1 };
        for (int i = 0; i < blockVerticalCount; i++)
        {
            for (int j = 0; j < blockHorizontalCount; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    if (j + x[k] >= 0 && j + x[k] < blockHorizontalCount && i + y[k] >= 0 && i + y[k] < blockVerticalCount)
                    {
                        GraphEdge e = new GraphEdge(blocks[i][j], blocks[i + y[k]][j + x[k]], 1);
                        map.AddEdge(e);
                        blocks[i][j].OutEdges.Add(e);
                    }
                }
            }
        }
    }

}
