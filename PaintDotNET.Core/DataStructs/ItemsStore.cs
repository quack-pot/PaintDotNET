using System.Runtime.InteropServices;

namespace PaintDotNET.Core.DataStructs;

using ItemID = uint;

public ref struct ItemsStoreEnumerator<T>(Span<T> items_span)
{
    private readonly Span<T> items = items_span;
    private int index = -1;

    public bool MoveNext() => ++index < items.Length;

    public readonly ref T Current => ref items[index];
}

public class ItemsStore<T>
{
    private ItemID next_item_id = 0;

    private readonly List<T> items = [];

    private readonly Dictionary<ItemID, int> id_to_index = [];
    private readonly Dictionary<int, ItemID> index_to_id = [];

    public ItemID AddItem(T player)
    {
        ItemID id = next_item_id++;
        int index = items.Count;

        id_to_index[id] = index;
        index_to_id[index] = id;

        items.Add(player);

        return id;
    }

    public void RemoveItem(ItemID id)
    {
        if (!id_to_index.TryGetValue(id, out int index))
        {
            return;
        }

        int last_index = items.Count - 1;
        ItemID last_id = index_to_id[last_index];

        id_to_index.Remove(id);
        index_to_id.Remove(last_index);

        if (last_id == id)
        {
            items.RemoveAt(last_index);
            return;
        }

        items[index] = items[last_index];
        items.RemoveAt(last_index);

        id_to_index[last_id] = index;
        index_to_id[index] = last_id;
    }

    public bool HasItem(ItemID id) => id_to_index.ContainsKey(id);

    public ref T GetItem(ItemID id)
    {
        int index = id_to_index[id];
        Span<T> span = CollectionsMarshal.AsSpan(items);
        return ref span[index];
    }

    public ItemsStoreEnumerator<T> GetEnumerator() => new(CollectionsMarshal.AsSpan(items));
}
