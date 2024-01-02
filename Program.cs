namespace TiledJson;
class Program
{
    static void Main(string[] args)
    {
        var map = TileMap.Load(new StreamReader("tests/map2.json"), path: "tests");
        Console.WriteLine($"Map: {map.Type} {map.Version} {map.Width}x{map.Height}");
        foreach (var prop in map.Properties)
        {
            Console.WriteLine(
                $"Property: name={prop.Name,-8} type={prop.Type,-8} value={prop.Value,-8} valueType={prop.Value.GetType().Name}"
                );
            if (prop.Name == "helmslot")
            {
                int id = prop.Value.GetInt32();
                Console.WriteLine($"id = {id + 2}");
            }
        }
        Console.WriteLine($"\nTileset:");
        var tileset = map.GetTilemap(65);
        var tileset2 = map.GetTilemap(400);

        // var tileset = Tileset.Load(new StreamReader("tests/tileset.json"));
        Console.WriteLine($"Tileset: {tileset.Name} {tileset.TileCount} {tileset.TileWidth}x{tileset.TileHeight}");
        Console.WriteLine($"Tileset2: {tileset2.Name} {tileset2.TileCount} {tileset2.TileWidth}x{tileset2.TileHeight}");

        var tileset1Again = map.GetTilemap(65, forceReload: true);

    }
}