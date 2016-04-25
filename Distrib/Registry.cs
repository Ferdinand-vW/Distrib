using System.Collections.Concurrent;

namespace Distrib
{
  internal static class Registry
  {
    private static ConcurrentDictionary<int,Process> _processes = new ConcurrentDictionary<int,Process>();
    public static ConcurrentDictionary<int,Process> Processes
    {
      get { return _processes; }
    }

    public static bool RegisterProcess(int taskId, Process p)
    {
      return _processes.TryAdd(taskId,p);
    }

    public static bool UnregisterProcess(int taskId)
    {
      Process dummy;
      return _processes.TryRemove(taskId,out dummy);
    }
  }
}