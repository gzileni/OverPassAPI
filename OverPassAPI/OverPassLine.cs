using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using OverPass.Utility;

namespace OverPass
{
    public class OverPassLine : OverPassPoint
    {
        public OverPassLine(TagType type) : base(type) { }

        public override void SetFilter(OverPassParameters parameters)
        {
            this.Parameters = parameters;
            this.Tags = new();
            List<OTag> list = new List<OTag>();
            switch (this.Type)
            {
                case TagType.RAILWAYS:
                    list.Add(new(parameters.BBox, "railway", "*"));
                    list.Add(new(parameters.BBox, "aerialway", "*"));
                    list.Add(new(parameters.BBox, "railway", "rail"));
                    list.Add(new(parameters.BBox, "railway", "light_rail"));
                    list.Add(new(parameters.BBox, "railway", "subway"));
                    list.Add(new(parameters.BBox, "railway", "tram"));
                    list.Add(new(parameters.BBox, "railway", "monorail"));
                    list.Add(new(parameters.BBox, "railway", "narrow_gauge"));
                    list.Add(new(parameters.BBox, "railway", "miniature"));
                    list.Add(new(parameters.BBox, "railway", "funicular"));
                    list.Add(new(parameters.BBox, "railway", "rack"));
                    list.Add(new(parameters.BBox, "aerialway", "drag_lift"));
                    list.Add(new(parameters.BBox, "aerialway", "chair_lift"));
                    list.Add(new(parameters.BBox, "aerialway", "cable_car"));
                    list.Add(new(parameters.BBox, "aerialway", "gondola"));
                    list.Add(new(parameters.BBox, "aerialway", "goods"));
                    break;
                case TagType.ROADS:
                    list.Add(new(parameters.BBox, "highway", "*"));
                    list.Add(new(parameters.BBox, "highway", "motorway"));
                    list.Add(new(parameters.BBox, "highway", "trunk"));
                    list.Add(new(parameters.BBox, "highway", "primary"));
                    list.Add(new(parameters.BBox, "highway", "secondary"));
                    list.Add(new(parameters.BBox, "highway", "tertiary"));
                    list.Add(new(parameters.BBox, "highway", "unclassified"));
                    list.Add(new(parameters.BBox, "highway", "residential"));
                    list.Add(new(parameters.BBox, "highway", "living_street"));
                    list.Add(new(parameters.BBox, "highway", "pedestrian"));
                    list.Add(new(parameters.BBox, "highway", "busway"));
                    list.Add(new(parameters.BBox, "highway", "motorway_link"));
                    list.Add(new(parameters.BBox, "highway", "trunk_link"));
                    list.Add(new(parameters.BBox, "highway", "primary_link"));
                    list.Add(new(parameters.BBox, "highway", "secondary_link"));
                    list.Add(new(parameters.BBox, "highway", "tertiary_link"));
                    list.Add(new(parameters.BBox, "highway", "service"));
                    list.Add(new(parameters.BBox, "highway", "track"));
                    list.Add(new(parameters.BBox, "highway", "bridleway"));
                    list.Add(new(parameters.BBox, "highway", "cycleway"));
                    list.Add(new(parameters.BBox, "highway", "footway"));
                    list.Add(new(parameters.BBox, "highway", "path"));
                    list.Add(new(parameters.BBox, "highway", "steps"));
                    break;
                case TagType.WATERWAYS:
                    list.Add(new(parameters.BBox, "waterway", "*"));
                    list.Add(new(parameters.BBox, "waterway", "river"));
                    list.Add(new(parameters.BBox, "waterway", "stream"));
                    list.Add(new(parameters.BBox, "waterway", "canal"));
                    list.Add(new(parameters.BBox, "waterway", "drain"));
                    break;
            }

            this.Tags.Add(this.Type, list);
        }
    }
}

