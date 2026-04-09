using System.Collections;
using System.Runtime.InteropServices;
using PaintDotNET.Core.Entities;

namespace PaintDotNET.Core.Stores;

using PlayerID = uint;

public ref struct PlayersStoreEnumerator(Span<Player> players_span)
{
    private Span<Player> players = players_span;
    private int index = -1;

    public bool MoveNext() => ++index < players.Length;

    public ref Player Current => ref players[index];
}

public class PlayersStore
{
    private PlayerID next_player_id = 0;

    private readonly List<Player> players = [];

    private readonly Dictionary<PlayerID, int> id_to_index = [];
    private readonly Dictionary<int, PlayerID> index_to_id = [];

    public PlayerID AddPlayer(Player player)
    {
        PlayerID id = next_player_id++;
        int index = players.Count;

        id_to_index[id] = index;
        index_to_id[index] = id;

        players.Add(player);

        return id;
    }

    public void RemovePlayer(PlayerID id)
    {
        int index = -1;
        if (!id_to_index.TryGetValue(id, out index))
        {
            return;
        }

        int last_index = players.Count - 1;
        PlayerID last_id = index_to_id[last_index];

        id_to_index.Remove(id);
        index_to_id.Remove(last_index);

        if (last_id == id)
        {
            players.RemoveAt(last_index);
            return;
        }

        players[index] = players[last_index];
        players.RemoveAt(last_index);

        id_to_index[last_id] = index;
        index_to_id[index] = last_id;
    }

    public PlayersStoreEnumerator GetEnumerator()
    {
        return new(CollectionsMarshal.AsSpan(players));
    }
}
