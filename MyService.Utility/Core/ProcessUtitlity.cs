using System.Diagnostics;

namespace MyService.Utility.Core
{
    public class ProcessUtitlity
    {
        /// <summary>
        /// 启动一个批处理文件
        /// </summary>
        /// <param name="batchFilePath">批处理文件的完整路径</param>
        public void CreateProcess(string batchFilePath)
        {
            // 创建一个 ProcessStartInfo 对象，用于指定要启动的应用程序或批处理文件
            ProcessStartInfo startInfo = new(batchFilePath)
            {
                // 设置窗口样式为隐藏
                WindowStyle = ProcessWindowStyle.Hidden,
                // 禁止创建新窗口
                CreateNoWindow = true,
                // 使用操作系统外壳程序来启动进程
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }

        /// <summary>
        /// 检查是否存在指定名称的进程
        /// </summary>
        /// <param name="processName">要检查的进程名称</param>
        /// <returns>如果存在指定名称的进程，返回 true；否则返回 false。</returns>
        public bool SelectProcess(string processName)
        {
            // 获取当前系统中所有进程
            Process[] processes = Process.GetProcesses();
            // 使用 LINQ 查询，检查是否存在指定名称的进程
            bool result = processes.Where(p => p.ProcessName.Contains(processName)).Any();
            return result;
        }

    }
}
