using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Mesh : MonoBehaviour
{
    // Start is called before the first frame update
    public Material material;

    void Start()
    {
        List<float> vertices = new List<float>()
        {
            0f, 0f,
           5f, 0f,
           7f, 2f,
           9f, 5f,
           5f, 7f,
           2f, 20f,
           20f, 100f,
           -20f, 100f,
           -50f, -300f
        };
        List<int> faceIndexes = EarcutLibrary.Earcut(vertices, new List<int>(), 2);
        List<Vector3> vecs = new List<Vector3>();
        List<Face> faces = new List<Face>();

        for (int i = 0; i < vertices.Count; i += 2)
        {
            float x = vertices[i];
            float z = vertices[i + 1];

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
        IList<Face> rebuildFaces = mesh.faces;
        List<Vertex> verticesWithNormal = new List<Vertex>();
        IList<SharedVertex> sharedVertices =  mesh.sharedVertices;
        List<Face> mesh2Faces = new List<Face>();

        for (int i = 0; i < rebuildFaces.Count; i++)
        {
            Face face = rebuildFaces[i];
            int[] indexes = new int[face.indexes.Count];
            face.indexes.CopyTo(indexes, 0);
            for (int j = 0; j < indexes.Length-1; j += 3)
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

                Vector3 side1 = v2 - v1;
                Vector3 side2 = v3 - v1;
                Vector3 normal = Vector3.Cross(side1, side2).normalized;

                Vertex v1WidthNormal = new Vertex();
                v1WidthNormal.position = v1;
                v1WidthNormal.normal = normal;
                verticesWithNormal.Add(v1WidthNormal);

                Vertex v2WidthNormal = new Vertex();
                v2WidthNormal.position = v2;
                v2WidthNormal.normal = normal;
                verticesWithNormal.Add(v2WidthNormal);

                Vertex v3WidthNormal = new Vertex();
                v3WidthNormal.position = v3;
                v1WidthNormal.normal = normal;
                verticesWithNormal.Add(v3WidthNormal);
            }

        }
        for (int i = 0; i < verticesWithNormal.Count; i+=3)
        {
            Face mesh2Face = new Face(new List<int>() { i, i+1, i+2 });
            mesh2Faces.Add(mesh2Face);
        }
        
        ProBuilderMesh mesh2 = ProBuilderMesh.Create(verticesWithNormal, mesh2Faces, sharedVertices, new List<SharedVertex>());
        mesh2.SetMaterial(mesh2.faces, material);
        mesh2.ToMesh();
        mesh.name = "23123";
        GameObject o = GameObject.Find("23123");


        Destroy(o);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
