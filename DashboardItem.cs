using System.Threading.Tasks;

/// <summary>
/// this is an object for use in cache.
/// </summary>
public sealed record DashboardItem
{
    /// <summary>
    /// Name (used as key) of the item.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Sample value of the item.
    /// </summary>
    public long Value { get; private set; } = DateTime.Now.Ticks;

    /// <summary>
    /// Time of create item.
    /// </summary>
    public DateTime CreateTime { get; private set; } = DateTime.Now;

    /// <summary>
    /// Friendly description of the current item.
    /// </summary>
    public object FriendlyDisplay => $"[{Name}] {Value}  🚩 {CreateTime:T} ";

    /// <summary>
    /// Create a new instance of <see cref="DashboardItem"/> (Simulation of heavy work).
    /// </summary>
    /// <param name="name">Introduce a name for the new item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A new instance of <see cref="DashboardItem"/>.</returns>
    public static async Task<DashboardItem> CreateAsync(string name, CancellationToken cancellationToken)
    {
        await Task.Delay(3000, cancellationToken); // heavy task...
        return new() { Name = name };
    }
}
