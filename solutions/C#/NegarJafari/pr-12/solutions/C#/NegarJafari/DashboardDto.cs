using System;

public class DashboardDto
{
    public int TotalUsers { get; set; }
    public int ActiveSessions { get; set; }
    public decimal Revenue { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
}

