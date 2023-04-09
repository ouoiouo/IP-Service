using MyService.Utility.Core;
using System.Diagnostics;

namespace MyService.Frp.Services
{
    public class WorkerService
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly ProcessUtitlity _processUtitlity;
        private readonly string _rootPath;

        public WorkerService(ILogger<WorkerService> logger, IHostEnvironment env)
        {
            _logger = logger;
            _processUtitlity = new ProcessUtitlity();
            _rootPath = env.ContentRootPath;
        }

        public void Start()
        {
            var processName = "frpc";

            //查找frpc进程
            var isFrpcProcess = _processUtitlity.SelectProcess(processName);
            if (isFrpcProcess)
            {
                return;
            }

            try
            {
                _logger.LogInformation("正在启动frpc进程...");
                StartFrpClient();
                _logger.LogInformation("frpc进程启动成功！");
            }
            catch (Exception ex)
            {
                _logger.LogError("frpc进程启动失败！", ex);
            }


        }

        public void StartFrpClient()
        {
            string frpcPath = Path.Combine(_rootPath, "frp\\frpc.exe");
            string frpcConfig = Path.Combine(_rootPath, "frp\\frpc.ini");
            _logger.LogInformation(frpcPath);
            ProcessStartInfo startInfo = new(frpcPath)
            {
                Arguments = $"-c {frpcConfig}",
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using Process process = new();
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            // 注册事件处理程序以记录输出
            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                {
                    // 使用日志库记录输出
                    _logger.LogInformation(args.Data);
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                {
                    // 使用日志库记录错误
                    _logger.LogError(args.Data);
                }
            };

            process.Start();
        }
    }
}
