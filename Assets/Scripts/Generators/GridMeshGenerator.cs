﻿using System;
using UnityEngine;

namespace Generators
{
    public class GridMeshGenerator
    {
        public static Mesh GenerateGrid(int sizeX, int sizeY)
        {
            return GenerateGrid(sizeX, sizeY, null);
        }

        public static Mesh GenerateGrid(int sizeX, int sizeY, int[] cutOut)
        {
            var mesh = RecenterPivot(new Mesh
            {
                name = "Procedural Grid",
                vertices = GenerateVertices(sizeX, sizeY),
                triangles = GenerateTriangles(sizeX, sizeY, cutOut)
            });

            return mesh;
        }

        private static Vector3[] GenerateVertices(int sizeX, int sizeY)
        {
            var vertices = new Vector3[(sizeX + 1) * (sizeY + 1)];

            for (int i = 0, y = 0; y <= sizeY; y++)
            {
                for (int x = 0; x <= sizeX; x++, i++)
                {
                    vertices[i] = new Vector3(x, 0, y);
                }
            }

            return vertices;
        }

        private static int[] GenerateTriangles(int sizeX, int sizeY, int[] cutOut)
        {
            var triangles = new int[sizeX * sizeY * 6];

            for (int ti = 0, vi = 0, y = 0; y < sizeY; y++, vi++)
            {
                for (int x = 0; x < sizeX; x++, ti += 6, vi++)
                {
                    if (Array.IndexOf(cutOut, vi) >= 0) continue;

                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + sizeX + 1;
                    triangles[ti + 5] = vi + sizeX + 2;
                }
            }

            return triangles;
        }

        public static Mesh RecenterPivot(Mesh mesh)
        {
            var difference = -mesh.bounds.extents - mesh.vertices[0];
            var vertices = mesh.vertices;
            
            Debug.Log(difference);
            
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] += difference;
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}