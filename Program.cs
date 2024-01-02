using System.Text.Json;

namespace TiledJson;
class Program
{
    static void Main(string[] args)
    {
        var map = TileMap.Load(new StreamReader("tests/map2.json"));
        Console.WriteLine($"Map: {map.Type} {map.Version} {map.Width}x{map.Height}");
        // Console.WriteLine($"stagger = {map.StaggerIndex}");
        // Console.WriteLine($"stagger axis = {map.StaggerAxis.GetType().Name}");
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

    }
}