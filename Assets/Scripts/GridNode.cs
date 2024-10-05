using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public enum Direction
    {
        North = 0,
        East,
        South,
        West
    }

    [HideInInspector] public int GridPosX;
    [HideInInspector] public int GridPosY;
    [HideInInspector] public GridNode[] AdjacentNodes; //Arranged [north, east, south, west]
    [HideInInspector] public bool[] AdjacentNodesConnected; //Arranged [north, east, south, west]

    [HideInInspector] public GridConnection[] Connections; //Arranged [north, east, south, west]

    [Header("Child Object References")]
    [SerializeField] private GameObject NorthWall;
    [SerializeField] private GameObject EastWall;
    [SerializeField] private GameObject SouthWall;
    [SerializeField] private GameObject WestWall;
    public GameObject MarbleSpawn;
    public GameObject DestinationSpawn;

    //Get or Set the north adjacent Node
    public GridNode North
    {
        get => Connections[0].NodeB;
        set => Connections[0].NodeB = value;
    }

    //Get or Set the east adjacent Node
    public GridNode East
    {
        get => Connections[1].NodeB;
        set => Connections[1].NodeB = value;
    }

    //Get or Set the south adjacent Node
    public GridNode South
    {
        get => Connections[2].NodeB;
        set => Connections[2].NodeB = value;
    }

    //Get or Set the west adjacent Node
    public GridNode West
    {
        get => Connections[3].NodeB;
        set => Connections[3].NodeB = value;
    }

    private void Awake()
    {
        AdjacentNodes = new GridNode[4];
        AdjacentNodesConnected = new bool[] { false, false, false, false };

        Connections = new GridConnection[4];
        Connections[0] = new GridConnection(this, null);
        Connections[1] = new GridConnection(this, null);
        Connections[2] = new GridConnection(this, null);
        Connections[3] = new GridConnection(this, null);
    }

    private void Update()
    {
        //DEBUG
        for (int i = 0; i < Connections.Length; i++)
        {
            if (Connections[i].NodeB == null) continue;

            if (Connections[i].Connected == true)
                Debug.DrawLine(transform.position, Connections[i].NodeB.transform.position, Color.green);
            else if (Connections[i].Connected == false)
                Debug.DrawLine(transform.position, Connections[i].NodeB.transform.position, Color.red);
        }
        //DEBUG
    }

    //Connect to the specified node and remove the interposing wall
    public void Connect(GridNode B)
    {
        if (B == North)
        {
            NorthWall.SetActive(false);
            if (B.Connections[(int)Direction.South].Connected == false)
                B.Connections[(int)Direction.South].SetConnected(true);
        }
        else if (B == East)
        {
            EastWall.SetActive(false);
            if (B.Connections[(int)Direction.West].Connected == false)
                B.Connections[(int)Direction.West].SetConnected(true);
        }
        else if (B == South)
        {
            SouthWall.SetActive(false);
            if (B.Connections[(int)Direction.North].Connected == false)
                B.Connections[(int)Direction.North].SetConnected(true);
        }
        else if (B == West)
        {
            WestWall.SetActive(false);
            if (B.Connections[(int)Direction.East].Connected == false)
                B.Connections[(int)Direction.East].SetConnected(true);
        }
    }

    //Disconnect from the specified node and add the interposing wall
    public void Disconnect(GridNode B)
    {
        if (B == North)
        {
            NorthWall.SetActive(true);
            if (B.Connections[(int)Direction.South].Connected == true)
                B.Connections[(int)Direction.South].SetConnected(false);
        }
        else if (B == East)
        {
            EastWall.SetActive(true);
            if (B.Connections[(int)Direction.West].Connected == true)
                B.Connections[(int)Direction.West].SetConnected(false);
        }
        else if (B == South)
        {
            SouthWall.SetActive(true);
            if (B.Connections[(int)Direction.North].Connected == true)
                B.Connections[(int)Direction.North].SetConnected(false);
        }
        else if (B == West)
        {
            WestWall.SetActive(true);
            if (B.Connections[(int)Direction.East].Connected == true)
                B.Connections[(int)Direction.East].SetConnected(false);
        }
    }
}

public class GridConnection
{
    public GridNode NodeA;
    public GridNode NodeB;

    public bool Connected;

    public GridConnection(GridNode A, GridNode B)
    {
        NodeA = A;
        NodeB = B;

        Connected = false;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as  GridConnection);
    }

    public bool Equals(GridConnection obj)
    {
        return (NodeA == obj.NodeA && NodeB == obj.NodeB) || //Same nodes and direction
           (NodeA == obj.NodeB && NodeB == obj.NodeA); //Same nodes, opposite direction
    }

    //Sets the connection between nodes A and B
    public void SetConnected(bool value)
    {
        if (value == true)
        {
            Connected = true;
            NodeA.Connect(NodeB);
        }
        else if (value == false)
        {
            Connected = false;
            NodeA.Disconnect(NodeB);
        }
    }
}
