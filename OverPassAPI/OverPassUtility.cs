using System;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using OverPass;
using static System.Reflection.Metadata.BlobBuilder;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System.Globalization;

namespace OverPass.Utility
{
	public class OverPassUtility
	{
		public OverPassUtility()
		{
		}

        public static Root JSonDeserializeResponse(string json)
        {
            var serializer = GeoJsonSerializer.Create();
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                return serializer.Deserialize<Root>(jsonReader);
            }
        }

        public static string SerializeFeatures(NetTopologySuite.Features.FeatureCollection? features)
        {
            var serializer = GeoJsonSerializer.Create();
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                serializer.Serialize(jsonWriter, features);
                return stringWriter.ToString();
            }
        }

        public static NetTopologySuite.Geometries.Geometry DeSerializeGeometry(string geometry)
        {
            var serializer = GeoJsonSerializer.Create();
            using (var stringReader = new StringReader(geometry))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                return serializer.Deserialize<NetTopologySuite.Geometries.Geometry>(jsonReader);
            }
        }

        public static string SerializeGeometry(NetTopologySuite.Geometries.Geometry? geometry)
        {
            var serializer = GeoJsonSerializer.Create();
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                serializer.Serialize(jsonWriter, geometry);
                return stringWriter.ToString();
            }
        }

        public static NetTopologySuite.Features.FeatureCollection? FeatureCollection(List<NetTopologySuite.Features.Feature>? features)
        {
            if (features is not null)
            {
                NetTopologySuite.Features.FeatureCollection fColl = new();
                foreach (NetTopologySuite.Features.Feature f in features)
                    fColl.Add(f);
                return fColl;
            }

            return null;
        }

        /** Get attributes from respons eopenstreetmap */
        public static AttributesTable GetProperties(Element e)
        {
            AttributesTable attributes = new AttributesTable();
            if (e.id is not null)
                attributes.Add("id", e.id);
            if (e.tags is not null)
                foreach (var p in e.tags.GetType().GetProperties())
                {
                    var value = p.GetValue(e.tags, null);
                    if (value is not null)
                        attributes.Add(p.Name, value);
                }

            return attributes;
        }

        public static GeometryFactory CreateGeometryFactory(int? code)
        {
            int c = 3857;
            if (code != null) c = (int)code;
            return NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(c);
        }

        /** fetch data by OpenStreetMap Data */
        public static async Task<string?> RunOverPassQuery(string url, string query)
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(15)
            };

            using HttpClient client = new(handler);
            var body = new Dictionary<string, string>
            {
                {"data", $"{query}"}
            };

            using HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(body));
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception("Non sono riuscito a leggere le geometrie da OpenstreetMap");
        }

        /** convert coordinate from WGS84 (EPSG:4326) to EPSG:3857 */
        public static Coordinate ConvertCoordinatesToWebMercator(double[] coords)
        {
            List<Coordinate> bounds = new();

            /** convert from WGS84 (EPSG:4326) to EPSG:3857 */
            var cs4326 = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
            var cs3857 = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator;

            // Transformation
            var ctfac = new CoordinateTransformationFactory();
            var trans = ctfac.CreateFromCoordinateSystems(cs4326, cs3857);

            double[] fromPoint = new double[] { coords[0], coords[1] };
            double[] toPoint = trans.MathTransform.Transform(fromPoint);
            return new Coordinate(toPoint[0], toPoint[1]);
        }

        /** Get attributes from response openstreetmap */
        public static Coordinate GetPoint(double[] coords, bool ToWebMercator)
        {
            if (ToWebMercator)
                return ConvertCoordinatesToWebMercator(coords);
            else
                return new(coords[0], coords[1]);
        }

        /** get coordinate from element openstreetmap response */
        public static NetTopologySuite.Geometries.Coordinate[]? GetCoordinates(Element e, bool ToWebMercator)
        {
            List<NetTopologySuite.Geometries.Coordinate>? points = new();

            if (e.geometry is not null)
            {
                foreach (Geometry g in e.geometry)
                {
                    double[]? coords = new double[] { Convert.ToDouble(g.lon), Convert.ToDouble(g.lat) };
                    Coordinate p = GetPoint(coords, ToWebMercator);
                    points.Add(p);
                }
                return points.ToArray();
            }

            return null;
        }

        /** geo bbox */
        public static NetTopologySuite.Geometries.Envelope? GetBBox(Element e)
        {
            if (e.bounds is not null)
            {
                NetTopologySuite.Geometries.Coordinate minCoordinate = new(Convert.ToDouble(e.bounds.minlon), Convert.ToDouble(e.bounds.minlat));
                NetTopologySuite.Geometries.Coordinate maxCoordinate = new(Convert.ToDouble(e.bounds.maxlon), Convert.ToDouble(e.bounds.maxlat));
                NetTopologySuite.Geometries.Envelope result = new(minCoordinate, maxCoordinate);
                return result;
            }

            return null;
        }

        public static Dictionary<TagType, List<OTag>>? UnionTags(Dictionary<TagType, List<OTag>>? a, Dictionary<TagType, List<OTag>>? b)
        {
            if (b is not null)
            {
                if (a is null)
                    return b;
                else
                    foreach (var item in b)
                    {
                        List<OTag>? value = null;
                        bool keyExists = a!.TryGetValue(item.Key, out value);
                        if (!keyExists)
                            a.Add(item.Key, new());
                        if (item.Value != null)
                            a[item.Key].AddRange(item.Value);
                    }
            }

            return a;
        }

        private static async Task<Root?> FetchOverpassData(string url, string payload)
        {
            string QueryStart = "[out:json][timeout:25];(";
            string QueryEnd = ");out geom;>;out body qt;";
            string q = $"{QueryStart}{payload}{QueryEnd}";

            try
            {
                string? r = await OverPassUtility.RunOverPassQuery(url, q);
                return OverPassUtility.JSonDeserializeResponse(r!);
            }
            catch (Exception e)
            {
                // Log Error
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /** create feature from element */
        private static Feature CreateFeature(Element e,
                                             NetTopologySuite.Geometries.Geometry geom,
                                             int srCode,
                                             double buffer)
        {
            GeometryFactory gf = OverPassUtility.CreateGeometryFactory(srCode);
            Feature? f = new();
            AttributesTable properties = OverPassUtility.GetProperties(e);
            f.Attributes = properties;
            f.Geometry = gf.CreateGeometry(geom);
            f.BoundingBox = OverPassUtility.GetBBox(e);
            /** add buffer to features */
            if (buffer > 0)
                f.Geometry = f.Geometry.Buffer(buffer, 8);
            f.BoundingBox = f.Geometry.EnvelopeInternal;
            return f;
        }

        /** create geometries */
        public static async Task<List<Feature>?> GetFeatures(string url,
                                                             string payload,
                                                             int srCode,
                                                             double buffer,
                                                             bool toWebMercator = true)
        {
            if (url is not null && payload is not null)
            {
                Root? resp = await FetchOverpassData(url, payload);

                if (resp is not null)
                {

                    List<Element> nodes = resp!.elements!.Where(e => e.type == "node").ToList();
                    List<Element> ways = resp!.elements!.Where(e => e.type == "way").ToList();
                    List<Element> lines = ways.Where(e =>
                    {
                        /** create array coordinates */
                        Coordinate[]? coordinates = OverPassUtility.GetCoordinates(e, toWebMercator);
                        LineString line = new(coordinates);
                        return !line.IsClosed;
                    }).ToList();

                    List<Element> polys = ways.Except(lines).ToList();

                    /** get all points */
                    List<Feature>? features = nodes.Select(e =>
                    {
                        double[]? coords = new double[] { Convert.ToDouble(e.lon), Convert.ToDouble(e.lat) };
                        NetTopologySuite.Geometries.Point geom = (NetTopologySuite.Geometries.Point)new(OverPassUtility.GetPoint(coords, toWebMercator));
                        Feature f = CreateFeature(e, geom, srCode, buffer);
                        return f;
                    }).ToList();

                    /** get all lines and polygons */
                    List<Feature>? featuresLines = lines.Select(e =>
                    {
                        /** create array coordinates */
                        Coordinate[]? coordinates = OverPassUtility.GetCoordinates(e, toWebMercator);
                        LineString line = new(coordinates);
                        Feature f = CreateFeature(e, line, srCode, buffer);
                        return f;
                    }).ToList();

                    List<Feature>? featuresPoly = polys.Select(e =>
                    {
                        /** create array coordinates */
                        Coordinate[]? coordinates = OverPassUtility.GetCoordinates(e, toWebMercator);
                        LinearRing lineRing = new(coordinates);
                        Polygon poly = new(lineRing);
                        Feature f = CreateFeature(e, poly, srCode, buffer);
                        return f;
                    }).ToList();

                    /** union features */
                    features.AddRange(featuresLines);
                    features.AddRange(featuresPoly);
                    /** remove empty geometries */
                    features = features.Where(f => !f.Geometry!.IsEmpty).ToList();
                    return features;
                }
            }

            return null;
        }

        public static string GetBBoxFromGeometry(NetTopologySuite.Geometries.Geometry filter)
        {
            NetTopologySuite.Geometries.Envelope Env = filter.EnvelopeInternal;
            return String.Format(CultureInfo.GetCultureInfo("en-US"), "{0},{1},{2},{3}", Env.MinY, Env.MinX, Env.MaxY, Env.MaxX);
        }
    }
}

