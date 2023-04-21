using System;
using System.ComponentModel;

namespace OverPass
{
    public enum TagType
    {
        /** Points */
        [Description("Location for cities, towns, etc. Typically somewhere in the centre of the town")]
        PLACES,
        [Description("Points of Interest")]
        POI,
        [Description("Places of Worship")]
        POFW,
        [Description("Natural Features")]
        NATURAL,
        [Description("Traffic")]
        TRAFFIC,
        [Description("Transport Infrastructure")]
        TRANSPORT,
        /** LineString */
        ROADS,
        [Description("Railways, Subways, Trams, Lifts, and Cable Cars")]
        RAILWAYS,
        [Description("Waterways")]
        WATERWAYS,
        /** Polygons */
        WATER,
        [Description("Building outlines")]
        BUILDINGS,
        [Description("Land use and land cover")]
        LANDUSE
    }
}

