using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraPathfinder : Pathfinder
{
    private Map map;
    public override void Request(PathRequester requester, ReqInput input, Action<List<GraphNode>> OnServe)
    {
        if(map == null)
            map = FindObjectOfType<MapMgr>().map;

        StartCoroutine(Search(input, OnServe, OnSearchBegin, OnSearchEnd));
    }
    public override IEnumerator Search(ReqInput input, Action<List<GraphNode>> OnServe, Action OnBegin, Action OnEnd)
    {
        Dictionary<GraphNode, KeyValuePair<GraphNode, int>> table = 
            new Dictionary<GraphNode, KeyValuePair<GraphNode, int>>();

        foreach(var node in map.Nodes)
            table.Add(node, new KeyValuePair<GraphNode, int>(null, int.MaxValue));

        table[input.start] = new KeyValuePair<GraphNode, int>(table[input.start].Key, 0);
        table = table.OrderBy((e) => e.Value.Value).ToDictionary((e) => e.Key, (e) => e.Value);

       


        
        yield return null;
    }
}
