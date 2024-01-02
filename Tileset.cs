namespace TiledJson;

public class Tileset
{
    public string? BackgroundColor { get; set; }
    public string? Class { get; set; }
    public int Columns { get; set; }
    public string FillMode { get; set; } = "stretch";
    public int FirstGid { get; set; }
    public Grid? Grid { get; set; }
    public string Image { get; set; } = "";
    public int ImageHeight { get; set; }
    public int ImageWidth { get; set; }
    public int Margin { get; set; }
    public string Name { get; set; } = "";
    public string ObjectAlignment { get; set; } = "unspecified";
    public List<Property> Properties { get; set; } = new();
    public string Source { get; set; } = "";
    public int Spacing { get; set; }
    public List<Terrain>? Terrains { get; set; }
    public int TileCount { get; set; }
    public string TiledVersion { get; set; } = "";
    public int TileHeight { get; set; }
    public TileOffset? TileOffset { get; set; }
    public string TileRenderSize { get; set; } = "tile";
    public List<Tile>? Tiles { get; set; }
    public int TileWidth { get; set; }
    public Transformations? Transformations { get; set; }
    public string? TransparentColor { get; set; }
    public string Type { get; set; } = "tileset";
    public string Version { get; set; } = "";
    public List<WangSet> WangSets { get; set; } = new();
}

public class Grid
{
    public int Height { get; set; }
    public string Orientation { get; set; } = "orthogonal";
    public int Width { get; set; }
}

public class TileOffset
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class Transformations
{
    public bool HFlip { get; set; }
    public bool VFlip { get; set; }
    public bool Rotate { get; set; }
    public bool PrefeRunTransformed { get; set; }
}

public class Tile
{
    public List<Frame> Animation { get; set; } = new();
    public int Id { get; set; }
    public string? Image { get; set; }
    public int ImageHeight { get; set; }
    public int ImageWidth { get; set; }
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    // TODO: should default to image width.. not sure if this is set by tiled
    public int Width { get; set; }
    public int Height { get; set; }
    public Layer? ObjectGroup { get; set; }
    public double? Probability { get; set; }
    public List<Property> Properties { get; set; } = new();
    public List<int>? Terrain { get; set; }
    public string? Type { get; set; }
}

public class Frame
{
    public int Duration { get; set; }
    public int TileId { get; set; }
}

public class Terrain
{
    public string Name { get; set; } = "";
    public List<Property> Properties { get; set; } = new();
    public int Tile { get; set; }
}

public class WangSet
{
    public string? Class { get; set; }
    public List<WangColor> Colors { get; set; } = new();
    public string Name { get; set; } = "";
    public List<Property> Properties { get; set; } = new();
    public int Tile { get; set; }
    public string Type { get; set; } = ""; // corner, edge, mixed
    public List<WangTile> WangTiles { get; set; } = new();
}

public class WangColor
{
    public string? Class { get; set; }
    public string Color { get; set; } = "";
    public string Name { get; set; } = "";
    public double Probability { get; set; }
    public List<Property> Properties { get; set; } = new();
    public int Tile { get; set; }
}


public class WangTile
{
    public int TileId { get; set; }
    public char[] WangId { get; set; } = new char[8]; // wang color indexes
}
