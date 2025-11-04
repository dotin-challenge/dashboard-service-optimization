using System.Runtime.CompilerServices;

namespace DashboardOptimization.Core.ApplicationService.Extensions;

public static class MethodExtension
{
    public static string GetAsyncMethodName([CallerMemberName] string caller = "")
    {
        return caller;
    }
}