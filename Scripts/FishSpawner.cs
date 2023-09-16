using Godot;
using System;
using System.Linq;

public class FishSpawner : Area2D
{
    [Export] public PackedScene FishScene;

    private RectangleShape2D _shape;

    public override void _Ready()
    {
        base._Ready();
        var shape = GetChildren().OfType<CollisionShape2D>().FirstOrDefault();
        if (shape == null)
            throw new Exception("No RectangleShape2D child found!");

        if(!(shape.Shape is RectangleShape2D rectShape))
            throw new Exception("Child is not a RectangleShape2D!");

        _shape = rectShape;
        // Spawn fish if already running to start a game
        if(GameManager.IsGameRunning)
            SpawnFish();

        GameManager.OnGameStart += SpawnFish;
        GameManager.OnFishCollected += (f, c) => OnFishDestroyed();
    }

    private void SpawnFish()
    {
        var fish = (Fish)FishScene.Instance();
        fish.Spawner = this;

        var min = GlobalPosition.x - _shape.Extents.x;
        var max = GlobalPosition.x + _shape.Extents.x;
        AddChild(fish);
        // TODO: Randomize fish position
        fish.GlobalPosition = new Vector2(0, GlobalPosition.y);

        GD.Print("Fish spawned at " + fish.Position + "!");
    }

    // CalDeferred to avoid area_set_shape_disabled errors
    private void OnFishDestroyed() => CallDeferred(nameof(SpawnFish));
}
