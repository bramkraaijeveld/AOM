using UnityEngine;
using System;

public abstract class Grid {
    protected Material material;
    protected int size;
    protected int partitionSize;
    protected int partitions;

    public int Size { get { return size; } }

    public Grid(int size, int partitionSize, Material material) {
        this.size = size;
        this.partitionSize = partitionSize;
        this.material = material;

        if (size % partitionSize != 0) throw new ArgumentException("Size must be Divisible by PartitionSize", "partitionSize");
        partitions = size / partitionSize;
    }

    public GameObject GameObject(string name = "Grid") {
        GameObject root = new GameObject(name);

        Mesh[,] meshes = Mesh();

        for (int x = 0; x < meshes.GetLength(0); x++) {
            for (int y = 0; y < meshes.GetLength(1); y++) {
                GameObject partition = new GameObject(name + "[" + x + ", " + y + "]");
                partition.AddComponent<MeshFilter>().mesh = meshes[x, y];
                partition.AddComponent<MeshRenderer>().material = material;
                partition.transform.parent = root.transform;
            }
        }

        return root;
    }

    public abstract Mesh[,] Mesh();
}

public class SquareGrid : Grid {

    public SquareGrid(int size, int partitionSize, Material material) : base(size, partitionSize, material) { }

    public override Mesh[,] Mesh() {
        Mesh[,] meshes = new Mesh[partitions, partitions];

        for (int i = 0; i < partitions; i++) {
            for (int j = 0; j < partitions; j++) {

                Vector3[] vertices = new Vector3[partitionSize * partitionSize * 6];
                Vector2[] uv = new Vector2[vertices.Length];
                Vector2[] uv2 = new Vector2[vertices.Length];
                int[] triangles = new int[vertices.Length];

                Func<int, int, Vector3> Coord = (x, y) => new Vector3(i * partitionSize + x, 0, j * partitionSize + y);
                Func<int, int, Vector2> UVCoord = (x, y) => new Vector2(i * partitionSize + x, j * partitionSize + y) / size;
                Func<Vector2, Vector2, Vector2, Vector2> UVCoordMean = (a, b, c) => new Vector2((a.x + b.x + c.x) / 3, (a.y + b.y + c.y) / 3);

                int n = 0;
                for (int x = 0; x < partitionSize; x++) {
                    for (int y = 0; y < partitionSize; y++) {

                        vertices[n + 0] = Coord(x, y);
                        vertices[n + 1] = Coord(x, y + 1);
                        vertices[n + 2] = Coord(x + 1, y + 1);

                        vertices[n + 3] = Coord(x, y);
                        vertices[n + 4] = Coord(x + 1, y + 1);
                        vertices[n + 5] = Coord(x + 1, y);

                        uv[n + 0] = UVCoord(x, y);
                        uv[n + 1] = UVCoord(x, y + 1);
                        uv[n + 2] = UVCoord(x + 1, y + 1);

                        uv[n + 3] = UVCoord(x, y);
                        uv[n + 4] = UVCoord(x + 1, y + 1);
                        uv[n + 5] = UVCoord(x + 1, y);

                        Vector2[] UVMean = new Vector2[] { UVCoordMean(uv[n + 0], uv[n + 1], uv[n + 2]), UVCoordMean(uv[n + 3], uv[n + 4], uv[n + 5]) };

                        uv2[n + 0] = UVMean[0]; uv2[n + 1] = UVMean[0]; uv2[n + 2] = UVMean[0];
                        uv2[n + 3] = UVMean[1]; uv2[n + 4] = UVMean[1]; uv2[n + 5] = UVMean[1];

                        n += 6;
                    }
                }

                for (int t = 0; t < triangles.Length; t++) triangles[t] = t;

                meshes[i, j] = new Mesh();
                meshes[i, j].vertices = vertices;
                meshes[i, j].triangles = triangles;
                meshes[i, j].uv = uv;
                meshes[i, j].uv2 = uv2;
            }
        }

        return meshes;
    }
}

public class HexagonGrid : Grid {

    public HexagonGrid(int size, int partitionSize, Material material) : base(size, partitionSize, material) { }

    public override Mesh[,] Mesh() {
        Mesh[,] meshes = new Mesh[partitions, partitions];

        for (int i = 0; i < partitions; i++) {
            for (int j = 0; j < partitions; j++) {

                Vector3[] vertices = new Vector3[partitionSize * partitionSize * 6];
                Vector2[] uv = new Vector2[vertices.Length];
                Vector2[] uv2 = new Vector2[vertices.Length];
                int[] triangles = new int[vertices.Length];

                Func<float, float, Vector3> Coord = (x, y) => new Vector3(i * partitionSize + x, 0, j * partitionSize + y);
                Func<float, float, Vector2> UVCoord = (x, y) => new Vector2(i * partitionSize + x, j * partitionSize + y) / size;
                Func<Vector2, Vector2, Vector2, Vector2> UVCoordMean = (a, b, c) => new Vector2((a.x + b.x + c.x) / 3, (a.y + b.y + c.y) / 3);

                int n = 0;
                for (int x = 0; x < partitionSize; x++) {
                    for (int y = 0; y < partitionSize; y++) {

                        if (x % 2 == 0) {
                            vertices[n + 0] = Coord(x, y + 0.5f);
                            vertices[n + 1] = Coord(x, y + 1.5f);
                            vertices[n + 2] = Coord(x + 1, y + 1);

                            vertices[n + 3] = Coord(x, y + 0.5f);
                            vertices[n + 4] = Coord(x + 1, y + 1);
                            vertices[n + 5] = Coord(x + 1, y);

                            uv[n + 0] = UVCoord(x, y + 0.5f);
                            uv[n + 1] = UVCoord(x, y + 1.5f);
                            uv[n + 2] = UVCoord(x + 1, y + 1);

                            uv[n + 3] = UVCoord(x, y + 0.5f);
                            uv[n + 4] = UVCoord(x + 1, y + 1);
                            uv[n + 5] = UVCoord(x + 1, y);
                        }
                        else {
                            vertices[n + 0] = Coord(x, y);
                            vertices[n + 1] = Coord(x, y + 1);
                            vertices[n + 2] = Coord(x + 1, y + 0.5f);

                            vertices[n + 3] = Coord(x + 1, y + 0.5f);
                            vertices[n + 4] = Coord(x, y + 1);
                            vertices[n + 5] = Coord(x + 1, y + 1.5f);

                            uv[n + 0] = UVCoord(x, y);
                            uv[n + 1] = UVCoord(x, y + 1);
                            uv[n + 2] = UVCoord(x + 1, y + 0.5f);

                            uv[n + 3] = UVCoord(x + 1, y + 0.5f);
                            uv[n + 4] = UVCoord(x, y + 1);
                            uv[n + 5] = UVCoord(x + 1, y + 1.5f);
                        }

                        Vector2[] UVMean = new Vector2[] { UVCoordMean(uv[n + 0], uv[n + 1], uv[n + 2]), UVCoordMean(uv[n + 3], uv[n + 4], uv[n + 5]) };

                        uv2[n + 0] = UVMean[0]; uv2[n + 1] = UVMean[0]; uv2[n + 2] = UVMean[0];
                        uv2[n + 3] = UVMean[1]; uv2[n + 4] = UVMean[1]; uv2[n + 5] = UVMean[1];

                        n += 6;
                    }
                }

                for (int t = 0; t < triangles.Length; t++) triangles[t] = t;

                meshes[i, j] = new Mesh();
                meshes[i, j].vertices = vertices;
                meshes[i, j].triangles = triangles;
                meshes[i, j].uv = uv;
                meshes[i, j].uv2 = uv2;
            }
        }

        return meshes;
    }
}