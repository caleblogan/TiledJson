﻿using System.Numerics;
using System.Text.Json;

namespace TiledJson;
class Program
{
    class Vec2
    {
        public int X { get; set; }
        public int Y { get; set; }
        public override string ToString() => $"({X}, {Y})";
    }
    static void Main(string[] args)
    {
        var map = TileMap.Load(new StreamReader("tests/map2-custom-class.json"), path: "tests");
        Console.WriteLine($"Map: {map.Type} {map.Version} {map.Width}x{map.Height}");
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

            // Console.WriteLine(
            //     $"Property: name={prop.Name,-8} type={prop.Type,-8} value={prop.Value,-8} propType={prop.PropertyType,-8}"
            //     );
            // if (prop.Name == "helmslot")
            // {
            //     int id = prop.Value.GetInt32();
            //     Console.WriteLine($"id = {id + 2}");
            // }
        }

        Console.WriteLine($"\nTileset:");
        // var tileGID = 21; // 65 - 1
        // var tileset = map.GetTilemap(tileGID);
        // var rect = map.GetTileRect(tileGID);
        // Console.WriteLine($"rect: {rect.X} {rect.Y} {rect.Width}x{rect.Height}");

        // 400 - 301
        // var tileset2 = map.GetTilemap(400);

        // var tileset = Tileset.Load(new StreamReader("tests/tileset.json"));
        // Console.WriteLine($"Tileset: {tileset.Name} {tileset.TileCount} {tileset.TileWidth}x{tileset.TileHeight}");
        // Console.WriteLine($"Tileset2: {tileset2.Name} {tileset2.TileCount} {tileset2.TileWidth}x{tileset2.TileHeight}");

        // var tileset1Again = map.GetTilemap(65, forceReload: true);

    }
}