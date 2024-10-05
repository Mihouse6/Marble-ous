using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        GenerateGrid();

        RandomiseMaze();

        SpawnMarble();

        SpawnDestination();
    }

    //Instantiates and arranges GridNode objects to fill the defined grid dimensions
    private void GenerateGrid()
    {
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
    }

    //Generates a random maze from an initilised grid
    private void RandomiseMaze()
    {
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

    //Randomly pick a start node and spawn the marble
    private void SpawnMarble()
    {
        //Select random node as start
        List<GridNode> nodesList = GetNodesAsList();
        nodesList.Remove(StartNode);
        StartNode = nodesList[Random.Range(0, nodesList.Count)];

        //Spawn the marble
        GameObject marble = Instantiate(Marble, transform);
        marble.name = Marble.name;
        marble.transform.position = StartNode.MarbleSpawn.transform.position;
    }

    //Randomly pick a destination node and spawn the destination
    private void SpawnDestination()
    {
        //Select random node as destination
        List<GridNode> nodesList = GetNodesAsList();
        DestinationNode = nodesList[Random.Range(0, nodesList.Count)];

        //Spawn the destination
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
}
