using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TiledJson;
public class TileMap
{
    // Hex-formatted color (#RRGGBB or #AARRGGBB) (optional)
    public string? BackgroundColor { get; set; }
    // The class of the map (since 1.9, optional)
    public string? Class { get; set; }
    // The compression level to use for tile layer data (defaults to -1, which means to use the algorithm default)
    public int CompressionLevel { get; set; } = -1;
    // Number of tile rows
    public int Height { get; set; }
    // Length of the side of a hex tile in pixels (hexagonal maps only)
    public int HexSideLength { get; set; }
    // Whether the map has infinite dimensions
    public bool Infinite { get; set; }
    // Array of Layers
    public List<Layer> Layers { get; set; }
    // Auto-increments for each layer
    public int NextLayerId { get; set; }
    // Auto-increments for each placed object
    public int NextObjectId { get; set; }
    // orthogonal, isometric, staggered or hexagonal
    [JsonConverter(typeof(StringEnumConverter<OrientationType>))]
    public OrientationType Orientation { get; set; }
    // X coordinate of the parallax origin in pixels (since 1.8, default: 0)
    public double ParallaxOriginX { get; set; } = 0;
    // Y coordinate of the parallax origin in pixels (since 1.8, default: 0)
    public double ParallaxOriginY { get; set; } = 0;
    // Array of Properties
    public List<Property> Properties { get; set; }
    // right-down (the default), right-up, left-down or left-up (currently only supported for orthogonal maps)
    [JsonConverter(typeof(StringEnumConverter<RenderOrderType>))]
    public RenderOrderType RenderOrder { get; set; } = RenderOrderType.RightDown;
    // x or y (staggered / hexagonal maps only)
    [JsonConverter(typeof(StringEnumConverter<StaggerAxisType>))]
    public StaggerAxisType StaggerAxis { get; set; }
    // odd or even (staggered / hexagonal maps only)
    [JsonConverter(typeof(StringEnumConverter<StaggerIndexType>))]
    public StaggerIndexType StaggerIndex { get; set; }
    // The Tiled version used to save the file
    public string TiledVersion { get; set; } = "";
    // Map grid height
    public int TileHeight { get; set; }
    // Array of Tilesets
    public List<TilesetRef> Tilesets { get; set; }
    // Map grid width
    public int TileWidth { get; set; }
    // map (since 1.0)
    public string Type { get; set; } = "map";
    // The JSON format version (previously a number, saved as string since 1.6)
    public string Version { get; set; } = "";
    // Number of tile columns
    public int Width { get; set; }
    public TileMap()
    {
        Layers = new List<Layer>();
        Properties = new List<Property>();
        Tilesets = new List<TilesetRef>();
    }
    public static TileMap Load(StreamReader fstream, string? path = "")
    {
        try
        {
            var map = JsonSerializer.Deserialize<TileMap>(fstream.ReadToEnd(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (map is null)
                throw new Exception("Failed to deserialize map");

            // Post deserialization conversions

            // Make sure tilesets are relative to the map file if path is passed in
            if (path is not null)
            {
                foreach (var tset in map.Tilesets)
                {
                    tset.Source = Path.Join(path, Path.GetFileNameWithoutExtension(tset.Source) + ".json");
                }
            }

            // conver layer data (it could be b64 string or array of uint)
            foreach (var layer in map.Layers.Where(x => x.RawData is not null))
            {
                if (layer.Encoding == "csv" || layer.Encoding == "")
                {
                    layer.Data = ((JsonElement)layer.RawData).Deserialize<List<uint>>() ?? new();
                }
                else if (layer.Encoding == "base64")
                {
                    layer.Data = B64Decode(layer.RawData, layer.Compression, map.CompressionLevel);
                }
                else
                {
                    throw new Exception($"Unsupported layer encoding {layer.Encoding}");
                }
            }
            return map;
        }
        catch (JsonException e)
        {
            throw new Exception($"Failed to deserialize map: {e.Message}");
        }
    }

    // TODO: support compression
    private static List<uint> B64Decode(JsonElement rawData, string compression, int level)
    {
        if (compression == "")
        {
            var bytes = rawData.GetBytesFromBase64();
            var res = new List<uint>();
            for (int i = 0; i < bytes.Length; i += 4)
            {
                res.Add(BitConverter.ToUInt32(bytes, i));
            }
            return res;
        }
        throw new Exception($"Unsupported compression {compression} {level}");
    }

    private Dictionary<int, Tileset> _tilesetCache = new();
    public Tileset GetTilemap(int gid, bool forceReload = false)
    {
        if (_tilesetCache.ContainsKey(gid) && !forceReload)
        {
            return _tilesetCache[gid];
        }
        var tref = GetTilesetRef(gid);
        var tileset = Tileset.Load(new StreamReader(tref.Source));
        return _tilesetCache[gid] = tileset;
    }

    public TilesetRef GetTilesetRef(int gid)
    {
        Tilesets.Sort((a, b) => b.FirstGID - a.FirstGID);
        var tsetMeta = Tilesets.First(x => x.FirstGID <= gid);
        return tsetMeta;
    }

    public Rect GetTileRect(int gid)
    {
        var tileset = GetTilemap(gid);
        var localId = gid - GetTilesetRef(gid).FirstGID;

        var x = localId % tileset.Columns;
        var y = localId / tileset.Columns;
        return new Rect(x * tileset.TileWidth, y * tileset.TileHeight, tileset.TileWidth, tileset.TileHeight);
    }
}

public class Rect
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public Rect(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}

internal class StringEnumConverter<T> : JsonConverter<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var rawValue = reader.GetString() ?? "";
        var parts = rawValue.Split('-');
        var res = string.Join("", parts.Select(x => x.Capitalize()));
        try
        {
            return (T)Enum.Parse(typeof(T), res);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
            throw new Exception($"Failed to convert to enum: {typeToConvert.Name} with value `{res}`");
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
public class Layer
{
    public List<Chunk>? Chunks { get; set; }
    public string? Class { get; set; }
    // zlib, gzip, zstd (since Tiled 1.3) or empty (default). tilelayer only.
    public string Compression { get; set; } = "";
    // TODO: support base64; make a hook and store it as List<unint>; dont need to store as string and array 
    // Array of unsigned int (GIDs) or base64-encoded data. tilelayer only.
    [JsonPropertyName("data")]
    public dynamic? RawData { get; set; } = null;
    [JsonIgnore]
    public List<uint> Data { get; set; } // this will get mapped after json deserialization
    // topdown (default) or index. objectgroup only.
    public string DrawOrder { get; set; } = "";
    // csv (default) or base64. tilelayer only.
    public string Encoding { get; set; } = "";
    public int Height { get; set; }
    public int Id { get; set; }
    public string Image { get; set; } = "";
    // TODO: not supported yet
    // groups only
    public List<Layer>? Layers { get; set; }
    public bool Locked { get; set; } = false;
    public string Name { get; set; } = "";
    public List<TiledObject>? Objects { get; set; }
    public double OffsetX { get; set; } = 0;
    public double OffsetY { get; set; } = 0;
    public double Opacity { get; set; } // value between 0 and 1
    public double ParallaxX { get; set; } = 1;
    public double ParallaxY { get; set; } = 1;
    public List<Property> Properties { get; set; }
    public bool RepeatX { get; set; }
    public bool RepeatY { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
    public string? TintColor { get; set; }
    public string? TransparentColor { get; set; }
    public string Type { get; set; } = "";
    public bool Visible { get; set; }
    public int Width { get; set; }
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public Layer()
    {
        Data = new();
        Properties = new();
    }
}

public class Chunk
{
    // TODO: support base64
    public List<uint> Data { get; set; } = new();
    public int Height { get; set; }
    public int Width { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

public class TiledObject
{
    public bool Ellipse { get; set; }
    public int Gid { get; set; }
    public double Height { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool Point { get; set; }
    public List<Point> Polygon { get; set; } = new();
    public List<Point> Polyline { get; set; } = new();
    public List<Property> Properties { get; set; } = new();
    public double Rotation { get; set; }
    // TODO: should point to object instance
    public string Template { get; set; } = "";
    public TextObject? Text { get; set; }
    public string? Type { get; set; }
    public bool Visible { get; set; }
    public double Width { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}

public class Point
{
    public double X { get; set; }
    public double Y { get; set; }
}

public class TextObject
{
    public bool Bold { get; set; } = false;
    public string Color { get; set; } = "#000000";
    public string FontFamily { get; set; } = "sans-serif";
    public string HAlign { get; set; } = "left";
    public bool Italic { get; set; } = false;
    public bool Kerning { get; set; } = true;
    public int PixelSize { get; set; } = 16;
    public bool Strikeout { get; set; } = false;
    public string Text { get; set; } = "";
    public bool Underline { get; set; } = false;
    public string VAlign { get; set; } = "top";
    public bool Wrap { get; set; } = false;

}


// TODO: convert Value to native type
public class Property
{
    public string Name { get; set; } = "";
    /*
    Type of the property (string (default), int, float, bool, color, file, object or class
    (since 0.16, with color and file added in 0.17, object added in 1.4 and class added in 1.8))
    */
    public string Type { get; set; } = "";
    public string? PropertyType { get; set; } // custom property type
    public JsonElement Value { get; set; }
    public T? Get<T>()
    {
        return Value.Deserialize<T>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public Property(string name, JsonElement value)
    {
        Name = name;
        Value = value;
    }
}

/// <summary>
/// Reference to a tileset
/// FirstGID: The first global tile ID of this tileset (this global ID maps to the first tile in this tileset).
///     This conversion is done by subtracting FirstGID from the global tile ID of a tile.
/// </summary>
public class TilesetRef
{
    public int FirstGID { get; set; }
    public string Source { get; set; } = "";
}

public enum OrientationType
{
    Orthogonal,
    Isometric,
    Staggered,
    Hexagonal
}

public enum RenderOrderType
{
    RightDown,
    RightUp,
    LeftDown,
    LeftUp
}
public enum StaggerAxisType
{
    X,
    Y
}
public enum StaggerIndexType
{
    Odd,
    Even
}
