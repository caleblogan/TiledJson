## Tiled Json Load for  C#
Library loader for the Tiled file formats that can be used in programming environemtn.
Only supports tileset and tilemaps in the json file format. Files exentions must be .json.

The library class properties are very close to the spec https://doc.mapeditor.org/en/stable/reference/json-map-format/.
The main difference is that properties are in pascal case.

### Install
`dotnet add package tiledjson`

### Base Usage
`
// Load map
// path should point to where tileset files are stored
var map = TileMap.Load(new StreamReader("tests/map2-custom-class.json"), path: "tests");

// Load tilset by using helper method on map .GetTileset
// This loads a tileset matching the tile gid
var tileset = map.GetTilemap(gid);

// Get the rect of the tile with gid.
// This method will map a gid to local id and return a rect that represents the coords of the tile in the tilsets image
// E.g.
//   first-gid=301, gid=302 maps to id 1 (this is the tileset local id)
//   if tile width is 12x12
//   then rect is Rect(x:12, y:0, width:12, height:12)
var rect = map.GetTileRect(gid);
`

### Raylib Example
`
TODO: raylib
`

### Custom Properties
You can get custom properties by using the `.Get<T>` help function
`
class Vec2
{
    public int X { get; set; }
    public int Y { get; set; }
    public override string ToString() => $"({X}, {Y})";
}
foreach (var prop in map.Properties)
{
    if (prop.Name == "playerPos")
    {
        Console.WriteLine($"Prop: {prop.Name}={prop.Get<Vec2>()}");
    }
    else if (prop.Type == "string")
    {
        Console.WriteLine($"Prop: {prop.Name}={prop.Get<string>()}");
    }
}
`

or you can get the string version of the props value and transform it however you wish.
`Value` is stored as a `JsonElement` type.
`
foreach (var prop in map.Properties)
{
    Console.WriteLine($"Prop: {pop.Value}");
}
`

### Not yet supported
- compression
- tilespacing on .GetRect