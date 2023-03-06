using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Extruder : MonoBehaviour
{
    private struct VerticesInfo
    {
        public List<Vector3> vertices;
        public IList<SharedVertex> sharedVertices;

    }

    static public void Extrude(List<Vector2> shape, Material material)
    {
        List<Face> faces = new List<Face>();
        List<Vertex> vertices = new List<Vertex>();
        VerticesInfo verticesInfo = GenExtrudeVertices(shape);
        for (int i = 0; i < verticesInfo.vertices.Count; i += 3)
        {
            Vector3 v1 = verticesInfo.vertices[i];
            Vector3 v2 = verticesInfo.vertices[i + 1];
            Vector3 v3 = verticesInfo.vertices[i + 2];

            Vector3 side1 = v2 - v1;
            Vector3 side2 = v3 - v1;
            Vector3 normal = Vector3.Cross(side1, side2).normalized;

            Vertex v1WidthNormal = new Vertex();
            v1WidthNormal.position = v1;
            v1WidthNormal.normal = normal;
            vertices.Add(v1WidthNormal);

            Vertex v2WidthNormal = new Vertex();
            v2WidthNormal.position = v2;
            v2WidthNormal.normal = normal;
            vertices.Add(v2WidthNormal);

            Vertex v3WidthNormal = new Vertex();
            v3WidthNormal.position = v3;
            v1WidthNormal.normal = normal;
            vertices.Add(v3WidthNormal);
        }

        for (int i = 0; i < vertices.Count; i += 3)
        {
            Face mesh2Face = new Face(new List<int>() { i, i + 1, i + 2 });
            faces.Add(mesh2Face);
        }

        ProBuilderMesh mesh = ProBuilderMesh.Create(vertices, faces, verticesInfo.sharedVertices, new List<SharedVertex>());
        mesh.SetMaterial(mesh.faces, material);
        mesh.ToMesh();
    }


    static private VerticesInfo GenExtrudeVertices(List<Vector2> vertices)
    {
        VerticesInfo res;
        List<float> verticesCoords = new List<float>();
        for (int i = 0; i < vertices.Count; i++)
        {
            verticesCoords.Add(vertices[i].x);
            verticesCoords.Add(vertices[i].y);
        }

        List<int> faceIndexes = EarcutLibrary.Earcut(verticesCoords, new List<int>(), 2);
        List<Vector3> vecs = new List<Vector3>();
        List<Face> faces = new List<Face>();

        for (int i = 0; i < verticesCoords.Count; i += 2)
        {
            float x = verticesCoords[i];
            float z = verticesCoords[i + 1];

            Vector3 vec3 = new Vector3(x, 0, z);
            vecs.Add(vec3);
        }

        for (int i = 0; i < faceIndexes.Count; i += 3)
        {
            List<int> indices = new List<int>()
            {
                faceIndexes[i],
                faceIndexes[i + 1],
                faceIndexes[i + 2],
            };
            Face face = new Face(indices);
            faces.Add(face);
        }

        ProBuilderMesh mesh = ProBuilderMesh.Create(vecs, faces);
        ExtrudeElements.Extrude(mesh, faces, ExtrudeMethod.FaceNormal, 10);

        res.sharedVertices = mesh.sharedVertices;
        res.vertices = new List<Vector3>();

        for (int i = 0; i < mesh.faces.Count; i++)
        {
            Face face = mesh.faces[i];
            int[] indexes = new int[face.indexes.Count];
            face.indexes.CopyTo(indexes, 0);
            for (int j = 0; j < indexes.Length - 1; j += 3)
            {
                List<int> triangleVerticesIndexes = new List<int>() { indexes[j], indexes[j + 1], indexes[j + 2] };
                Vertex[] triangleVertices = mesh.GetVertices(triangleVerticesIndexes);
                Vector3 v1 = triangleVertices[0].position;
                if (v1.y != 0)
                {
                    v1.y = 10;
                }
                Vector3 v2 = triangleVertices[1].position;
                if (v2.y != 0)
                {
                    v2.y = 10;
                }
                Vector3 v3 = triangleVertices[2].position;
                if (v3.y != 0)
                {
                    v3.y = 10;
                }
                res.vertices.Add(v1);
                res.vertices.Add(v2);
                res.vertices.Add(v3);
            }
        }

        mesh.name = "__TEMP_OBJ__";
        GameObject o = GameObject.Find("__TEMP_OBJ__");
        Destroy(o);
        return res;
    }
}
