namespace GeoJSON
{
    public struct Parent
    {
        public int adcode;
    }

    public struct MutilPolgon
    {
        public string type;
        public double[][][][] coordinates;
    }

    public struct LineString
    {
        public string type;
        public double[][] coordinates;
    }

    public struct Properties
    {
        public int adcode;
        public string name;
        public double[] center;
        public double[] centroid;
        public double[] acroutes;
        public int childrenNum;
        public string level;
        public Parent parent;
    }

    public struct Feature<T>
    {
        public string type;
        public Properties properties;
        public T geometry;

    }

    public class FeatureCollection<T>
    {
        public string type;
        public Feature<T>[] features;
    }
}


