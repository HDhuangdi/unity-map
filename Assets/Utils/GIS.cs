using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIS 
{
    public static Vector2 LngLatToWorldSpace(Vector2 lnglat, Vector2 offsetOriginPoint)
    {
        return (lnglat - offsetOriginPoint) * 10;
    }
}
