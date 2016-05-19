using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour
{
    public struct Triangle
    {
        public int VertexIndexA;
        public int VertexIndexB;
        public int VertexIndexC;

        private int[] Vertices;

        public Triangle(int vertexIndexA, int vertexIndexB, int vertexIndexC) : this()
        {
            VertexIndexA = vertexIndexA;
            VertexIndexB = vertexIndexB;
            VertexIndexC = vertexIndexC;

            Vertices = new[] {vertexIndexA, vertexIndexB, vertexIndexC};
        }

        public int this[int i]
        {
            get { return Vertices[i]; }
        }

        public bool Contains(int vertexIndex)
        {
            return vertexIndex == VertexIndexA
                   || vertexIndex == VertexIndexB
                   || vertexIndex == VertexIndexC;
        }
    }

    public MeshFilter Walls;
    public SquareGrid grid;
    private List<Vector3> vertices;
    private List<int> triangles; 


    Dictionary<int,List<Triangle>> trianglesDictionary = new Dictionary<int, List<Triangle>>(); 
    List<List<int>>  outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>(); 

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
                MeshFromPoints(square.CenterLeft,square.CenterBottom,square.BottomLeft);
                break;
            case 2:
                MeshFromPoints(square.BottomRight, square.CenterBottom, square.CenterRight);
                break;
            case 4:
                MeshFromPoints(square.TopRight, square.CenterRight, square.CenterTop);
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
                // optime !
                checkedVertices.Add(square.TopLeft.VertexIndex);
                checkedVertices.Add(square.TopRight.VertexIndex);
                checkedVertices.Add(square.BottomRight.VertexIndex);
                checkedVertices.Add(square.BottomLeft.VertexIndex);
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

        Triangle triangle = new Triangle(a.VertexIndex, b.VertexIndex,c.VertexIndex);
        AddTriangleToDictionary(triangle.VertexIndexA,triangle);
        AddTriangleToDictionary(triangle.VertexIndexB,triangle);
        AddTriangleToDictionary(triangle.VertexIndexC,triangle);
        
    }

    void AddTriangleToDictionary(int vertex, Triangle triangle)
    {
        if (!trianglesDictionary.ContainsKey(vertex))
        {
            trianglesDictionary.Add(vertex, new List<Triangle>());
        }
        trianglesDictionary[vertex].Add(triangle);
        
    }

    void CalculateMeshOutlines()
    {
        for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
        {
            if (!checkedVertices.Contains(vertexIndex))
            {
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if (newOutlineVertex != -1)
                {
                    checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);
                    FollowOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count-1].Add(vertexIndex);
                }
            }
        }
    }

    private void FollowOutline(int vertexIndex, int outlineIndex)
    {
       outlines[outlineIndex].Add(vertexIndex);
       checkedVertices.Add(vertexIndex);

        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);
        if (nextVertexIndex != -1)
        {
            FollowOutline(nextVertexIndex, outlineIndex);
        }
    }

    int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = trianglesDictionary[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            var triangle = trianglesContainingVertex[i];

            for (int j = 0; j < 3; j++)
            {
                int vertexB = triangle[j];
                if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB))
                {
                    if (IsOutlineEdge(vertexIndex, vertexB))
                    {
                        return vertexB;
                    }
                }
            }
        }

        return -1;
    }

    bool IsOutlineEdge(int vertexA, int vertexB)
    {
        List<Triangle> trianglesContainingVertexA = trianglesDictionary[vertexA];
        int sharedTriangleCount = 0;
        for (int i = 0; i < trianglesContainingVertexA.Count; i++)
        {
            if (trianglesContainingVertexA[i].Contains(vertexB))
            {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1)
                {
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;
    }

    public void GenerateMesh(int[,] map, float squareSize)
    {
        trianglesDictionary.Clear();
        outlines.Clear();
        checkedVertices.Clear();

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

        CreateWallMesh();
    }

    private void CreateWallMesh()
    {
        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 5;

        foreach (var outline in outlines)
        {
            for (int i = 0; i < outline.Count - 1; i++)
            {
                int startIndex = wallVertices.Count;
                wallVertices.Add(vertices[outline[i]]); // left
                wallVertices.Add(vertices[outline[i+1]]); // right
                wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
                wallVertices.Add(vertices[outline[i+1]] - Vector3.up * wallHeight); // bottom right

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);

            }
        }

        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();

        Walls.mesh = wallMesh;
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
