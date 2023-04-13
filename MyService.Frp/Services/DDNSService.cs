using Microsoft.AspNetCore.Hosting;
using MyService.Logic;
using MyService.Logic.Sql;
using MyService.Utility.Core;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Dnspod.V20210323;
using TencentCloud.Dnspod.V20210323.Models;

namespace MyService.Frp.Services
{
    public class DDNSService
    {
        private readonly ILogger<DDNSService> _logger;
        private readonly IHostEnvironment _env;
        private readonly EFCoreHelper _context;
        private readonly BaseLogic _baseLogic;
        private readonly IpUtility _ipUtility;

        public DDNSService(ILogger<DDNSService> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
            _context = new EFCoreHelper(env);
            _baseLogic = new BaseLogic(_context);
            _ipUtility = new IpUtility();
        }

        public void Start()
        {
            try
            {
                StartDdnsClient();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

        }

        private async void StartDdnsClient()
        {
            var newIPv6 = _ipUtility.GetIPv6Address();

            var resIp = (await _baseLogic.GetByFuncAsync<ServiceIpInfo>(s => s.IsDelete == 0)).OrderByDescending(s => s.CreateTime);

            if (resIp.Any())
            {
                var resOldIp = resIp.FirstOrDefault();
                var oldIPv6 = resOldIp.Ipv6Value;
                if (oldIPv6 == newIPv6)
                {
                    return;
                }
                _logger.LogInformation("监测到IPv6地址变更,开始更新IP地址");

                resOldIp.IsDelete = 1;
                await _baseLogic.UpdateAsync(resOldIp);

                ServiceIpInfo serviceIpInfo = new()
                {
                    CreateTime = DateTime.Now,
                    Ipv6Value = newIPv6,
                };
                await _baseLogic.AddAsync(serviceIpInfo);

                // 实例化一个认证对象，入参需要传入腾讯云账户 SecretId 和 SecretKey，此处还需注意密钥对的保密
                // 代码泄露可能会导致 SecretId 和 SecretKey 泄露，并威胁账号下所有资源的安全性。以下代码示例仅供参考，建议采用更安全的方式来使用密钥，请参见：https://cloud.tencent.com/document/product/1278/85305
                // 密钥可前往官网控制台 https://console.cloud.tencent.com/cam/capi 进行获取
                Credential cred = new()
                {
                    SecretId = "",
                    SecretKey = ""
                };
                // 实例化一个client选项，可选的，没有特殊需求可以跳过
                ClientProfile clientProfile = new();
                // 实例化一个http选项，可选的，没有特殊需求可以跳过
                HttpProfile httpProfile = new()
                {
                    Endpoint = ("dnspod.tencentcloudapi.com")
                };
                clientProfile.HttpProfile = httpProfile;

                // 实例化要请求产品的client对象,clientProfile是可选的
                DnspodClient client = new(cred, "", clientProfile);
                // 实例化一个请求对象,每个接口都会对应一个request对象
                ModifyRecordRequest req = new()
                {
                    Domain = "",
                    SubDomain = "",
                    RecordType = "AAAA",
                    RecordLine = "默认",
                    TTL = 600,
                    Value = newIPv6,
                    RecordId = 
                };
                // 返回的resp是一个ModifyRecordResponse的实例，与请求对象对应
                ModifyRecordResponse resp = client.ModifyRecordSync(req);
                // 输出json格式的字符串回包

                _logger.LogInformation(AbstractModel.ToJsonString(resp));
            }
            else
            {
                ServiceIpInfo serviceIpInfo = new()
                {
                    CreateTime = DateTime.Now,
                    Ipv6Value = "abcdefg",
                };
                await _baseLogic.AddAsync(serviceIpInfo);
            }
        }
    }
}
