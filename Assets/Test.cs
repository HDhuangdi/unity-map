using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Material material;

    // Start is called before the first frame update
    void Start()
    {

        List<Vector2> vertices = new List<Vector2>()
        {
           new Vector2(0f, 0f),
           new Vector2(5f, 0f),
           new Vector2(7f, 2f),
           new Vector2(9f, 5f),
           new Vector2(5f, 7f),
           new Vector2(2f, 20f),
           new Vector2(20f, 100f),
           new Vector2(-20f, 100f),
           new Vector2(-50f, -300f),
        };
        Extruder.Extrude(vertices, material);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
