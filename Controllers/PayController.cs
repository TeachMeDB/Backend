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

                // 下面的内容是假的，生产环境会进行替换
                AppId = "2021000121644403",
                MerchantPrivateKey = "MIIEogIBAAKCAQEAoPAo9p3LeqUceDRm48kiPTZFJ+MeKPuK67lDLuP9nGX3qi51IlMC1VB6yqNLt3/h+xgLGP39lqhjz47rNRUKapS5AQFLU9DF/A0HeYiHMl4hHXIvOocE+4IoDf8RXVpggWZj3s1pVp8TImdReyR6QRMKy8iGIm4DCvb/s2UlKeptRvvlIoy3b9tHp/kNW3Z7hsBjnlg+g/8ISfcImxK+/IUxwwPsaTCxRkb866kHnc0KLnSYhpi0WvPX3KqnEKVI0fxMOsEyeqVdmMbw/0RRmBK5CjmavMABHrzl8aRekt0DT3fbsTgfaZefratKMiLRKYpzjoKR31R0qZ1+3opICwIDAQABAoIBAAanp6n7AG0D4td9ozuMF6RcWCO8GoUrUaVJteN8fgI3nTQmU6WltRpJB6rVcc4WtauCZQMVjaTrNaVhjYxWYGXj3HqUKR9AJM+1Yje+U4jbp5bmH9nVt3kG+s0JXRh1IkR3jW2rJM32Kf5kM1RaP8vtE6c1lYZa8jlD2XL7nbvL6pj7/jH6wDQ34qL2LCPWa5+lqzFzdF0/v5j2ofulK5W+zCdP/ptaH+bjw8qqjmmmKtSclpA4G3mk+HrlO4kDzdcHFXsQNoZ6r7p1jH11w13NGzu2eCHuUiAfJmqePcNAQgM5J68zA2Ge4u4Y6xtF2h9HOk+6jTf8qLxcnJsajPECgYEA/4vHxsQcMWJjLru13EvSMlmctw+O7ChFuDbcDR7NwLGsqy+SVS6XOGc6dgFPydSi0BmW+cCZ0Dkw6o6oJdk5iMQNes8rt6MRCPPLqUhQZO2LiXkgWGN7ABf3+oWE2xAVBMMiWZw8yJ9cBf2IZ1Mlyh+BX6RP7jmNiEoW9mxMRicCgYEAoTlaXCbOBs+uPU5t+YPH2wYZA7zS+X6qTq9UI4MmpfnCPdPa1RIZcrrOTgG+N4geR2wEbcOqR4hO6YntfiuUqtDSPdojBvZzDzLvqm4AgTTHs72GearYLND+HVJd0xkq3803MZB53gD/R5l6n/jH/s72OTuLiSWBVDAqbsdXIX0CgYBgWc27TdnrbNNEnZXWN+hK/6qtAvq6Y/zVliml8MVXMCaQbOFZqLVvmMxcDeaNjNTObbeU91+HOOkOpzcS/jJJNNJ4SYCMPrl/jJxveUqGo2IHNVpSuT95K5BeLtWG2ytxnjlnXBc29Y722z8A5kLLUNGww/03LxAEIRtDUlX35QKBgHAojgv9kuZ6JCTSIv+qBacBHvXSrpKS47fJkScDXiepENAiSeJgrQN7oXnzDHllYGsfYhVaQWNyGDOvmGBlwQ70Js6Qqj11xaWVMdXGoONB7IvGFX8eszS6T43mdw8cOw9mnDcG40DmQLF2vC/9ymQhZnl6SyAlz0raFyrp0luhAoGAJGt/aKboJ6hnzw4EhSBHUAXQgQSo0H3/OX/iX4cCzLFvwyJ7SDH72VdRlcR7aMr+f0t2B1wzwfwhHzKluj4PFodNrtcrcpQ+6gz+JfSUX26I41qOSFfBpc7UNb+1ZopdZ68kuZpaHZFi4z4IA/gWqe3TGhZApWht8SbdDwEyU8s=",
                AlipayPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAhnhgbXbuRJCxnZMc+GBpETCMRE7VKDDNKTi+t2UsxbwJgpdYTkDkrnymCNM0oOq3q0wfNVS6De5GMmSpzv0nm6zWSCD+leLEcTLsYpq3LOQ6wyUQNBCKH6SBCGYjtRWL4dSfdgdmT4xC/JtQit3ph3PK9bL5GHLwjbaHT1WOn7pEqUi9XB3dnSpNouHJnohbIG08DZeoz9LKvVU1w18Mc0G0iVWnOybU7gDy6txhTv83Bew1St9lsk95XmylxWur+PsnffT4R8zQiidkv13sSIbw3cpURDDFD8QwezVJ+m+r+ujY0MLStXrbU8def7RVvowuBB2HloWUU9Lt/3fPGQIDAQAB",
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
                .PreCreate("Apple iPhone11 128G", "2234567234842", "5799.00");
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
