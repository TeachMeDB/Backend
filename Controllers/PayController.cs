using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using youAreWhatYouEat.Models;
using Alipay.EasySDK.Factory;
using Alipay.EasySDK.Kernel;
using Alipay.EasySDK.Kernel.Util;
using Alipay.EasySDK.Payment.FaceToFace.Models;

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {
        private readonly ModelContext _context;

        public PayController()
        {
            _context = new ModelContext();
            // Factory.SetOptions(GetConfig());
        }

        static private Config GetConfig()
        {
            return new Config()
            {
                Protocol = "https",
                GatewayHost = "openapi.alipay.com",
                SignType = "RSA2",

                AppId = "***",

                // 为避免私钥随源码泄露，推荐从文件中读取私钥字符串而不是写入源码中
                AlipayPublicKey = "***",
            };
        }

        // GET: api/OrderNumbers
        [HttpGet]
        public string TryAlipay()
        {
            var ret = "ERR";
            Factory.SetOptions(GetConfig());
            // 2. 发起API调用（以创建当面付收款二维码为例）
            AlipayTradePrecreateResponse response = Factory.Payment.FaceToFace()
                .PreCreate("Apple iPhone11 128G", "2234567234894", "5799.00");
            // 3. 处理响应或异常
            if (ResponseChecker.Success(response))
            {
                Console.WriteLine("调用成功");
                ret = response.ToString();
            }
            else
            {
                Console.WriteLine("调用失败，原因：" + response.Msg + "，" + response.SubMsg);
                ret = "调用失败，原因：" + response.Msg + "，" + response.SubMsg;
            }
            return ret;
        }
    }
}
