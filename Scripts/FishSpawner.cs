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

        var gameManager = this.GetManager();
        // Spawn fish if already running to start a game
        if(gameManager.IsGameRunning)
            SpawnFish();

        gameManager.Connect(nameof(GameManager.OnGameStart), this, nameof(SpawnFish));
        gameManager.Connect(nameof(GameManager.OnFishCollected), this, nameof(OnFishDestroyed));
    }

    private void SpawnFish()
    {
        var manager = this.GetManager();
        if (!manager.IsGameRunning)
            return;

        var fish = (Fish)FishScene.Instance();
        fish.Spawner = this;

        var min = GlobalPosition.x - _shape.Extents.x;
        var max = GlobalPosition.x + _shape.Extents.x;
        AddChild(fish);
        fish.GlobalPosition = new Vector2((float)GD.RandRange(min, max), GlobalPosition.y);

        GD.Print("Fish spawned at " + fish.Position + "!");
    }

    // CalDeferred to avoid area_set_shape_disabled errors
    private void OnFishDestroyed(Fish f, bool c) => CallDeferred(nameof(SpawnFish));
}
