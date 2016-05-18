using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour
{

    public SquareGrid grid;
    private List<Vector3> vertices;
    private List<int> triangles;

    //void OnDrawGizmos()
    //{
    //    if (grid != null)
    //    {

    //        for (int x = 0; x < grid.Squares.GetLength(0); x++)
    //        {
    //            for (int y = 0; y < grid.Squares.GetLength(1); y++)
    //            {
    //                Gizmos.color = grid.Squares[x, y].TopLeft.Active ? Color.black : Color.white;
    //                Gizmos.DrawCube(grid.Squares[x, y].TopLeft.Position, Vector3.one * .4f);

    //                Gizmos.color = grid.Squares[x, y].TopRight.Active ? Color.black : Color.white;
    //                Gizmos.DrawCube(grid.Squares[x, y].TopRight.Position, Vector3.one * .4f);

    //                Gizmos.color = grid.Squares[x, y].BottomLeft.Active ? Color.black : Color.white;
    //                Gizmos.DrawCube(grid.Squares[x, y].BottomLeft.Position, Vector3.one * .4f);

    //                Gizmos.color = grid.Squares[x, y].BottomRight.Active ? Color.black : Color.white;
    //                Gizmos.DrawCube(grid.Squares[x, y].BottomRight.Position, Vector3.one * .4f);

    //                Gizmos.color = Color.grey;
    //                Gizmos.DrawCube(grid.Squares[x, y].CenterTop.Position, Vector3.one * .15f);
    //                Gizmos.DrawCube(grid.Squares[x, y].CenterRight.Position, Vector3.one * .15f);
    //                Gizmos.DrawCube(grid.Squares[x, y].CenterBottom.Position, Vector3.one * .15f);
    //                Gizmos.DrawCube(grid.Squares[x, y].CenterLeft.Position, Vector3.one * .15f);

    //            }
    //        }
    //    }
    //}

    void TriangulateSquare(Square square)
    {
        switch (square.Configuration)
        {
            case 0: break;

                // 1 point
            case 1:
                MeshFromPoints(square.CenterBottom,square.BottomLeft,square.CenterLeft);
                break;
            case 2:
                MeshFromPoints(square.CenterRight, square.BottomRight, square.CenterBottom);
                break;
            case 4:
                MeshFromPoints(square.CenterTop, square.TopRight, square.CenterRight);
                break;
            case 8:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterLeft);
                break;

            // 2 points
            case 3:
                MeshFromPoints(square.CenterRight, square.BottomRight, square.BottomLeft, square.CenterLeft);
                break;
            case 6:
                MeshFromPoints(square.CenterTop, square.TopRight, square.BottomRight, square.CenterBottom);
                break;
            case 9:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterBottom, square.BottomLeft);
                break;
            case 12:
                MeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterLeft);
                break;

            case 5:
                MeshFromPoints(square.CenterTop, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft, square.CenterLeft);
                break;

            case 10:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight, square.CenterBottom, square.CenterLeft);
                break;

            //3points
            case 7:
                MeshFromPoints(square.CenterTop, square.TopRight, square.BottomRight, square.BottomLeft, square.CenterLeft);
                break;
            case 11:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight, square.BottomLeft);
                break;
            case 13:
                MeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft);
                break;
            case 14:
                MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.CenterBottom, square.CenterLeft);
                break;

            //4points
            case 15:
                MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
                break;
        }
    }

    void MeshFromPoints(params Node[] points )
    {
        AssignVertices(points);
        if (points.Length >= 3)
        {
            CreateTriangle(points[0],points[1],points[2]);
        }
        if (points.Length >= 4)
        {
            CreateTriangle(points[0], points[2], points[3]);
        }
        if (points.Length >= 5)
        {
            CreateTriangle(points[0], points[3], points[4]);
        }
        if (points.Length >= 6)
        {
            CreateTriangle(points[0], points[4], points[5]);
        }

    }

    private void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].VertexIndex == -1)
            {
                points[i].VertexIndex = vertices.Count;
                vertices.Add(points[i].Position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.VertexIndex);
        triangles.Add(b.VertexIndex);
        triangles.Add(c.VertexIndex);
    }

    public void GenerateMesh(int[,] map, float squareSize)
    {
        grid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < grid.Squares.GetLength(0); x++)
        {
            for (int y = 0; y < grid.Squares.GetLength(1); y++)
            {
                TriangulateSquare(grid.Squares[x,y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    public class SquareGrid
    {
        public Square[,] Squares;


        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);

            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(
                                    -mapWidth / 2 + x * squareSize + squareSize / 2,
                                    0,
                                    -mapHeight / 2 + y * squareSize + squareSize / 2);

                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            Squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    Squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }
        }
    }

    public class Square
    {
        public ControlNode TopLeft, TopRight, BottomRight, BottomLeft;
        public Node CenterTop, CenterRight, CenterBottom, CenterLeft;
        public int Configuration;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;

            CenterTop = topLeft.Right;
            CenterRight = bottomRight.Above;
            CenterBottom = bottomLeft.Right;
            CenterLeft = bottomLeft.Above;

            if (topLeft.Active) Configuration += 8;
            if (topRight.Active) Configuration += 4;
            if (bottomRight.Active) Configuration += 2;
            if (bottomLeft.Active) Configuration += 1;
        }
    }

    public class Node
    {
        public Vector3 Position;
        public int VertexIndex = -1;

        public Node(Vector3 position)
        {
            Position = position;
        }
    }

    public class ControlNode : Node
    {
        public bool Active;

        public Node Above, Right;

        public ControlNode(Vector3 position, bool active, float squareSize) : base(position)
        {
            Active = active;
            Above = new Node(position + Vector3.forward * squareSize / 2f);
            Right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }
}
