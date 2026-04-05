using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AoShinhoServ_Monitor
{
    public class IProcess
    {

        public static bool KillAll(int ProcessId, ROServers.Type type)
        {
            try
            {
                switch (type)
                {
                    case ROServers.Type.WSproxy:
                    case ROServers.Type.DevConsole:
                    case ROServers.Type.ROBrowser:
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "taskkill",
                            Arguments = $"/PID {ProcessId} /F /T",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        });
                        break;
                    default:
                        Process p = Process.GetProcessById(ProcessId);
                        p.Kill();
                        break;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Do_Kill_All(bool is_serv = false)
        {
            Parallel.ForEach(ILogging.processesInfos.ToArray().ToList(), it =>  {
                if (!is_serv)
                {
                    ILogging.processesInfos.Remove(it);
                    KillAll(it.pID, it.type);
                }
                else
                {
                    switch (it.type)
                    {
                        case ROServers.Type.WSproxy:
                        case ROServers.Type.DevConsole:
                        case ROServers.Type.ROBrowser:
                            break;
                        default:
                            ILogging.processesInfos.Remove(it);
                            KillAll(it.pID, it.type);
                            break;
                    }
                }
            });
            return true;
        }

        private static readonly object _trackLock = new object();

        public static void SaveTrackedPIDs()
        {
            lock (_trackLock)
            {
                var pids = string.Join(",", ILogging.processesInfos.ToArray().Select(p => $"{p.pID}:{p.type}"));
                Properties.Settings.Default.TrackedPIDs = pids;
                Properties.Settings.Default.Save();
            }
        }

        public static void KillOrphanProcesses()
        {
            string tracked = Properties.Settings.Default.TrackedPIDs;
            if (string.IsNullOrEmpty(tracked)) return;

            foreach (var entry in tracked.Split(','))
            {
                var parts = entry.Split(':');
                if (parts.Length == 2
                    && int.TryParse(parts[0], out int pid)
                    && Enum.TryParse(parts[1], out ROServers.Type type))
                {
                    try
                    {
                        var p = Process.GetProcessById(pid);
                        if (!p.HasExited)
                            KillAll(pid, type);
                    }
                    catch { }
                }
            }

            Properties.Settings.Default.TrackedPIDs = "";
            Properties.Settings.Default.Save();
        }

        public static string GetFileName(string FilePath) => System.IO.Path.GetFileNameWithoutExtension(FilePath);

        #region ValidatePathConfig

        public static bool CheckServerPath()
        {
            if (CheckMissingFile(Configuration.LoginPath, "login-server.exe") ||
               CheckMissingFile(Configuration.CharPath, "char-server.exe") ||
               CheckMissingFile(Configuration.WebPath, "web-server.exe") ||
               CheckMissingFile(Configuration.MapPath, "map-server.exe"))
            {
                ErrorHandler.ShowError($"Failed to find Servers", "Missing File");
                return false;
            }

            return true;
        }

        public static bool CheckMissingFile(string file, string mes)
        {
            if (!File.Exists(file) || file == string.Empty)
            {
                return true;
            }

            return false;
        }

        #endregion ValidatePathConfig

        public static ROServers.Type GetProcessType(Process ROProcess)
        {
            ROServers.Type type = ROServers.Type.DevConsole;
            Parallel.ForEach(ILogging.processesInfos, it =>
            {
                if (it.pID == ROProcess.Id)
                    type = it.type;
            });

            if (type != ROServers.Type.DevConsole)
                return type;

            switch (ROProcess.ProcessName.ToLowerInvariant())
            {
                case var n when n == GetFileName(Configuration.LoginPath).ToLowerInvariant():
                    return ROServers.Type.Login;
                case var n when n == GetFileName(Configuration.CharPath).ToLowerInvariant():
                    return ROServers.Type.Char;
                case var n when n == GetFileName(Configuration.WebPath).ToLowerInvariant():
                    return ROServers.Type.Web;
                case var n when n == GetFileName(Configuration.MapPath).ToLowerInvariant():
                    return ROServers.Type.Map;
                default:
                    return ROServers.Type.DevConsole;
            }
        }
    }
}