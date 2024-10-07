using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Notifications.iOS;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private GridNode[,] Nodes;

    //Maze Randomisation Variables
    private List<GridNode> Unexplored;
    private List<GridNode> Explored;
    private List<GridConnection> PotentialConnections;
    private GridNode StartNode;
    private GridNode DestinationNode;

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

    private void Start()
    {
        Nodes = new GridNode[DimensionX, DimensionY];

        //DEBUG
        //long startTicks = System.DateTime.Now.Ticks;

        GenerateGrid();

        //DEBUG
        //long endTicks = System.DateTime.Now.Ticks;
        //System.TimeSpan span = new System.TimeSpan(endTicks - startTicks);
        //Debug.Log("Generate Grid: " + span.Milliseconds);
        //startTicks = System.DateTime.Now.Ticks;

        RandomiseMaze();

        //DEBUG
        //endTicks = System.DateTime.Now.Ticks;
        //span = new System.TimeSpan(endTicks - startTicks);
        //Debug.Log("Randomise Maze: " + span.Milliseconds);
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

        //Loop through board dimensions & create grid nodes
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

    //Generates a random maze from an initilised grid
    private void RandomiseMaze()
    {
        //DEBUG
        //long startTicks = System.DateTime.Now.Ticks;

        //Initialise lists
        Unexplored = GetNodesAsList();
        Explored = new List<GridNode>();
        PotentialConnections = new List<GridConnection>();

        //Select one node arbitrarily as the first node and explore it
        ExploreNode(Unexplored[Random.Range(0, Unexplored.Count)]);

        //Create connections and explore nodes until all nodes have been explored
        while (Unexplored.Count > 0)
        {
            //Pick a random potential connection
            GridConnection connection = PotentialConnections[Random.Range(0, PotentialConnections.Count)];

            //Enable the chosen connection and explore the connected node
            connection.SetConnected(true);
            ExploreNode(connection.NodeB);
        }

        //DEBUG
        //long endTicks = System.DateTime.Now.Ticks;
        //System.TimeSpan span = new System.TimeSpan(endTicks - startTicks);
        //Debug.Log("Randomise Maze - Randomisation: " + span.Milliseconds);
        //startTicks = System.DateTime.Now.Ticks;

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

        //DEBUG
        //endTicks = System.DateTime.Now.Ticks;
        //span = new System.TimeSpan(endTicks - startTicks);
        //Debug.Log("Randomise Maze - Spawn Start & End: " + span.Milliseconds);
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
    }

    //SPawn the goal at the destination node
    private void SpawnDestination()
    {
        GameObject destination = Instantiate(Destination, transform);
        destination.name = Destination.name;
        destination.transform.position = DestinationNode.DestinationSpawn.transform.position;
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
        //Initialise lists
        OpenList = new List<GridNode>();
        ClosedList = new List<GridNode>();
        CameFrom = new Dictionary<GridNode, GridNode>();

        //Begin
        OpenList.Add(start);

        float gScore = 0;
        float fScore = gScore + Heuristic(start, goal);

        while (OpenList.Count > 0)
        {
            //Find the Node in openList that has the lowest fScore
            GridNode currentNode = BestOpenListFScore(start, goal);

            //Found the end, reconstruct entire path and return
            if (currentNode == goal)
                return ReconstructPath(CameFrom, currentNode);

            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            for (int i = 0; i < currentNode.Connections.Length; i++)
            {
                if (currentNode.Connections[i] == null) continue;
                if (!currentNode.Connections[i].Connected) continue;
                GridNode thisNeighbourNode = currentNode.Connections[i].NodeB;

                //Ignore if neighbour node is attached
                if (!ClosedList.Contains(thisNeighbourNode))
                {
                    //Distance from current to the nextNode
                    float tentativeGScore = Heuristic(start, currentNode) + Heuristic(currentNode, thisNeighbourNode);

                    //Check to see if in openList or if new GScore is more sensible
                    if (!OpenList.Contains(thisNeighbourNode) || tentativeGScore < gScore)
                        OpenList.Add(thisNeighbourNode);

                    //Add to Dictionary - this neighour came from this parent
                    if (!CameFrom.ContainsKey(thisNeighbourNode))
                        CameFrom.Add(thisNeighbourNode, currentNode);

                    gScore = tentativeGScore;
                    fScore = Heuristic(start, thisNeighbourNode) + Heuristic(thisNeighbourNode, goal);
                }
            }
        }

        return null;
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
