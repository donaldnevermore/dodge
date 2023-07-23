global using Godot;

public partial class Main : Node {
  // Don't forget to rebuild the project so the editor knows about the new export variable.

  [Export]
  public PackedScene MobScene { get; set; }

  private int _score;

  public void GameOver() {
    GetNode<Timer>("MobTimer").Stop();
    GetNode<Timer>("ScoreTimer").Stop();
    GetNode<HUD>("HUD").ShowGameOver();

    var music = GetNode<AudioStreamPlayer>("Music");
    music.Stop();
    var deathSound = GetNode<AudioStreamPlayer>("DeathSound");
    deathSound.Play();
  }

  public void NewGame() {
    _score = 0;

    var player = GetNode<Player>("Player");
    var startPosition = GetNode<Marker2D>("StartPosition");
    player.Start(startPosition.Position);

    var hud = GetNode<HUD>("HUD");
    hud.UpdateScore(_score);
    hud.ShowMessage("Get Ready!");

    // Note that for calling Godot-provided methods with strings,
    // we have to use the original Godot snake_case name.
    GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

    var music = GetNode<AudioStreamPlayer>("Music");
    music.Play();

    GetNode<Timer>("StartTimer").Start();
  }

  private void OnScoreTimerTimeout() {
    _score++;
    GetNode<HUD>("HUD").UpdateScore(_score);
  }

  private void OnStartTimerTimeout() {
    GetNode<Timer>("MobTimer").Start();
    GetNode<Timer>("ScoreTimer").Start();
  }

  private void OnMobTimerTimeout() {
    // Note: Normally it is best to use explicit types rather than the `var`
    // keyword. However, var is acceptable to use here because the types are
    // obviously Mob and PathFollow2D, since they appear later on the line.

    // Create a new instance of the Mob scene.
    Mob mob = MobScene.Instantiate<Mob>();

    // Choose a random location on Path2D.
    var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
    mobSpawnLocation.ProgressRatio = GD.Randf();

    // Set the mob's direction perpendicular to the path direction.
    float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

    // Set the mob's position to a random location.
    mob.Position = mobSpawnLocation.Position;

    // Add some randomness to the direction.
    direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
    mob.Rotation = direction;

    // Choose the velocity.
    var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
    mob.LinearVelocity = velocity.Rotated(direction);

    // Spawn the mob by adding it to the Main scene.
    AddChild(mob);
  }
}
