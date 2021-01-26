using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraPathfinder : Pathfinder
{
    private Map map;

    public class TableElement
    {
        public readonly GraphNode node;
        public GraphNode node_via;
        public int cost;

        public TableElement(GraphNode node, GraphNode node_via, int cost)
        {
            this.node = node;
            this.node_via = node_via;
            this.cost = cost;
        }
    }

    public class Table 
    {
        public List<TableElement> table;

        public Table(List<GraphNode> nodes)
        {
            table = new List<TableElement>();
            foreach (var node in nodes)
                table.Add(new TableElement(node, null, int.MaxValue));
        }

        public TableElement this[GraphNode node]
        {
            get => table.Find((e) => e.node == node);
            set
            {
                var ele = table.Find((e) => e.node == node);
                table[table.IndexOf(ele)] = value;
            }
        }

    }


    public override IEnumerator Search(ReqInput input, Action<List<GraphNode>> OnServe)
    {
        OnSearchBegin();

        // Get the Map
        if (map == null)
            map = FindObjectOfType<MapMgr>().map;

        // Table : Node, FromNode, Cost
        Table table = 
            new Table(map.Nodes.ToList());

        // Visited : Node, bool
        Dictionary<GraphNode, bool> visited = new Dictionary<GraphNode, bool>();

        // initialization of data structures  
        foreach (var n in map.Nodes)
            visited.Add(n, false);

        table[input.start].cost = 0;


        while(true)
        {
            // find least cost && unvisited node
            GraphNode currNode = null;
            int tempCost = int.MaxValue;
            foreach (var element in table.table)
            {
                if (element.cost < tempCost && visited[element.node] == false)
                {
                    tempCost = element.cost;
                    currNode = element.node;
                }
            }
            if (currNode == null) break;


            visited[currNode] = true;

            Debug.Log("currNode " + currNode.gameObject.name);

            // update via currNode
            bool updateSome = false;
            foreach(var adjEdge in currNode.OutEdges)
            {
                if(table[currNode].cost + adjEdge.Cost < table[adjEdge.To].cost)
                {
                    updateSome = true;
                    table[adjEdge.To].node_via = currNode;
                    table[adjEdge.To].cost = table[currNode].cost + adjEdge.Cost;
                }
                    
            }

            // check if visited all nodes
            bool visitedAll = true;
            foreach (var v in visited)
                if (v.Value == false) visitedAll = false;

            if (!updateSome && visitedAll)
                break;

            yield return null;
        }


        // make path
        List<GraphNode> path = new List<GraphNode>() { input.destination };
        GraphNode node = input.destination;
        while(table[node].node_via != null && node != input.start)
        {
            path.Add(table[node].node_via);
            node = table[node].node_via;
            yield return null;
        }

        // success
        if(node == input.start)
        {
            Debug.Log("success");
            path.Add(node);
            path.Reverse();
            OnServe(path);
        }
        // fail
        else
        {
            Debug.Log("fail");
            OnServe(null);
        }

        OnSearchEnd();
    }
}
