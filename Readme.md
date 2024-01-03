## Tiled Json Loader for  C#
Library loader for the Tiled file formats that can be used in any programming environment.
Only supports tileset and tilemaps in the json file format. Files exentions must be .json.

The library class properties are very close to the spec https://doc.mapeditor.org/en/stable/reference/json-map-format/.
The main difference is that properties are in pascal case.

### Install
`dotnet add package tiledjson`

### Base Usage
```C#
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
```


### Custom Properties
You can get custom properties by using the `.Get<T>` help function

```C#
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
```

or you can get the string version of the props value and transform it however you wish.
`Value` is stored as a `JsonElement` type.
```C#
foreach (var prop in map.Properties)
{
    Console.WriteLine($"Prop: {pop.Value}");
}
```

### Raylib Example
If you have more than one tileset, you will need to precompute a texture for each tileset image.
```C#
using TiledJson;
using Raylib_cs;
using System.Numerics;

Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
Raylib.InitWindow(1000, 600, "Tiled Json Demo");
Raylib.SetTargetFPS(60);

var map = Tilemap.Load(new StreamReader("tests/map.json"), path: "tests");

var tileset = map.GetTileset(1);
var imgSrc = Path.Join("tests", Path.GetFileName(tileset.Image));
var tilesetImg = Raylib.LoadImage(imgSrc);
var texture = Raylib.LoadTextureFromImage(tilesetImg);

var layer = map.Layers.First(x => x.Data.Count > 0);
while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.BLACK);

    for (int y = 0; y < layer.Height; y++)
    {
        for (int x = 0; x < layer.Width; x++)
        {
            var gid = layer.Data[y * layer.Width + x];
            if (gid == 0) continue; // 0 represents no tile
            var rect = map.GetTileRect(gid);
            Raylib.DrawTexturePro(
                texture,
                new Rectangle(rect.X, rect.Y, rect.Width, rect.Height), // src rect in sprite
                new Rectangle(x * rect.Width, y * rect.Height, rect.Width, rect.Height), // dest rect in window
                new Vector2(0, 0),
                0,
                Color.WHITE
            );
        }
    }
    Raylib.EndDrawing();
}
```

### Not yet supported
- compression
- tilespacing on .GetRect
