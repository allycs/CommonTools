namespace Allycs.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetTopologySuite.Geometries;
    using Newtonsoft.Json;

    /// <summary>
    /// GeoJSON数据处理工具类，提供GeoJSON格式数据的解析、验证和转换功能
    /// </summary>
    public static class GeoTypeHelper
    {
        private static readonly GeometryFactory GeometryFactory = new GeometryFactory(new PrecisionModel(), 4326);

        /// <summary>
        /// 处理GeoJSON数据，转换为Geometry对象并生成原始坐标和GCJ-02坐标的JSON字符串
        /// </summary>
        /// <param name="type">几何类型</param>
        /// <param name="geometryCollection">几何数据集合（五维数组）</param>
        /// <param name="geoJson">输出原始坐标的GeoJSON字符串</param>
        /// <param name="geoJsonGcj02">输出GCJ-02坐标的GeoJSON字符串</param>
        /// <returns>对应的Geometry对象</returns>
        public static Geometry DealGeoJson(GeometryType type, List<List<List<List<double[]>>>> geometryCollection, ref string geoJson, ref string geoJsonGcj02)
        {
            switch (type)
            {
                case GeometryType.Point:
                    return HandlePoint(geometryCollection, ref geoJson, ref geoJsonGcj02);

                case GeometryType.MultiPoint:
                    return HandleMultiPoint(geometryCollection, ref geoJson, ref geoJsonGcj02);

                case GeometryType.LineString:
                    return HandleLineString(geometryCollection, ref geoJson, ref geoJsonGcj02);

                case GeometryType.LinearRing:
                    return HandleLinearRing(geometryCollection, ref geoJson, ref geoJsonGcj02);

                case GeometryType.MultiLineString:
                    return HandleMultiLineString(geometryCollection, ref geoJson, ref geoJsonGcj02);

                case GeometryType.Polygon:
                    return HandlePolygon(geometryCollection, ref geoJson, ref geoJsonGcj02);

                case GeometryType.MultiPolygon:
                    return HandleMultiPolygon(geometryCollection, ref geoJson, ref geoJsonGcj02);

                case GeometryType.GeometryCollection:
                    HandleGeometryCollection(geometryCollection, ref geoJson, ref geoJsonGcj02);
                    break;

                case GeometryType.Circle:
                    return HandlePoint(geometryCollection, ref geoJson, ref geoJsonGcj02);

                default:
                    break;
            }
            return null;
        }

        private static Point HandlePoint(List<List<List<List<double[]>>>> geometryCollection, ref string geoJson, ref string geoJsonGcj02)
        {
            var coord = geometryCollection[0][0][0][0];
            var point = GeometryFactory.CreatePoint(new Coordinate(coord[0], coord[1]));
            geoJson = JsonConvert.SerializeObject(coord);
            var gcj02 = CoordinateTransform.WGS84ToGCJ02Arr(coord[0], coord[1]);
            geoJsonGcj02 = JsonConvert.SerializeObject(gcj02);
            return point;
        }

        private static MultiPoint HandleMultiPoint(List<List<List<List<double[]>>>> geometryCollection, ref string geoJson, ref string geoJsonGcj02)
        {
            var points = geometryCollection[0][0][0];
            var pointArray = new Point[points.Count];
            var gcj02List = new List<double[]>();

            for (int i = 0; i < points.Count; i++)
            {
                pointArray[i] = new Point(new Coordinate(points[i][0], points[i][1]));
                gcj02List.Add(CoordinateTransform.WGS84ToGCJ02Arr(points[i][0], points[i][1]));
            }

            geoJson = JsonConvert.SerializeObject(points);
            geoJsonGcj02 = JsonConvert.SerializeObject(gcj02List);
            return GeometryFactory.CreateMultiPoint(pointArray);
        }

        private static LineString HandleLineString(List<List<List<List<double[]>>>> geometryCollection, ref string geoJson, ref string geoJsonGcj02)
        {
            var coords = geometryCollection[0][0][0];
            var coordinates = CreateCoordinates(coords, out var gcj02List);
            var lineString = GeometryFactory.CreateLineString(coordinates);
            lineString.SRID = 4326;
            geoJson = JsonConvert.SerializeObject(coords);
            geoJsonGcj02 = JsonConvert.SerializeObject(gcj02List);
            return lineString;
        }

        private static LinearRing HandleLinearRing(List<List<List<List<double[]>>>> geometryCollection, ref string geoJson, ref string geoJsonGcj02)
        {
            var coords = geometryCollection[0][0][0];
            var coordinates = CreateCoordinates(coords, out var gcj02List);
            var linearRing = GeometryFactory.CreateLinearRing(coordinates);
            linearRing.SRID = 4326;
            geoJson = JsonConvert.SerializeObject(coords);
            geoJsonGcj02 = JsonConvert.SerializeObject(gcj02List);
            return linearRing;
        }

        private static MultiLineString HandleMultiLineString(List<List<List<List<double[]>>>> geometryCollection, ref string geoJson, ref string geoJsonGcj02)
        {
            var lines = geometryCollection[0][0];
            var lineStrings = new LineString[lines.Count];
            var gcj02List = new List<List<double[]>>();

            for (int i = 0; i < lines.Count; i++)
            {
                var coordinates = CreateCoordinates(lines[i], out var lineGcj02);
                lineStrings[i] = GeometryFactory.CreateLineString(coordinates);
                gcj02List.Add(lineGcj02);
            }

            geoJson = JsonConvert.SerializeObject(lines);
            geoJsonGcj02 = JsonConvert.SerializeObject(gcj02List);
            return GeometryFactory.CreateMultiLineString(lineStrings);
        }

        private static Polygon HandlePolygon(List<List<List<List<double[]>>>> geometryCollection, ref string geoJson, ref string geoJsonGcj02)
        {
            var rings = geometryCollection[0][0];
            var linearRings = new LinearRing[rings.Count - 1];
            LinearRing outerRing = null;
            var gcj02List = new List<List<double[]>>();

            for (int i = 0; i < rings.Count; i++)
            {
                var coordinates = CreateCoordinates(rings[i], out var ringGcj02);
                var ring = GeometryFactory.CreateLinearRing(coordinates);
                if (i == 0)
                    outerRing = ring;
                else
                    linearRings[i - 1] = ring;
                gcj02List.Add(ringGcj02);
            }

            geoJson = JsonConvert.SerializeObject(rings);
            geoJsonGcj02 = JsonConvert.SerializeObject(gcj02List);
            return GeometryFactory.CreatePolygon(outerRing, linearRings);
        }

        private static MultiPolygon HandleMultiPolygon(List<List<List<List<double[]>>>> geometryCollection, ref string geoJson, ref string geoJsonGcj02)
        {
            var polygons = geometryCollection[0];
            var polygonArray = new Polygon[polygons.Count];
            var gcj02List = new List<List<List<double[]>>>();

            for (int k = 0; k < polygons.Count; k++)
            {
                var rings = polygons[k];
                var linearRings = new LinearRing[rings.Count - 1];
                LinearRing outerRing = null;
                var polygonGcj02 = new List<List<double[]>>();

                for (int i = 0; i < rings.Count; i++)
                {
                    var coordinates = CreateCoordinates(rings[i], out var ringGcj02);
                    var ring = GeometryFactory.CreateLinearRing(coordinates);
                    if (i == 0)
                        outerRing = ring;
                    else
                        linearRings[i - 1] = ring;
                    polygonGcj02.Add(ringGcj02);
                }

                polygonArray[k] = GeometryFactory.CreatePolygon(outerRing, linearRings);
                gcj02List.Add(polygonGcj02);
            }

            geoJson = JsonConvert.SerializeObject(polygons);
            geoJsonGcj02 = JsonConvert.SerializeObject(gcj02List);
            return GeometryFactory.CreateMultiPolygon(polygonArray);
        }

        private static void HandleGeometryCollection(List<List<List<List<double[]>>>> geometryCollection, ref string geoJson, ref string geoJsonGcj02)
        {
            var gcj02Result = new List<List<List<List<double[]>>>>();
            foreach (var collection in geometryCollection)
            {
                var collectionGcj02 = new List<List<List<double[]>>>();
                foreach (var multiGeo in collection)
                {
                    var geoGcj02 = new List<List<double[]>>();
                    foreach (var geo in multiGeo)
                    {
                        var pointGcj02 = new List<double[]>();
                        foreach (var point in geo)
                        {
                            pointGcj02.Add(CoordinateTransform.WGS84ToGCJ02Arr(point[0], point[1]));
                        }
                        geoGcj02.Add(pointGcj02);
                    }
                    collectionGcj02.Add(geoGcj02);
                }
                gcj02Result.Add(collectionGcj02);
            }
            geoJson = JsonConvert.SerializeObject(geometryCollection);
            geoJsonGcj02 = JsonConvert.SerializeObject(gcj02Result);
        }

        private static Coordinate[] CreateCoordinates(List<double[]> coords, out List<double[]> gcj02List)
        {
            var coordinates = new Coordinate[coords.Count];
            gcj02List = new List<double[]>(coords.Count);

            for (int i = 0; i < coords.Count; i++)
            {
                coordinates[i] = new Coordinate(coords[i][0], coords[i][1]);
                gcj02List.Add(CoordinateTransform.WGS84ToGCJ02Arr(coords[i][0], coords[i][1]));
            }

            return coordinates;
        }

        /// <summary>
        /// 验证多维数组格式的几何数据是否符合规则
        /// </summary>
        /// <param name="type">几何类型</param>
        /// <param name="geometryCollection">几何数据集合（五维数组）</param>
        /// <returns>如果验证通过返回null，否则返回错误信息</returns>
        public static string CheckGeoJson(GeometryType type, List<List<List<List<double[]>>>> geometryCollection)
        {
            switch (type)
            {
                case GeometryType.Point:
                    return ValidatePoint(geometryCollection) ? null : "点类型错误";

                case GeometryType.MultiPoint:
                    return ValidateMultiPoint(geometryCollection) ? null : "多点类型错误";

                case GeometryType.LineString:
                    return ValidateLineString(geometryCollection) ? null : "线类型错误";

                case GeometryType.LinearRing:
                    return ValidateLinearRing(geometryCollection) ? null : "环形线类型错误";

                case GeometryType.MultiLineString:
                    return ValidateMultiLineString(geometryCollection) ? null : "多线类型错误";

                case GeometryType.Polygon:
                    return ValidatePolygon(geometryCollection) ? null : "多边形类型错误";

                case GeometryType.MultiPolygon:
                    return ValidateMultiPolygon(geometryCollection) ? null : "多边形类型错误";

                case GeometryType.GeometryCollection:
                    return geometryCollection.Count < 1 ? "Geo集合错误" : "系统升级后支持";

                case GeometryType.Circle:
                    return ValidatePoint(geometryCollection) ? null : "圆类型错误";

                default:
                    return null;
            }
        }

        private static bool ValidatePoint(List<List<List<List<double[]>>>> geometryCollection)
        {
            return geometryCollection.Count == 1 &&
                   geometryCollection[0].Count == 1 &&
                   geometryCollection[0][0].Count == 1 &&
                   geometryCollection[0][0][0].Count == 1 &&
                   geometryCollection[0][0][0][0].Length == 2;
        }

        private static bool ValidateMultiPoint(List<List<List<List<double[]>>>> geometryCollection)
        {
            if (geometryCollection.Count != 1 || geometryCollection[0].Count != 1 || geometryCollection[0][0].Count != 1 || geometryCollection[0][0][0].Count < 1)
                return false;

            return geometryCollection[0][0][0].All(p => p.Length == 2);
        }

        private static bool ValidateLineString(List<List<List<List<double[]>>>> geometryCollection)
        {
            if (geometryCollection.Count != 1 || geometryCollection[0].Count != 1 || geometryCollection[0][0].Count != 1 || geometryCollection[0][0][0].Count < 1)
                return false;

            return geometryCollection[0][0][0].All(p => p.Length == 2);
        }

        private static bool ValidateLinearRing(List<List<List<List<double[]>>>> geometryCollection)
        {
            if (geometryCollection.Count != 1 || geometryCollection[0].Count != 1 || geometryCollection[0][0].Count != 1 || geometryCollection[0][0][0].Count < 3)
                return false;

            var first = geometryCollection[0][0][0][0];
            var last = geometryCollection[0][0][0].Last();
            if (!first.SequenceEqual(last))
                return false;

            return geometryCollection[0][0][0].All(p => p.Length == 2);
        }

        private static bool ValidateMultiLineString(List<List<List<List<double[]>>>> geometryCollection)
        {
            if (geometryCollection.Count != 1 || geometryCollection[0].Count != 1 || geometryCollection[0][0].Count < 1)
                return false;

            foreach (var line in geometryCollection[0][0])
            {
                if (line.Count < 2)
                    return false;
                if (line.Any(p => p.Length != 2))
                    return false;
            }
            return true;
        }

        private static bool ValidatePolygon(List<List<List<List<double[]>>>> geometryCollection)
        {
            if (geometryCollection.Count != 1 || geometryCollection[0].Count != 1 || geometryCollection[0][0].Count < 1)
                return false;

            foreach (var ring in geometryCollection[0][0])
            {
                if (ring.Count < 4)
                    return false;
                if (ring.Any(p => p.Length != 2))
                    return false;
            }
            return true;
        }

        private static bool ValidateMultiPolygon(List<List<List<List<double[]>>>> geometryCollection)
        {
            if (geometryCollection.Count != 1 || geometryCollection[0].Count < 1)
                return false;

            foreach (var polygon in geometryCollection[0])
            {
                foreach (var ring in polygon)
                {
                    if (ring.Count < 4)
                        return false;
                    if (ring.Any(p => p.Length != 2))
                        return false;
                }
            }
            return true;
        }
    }
}
