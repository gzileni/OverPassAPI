using OverPass;

public class OverPassPolygon : OverPassLine
{
    public OverPassPolygon(TagType type) : base(type) { }

    public override void SetFilter(OverPassParameters parameters)
    {
        this.Parameters = parameters;
        this.Tags = new();
        List<OTag> list = new List<OTag>();
        switch (this.Type)
        {
            case TagType.BUILDINGS:
                list.Add(new(parameters.BBox, "building", "*"));
                break;
            case TagType.LANDUSE:
                list.Add(new(parameters.BBox, "landuse", "*"));
                list.Add(new(parameters.BBox, "natural", "*"));
                list.Add(new(parameters.BBox, "leisure", "*"));
                list.Add(new(parameters.BBox, "boundary", "*"));
                list.Add(new(parameters.BBox, "landuse", "forest"));
                list.Add(new(parameters.BBox, "natural", "wood"));
                list.Add(new(parameters.BBox, "leisure", "park"));
                list.Add(new(parameters.BBox, "leisure", "common"));
                list.Add(new(parameters.BBox, "landuse", "residential"));
                list.Add(new(parameters.BBox, "landuse", "industrial"));
                list.Add(new(parameters.BBox, "landuse", "cemetery"));
                list.Add(new(parameters.BBox, "landuse", "allotments"));
                list.Add(new(parameters.BBox, "landuse", "meadown"));
                list.Add(new(parameters.BBox, "landuse", "commercial"));
                list.Add(new(parameters.BBox, "leisure", "nature_reserve"));
                list.Add(new(parameters.BBox, "leisure", "recreation_ground"));
                list.Add(new(parameters.BBox, "leisure", "retail"));
                list.Add(new(parameters.BBox, "landuse", "military"));
                list.Add(new(parameters.BBox, "landuse", "quarry"));
                list.Add(new(parameters.BBox, "landuse", "orchad"));
                list.Add(new(parameters.BBox, "landuse", "vineyard"));
                list.Add(new(parameters.BBox, "landuse", "scrub"));
                list.Add(new(parameters.BBox, "landuse", "grass"));
                list.Add(new(parameters.BBox, "natural", "health"));
                list.Add(new(parameters.BBox, "boundary", "national_park"));
                list.Add(new(parameters.BBox, "landuse", "farmland"));
                list.Add(new(parameters.BBox, "landuse", "farmyard"));
                break;
            case TagType.WATER:
                list.Add(new(parameters.BBox, "natural", "*"));
                list.Add(new(parameters.BBox, "landuse", "*"));
                list.Add(new(parameters.BBox, "waterway", "*"));
                list.Add(new(parameters.BBox, "natural", "water"));
                list.Add(new(parameters.BBox, "landuse", "reservoir"));
                list.Add(new(parameters.BBox, "waterway", "riverbank"));
                list.Add(new(parameters.BBox, "waterway", "dock"));
                list.Add(new(parameters.BBox, "natural", "glacier"));
                list.Add(new(parameters.BBox, "natural", "wetland"));
                break;
        }

        this.Tags.Add(this.Type, list);
    }
}

