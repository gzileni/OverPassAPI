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
        public TagType Type { get; set; }
        public Dictionary<TagType, List<OTag>>? Tags;
        public OverPassParameters? Parameters { get; set; }

        public OverPass(TagType type) {
            this.Type = type;
        }

        public int SRCode
        {
            get
            {
                if (this.Parameters!.ToWebMercator)
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

        public string? GetPayload()
        {
            string? q = null;

            foreach (OTag t in this.Tags![this.Type])
            {
                if ((this.Parameters!.Query == null || this.Parameters!.Query.Count == 0) && t.ValueTag == "*")
                    q += t.Query;
                else
                {
                    List<string>? value = null;
                    bool keyExists = this.Parameters!.Query!.TryGetValue(t.KeyTag, out value);
                    if (keyExists && value != null)
                        if (value!.Contains(t.ValueTag))
                            q += t.Query;
                }
            }

            return q;
        }

        public async virtual Task<string> FeatureCollectionSerialized() => OverPassUtility.SerializeFeatures(await this.FeatureCollection());
        public async virtual Task<FeatureCollection?> FeatureCollection() => OverPassUtility.FeatureCollection(await OverPassUtility.GetFeatures(this.Parameters!.OverPassUrl!, this.GetPayload()!, this.SRCode, this.Parameters!.Buffer, this.Parameters!.ToWebMercator));
        public virtual void SetFilter(OverPassParameters parameters) { }
    }
}

