using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private GridNode[,] Nodes;

    [Header("Randomisation")]
    private List<GridNode> Unexplored;
    private List<GridNode> Explored;
    private List<GridConnection> PotentialConnections;
    private GridNode StartNode;
    private GridNode DestinationNode;
    [SerializeField] private int Weight1Connection;
    [SerializeField] private int Weight2Connection;
    [SerializeField] private int Weight3Connection;

    //Path Finding Variables
    private List<GridNode> OpenList;
    private List<GridNode> ClosedList;
    private Dictionary<GridNode, GridNode> CameFrom;
    private List<GridNode> Path; //DEBUG

    public GameObject GridNodePrefab;

    [Header("Dimensions")]
    public int DimensionX;
    public int DimensionY;

    [Header("Prefab References")]
    [SerializeField] private GameObject Marble;
    [SerializeField] private GameObject Destination;
    private GameObject MarbleObject;
    private GameObject DestinationObject;

    private void Start()
    {
        GenerateGrid();
        RandomiseMaze();
    }

    private void Update()
    {
        //DEBUG
        if (Path != null)
        {
            for (int i = 0; i < Path.Count - 1; i++)
            {
                Debug.DrawLine(Path[i].transform.position, Path[i + 1].transform.position, Color.yellow);
            }
        }
    }

    //Clears the current maze and creates a new one
    public void NextLevel()
    {
        //DEBUG
        var watchStart = System.Diagnostics.Stopwatch.GetTimestamp();

        ClearBoard();

        //DEBUG
        Debug.Log($"Clear Level: {(System.Diagnostics.Stopwatch.GetTimestamp() - watchStart) / 10000}");

        GenerateGrid();

        //DEBUG
        Debug.Log($"Generate Grid: {(System.Diagnostics.Stopwatch.GetTimestamp() - watchStart) / 10000}");

        RandomiseMaze();

        //DEBUG
        Debug.Log($"Randomise Maze: {(System.Diagnostics.Stopwatch.GetTimestamp() - watchStart) / 10000}");
    }

    //Instantiates and arranges GridNode objects to fill the defined grid dimensions
    private void GenerateGrid()
    {
        //DEBUG
        //long startTicks = System.DateTime.Now.Ticks;
        //long endTicks;
        //System.TimeSpan span;

        //Geometry calculations
        float boardWidth = DimensionX;
        float boardHeight = DimensionY;
        float boardMaxX = (boardWidth - 1) / 2f;
        float boardMinX = -boardMaxX;
        float boardMaxY = (boardHeight - 1) / 2f;
        float boardMinY = -boardMaxY;

        //Store current transform rotation & set to 0's
        Quaternion rotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        //Loop through board dimensions & create grid nodes
        Nodes = new GridNode[DimensionX, DimensionY];
        float currentX = boardMinX;
        float currentY = boardMinY;
        for (int i = 0; i < DimensionX; i++)
        {
            for (int j = 0; j < DimensionY; j++)
            {
                //Create new node & move to position
                GameObject obj = Instantiate(GridNodePrefab, transform);
                obj.name = $"Node[{i}, {j}]";
                obj.transform.position = new Vector3(currentX, 0f, currentY);

                //Add internal grid node reference
                Nodes[i, j] = obj.GetComponent<GridNode>();

                //Increment y position
                currentY += 1f;
            }

            //Increment x position, reset y position
            currentX += 1f;
            currentY = boardMinY;
        }

        //Restore transform rotation
        transform.rotation = rotation;

        //DEBUG
        //endTicks = System.DateTime.Now.Ticks;
        //span = new System.TimeSpan(endTicks - startTicks);
        //Debug.Log("Generate Grid - Nodes: " + span.Milliseconds);
        //startTicks = System.DateTime.Now.Ticks;

        //Loop through grid nodes and set node properties
        for (int i = 0; i < Nodes.GetLength(0); i++)
        {
            for (int j = 0; j < Nodes.GetLength(1); j++)
            {
                //Retrieve grid node
                GridNode node = Nodes[i, j];

                //Set node properties
                node.GridPosX = i;
                node.GridPosY = j;
                node.North = GetNode(i, j + 1);
                node.East = GetNode(i + 1, j);
                node.South = GetNode(i, j - 1);
                node.West = GetNode(i - 1, j);
            }
        }

        //DEBUG
        //endTicks = System.DateTime.Now.Ticks;
        //span = new System.TimeSpan(endTicks - startTicks);
        //Debug.Log("Generate Grid - Connections: " + span.Milliseconds);
    }

    //Destroys all level objects
    private void ClearBoard()
    {
        //Loop through board dimensions & destroy grid nodes
        for (int i = 0; i < DimensionX; i++)
        {
            for (int j = 0; j < DimensionY; j++)
            {
                //Destroy Node
                Destroy(Nodes[i, j].gameObject);
                Nodes[i, j] = null;
            }
        }

        //Destroy the marble & destination object
        Destroy(MarbleObject);
        MarbleObject = null;
        Destroy(DestinationObject);
        DestinationObject = null;
    }

    //Generates a random maze from an initilised grid
    private void RandomiseMaze()
    {
        //Initialise lists
        Unexplored = GetNodesAsList();
        Explored = new List<GridNode>();
        PotentialConnections = new List<GridConnection>();
        CameFrom = new Dictionary<GridNode, GridNode>();

        //Select one node arbitrarily as the home node and explore it
        GridNode homeNode = Unexplored[Random.Range(0, Unexplored.Count)];
        ExploreNode(homeNode);

        //Create connections and explore nodes until all nodes have been explored
        while (Unexplored.Count > 0)
        {
            //Separate potential connections into classifications based on their "from" node (NodeA)
            List<GridConnection> connections0Paths = new List<GridConnection>();
            List<GridConnection> connections1Paths = new List<GridConnection>();
            List<GridConnection> connections2Paths = new List<GridConnection>();
            List<GridConnection> connections3Paths = new List<GridConnection>();
            for (int i = 0; i < PotentialConnections.Count; i++)
            {
                switch (PotentialConnections[i].NodeA.ConnectionsCount)
                {
                    case 0:
                        connections0Paths.Add(PotentialConnections[i]);
                        break;
                    case 1:
                        connections1Paths.Add(PotentialConnections[i]);
                        break;
                    case 2:
                        connections2Paths.Add(PotentialConnections[i]);
                        break;
                    case 3:
                        connections3Paths.Add(PotentialConnections[i]);
                        break;
                }
            }

            //Choose which class of connection to explore
            List<GridConnection> chosenConnections;
            if (connections0Paths.Count > 0) //Choose this list if it has items, handles home node case
            {
                chosenConnections = connections0Paths;
            }
            else
            {
                //Initialise RV thresholds
                int connection1Threshold = -1;
                int connection2Threshold = -1;
                int connection3Threshold = -1;

                //Calculate RV thresholds
                int currentThreshold = 0;
                if (connections1Paths.Count > 0)
                {
                    connection1Threshold = currentThreshold + Weight1Connection;
                    currentThreshold = connection1Threshold;
                }
                if (connections2Paths.Count > 0)
                {
                    connection2Threshold = currentThreshold + Weight2Connection;
                    currentThreshold = connection2Threshold;
                }
                if (connections3Paths.Count > 0)
                {
                    connection3Threshold = currentThreshold + Weight3Connection;
                    currentThreshold = connection3Threshold;
                }

                //Generate RV and evaluate thresholds
                int value = Random.Range(0, currentThreshold);
                if (value < connection1Threshold)
                    chosenConnections = connections1Paths;
                else if (value < connection2Threshold)
                    chosenConnections = connections2Paths;
                else
                    chosenConnections = connections3Paths;
            }

            //Randomly select a connection from the chosen class
            GridConnection connection = chosenConnections[Random.Range(0, chosenConnections.Count)];

            //Enable the chosen connection and explore the connected node
            connection.SetConnected(true);
            ExploreNode(connection.NodeB);

            CameFrom.Add(connection.NodeB, connection.NodeA);
        }

        //Collect a list of 1-connection GridNodes
        List<GridNode> deadEnds = Explored.Where(gn => gn.ConnectionsCount == 1).ToList();

        //Iterate through node pairs and find the pair with the longest shortest-path
        int longestPathLength = int.MinValue;
        GridNode chosenA = deadEnds[0];
        GridNode chosenB = deadEnds[1];
        for (int a = 0; a < deadEnds.Count - 1; a++)
        {
            for (int b = a + 1; b < deadEnds.Count; b++)
            {
                //Retrieve GridNodes
                GridNode currentA = deadEnds[a];
                GridNode currentB = deadEnds[b];

                //Find shortest path from a - b
                List<GridNode> path = FindPath(currentA, currentB);

                //Check if new longest
                if (path.Count > longestPathLength)
                {
                    //Assign new lngest path
                    chosenA = currentA;
                    chosenB = currentB;
                    longestPathLength = path.Count;
                    Path = path;
                }
            }
        }

        //Set chosen nodes A and B as the start and destination respectively
        StartNode = chosenA;
        DestinationNode = chosenB;

        //Spawn the marble & goal objects
        SpawnMarble();
        SpawnDestination();
    }

    //Explored the current node and collects its potential connections
    private void ExploreNode(GridNode node)
    {
        //Collection connections and add those that connect to unexplored nodes to the potential connections list
        List<GridConnection> connections = node.Connections.Where(c => c.NodeB != null).ToList();
        List<GridConnection> unexploredConnections = connections.Where(c => Unexplored.Contains(c.NodeB)).ToList();
        foreach (GridConnection connection in unexploredConnections)
            if (!PotentialConnections.Contains(connection))
                PotentialConnections.Add(connection);

        //Remove connections that connect to already explored nodes from the potential connections list
        List<GridConnection> exploredConnections = connections.Where(c => Explored.Contains(c.NodeB)).ToList();
        foreach (GridConnection connection in exploredConnections)
            if (PotentialConnections.Contains(connection))
                PotentialConnections.Remove(connection);

        //Mark the node as explored
        Explored.Add(node);
        Unexplored.Remove(node);
    }

    //Spawn the marble at the start node
    private void SpawnMarble()
    {
        GameObject marble = Instantiate(Marble, transform);
        marble.name = Marble.name;
        marble.transform.position = StartNode.MarbleSpawn.transform.position;

        MarbleObject = marble;
    }

    //SPawn the goal at the destination node
    private void SpawnDestination()
    {
        GameObject destination = Instantiate(Destination, transform);
        destination.name = Destination.name;
        destination.transform.position = DestinationNode.DestinationSpawn.transform.position;

        DestinationObject = destination;
    }

    //Retrieve the GridNode reference with the specified indices, null if out of array bounds
    public GridNode GetNode(int x, int y)
    {
        //Check if indices are within array bounds
        if (x < 0 || y < 0 || x >= Nodes.GetLength(0) || y >= Nodes.GetLength(1))
            return null;

        return Nodes[x, y];
    }

    //Retrieve the Nodes array as a list object
    public List<GridNode> GetNodesAsList()
    {
        List<GridNode> nodesList = new List<GridNode>();

        for (int i = 0; i < Nodes.GetLength(0); i++)
            for (int j = 0; j < Nodes.GetLength(1); j++)
                nodesList.Add(Nodes[i, j]);

        return nodesList;
    }

    //Finds the shortest path from the start node to the end node, if exists. Returns an ordered list of sequential nodes, or null if no path exists
    private List<GridNode> FindPath(GridNode start, GridNode goal)
    {
        #region OLD PATH FINDING

        ////Initialise lists
        //OpenList = new List<GridNode>();
        //ClosedList = new List<GridNode>();
        //CameFrom = new Dictionary<GridNode, GridNode>();

        ////Begin
        //OpenList.Add(start);

        //float gScore = 0;
        //float fScore = gScore + Heuristic(start, goal);

        //while (OpenList.Count > 0)
        //{
        //    //Find the Node in openList that has the lowest fScore
        //    GridNode currentNode = BestOpenListFScore(start, goal);

        //    //Found the end, reconstruct entire path and return
        //    if (currentNode == goal)
        //        return ReconstructPath(CameFrom, currentNode);

        //    OpenList.Remove(currentNode);
        //    ClosedList.Add(currentNode);

        //    for (int i = 0; i < currentNode.Connections.Length; i++)
        //    {
        //        if (currentNode.Connections[i] == null) continue;
        //        if (!currentNode.Connections[i].Connected) continue;
        //        GridNode thisNeighbourNode = currentNode.Connections[i].NodeB;

        //        //Ignore if neighbour node is attached
        //        if (!ClosedList.Contains(thisNeighbourNode))
        //        {
        //            //Distance from current to the nextNode
        //            float tentativeGScore = Heuristic(start, currentNode) + Heuristic(currentNode, thisNeighbourNode);

        //            //Check to see if in openList or if new GScore is more sensible
        //            if (!OpenList.Contains(thisNeighbourNode) || tentativeGScore < gScore)
        //                OpenList.Add(thisNeighbourNode);

        //            //Add to Dictionary - this neighour came from this parent
        //            if (!CameFrom.ContainsKey(thisNeighbourNode))
        //                CameFrom.Add(thisNeighbourNode, currentNode);

        //            gScore = tentativeGScore;
        //            fScore = Heuristic(start, thisNeighbourNode) + Heuristic(thisNeighbourNode, goal);
        //        }
        //    }
        //}

        //return null;

        #endregion

        //Retrieve paths from home node to start & destination nodes
        List<GridNode> startPath = ReconstructPath(CameFrom, start);
        List<GridNode> destinationPath = ReconstructPath(CameFrom, goal);

        //Loop through paths and find the leading common path length
        int commonPathCount = 0;
        for (int i = 0; i < Mathf.Min(startPath.Count, destinationPath.Count); i++)
        {
            if (startPath[i] == destinationPath[i]) commonPathCount++;
            else break;
        }

        //Remove common path elements from both paths
        startPath.RemoveRange(0, commonPathCount);
        destinationPath.RemoveRange(0, commonPathCount - 1); //Keep the last common node

        //Reverse start path & combine with destination path
        startPath.Reverse();
        List<GridNode> path = startPath;
        path.AddRange(destinationPath);

        return path;
    }

    //Heuristic used for path finding assessment
    private  float Heuristic(GridNode a, GridNode b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    //Finds the node with the best FScore in OpenList
    private GridNode BestOpenListFScore(GridNode start, GridNode goal)
    {
        int bestIndex = 0;

        for (int i = 0; i < OpenList.Count; i++)
        {
            if ((Heuristic(OpenList[i], start) + Heuristic(OpenList[i], goal)) < (Heuristic(OpenList[bestIndex], start) + Heuristic(OpenList[bestIndex], goal)))
                bestIndex = i;
        }

        GridNode bestNode = OpenList[bestIndex];
        return bestNode;
    }

    //Backtrack along the explored path to reconstruct a path sequence
    public List<GridNode> ReconstructPath(Dictionary<GridNode, GridNode> CF, GridNode current)
    {
        List<GridNode> finalPath = new List<GridNode>();
        finalPath.Add(current);

        while (CF.ContainsKey(current))
        {
            current = CF[current];
            finalPath.Add(current);
        }

        finalPath.Reverse();
        return finalPath;
    }
}
