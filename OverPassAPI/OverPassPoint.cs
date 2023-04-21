using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using OverPass.Utility;

namespace OverPass
{
    public class OverPassPoint : OverPass
    {
        public OverPassPoint(TagType type) : base(type) {}

        public override void SetFilter(OverPassParameters parameters)
        {
            this.Parameters = parameters;
            this.Tags = new();

            List<OTag> list = new List<OTag>();

            switch (this.Type)
            {
                case TagType.NATURAL:
                    list.Add(new(parameters.BBox, "natural", "*"));
                    list.Add(new(parameters.BBox, "natural", "spring"));
                    list.Add(new(parameters.BBox, "natural", "glacier"));
                    list.Add(new(parameters.BBox, "natural", "peak"));
                    list.Add(new(parameters.BBox, "natural", "cliff"));
                    list.Add(new(parameters.BBox, "natural", "volcano"));
                    list.Add(new(parameters.BBox, "natural", "tree"));
                    list.Add(new(parameters.BBox, "natural", "mine"));
                    list.Add(new(parameters.BBox, "natural", "cave_entrance"));
                    list.Add(new(parameters.BBox, "natural", "beach"));
                    break;
                case TagType.PLACES:
                    list.Add(new(parameters.BBox, "place", "*"));
                    list.Add(new(parameters.BBox, "place", "city"));
                    list.Add(new(parameters.BBox, "place", "town"));
                    list.Add(new(parameters.BBox, "place", "village"));
                    list.Add(new(parameters.BBox, "place", "hamlet"));
                    list.Add(new(parameters.BBox, "place", "suburb"));
                    list.Add(new(parameters.BBox, "place", "island"));
                    list.Add(new(parameters.BBox, "place", "farm"));
                    list.Add(new(parameters.BBox, "place", "isolated_dwelling"));
                    list.Add(new(parameters.BBox, "place", "region"));
                    list.Add(new(parameters.BBox, "place", "county"));
                    list.Add(new(parameters.BBox, "place", "locality"));
                    break;
                case TagType.POFW:
                    list.Add(new(parameters.BBox, "amenity", "*"));
                    list.Add(new(parameters.BBox, "religion", "*"));
                    list.Add(new(parameters.BBox, "denomination", "*"));
                    list.Add(new(parameters.BBox, "amenity", "place_of_worship"));
                    list.Add(new(parameters.BBox, "religion", "christian"));
                    list.Add(new(parameters.BBox, "religion", "jewish"));
                    list.Add(new(parameters.BBox, "religion", "muslim"));
                    list.Add(new(parameters.BBox, "denomination", "anglican"));
                    list.Add(new(parameters.BBox, "denomination", "catholic"));
                    list.Add(new(parameters.BBox, "denomination", "evangelical"));
                    list.Add(new(parameters.BBox, "denomination", "lutheran"));
                    list.Add(new(parameters.BBox, "denomination", "methodist"));
                    list.Add(new(parameters.BBox, "denomination", "orthodox"));
                    list.Add(new(parameters.BBox, "denomination", "protestant"));
                    list.Add(new(parameters.BBox, "denomination", "baptist"));
                    list.Add(new(parameters.BBox, "denomination", "mormon"));
                    list.Add(new(parameters.BBox, "denomination", "sunni"));
                    list.Add(new(parameters.BBox, "denomination", "shia"));
                    list.Add(new(parameters.BBox, "religion", "buddhist"));
                    list.Add(new(parameters.BBox, "religion", "hindu"));
                    list.Add(new(parameters.BBox, "religion", "taoist"));
                    list.Add(new(parameters.BBox, "religion", "shintoist"));
                    list.Add(new(parameters.BBox, "religion", "sikh"));
                    break;
                case TagType.POI:
                    list.Add(new(parameters.BBox, "amenity", "*"));
                    list.Add(new(parameters.BBox, "office", "*"));
                    list.Add(new(parameters.BBox, "landuse", "*"));
                    list.Add(new(parameters.BBox, "leisure", "*"));
                    list.Add(new(parameters.BBox, "sport", "*"));
                    list.Add(new(parameters.BBox, "shop", "*"));
                    list.Add(new(parameters.BBox, "tourism", "*"));
                    list.Add(new(parameters.BBox, "tourist", "*"));
                    list.Add(new(parameters.BBox, "man_made", "*"));
                    list.Add(new(parameters.BBox, "emergency", "*"));
                    list.Add(new(parameters.BBox, "highway", "*"));
                    list.Add(new(parameters.BBox, "amenity", "police"));
                    list.Add(new(parameters.BBox, "amenity", "fire_station"));
                    list.Add(new(parameters.BBox, "amenity", "post_office"));
                    list.Add(new(parameters.BBox, "amenity", "telephone"));
                    list.Add(new(parameters.BBox, "amenity", "library"));
                    list.Add(new(parameters.BBox, "amenity", "townhall"));
                    list.Add(new(parameters.BBox, "amenity", "courthouse"));
                    list.Add(new(parameters.BBox, "amenity", "prison"));
                    list.Add(new(parameters.BBox, "amenity", "embassy"));
                    list.Add(new(parameters.BBox, "office", "diplomatic"));
                    list.Add(new(parameters.BBox, "amenity", "community_centre"));
                    list.Add(new(parameters.BBox, "amenity", "nursing_home"));
                    list.Add(new(parameters.BBox, "amenity", "arts_centre"));
                    list.Add(new(parameters.BBox, "amenity", "grave_yard"));
                    list.Add(new(parameters.BBox, "landuse", "cemetery"));
                    list.Add(new(parameters.BBox, "amenity", "marketplace"));
                    list.Add(new(parameters.BBox, "amenity", "university"));
                    list.Add(new(parameters.BBox, "amenity", "school"));
                    list.Add(new(parameters.BBox, "amenity", "kindergarten"));
                    list.Add(new(parameters.BBox, "amenity", "college"));
                    list.Add(new(parameters.BBox, "amenity", "public_building"));
                    list.Add(new(parameters.BBox, "amenity", "pharmacy"));
                    list.Add(new(parameters.BBox, "amenity", "hospital"));
                    list.Add(new(parameters.BBox, "amenity", "doctors"));
                    list.Add(new(parameters.BBox, "amenity", "dentist"));
                    list.Add(new(parameters.BBox, "amenity", "veterinary"));
                    list.Add(new(parameters.BBox, "amenity", "theatre"));
                    list.Add(new(parameters.BBox, "amenity", "nightclub"));
                    list.Add(new(parameters.BBox, "amenity", "cinema"));
                    list.Add(new(parameters.BBox, "leisure", "park"));
                    list.Add(new(parameters.BBox, "leisure", "playground"));
                    list.Add(new(parameters.BBox, "leisure", "dog_park"));
                    list.Add(new(parameters.BBox, "leisure", "sports_centre"));
                    list.Add(new(parameters.BBox, "leisure", "pitch"));
                    list.Add(new(parameters.BBox, "amenity", "swimming_pool"));
                    list.Add(new(parameters.BBox, "leisure", "swimming_pool"));
                    list.Add(new(parameters.BBox, "sport", "swimming"));
                    list.Add(new(parameters.BBox, "leisure", "water_park"));
                    list.Add(new(parameters.BBox, "sport", "tennis"));
                    list.Add(new(parameters.BBox, "leisure", "golf_course"));
                    list.Add(new(parameters.BBox, "leisure", "stadium"));
                    list.Add(new(parameters.BBox, "leisure", "ice_rink"));
                    list.Add(new(parameters.BBox, "amenity", "restaurant"));
                    list.Add(new(parameters.BBox, "amenity", "fast_food"));
                    list.Add(new(parameters.BBox, "amenity", "cafe"));
                    list.Add(new(parameters.BBox, "amenity", "pub"));
                    list.Add(new(parameters.BBox, "amenity", "bar"));
                    list.Add(new(parameters.BBox, "amenity", "food_court"));
                    list.Add(new(parameters.BBox, "amenity", "biergarten"));
                    list.Add(new(parameters.BBox, "tourism", "hotel"));
                    list.Add(new(parameters.BBox, "tourism", "motel"));
                    list.Add(new(parameters.BBox, "tourism", "bed_and_breakfast"));
                    list.Add(new(parameters.BBox, "tourism", "guest_house"));
                    list.Add(new(parameters.BBox, "tourism", "hostel"));
                    list.Add(new(parameters.BBox, "tourism", "chalet"));
                    list.Add(new(parameters.BBox, "amenity", "shelter"));
                    list.Add(new(parameters.BBox, "tourism", "camp_site"));
                    list.Add(new(parameters.BBox, "tourism", "alpine_hut"));
                    list.Add(new(parameters.BBox, "tourism", "caravan_site"));
                    list.Add(new(parameters.BBox, "shop", "supermarket"));
                    list.Add(new(parameters.BBox, "shop", "bakery"));
                    list.Add(new(parameters.BBox, "shop", "kiosk"));
                    list.Add(new(parameters.BBox, "shop", "mall"));
                    list.Add(new(parameters.BBox, "shop", "department_store"));
                    list.Add(new(parameters.BBox, "shop", "general"));
                    list.Add(new(parameters.BBox, "shop", "convenience"));
                    list.Add(new(parameters.BBox, "shop", "clothes"));
                    list.Add(new(parameters.BBox, "shop", "florist"));
                    list.Add(new(parameters.BBox, "shop", "chemist"));
                    list.Add(new(parameters.BBox, "shop", "books"));
                    list.Add(new(parameters.BBox, "shop", "butcher"));
                    list.Add(new(parameters.BBox, "shop", "shoes"));
                    list.Add(new(parameters.BBox, "shop", "alcohol"));
                    list.Add(new(parameters.BBox, "shop", "beverages"));
                    list.Add(new(parameters.BBox, "shop", "optician"));
                    list.Add(new(parameters.BBox, "shop", "jewerly"));
                    list.Add(new(parameters.BBox, "shop", "gift"));
                    list.Add(new(parameters.BBox, "shop", "sports"));
                    list.Add(new(parameters.BBox, "shop", "stationery"));
                    list.Add(new(parameters.BBox, "shop", "outdoor"));
                    list.Add(new(parameters.BBox, "shop", "mobile_phone"));
                    list.Add(new(parameters.BBox, "shop", "toys"));
                    list.Add(new(parameters.BBox, "shop", "newsagent"));
                    list.Add(new(parameters.BBox, "shop", "greengrocer"));
                    list.Add(new(parameters.BBox, "shop", "beauty"));
                    list.Add(new(parameters.BBox, "shop", "video"));
                    list.Add(new(parameters.BBox, "shop", "car"));
                    list.Add(new(parameters.BBox, "shop", "bicycle"));
                    list.Add(new(parameters.BBox, "shop", "doityourself"));
                    list.Add(new(parameters.BBox, "shop", "hardware"));
                    list.Add(new(parameters.BBox, "shop", "furniture"));
                    list.Add(new(parameters.BBox, "shop", "computer"));
                    list.Add(new(parameters.BBox, "shop", "garden_centre"));
                    list.Add(new(parameters.BBox, "shop", "hairdresser"));
                    list.Add(new(parameters.BBox, "shop", "car_repair"));
                    list.Add(new(parameters.BBox, "amenity", "car_rental"));
                    list.Add(new(parameters.BBox, "amenity", "car_wash"));
                    list.Add(new(parameters.BBox, "amenity", "car_sharing"));
                    list.Add(new(parameters.BBox, "amenity", "bicycle_rental"));
                    list.Add(new(parameters.BBox, "amenity", "travel_agency"));
                    list.Add(new(parameters.BBox, "amenity", "laundry"));
                    list.Add(new(parameters.BBox, "amenity", "dry_cleaning"));
                    list.Add(new(parameters.BBox, "shop", "travel_agency"));
                    list.Add(new(parameters.BBox, "shop", "laundry"));
                    list.Add(new(parameters.BBox, "shop", "dry_cleaning"));
                    list.Add(new(parameters.BBox, "amenity", "vending_machine"));
                    list.Add(new(parameters.BBox, "amenity", "bank"));
                    list.Add(new(parameters.BBox, "amenity", "atm"));
                    list.Add(new(parameters.BBox, "tourist", "information"));
                    list.Add(new(parameters.BBox, "tourist", "attraction"));
                    list.Add(new(parameters.BBox, "historic", "monument"));
                    list.Add(new(parameters.BBox, "historic", "memorial"));
                    list.Add(new(parameters.BBox, "tourist", "artwork"));
                    list.Add(new(parameters.BBox, "historic", "castle"));
                    list.Add(new(parameters.BBox, "historic", "ruins"));
                    list.Add(new(parameters.BBox, "historic", "archaeological_site"));
                    list.Add(new(parameters.BBox, "historic", "wayside_criss"));
                    list.Add(new(parameters.BBox, "historic", "wayside_shrine"));
                    list.Add(new(parameters.BBox, "historic", "battlefield"));
                    list.Add(new(parameters.BBox, "historic", "fort"));
                    list.Add(new(parameters.BBox, "tourism", "picnic_site"));
                    list.Add(new(parameters.BBox, "tourism", "viewpoint"));
                    list.Add(new(parameters.BBox, "tourism", "zoo"));
                    list.Add(new(parameters.BBox, "tourism", "theme_park"));
                    list.Add(new(parameters.BBox, "amenity", "toilets"));
                    list.Add(new(parameters.BBox, "amenity", "bench"));
                    list.Add(new(parameters.BBox, "amenity", "drinking_water"));
                    list.Add(new(parameters.BBox, "amenity", "fountain"));
                    list.Add(new(parameters.BBox, "amenity", "hunting_stand"));
                    list.Add(new(parameters.BBox, "amenity", "waste_basket"));
                    list.Add(new(parameters.BBox, "man_made", "surveillance"));
                    list.Add(new(parameters.BBox, "amenity", "emergency_phone"));
                    list.Add(new(parameters.BBox, "emergency", "phone"));
                    list.Add(new(parameters.BBox, "amenity", "fire_hydrant"));
                    list.Add(new(parameters.BBox, "highway", "emergency_access_point"));
                    list.Add(new(parameters.BBox, "man_made", "tower"));
                    break;

                case TagType.TRAFFIC:
                    list.Add(new(parameters.BBox, "highway", "*"));
                    list.Add(new(parameters.BBox, "amenity", "*"));
                    list.Add(new(parameters.BBox, "leisure", "*"));
                    list.Add(new(parameters.BBox, "man_made", "*"));
                    list.Add(new(parameters.BBox, "waterway", "*"));
                    list.Add(new(parameters.BBox, "highway", "traffic_signals"));
                    list.Add(new(parameters.BBox, "highway", "mini_roundabou"));
                    list.Add(new(parameters.BBox, "highway", "stop"));
                    list.Add(new(parameters.BBox, "highway", "crossing"));
                    list.Add(new(parameters.BBox, "highway", "level_crossing"));
                    list.Add(new(parameters.BBox, "highway", "ford"));
                    list.Add(new(parameters.BBox, "highway", "motorway_junction"));
                    list.Add(new(parameters.BBox, "highway", "turning_circle"));
                    list.Add(new(parameters.BBox, "highway", "speed_camera"));
                    list.Add(new(parameters.BBox, "highway", "street_lamp"));
                    list.Add(new(parameters.BBox, "amenity", "fuel"));
                    list.Add(new(parameters.BBox, "highway", "services"));
                    list.Add(new(parameters.BBox, "amenity", "park"));
                    list.Add(new(parameters.BBox, "highway", "bicycle_parking"));
                    list.Add(new(parameters.BBox, "leisure", "slipway"));
                    list.Add(new(parameters.BBox, "leisure", "marina"));
                    list.Add(new(parameters.BBox, "man_made", "pier"));
                    list.Add(new(parameters.BBox, "waterway", "dam"));
                    list.Add(new(parameters.BBox, "waterway", "waterfall"));
                    list.Add(new(parameters.BBox, "waterway", "lock_gate"));
                    list.Add(new(parameters.BBox, "waterway", "weir"));
                    break;
                case TagType.TRANSPORT:
                    list.Add(new(parameters.BBox, "railway", "*"));
                    list.Add(new(parameters.BBox, "highway", "*"));
                    list.Add(new(parameters.BBox, "amenity", "*"));
                    list.Add(new(parameters.BBox, "military", "*"));
                    list.Add(new(parameters.BBox, "aeroway", "*"));
                    list.Add(new(parameters.BBox, "aerialway", "*"));
                    list.Add(new(parameters.BBox, "railway", "station"));
                    list.Add(new(parameters.BBox, "railway", "halt"));
                    list.Add(new(parameters.BBox, "railway", "tram_stop"));
                    list.Add(new(parameters.BBox, "highway", "bus_stop"));
                    list.Add(new(parameters.BBox, "amenity", "bus_station"));
                    list.Add(new(parameters.BBox, "amenity", "taxi"));
                    list.Add(new(parameters.BBox, "amenity", "airport"));
                    list.Add(new(parameters.BBox, "amenity", "airfield"));
                    list.Add(new(parameters.BBox, "military", "airfield"));
                    list.Add(new(parameters.BBox, "aeroway", "helipad"));
                    list.Add(new(parameters.BBox, "aeroway", "apron"));
                    list.Add(new(parameters.BBox, "amenity", "ferry_terminal"));
                    list.Add(new(parameters.BBox, "aerialway", "station"));
                    break;
            }

            this.Tags.Add(this.Type, list);
        }

    }
}

