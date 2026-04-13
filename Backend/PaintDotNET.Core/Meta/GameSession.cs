using System.Diagnostics;
using PaintDotNET.Core.DataStructs;
using PaintDotNET.Core.Entities;
using PaintDotNET.Core.Enums;
using PaintDotNET.Core.Math;
using PaintDotNET.Core.Systems;

namespace PaintDotNET.Core.Meta;

public class GameSession
{
    private readonly GameState game_state;
    private readonly ItemsStore<Player> players;

    private readonly PaintSystem paint_system;
    private readonly MovementSystem move_system;
    private readonly GameClockSystem clock_system;

    private readonly Stopwatch stopwatch = new();
    private float last_frame_time = 0.0f;

    private uint red_team_player_count = 0u;
    private uint blue_team_player_count = 0u;
    
    private readonly List<TileUpdateData> tile_updates = [];
    private readonly List<PlayerJoinData> join_updates = [];
    private readonly List<PlayerUpdateData> player_updates = [];

    private bool is_running = false;

    public GameSession()
    {
        game_state = new(
            GameRules.TILE_GRID_WIDTH,
            GameRules.TILE_GRID_HEIGHT,
            GameRules.GAME_TIME_SECS
        );
        players = new();

        paint_system = new(game_state, players);
        move_system = new(game_state, players);
        clock_system = new(game_state);
    }

    public bool IsRunning() => is_running;

    public void StartGame()
    {
        if (is_running)
        {
            return;
        }

        is_running = true;

        game_state.game_time_secs = GameRules.GAME_TIME_SECS;

        foreach (ref Player player in players)
        {
            move_system.PickPlayerSpawn(ref player);
        }

        last_frame_time = 0.0f;
        stopwatch.Reset();
        stopwatch.Start();
    }

    public bool AttemptUpdate()
    {
        if (!is_running)
        {
            return false;
        }

        float current_frame_time = (float)stopwatch.Elapsed.TotalSeconds;
        float delta_time = current_frame_time - last_frame_time;
        last_frame_time = current_frame_time;

        move_system.UpdatePlayers(delta_time, player_updates);
        paint_system.UpdatePainting(delta_time, tile_updates);
        clock_system.TickGameClock(delta_time);

        is_running = clock_system.IsGameStillGoing();
        return is_running;
    }

    public void ApplyPlayerInput(in PlayerInputData input)
    {
        if (!players.HasItem(input.PlayerID))
        {
            return;
        }

        ref Player player = ref players.GetItem(input.PlayerID);

        player.input_direction.X = MathGen.GetAxis<float>(input.IsRightPressed, input.IsLeftPressed);
        player.input_direction.Y = MathGen.GetAxis<float>(input.IsDownPressed, input.IsUpPressed);
    }

    public float GetTeamCoverage(Team team)
    {
        uint total_tiles = game_state.grid_width * game_state.grid_height;
        float max_coverage = GameRules.MAX_PAINT_STRENGTH * total_tiles;

        if (max_coverage <= 0.0f)
        {
            return 0.0f;
        }

        float team_coverage = 0.0f;
        for (uint idx = 0; idx < total_tiles; ++idx)
        {
            ref Tile tile = ref game_state.grid[idx];

            if (tile.team == team)
            {
                team_coverage += tile.strength;
            }
        }

        return team_coverage / max_coverage;
    }

    public PlayerAddData AddNewPlayer()
    {
        Team player_team;

        if (red_team_player_count < blue_team_player_count)
        {
            player_team = Team.RED_TEAM;
            ++red_team_player_count;
        }
        else
        {
            player_team = Team.BLUE_TEAM;
            ++blue_team_player_count;
        }

        Player new_player = new(players.GetNextItemID(), player_team, new());
        move_system.PickPlayerSpawn(ref new_player);

        uint player_id = players.AddItem(new_player);

        join_updates.Add(new(
            player_id,
            new_player.position.X,
            new_player.position.Y,
            false,
            player_team == Team.RED_TEAM
        ));

        return new(
            player_id,
            new_player.position,
            player_team
        );
    }

    public void RemovePlayer(uint id) {
        players.RemoveItem(id);
        join_updates.Add(new(id, 0.0f, 0.0f, true, false));
    }

    public uint GetGridWidth() => game_state.grid_width;
    public uint GetGridHeight() => game_state.grid_height;

    public float GetGameTime() => game_state.game_time_secs;

    public TileUpdateData[] GetTileUpdates()
    {
        TileUpdateData[] updates = [.. tile_updates];
        tile_updates.Clear();

        return updates;
    }

    public PlayerJoinData[] GetJoinUpdates()
    {
        PlayerJoinData[] updates = [.. join_updates];
        join_updates.Clear();

        return updates;
    }

    public PlayerUpdateData[] GetPlayerUpdates()
    {
        PlayerUpdateData[] updates = [.. player_updates];
        player_updates.Clear();

        return updates;
    }

    public PlayerJoinData[] GetPlayerInitialValues()
    {
        if (players.Count == 0)
        {
            return [];
        }

        PlayerJoinData[] initial_values = new PlayerJoinData[players.Count];

        int idx = 0;
        foreach (ref Player player in players)
        {
            initial_values[idx++] = new(
                player.id,
                player.position.X,
                player.position.Y,
                false,
                player.team == Team.RED_TEAM
            );
        }

        return initial_values;
    }
}
