namespace DSO.Core.ApplicationServices.Config;

public class CacheSettings
{
    public int AbsoluteExpirationMinutes { get; set; }
    public int SlidingExpirationMinutes { get; set; }
    public string StampedeStrategy { get; set; } = "Semaphore";
    public long SizeLimit { get; set; } = 1024;
}
