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
using StackExchange.Redis;
#pragma warning disable CS8629
#pragma warning disable CS8601
#pragma warning disable CA2200
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
                MerchantPrivateKey = "MIIEowIBAAKCAQEAiQZ+zt1BUt3HBwwonSgEcfu2lB6zJkRiSxW7Y7gFiv7vhwz0Rzdfla/FFhdUREGhVToUgc16GqkA1yjk0ofM4TTJMFbGf5ikHhhIZeBLfZnQ97L8L8o+FNu5kt6j5wdhd094ivvwvFfH47CAUTLZwJJ3/dJiEb/SOyHv2ccRo+1gQI8JZltG0KdDsphJy+K7jpDp9pTtZe8zs8mR+OPOGynEOgX49Pk9GaFYa9M3L3r2Ge3svDDP80f3aXixbaQFtUie6UAvt6nNGdh9ntHGrQRrt9LAxFCWeKHLK2bJtvv7ly0klEQ2hZcf4e7Gp5+SrMdz3/mWXYlywtrq96sj5wIDAQABAoIBADuwPKTX9JCoUv5a6YYr3GPahhnXlapht3+Fo+84RhHaI3Tsy6wKDvUhb/TDjM1+2UxK1IwGed4LKHaRNbwPs8uWNt17r1P9peAGK+NU5Q19Z22+5ePbcVecryWvHi6RJH/YemXwgVlJ06T3Sa4VPI2KQAKtFlMs/+MKUXTIJoMFTMlmUyRDoUWzB+kLbXR/UU0ifxFCW3CbKzLVWulwSAuvps5Lt3jrs1ZECA9NQslUwdLTI3MfGWrJffcOQqbRWW2suye06ds95GF8Q7USFin3xLgVRIu5I7vnbfDiRJEjgC1i3l34TiYeeb0vtbsi49SzpoYLCBsqUUOwnfyMRGECgYEAw4tlVPCTBcDlWbSZvKEnnCw9ylQMolxKTe5QFNKWO4q0oAdV86dh5/Fn93N6362Iba3KhDEqK0v4tGrpQ8rFDtQhHdM7j0XppW+33iilgsw2iWSFDJUediISHuhkBEOdikAcPwXC1t8EFqK/SMajnUEFvUF9O47ZTXg2h+ikBUMCgYEAs2OIN0nabRjmxfqFXLVAbt3cW663GK+yNPP+Arz2N5mJBs2xS9EPWXGeXb9xwSfY9zDfmEPWksYPOmF8cCMtA5YGPvBaxWNw4w1M8mE1hrz9PlTvin7GYSURpaiy4ImSTFCeOXKcYdpUDPry6wECkWKBzQ7i9TSbcfZeNhqX6o0CgYBmCNW+Ra4h4W6LqX5S/DBkAH3ZlpPlII0xLvogc2Yq+YyYuFs0QZIH6mKar2pdTvqSaI30/oaHgyPqWKMRWbwLqcsB+hfpAc0SI0JpLQJ4RsDqXkZCs4jqkYqtEAwoYXPSEPPsOsBZSJOkkBWKDwSO6L/q6kJDkp13HBQwRMdZawKBgBzUDfv0nmmR6uQ+EliHuEcY2GTTexE4iwb/fv6pp1fgEKlLPQbWHgVdwPmH3pwP70oqTISHwl/r/F6BdCIBRSaiYXaT4cJ+7JFQfeElxhhVHs15kNVM8kyCTIe2yKhU83HYMtVAmekP0RoU6hziGKuMjReLIOYPAjwaP6GqPGexAoGBAIv5Nzus7n5Da8DaDdbnh9MsNM4SuP9drC3Q4aMO+cRPXnEb2xcUtbhbx1CMJ15RJiXv5VSk1IToLLH82GsC9VypoFyw0b290E7YUWqU2cRRciV2N2TY940vC+i5Q4lnhFYoGk8LKE7QjOr1L4TVDAH+rANW8FEWjKyKVSTP1d1f",


                MerchantCertPath = "Certificate/appCertPublicKey_2021000121644403.crt",
                AlipayCertPath = "Certificate/alipayCertPublicKey_RSA2.crt",
                AlipayRootCertPath = "Certificate/alipayRootCert.crt",

                // 如果采用非证书模式，则无需赋值上面的三个证书路径，改为赋值如下的支付宝公钥字符串即可
                // AlipayPublicKey = "<-- 请填写您的支付宝公钥，例如：MIIBIjANBg... -->"

                //可设置异步通知接收服务地址（可选）
                // NotifyUrl = "<-- 请填写您的支付类接口异步通知接收服务地址，例如：https://www.test.com/callback -->",

                //可设置AES密钥，调用AES加解密相关接口时需要（可选）
                // EncryptKey = "<-- 请填写您的AES密钥，例如：aa4BtZ4tspm2wnXLb1ThQA== -->"
            };
        }

        static private string? putredis(string k, string v)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(System.Configuration.ConfigurationManager.ConnectionStrings["Redis"].ConnectionString);
            IDatabase db = redis.GetDatabase();
            db.StringSet(k, v);
            var value = db.StringGet(k);
            return value;
        }

        static private string? getredis(string k)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(System.Configuration.ConfigurationManager.ConnectionStrings["Redis"].ConnectionString);
            IDatabase db = redis.GetDatabase();
            var value = db.StringGet(k);
            return value;
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

        public class FinalPayRes
        {
            public string qrcode { get; set; } = String.Empty;
        }
        [HttpGet("FinalPay")]
        public async Task<ActionResult<FinalPayRes>> FinalPayOld(string order_id, decimal final_price)
        {
            FinalPayRes ret = new FinalPayRes();
            // long timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var order = await _context.Orderlists
            .Include(o => o.Dishorderlists)
            .FirstOrDefaultAsync(o => o.OrderId == order_id);
            if (order == null)
            {
                return NoContent();
            }
            if (order.OrderStatus == "已支付")
            {
                return NoContent();
            }
            decimal price = final_price;
            Factory.SetOptions(GetConfig());
            string oid = order.CreationTime.ToString("yyyyMMddHHmmss");
            string? payid = getredis(oid);
            try
            {
                if (payid != null)
                {
                    if (getredis("status:" + payid) == "Waiting")
                    {
                        ret.qrcode = getredis("qr:" + oid);
                    }
                    else
                    {
                        return NotFound();
                    }
                    ret.qrcode = getredis("qr:" + oid);
                }
                else
                {
                    AlipayTradePrecreateResponse response = Factory.Payment.FaceToFace().PreCreate("菜品结算", oid, price.ToString());
                    if (ResponseChecker.Success(response))
                    {
                        // Console.WriteLine("调用成功");
                        ret.qrcode = response.QrCode;
                        putredis(oid, response.OutTradeNo);
                        putredis("qr:" + oid, response.QrCode);
                        putredis("status:" + response.OutTradeNo, "Waiting");
                        // await _context.SaveChangesAsync();
                    }
                    else
                    {
                        Console.WriteLine("调用失败，原因：" + response.Msg + "，" + response.SubMsg);
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine("调用遭遇异常，原因：" + ex.Message);
                // throw ex;
                return BadRequest(ex);
            }
            return Ok(ret);
        }

        public class StatusInfo
        {
            public string order_status { get; set; } = String.Empty;
        }

        // GET 获取订单支付状态
        [HttpGet("GetOrderStatus")]
        public async Task<ActionResult<StatusInfo>> GetOrderStatus(string order_id)
        {
            if (order_id == null) return BadRequest();
            var order = await _context.Orderlists
                .FirstOrDefaultAsync(o => o.OrderId == order_id);
            if (order == null) return NoContent();
            StatusInfo info = new StatusInfo();

            string oid = order.CreationTime.ToString("yyyyMMddHHmmss");
            string? payid = getredis(oid);

            if (payid == null)
            {
                return Ok(order.OrderStatus);
            }
            Factory.SetOptions(GetConfig());
            try
            {
                var response = Factory.Payment.Common().Query(payid);
                if (ResponseChecker.Success(response))
                {
                    if (response.TradeStatus == "TRADE_SUCCESS")
                    {
                        order.OrderStatus = "已支付";
                        putredis("status:" + response.OutTradeNo, "Done");
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine("调用遭遇异常，原因：" + ex.Message);
                // throw ex;
                return BadRequest(ex);
            }

            info.order_status = order.OrderStatus;

            return Ok(info);
        }

        /*        [HttpPost("redis")]
                public ActionResult<string> testpostredis(string k, string v)
                {
                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(System.Configuration.ConfigurationManager.ConnectionStrings["Redis"].ConnectionString);
                    IDatabase db = redis.GetDatabase();
                    db.StringSet(k, v);
                    var value = db.StringGet(k);
                    return Ok(value);
                }
                [HttpGet("redis")]
                public ActionResult<string> testgetredis(string k)
                {
                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(System.Configuration.ConfigurationManager.ConnectionStrings["Redis"].ConnectionString);
                    IDatabase db = redis.GetDatabase();
                    var value = db.StringGet(k);
                    return Ok(value);
                }*/
    }
}
