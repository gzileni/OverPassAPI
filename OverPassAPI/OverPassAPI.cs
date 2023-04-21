using System.Collections.Generic;
﻿using System;
using System.Globalization;
using OverPass;
using OverPass.Utility;
using System.Collections;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Buffer;

namespace OverPass
{
    public class OverPassParameters
    {
        public double Buffer { get; set; } = 10;
        public string OverPassUrl { get; set; } = "https://overpass-api.de/api/interpreter";
        public string BBox { get; set; } = "41.888221345535,12.484095096588,41.895009993745,12.495414018631"; // Rome
        public int SRCode { get; set; } = 3857;
        public bool ToWebMercator { get; set; } = false;
        public NetTopologySuite.Geometries.Geometry? Geom { get; set; }
        public Dictionary<string, List<string>>? Query { get; set; }
    }

    public interface IOverPassAPIInterface
    {
        abstract Task<List<NetTopologySuite.Features.Feature>?> Features();
        void SetFilter(OverPassParameters parameters);
    }

    public class OverPassAPI : IOverPassAPIInterface
    {
        /** Point */
        public OverPassPoint Places { get; set; } = new(TagType.PLACES);
        public OverPassPoint PoI { get; set; } = new(TagType.POI);
        public OverPassPoint PoFW { get; set; } = new (TagType.POFW);
        public OverPassPoint Natural { get; set; } = new(TagType.NATURAL);
        public OverPassPoint Traffic { get; set; } = new(TagType.TRAFFIC);
        public OverPassPoint Transport { get; set; } = new(TagType.TRANSPORT);
        /** LineString */
        public OverPassLine Roads { get; set; } = new(TagType.ROADS);
        public OverPassLine Railways { get; set; } = new(TagType.RAILWAYS);
        public OverPassLine Waterways { get; set; } = new(TagType.WATERWAYS);
        /** Polygons */
        public OverPassPolygon Buildings { get; set; } = new(TagType.BUILDINGS);
        public OverPassPolygon Landuse { get; set; } = new(TagType.LANDUSE);
        public OverPassPolygon Water { get; set; } = new(TagType.WATER);

        public OverPassParameters? Parameters { get; set; }

        public OverPassAPI() {
            this.SetFilter(new OverPassParameters());
        }

        public virtual async Task<List<NetTopologySuite.Features.Feature>?> Features()
        {
            List<NetTopologySuite.Features.Feature>? result = new();

            string? q = this.Places.GetPayload();
            q += this.PoI.GetPayload();
            q += this.PoFW.GetPayload();
            q += this.Natural.GetPayload();
            q += this.Traffic.GetPayload();
            q += this.Transport.GetPayload();
            q += this.Roads.GetPayload();
            q += this.Railways.GetPayload();
            q += this.Waterways.GetPayload();
            q += this.Buildings.GetPayload();
            q += this.Buildings.GetPayload();
            q += this.Landuse.GetPayload();
            q += this.Water.GetPayload();

            return await OverPassUtility.GetFeatures(this.Parameters!.OverPassUrl!,
                                                     q,
                                                     this.Parameters!.SRCode,
                                                     this.Parameters!.Buffer,
                                                     this.Parameters!.ToWebMercator);
        }

        public void SetFilter(OverPassParameters parameters)
        {
            this.Parameters = parameters;

            if (this.Parameters.Geom is not null)
                this.Parameters.BBox = OverPassUtility.GetBBoxFromGeometry(this.Parameters.Geom);

            this.Parameters.SRCode = this.Parameters.ToWebMercator == true ? 3857 : 4326;

            this.Places.SetFilter(this.Parameters);
            this.PoI.SetFilter(this.Parameters);
            this.PoFW.SetFilter(this.Parameters);
            this.Natural.SetFilter(this.Parameters);
            this.Traffic.SetFilter(this.Parameters);
            this.Transport.SetFilter(this.Parameters);
            this.Roads.SetFilter(this.Parameters);
            this.Railways.SetFilter(this.Parameters);
            this.Waterways.SetFilter(this.Parameters);
            this.Buildings.SetFilter(this.Parameters);
            this.Landuse.SetFilter(this.Parameters);
            this.Water.SetFilter(this.Parameters);
        }
    }
}

