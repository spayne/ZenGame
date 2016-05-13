using UnityEngine;
using System;
using System.Collections.Generic;

public class LevelData : MonoBehaviour
{
    public SphereController sphere_ref;
    public Camera camera_ref;

    public Queue<CellCoord> current_sphere_path;  // 3.3 then list might look like 3,2  3,1  3,0

    private ElementController[,] elements;

    private int num_rows;
    private int num_cols;
    private float min_x = float.MaxValue;
    private float min_y = float.MaxValue;

    private float max_x = float.MinValue;
    private float max_y = float.MinValue;

    private float cell_width;
    private float cell_height;

    // Use this for initialization
    void Start()
    {
        elements = new ElementController[4, 4];
        // find each element and add it to my list
        foreach (Transform child in transform)
        {

            GameObject obj = child.gameObject;
            if (obj.name.Length == 2)
            {
                int row = obj.name[0] - '1';
                int col = obj.name[1] - 'A';
                elements[row, col] = obj.GetComponent<ElementController>();
                num_rows = Math.Max(row + 1, num_rows);
                num_cols = Math.Max(col + 1, num_cols);
                min_x = Math.Min(min_x, child.position.x);
                min_y = Math.Min(min_y, child.position.y);

                max_x = Math.Max(max_x, child.position.x);
                max_y = Math.Max(max_y, child.position.y);
            }
        }
        cell_width = (max_x - min_x) / (num_cols - 1);
        cell_height = (max_y - min_y) / (num_rows - 1);

        sphere_ref.transform.position = CellCoordToVector3(new CellCoord(1, 1));
    }

    CellCoord Local2CellCoord(Vector3 local)
    {
        int col = (int)Math.Round((local.x - min_x) / cell_width);
        int row = (int)Math.Round((-local.y + cell_height) / cell_height);
        return new CellCoord(row, col);
    }

    CellCoord ScreenPositionToCellCoord(Vector3 mouse_position)
    {
        Vector3 local = this.transform.InverseTransformPoint(camera_ref.ScreenToWorldPoint(mouse_position));
        return Local2CellCoord(local);
    }
    CellCoord ChildPositionToCellCoord(Vector3 child_position)
    {
        CellCoord cell_coord = Local2CellCoord(child_position);
        return cell_coord;
    }

    // use the stored position of the elements
    public Vector3 CellCoordToVector3(CellCoord cell_coord)
    {
        ElementController element = ElementControllerAtCellCoord(cell_coord);
        return element.gameObject.transform.position;

    }

    bool CoordInBounds(CellCoord cell_coord)
    {
        return (cell_coord.row >= 0 && cell_coord.row < num_rows &&
                cell_coord.col >= 0 && cell_coord.col < num_cols);
    }
    // local assumed to be relative to the left,top of the container level
    ElementController ElementControllerAtCellCoord(CellCoord cell_coord)
    {
        if (CoordInBounds(cell_coord))
        {
            return elements[cell_coord.row, cell_coord.col];
        }
        else
        {
            return null;
        }
    }

 

    // The following table assumes a unity rotation of 0  

    // given a an object, return a map of possible connection directions
    // N,E,S,W
    [Flags]
    enum Directions
    {
        None = 0,
        West = 1,
        South = 1 << 1,   // 2
        East = 1 << 2,   // 4
        North = 1 << 3,   // 8
        West2 = 1 << 4,
        South2 = 1 << 5,   // 2
        East2 = 1 << 6,   // 4
        North2 = 1 << 7,   // 8
        TConnections = (North | South | West | North2 | South2 | West2),
        IConnections = (North | South | North2 | South2),
        LConnections = (West | South | West2 | South2)
    };

    UInt32 ConnectionDirections(ElementController element)
    {
        char pipe_type = element.pipe_type;
        float rotation = element.transform.eulerAngles.z;
        UInt32 connections = 0;

        if (pipe_type == 't')
        {
            connections = (UInt32)Directions.TConnections;
        }
        else if (pipe_type == 'i')
        {
            connections = (UInt32)Directions.IConnections;
        }
        else if (pipe_type == 'l')
        {
            connections = (UInt32)Directions.LConnections;
        }
        // shift table
        ;
        int num_shifts = ((360 - (int)rotation) % 360) / 90;
        connections >>= num_shifts;
        return connections;
    }

    bool AreConnected(CellCoord a, CellCoord b)
    {
        CellCoord start;
        CellCoord adjacent;
        if (a.col < b.col || a.row < b.row)
        {
            start = a;
            adjacent = b;
        }
        else
        {
            start = b;
            adjacent = a;
        }
            
        if (!CoordInBounds(start) || !CoordInBounds(adjacent))
        {
            return false;
        }
        UInt32 start_connection_points = ConnectionDirections(ElementControllerAtCellCoord(start));
        UInt32 end_connection_points = ConnectionDirections(ElementControllerAtCellCoord(adjacent));

        // on same row, compare east and west
        bool connected;
        if (start.row == adjacent.row)
        {
            connected = ((start_connection_points & (UInt32)Directions.East) != 0 && (end_connection_points & (UInt32)Directions.West) != 0);
        }
        else
        {
            connected = ((start_connection_points & (UInt32)Directions.South) != 0 && (end_connection_points & (UInt32)Directions.North) != 0);
        }
        

        return connected;
    }

    struct DistanceAndParent
    {
        public DistanceAndParent(int d, CellCoord p)
        {
            distance = d;
            parent = p;
        }
        public int distance;
        public CellCoord parent;
    }

    Queue<CellCoord> MakeShortestPath(CellCoord start, CellCoord end)
    {
        // bredth first search
        Queue<CellCoord> open_q = new Queue<CellCoord>();

        
        // node 2 distanceandparentmap cell coord and how I got there
        Dictionary<CellCoord, DistanceAndParent> visited = new Dictionary<CellCoord, DistanceAndParent>();
        open_q.Enqueue(start);
        visited[start] = new DistanceAndParent(0, start);
        CellCoord current = new CellCoord();

        while (open_q.Count > 0)
        {
            current = open_q.Dequeue();

            if (current == end)
                break;
            // otherwise walk each neighbor and add any accessible ones
            int current_distance = visited[current].distance;
            
            //0   0,1
            //0   0,-1
            //1   1, 0
            //1   -1,0
            for (int i = 0; i < 4; i++)
            {
                int drow = 0;
                int dcol = 0;
                switch (i)
                {
                    case 0: drow = 0; dcol = 1; break;
                    case 1: drow = 0; dcol = -1; break;
                    case 2: drow = 1; dcol = 0;break;
                    case 3: drow = -1; dcol = 0; break;

                }
                CellCoord tentative = new CellCoord(current.row + drow, current.col + dcol);
                if (!visited.ContainsKey(tentative) && AreConnected(current, tentative))
                {
                    visited[tentative] = new DistanceAndParent(current_distance+1, current);
                    open_q.Enqueue(tentative);
                }
                
            }
        }

        if (current == end)
        {
            // todo: make path
            Stack<CellCoord> backwards_path = new Stack<CellCoord>();

            backwards_path.Push(end);

            DistanceAndParent cur = visited[end];
            while (cur.distance > 1)
            {
                backwards_path.Push(cur.parent);
                cur = visited[cur.parent];
            }
            
            return new Queue<CellCoord>(backwards_path.ToArray());
        }
        else
        {
            return null;
        }

    }




    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            CellCoord sphere_cell = ChildPositionToCellCoord(sphere_ref.transform.position); // 0,0
            CellCoord clicked_cell = ScreenPositionToCellCoord(Input.mousePosition);        // 3,3
            
            // i'm doing everying in cell space, did the player click the sphere? then rotate
            if (sphere_cell == clicked_cell)
            {
                // clicking on the sphere, rotoates the current element
                ElementController clicked_element = ElementControllerAtCellCoord(clicked_cell);
                clicked_element.transform.Rotate(new Vector3(0, 0, 90));
            }
            else
            {
                // *** otherwise ** try and build a path to the destination
                Queue<CellCoord> new_sphere_path = MakeShortestPath(sphere_cell, clicked_cell);
                if (new_sphere_path != null)
                {
                    // overwrite the current path
                    current_sphere_path = new_sphere_path;
                    
                    
                }
                // build a path to the clicked cell
                // start: sphere_cell
                // end: clicked_cell
            }
        }
    }
}
