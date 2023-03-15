using System.Collections;
using System.Collections.Generic;
using System;
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
    private struct ExtrudeByEarCuttedVec2Result
    {
        public List<Vector3> vertices;
        public ProBuilderMesh mesh;

    }

    static public void ExtrudeMesh(GameObject parent, List<Vector2> shape, float height, Material material)
    {
        List<Face> faces = new List<Face>();
        List<Vertex> vertices = new List<Vertex>();
        VerticesInfo verticesInfo = GenExtrudeMeshVertices(shape, height);
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

        string uuid = Guid.NewGuid().ToString();
        string name = "__EXTRUDED_MESH__" + uuid;
        mesh.name = name;
        GameObject extrudedMesh = GameObject.Find(name);
        extrudedMesh.transform.parent = parent.transform;
    }

    static private VerticesInfo GenExtrudeMeshVertices(List<Vector2> vertices, float height)
    {
        VerticesInfo res;
        List<float> verticesCoords = new List<float>();
        for (int i = 0; i < vertices.Count; i++)
        {
            verticesCoords.Add(vertices[i].x);
            verticesCoords.Add(vertices[i].y);
        }

        List<int> faceIndexes = EarcutLibrary.Earcut(verticesCoords, new List<int>(), 2);
        ExtrudeByEarCuttedVec2Result extrudeByEarCuttedVec2Result = ExtrudeByEarCuttedVec2(faceIndexes, verticesCoords, height);
        res.vertices = extrudeByEarCuttedVec2Result.vertices;
        res.sharedVertices = extrudeByEarCuttedVec2Result.mesh.sharedVertices;

        return res;
    }

    static public void FatLine(GameObject parent, List<Vector3> shape, float width, Material material)
    {
        Vector3[] fatLineVec3 = GenFatLineVertices(shape, width);
        List<float> earcutVertices = new List<float>();
        for (int i = 0; i < fatLineVec3.Length; i++)
        {
            var vec3 = fatLineVec3[i];
            earcutVertices.Add(vec3.x);
            earcutVertices.Add(vec3.z);
        }
        List<int> faceIndexes = EarcutLibrary.Earcut(earcutVertices, new List<int>(), 2);
        List<Face> faces = new List<Face>();
        List<Vertex> vertices = new List<Vertex>();
        for (int i = 0; i < faceIndexes.Count; i += 3)
        {
            Vector3 v1 = fatLineVec3[faceIndexes[i]];
            Vector3 v2 = fatLineVec3[faceIndexes[i + 1]];
            Vector3 v3 = fatLineVec3[faceIndexes[i + 2]];

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

            Face face = new Face(new List<int>() { i, i + 1, i + 2 });
            faces.Add(face);
        }

        ProBuilderMesh mesh = ProBuilderMesh.Create(vertices, faces, new List<SharedVertex>(), new List<SharedVertex>());
        mesh.SetMaterial(mesh.faces, material);
        mesh.ToMesh();

        string uuid = Guid.NewGuid().ToString();
        string name = "__FAT_LINE__" + uuid;
        mesh.name = name;
        GameObject fatLine = GameObject.Find(name);
        fatLine.transform.parent = parent.transform;
    }

    static public GameObject ExtrudeLine(GameObject parent, List<Vector3> shape, float height, float width, Material material)
    {
        Vector3[] fatLineVec3 = GenFatLineVertices(shape, width);
        List<float> earcutVertices = new List<float>();
        for (int i = 0; i < fatLineVec3.Length; i++)
        {
            var vec3 = fatLineVec3[i];
            earcutVertices.Add(vec3.x);
            earcutVertices.Add(vec3.z);
        }
        List<int> faceIndexes = EarcutLibrary.Earcut(earcutVertices, new List<int>(), 2);
        ExtrudeByEarCuttedVec2Result extrudeByEarCuttedVec2Result = ExtrudeByEarCuttedVec2(faceIndexes, earcutVertices, height);

        List<Face> faces = new List<Face>();
        List<Vertex> vertices = new List<Vertex>();
        for (int i = 0; i < extrudeByEarCuttedVec2Result.vertices.Count; i += 3)
        {
            Vector3 v1 = extrudeByEarCuttedVec2Result.vertices[i];
            Vector3 v2 = extrudeByEarCuttedVec2Result.vertices[i + 1];
            Vector3 v3 = extrudeByEarCuttedVec2Result.vertices[i + 2];

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

        ProBuilderMesh mesh = ProBuilderMesh.Create(vertices, faces, extrudeByEarCuttedVec2Result.mesh.sharedVertices, new List<SharedVertex>());
        mesh.SetMaterial(mesh.faces, material);
        mesh.ToMesh();

        string uuid = Guid.NewGuid().ToString();
        string name = "__EXTRUDED_LINE__" + uuid;
        mesh.name = name;
        GameObject extrudedLine = GameObject.Find(name);
        extrudedLine.transform.parent = parent.transform;

        return extrudedLine;
    }

    // transplanted from JavaScript code: https://github.com/HDhuangdi/threejs-demos/blob/master/src/examples/fatLine/index.js
    static private Vector3[] GenFatLineVertices(List<Vector3> vertices, float width)
    {
        Matrix4x4 clockwise90 = Matrix4x4.identity;
        clockwise90.SetTRS(new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0), new Vector3(1, 1, 1));
        Matrix4x4 antiClockwise90 = Matrix4x4.identity;
        antiClockwise90.SetTRS(new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0), new Vector3(1, 1, 1));
        float raduis = width / 2;

        int len = vertices.Count;
        var downPoints = new Vector3[len];
        var upPoints = new Vector3[len];

        for (var i = 0; i < len; i++)
        {
            var cur = vertices[i];
            var prev = new Vector3();
            var next = new Vector3();
            if (i == 0)
            {
                prev = cur;
                next = vertices[i + 1];
            }
            else if (i == len - 1)
            {
                next = cur;
                prev = vertices[i - 1];
            } else
            {
                prev = vertices[i - 1];
                next = vertices[i + 1];
            }

            var dir1 = (cur - prev).normalized;
            var dir2 = (next - cur).normalized;
            var avg = (dir1 + dir2).normalized;

            var rotatedAvg1 = clockwise90.MultiplyVector(avg);
            var rotatedAvg2 = antiClockwise90.MultiplyVector(avg);
            var ray1 = new Ray(cur, rotatedAvg1);
            var ray2 = new Ray(cur, rotatedAvg2);
            var cosA = Vector3.Dot(dir1, rotatedAvg1);
            var sinA = Mathf.Sqrt(1 - Mathf.Pow(cosA, 2));
            var dist = raduis / sinA;
            downPoints[i] = ray1.GetPoint(dist);
            upPoints[i] = ray2.GetPoint(dist);
        }
        Array.Reverse(downPoints);
        var res = new Vector3[downPoints.Length + upPoints.Length];
        upPoints.CopyTo(res, 0);
        downPoints.CopyTo(res, upPoints.Length);

        return res;
    }

    static private ExtrudeByEarCuttedVec2Result ExtrudeByEarCuttedVec2(List<int> earcuttedResult, List<float> vertices, float height)
    {
        ExtrudeByEarCuttedVec2Result res;
        List<Vector3> vecs = new List<Vector3>();
        List<Face> faces = new List<Face>();

        for (int i = 0; i < vertices.Count; i += 2)
        {
            float x = vertices[i];
            float z = vertices[i + 1];

            Vector3 vec3 = new Vector3(x, 0, z);
            vecs.Add(vec3);
        }

        for (int i = 0; i < earcuttedResult.Count; i += 3)
        {
            List<int> indices = new List<int>()
            {
                earcuttedResult[i],
                earcuttedResult[i + 1],
                earcuttedResult[i + 2],
            };
            Face face = new Face(indices);
            faces.Add(face);
        }

        res.mesh = ProBuilderMesh.Create(vecs, faces);
        ExtrudeElements.Extrude(res.mesh, faces, ExtrudeMethod.FaceNormal, height);

        res.vertices = new List<Vector3>();

        for (int i = 0; i < res.mesh.faces.Count; i++)
        {
            Face face = res.mesh.faces[i];
            int[] indexes = new int[face.indexes.Count];
            face.indexes.CopyTo(indexes, 0);
            for (int j = 0; j < indexes.Length - 1; j += 3)
            {
                List<int> triangleVerticesIndexes = new List<int>() { indexes[j], indexes[j + 1], indexes[j + 2] };
                Vertex[] triangleVertices = res.mesh.GetVertices(triangleVerticesIndexes);
                Vector3 v1 = triangleVertices[0].position;
                if (v1.y != 0)
                {
                    v1.y = height;
                }
                Vector3 v2 = triangleVertices[1].position;
                if (v2.y != 0)
                {
                    v2.y = height;
                }
                Vector3 v3 = triangleVertices[2].position;
                if (v3.y != 0)
                {
                    v3.y = height;
                }
                res.vertices.Add(v1);
                res.vertices.Add(v2);
                res.vertices.Add(v3);
            }
        }

        string uuid = System.Guid.NewGuid().ToString();
        string name = "__TEMP_OBJ__" + uuid;
        res.mesh.name = name;
        GameObject o = GameObject.Find(name);
        Destroy(o);
        return res;
    }
}
