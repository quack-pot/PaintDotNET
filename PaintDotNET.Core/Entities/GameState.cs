namespace PaintDotNET.Core.Entities;

// Stored as a class to avoid copying, opting for shared heap-allocated state.
public class GameState(uint grid_width, uint grid_height, uint initial_game_time_secs)
{
    public readonly uint grid_width = grid_width != 0
        ? grid_width
        : throw new ArgumentOutOfRangeException(nameof(grid_width), "Must be non-zero.");

    public readonly uint grid_height = grid_height != 0
        ? grid_height
        : throw new ArgumentOutOfRangeException(nameof(grid_height), "Must be non-zero.");

    public readonly Tile[] grid = new Tile[grid_width * grid_height];

    public float game_time_secs = initial_game_time_secs;

    public bool IsInBounds(int x_index, int y_index)
        => x_index >= 0 && y_index >= 0 && x_index < grid_width && y_index < grid_height;

    public ref Tile GetTile(int x_index, int y_index) => ref grid[x_index + (y_index * grid_width)];
}
