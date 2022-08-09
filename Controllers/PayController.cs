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
                GatewayHost = "openapi.alipaydev.com",
                SignType = "RSA2",

                // 为避免私钥随源码泄露，推荐从文件中读取私钥字符串而不是写入源码中
                AppId = "2021000121644403",
                MerchantPrivateKey = "MIIEpQIBAAKCAQEAlxzdatJUTT2/IqzNBJVCb98p4822X4N44gtgoQU7xDbgvEFyfQEMGe3Wagc937SJ7mLJK9b+nRKzyjMcnOKvwkn27JGJjCOyrTDtM9f6M1aB8qXnS6nZmUu1eju6zh5wsb/OL0TWW1zd/NMXMAMDaVVmrPn7+ta5sGXJqdS4e3V5X4g+xFuZFlkPhtOMiKK0Hk63LNK4yU/SdkP+NhaCcJ6kTKQxLz/dEs0tihUZ5HFV3euCAoROgU/hlvM3xWEKQ36W06HMoeVAjhDkWI1PL+j7AlbplC3f+KVcLiWCmKJQIC23fOl0PX+9F0+39O0QRauelwO3FIRfMFGqFeMPTwIDAQABAoIBAF3FT7Dl6+IymtUNnftmE+AP/5KZKL6EexLqNIA3GytbUoH/quffKn88k30gGUkkYyNGscc+lqkp3d9vcCX6lU5fT5bliREpZcsLPWaac2xAFktYn/rX32u9C1RalEUrpormly/F666uJiKpaw4kEI3grOJSDgdOD9RilY2JryEMgHDV+6VaI3S+Dn0Azhfs86R8WXwnPmp5x0a6dSIbRXahHi4+iUdU07IAPqiNTpBcb3njkfqqXCI7sIEc6fAax27G7vgOqqwYb5znNTov4ny+kCGUnjnRmKAHtl+Ftn48o85uFJnT1jzG48XXVlrgfPxd7joftTj7wt3NYZGL0+ECgYEA7RUSiceFyGs27PPsQWUteadTUqce3yajYIvBSh65pkMY8N0fpeHSJ583+sqpcffFI5JlnbPYucgBC9hFQ8Vv8W0Kw+H063qK8A80/H9F1OGQTfaT/dfTLIADlHtVisNMQMMSZDDLcYDJGW7DaKbz10lF9x/zwvGjyS63HYzDTaUCgYEAoyus2NjxdQWrV88pmMmZKYj8QvRv+acngui2Py3lVe8WBlSfHI0ek2QZCLFlUPgcRX4KAm7faG3r8BPaU0w7LZNo4pY8/WNCrDdOaJuh/ms+798Wl/K+PtRb12pR7pfAqDv722sV07KvVXI90vRKmoajrhy0wfL6zHwgJ8YmfuMCgYEApcB0zIUFIIDszq8evv241HPKil+y8rq1wJsU0Pg4PWhtuIAwTy5ZWkAzyBAGdVqPpnYEu0Pwwrp1GM3kZyRTRkxBGYARVsy+9GT9W4az0Mi+tfMttmw9BMCfQCEWRkSSPDjl08xVgA5VCoXo8nK+ygY9dzAMoX+FTfNuLKesJ80CgYEAkJbxMa78N/BwJv2CJMvwFyg2MbadSyLeFljUtiObZ4zXfYCbBhffEkjPDi70Us2pb8MCZ6mL+uP9AN72xn10qyxG8xKitP6yZB0WNAhFgkfSm4iaYsdN4isXv0mNlNmQdQPCaGr6Cn/6csMrGbY0Lb5FaNIzMvJ3X6BWdihWkHUCgYEA01nwnI7tIMR7wW8aD2jgJOu+tv3mdDIqKO3WoyPOLo4THv8dGy8E4ZqTxNjJsApxJczdqoKezZ8FaJRWMwz5zO2Vrt+Yx4n4IBuct9G0ytMDPqVfGGuQmmRa4PIbKuY3hdKClJF64Lr+mQDZa6d4emm6MrkUN9TutCX26/NqG/0=",


                MerchantCertPath = "appCertPublicKey_2021000121644403.crt",
                AlipayCertPath = "alipayCertPublicKey_RSA2.crt",
                AlipayRootCertPath = "alipayRootCert.crt",

                // 如果采用非证书模式，则无需赋值上面的三个证书路径，改为赋值如下的支付宝公钥字符串即可
                // AlipayPublicKey = "<-- 请填写您的支付宝公钥，例如：MIIBIjANBg... -->"

                //可设置异步通知接收服务地址（可选）
                // NotifyUrl = "<-- 请填写您的支付类接口异步通知接收服务地址，例如：https://www.test.com/callback -->",

                //可设置AES密钥，调用AES加解密相关接口时需要（可选）
                // EncryptKey = "<-- 请填写您的AES密钥，例如：aa4BtZ4tspm2wnXLb1ThQA== -->"
            };
        }

        // GET: api/OrderNumbers
        [HttpGet]
        public string TryAlipay()
        {
            var ret = "233";
            Factory.SetOptions(GetConfig());
            try
            {
                // 2. 发起API调用（以创建当面付收款二维码为例）
                AlipayTradePrecreateResponse response = Factory.Payment.FaceToFace()
                    .PreCreate("Apple iPhone11 128G", "2234567234894", "5799.00");
                // 3. 处理响应或异常
                if (ResponseChecker.Success(response))
                {
                    Console.WriteLine("调用成功");
                    ret = response.QrCode;
                }
                else
                {
                    Console.WriteLine("调用失败，原因：" + response.Msg + "，" + response.SubMsg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("调用遭遇异常，原因：" + ex.Message);
                throw ex;
            }
            return ret;
        }
    }
}
