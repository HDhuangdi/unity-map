using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class MapChunk : MonoBehaviour
{
    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WebGLFileReader.Read("json/浙江省.json", Callback));

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Callback(string res)
    {

        GeoJSON.FeatureCollection<GeoJSON.MutilPolgon> obj = JsonMapper.ToObject<GeoJSON.FeatureCollection<GeoJSON.MutilPolgon>>(res);
        Vector2 origin = new Vector2();
        for (int i = 0; i < obj.features[0].geometry.coordinates.Length; i++)
        {
            double[][] polygon = obj.features[0].geometry.coordinates[i][0];
            List<Vector2> shape = new List<Vector2>();
            for (int j = 0; j < polygon.Length; j++)
            {
                float x = (float)polygon[j][0];
                float y = (float)polygon[j][1];
                Vector2 point;
                if (i == 0 && j == 0)
                {
                    origin.x = x;
                    origin.y = y;
                    point = new Vector2(0, 0);
                }
                else
                {
                    point = GIS.LngLatToWorldSpace(new Vector2(x, y), origin);
                }
                shape.Add(point);
            }
            Extruder.ExtrudeMesh(gameObject, shape, 2, material);
        }

    }
}
