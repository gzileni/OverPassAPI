using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OverPass.Utility;

namespace OverPass
{
    public class OverPass
    {
        protected string QueryStart = "[out:json][timeout:25];(";
        protected string QueryEnd = ");out geom;>;out body qt;";
        public string? OverPassUrl { get; set; } = "https://overpass-api.de/api/interpreter";
        public string BBox { get; set; } = "41.888221345535,12.484095096588,41.895009993745,12.495414018631"; // Rome
        public Dictionary<string, List<string>>? Query;
        public TagType Type { get; set; } = TagType.ALL;
        public Dictionary<TagType, List<OTag>>? Tags;
        public NetTopologySuite.Geometries.Geometry? Filter { get; set; }
        public List<OTag> AllTags { get; set; } = new List<OTag>();
        public bool ToWebMercator { get; set; } = false;
        public double Buffer { get; set; } = 10.0; /** meters */

        public OverPass(string bbox) => this.BBox = bbox;
        public OverPass(string bbox, Dictionary<string, List<string>>? query) : this(bbox) => this.Query = query;

        public OverPass(NetTopologySuite.Geometries.Geometry filter)
        {
            this.Filter = filter;
            NetTopologySuite.Geometries.Envelope Env = filter.EnvelopeInternal;
            this.BBox = String.Format(CultureInfo.GetCultureInfo("en-US"), "{0},{1},{2},{3}", Env.MinY, Env.MinX, Env.MaxY, Env.MaxX);
        }

        public OverPass(NetTopologySuite.Geometries.Geometry filter, Dictionary<string, List<string>>? query) : this(filter) => this.Query = query;

        public void BaseUrl(string overpassUrl)
        {
            string url = $"{overpassUrl}/api/interpreter";
            this.OverPassUrl = url;
        }

        public int SRCode
        {
            get
            {
                if (this.ToWebMercator)
                    return 3857;
                else
                    return 4326;
            }
        } 

        /**
         * Get List of tag query
         */
        public List<string>? TagKeys
        {
            get
            {
                List<string>? q = new();
                foreach (OTag t in this.Tags![this.Type])
                {
                    if (!q.Contains(t.KeyTag))
                        q.Add(t.KeyTag);
                }
                return q;
            }
        }

        public List<string>? TagValues
        {
            get
            {
                List<string>? q = new();
                foreach (OTag t in this.Tags![this.Type])
                {
                    if (!q.Contains(t.ValueTag))
                        q.Add(t.ValueTag);
                }
                return q;
            }
        }

        

        public Dictionary<TagType, List<OTag>>? AllTagsDictonary
        {
            get {
                return new()
                {
                    { this.Type, this.AllTags }
                };
            }
            
        }

        private string? GetBody()
        {
            string? q = null;

            foreach (OTag t in this.Tags![this.Type])
            {
                if ((this.Query == null || this.Query.Count == 0) && t.ValueTag == "*")
                    q += t.Query;
                else
                {
                    List<string>? value = null;
                    bool keyExists = this.Query!.TryGetValue(t.KeyTag, out value);
                    if (keyExists && value != null)
                        if (value!.Contains(t.ValueTag)) q += t.Query;
                }
            }

            return q;
        }

        protected async Task<Root?> Response()
        {
            string? b = this.GetBody();

            if (b is not null && this.OverPassUrl is not null)
            {
                string q = $"{this.QueryStart}{b}{this.QueryEnd}";
                try
                {
                    string? r = await OverPassUtility.RunOverPassQuery(this.OverPassUrl, q);
                    return OverPassUtility.JSonDeserializeResponse(r!);
                }
                catch (Exception e)
                {
                    // Log Error
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
            else
                return null;
        }

        /** create geometries */
        public virtual async Task<List<NetTopologySuite.Features.Feature>?> Features()
        {
            Root? resp = await this.Response();
            if (resp is not null)
            {

                List<Element> nodes = resp!.elements!.Where(e => e.type == "node").ToList();
                List<Element> ways = resp!.elements!.Where(e => e.type == "way").ToList();
                List<Element> lines = ways.Where(e =>
                {
                    /** create array coordinates */
                    NetTopologySuite.Geometries.Coordinate[]? coordinates = OverPassUtility.GetCoordinates(e, this.ToWebMercator);
                    NetTopologySuite.Geometries.LineString line = new(coordinates);
                    return !line.IsClosed;
                }).ToList();

                List<Element> polys = ways.Except(lines).ToList();

                /** get all points */
                List<NetTopologySuite.Features.Feature>? features = nodes.Select(e =>
                {
                    GeometryFactory gf = OverPassUtility.CreateGeometryFactory(this.SRCode);
                    NetTopologySuite.Features.Feature? f = new();
                    AttributesTable properties = OverPassUtility.GetProperties(e);
                    f.Attributes = properties;
                    double[]? coords = new double[] { Convert.ToDouble(e.lon), Convert.ToDouble(e.lat) };
                    NetTopologySuite.Geometries.Point geom = (NetTopologySuite.Geometries.Point)new(OverPassUtility.GetPoint(coords, this.ToWebMercator));
                    f.Geometry = gf.CreateGeometry(geom);
                    return f;
                }).ToList();

                /** get all lines and polygons */
                List<NetTopologySuite.Features.Feature>? featuresLines = lines.Select(e =>
                {
                    GeometryFactory gf = OverPassUtility.CreateGeometryFactory(this.SRCode);
                    NetTopologySuite.Features.Feature? f = new();
                    AttributesTable properties = OverPassUtility.GetProperties(e);
                    f.Attributes = properties;
                    /** create array coordinates */
                    NetTopologySuite.Geometries.Coordinate[]? coordinates = OverPassUtility.GetCoordinates(e, this.ToWebMercator);
                    NetTopologySuite.Geometries.LineString line = new(coordinates);
                    f.Geometry = gf.CreateGeometry(line);
                    f.BoundingBox = OverPassUtility.GetBBox(e);
                    return f;
                }).ToList();

                List<NetTopologySuite.Features.Feature>? featuresPoly = polys.Select(e =>
                {
                    GeometryFactory gf = OverPassUtility.CreateGeometryFactory(this.SRCode);
                    NetTopologySuite.Features.Feature? f = new();
                    AttributesTable properties = OverPassUtility.GetProperties(e);
                    f.Attributes = properties;
                    /** create array coordinates */
                    NetTopologySuite.Geometries.Coordinate[]? coordinates = OverPassUtility.GetCoordinates(e, this.ToWebMercator);
                    NetTopologySuite.Geometries.LinearRing lineRing = new(coordinates);
                    NetTopologySuite.Geometries.Polygon poly = new(lineRing);
                    f.Geometry = gf.CreateGeometry(poly);
                    f.BoundingBox = OverPassUtility.GetBBox(e);
                    return f;
                }).ToList();

                /** union features */
                features.AddRange(featuresLines);
                features.AddRange(featuresPoly);
                /** remove empty geometries */
                features = features.Where(f => !f.Geometry!.IsEmpty).ToList();

                /** add buffer to features */
                if (this.Buffer > 0)
                {
                    List<NetTopologySuite.Features.Feature>? featuresWithBuffer = features.Select(f =>
                    {
                        f.Geometry = f.Geometry.Buffer(this.Buffer, 8);
                        return f;
                    }).ToList();
                    return featuresWithBuffer;
                }

                return features;
            }

            return null;
        }

        public async virtual Task<string> GeoJSon() => OverPassUtility.SerializeFeatures(await this.FeatureCollection());
        public async virtual Task<NetTopologySuite.Features.FeatureCollection?> FeatureCollection() => OverPassUtility.FeatureCollection(await this.Features());

    }
}

