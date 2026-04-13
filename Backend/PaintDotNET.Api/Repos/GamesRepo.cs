using PaintDotNET.Core.DataStructs;
using PaintDotNET.Core.Meta;

namespace PaintDotNET.Api.Repos;

public record Game(
    uint HostPlayerID,
    string ClientGroupID,
    GameSession Session
);

public class GamesRepo
{
    private readonly ItemsStore<Game> games = new();

    public uint GetNextID() => games.GetNextItemID();

    public uint InsertGame(in Game game) => games.AddItem(game);
    public void RemoveGame(uint id) => games.RemoveItem(id);
    public bool HasGame(uint id) => games.HasItem(id);
    public ref Game GetGame(uint id) => ref games.GetItem(id);

    public ItemsStoreEnumerator<Game> GetEnumerator() => games.GetEnumerator();
    public IEnumerable<Game> AsEnumerable() => games.AsEnumerable();
}
