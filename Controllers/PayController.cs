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
                MerchantPrivateKey = "MIIEpQIBAAKCAQEAjc4TLUdCaSufVWOYmLCv+1fKIGGHE0Zt0CTAP0u8py8yQBBra2shz35L2uFRWvi/Sh4vwhs6YdnxeiltWKdGYxSE0alhwwjc3Oi+7HAm3l4+oxD3OZz7X77RpXSFQv0sUgwjpxqAlR9CvQVExNYtDDcpro/LTh2Txtd4pBM68AjJz8f2ea4lTfUXZkAP3tviSwNsMfO3zqkbzHu5tfb7L0hjshwfet0Y4obYG8GXCk6Yne0IVjTLT5hOPq+mMcTVZDGqUgXoQMavqUoqMcy09mEVOCc+ebW6PMdj5g4kCJ9LkFzLQxGtv5Pm/nhDmUM4s+H9UC1zjIxVenc1ceY7yQIDAQABAoIBAGDVdXD49Ve+KyrzPRQfq7zWITflgYjfEkQNpJ5UiLod15G55LDQ0qDPnNsV/ClELdReMry5PS/u1SBXw8zDRaSJMX2+zfySQ8gyw5XPrCszblCKWFimY13pf14d6JZrLmXE1l/ZEC9Ezp0nSOOlYdOyo6sJlIp+Vhh1sXNRknCwey53/QD2kIJSsR4SL8gc6cuuwhA0a2daL07olg9roimTpmMQj2j942lEKNhVmeACRKtKSkw/NzE6SZCp9x7qGwkZ7bSiAvXzPUehYvGGVxBfEN2k/8brXN8CcSAL3MPXF39jonRxxhopZX8m69LPArNWAZS9n2fo8RGbb7wlIIECgYEAxyUdJtMy/YRN3N+5Kp5LsIQfFBNhUOKT+k+L4HXV2vcTAoLzPlQTz8rwPKwVlte6LBrPu6PcrDh35lubBa4Wg/2mmu+Zv/vm95fBqyKxAi3SMAYfXFLVtw/62rJ7HSH9iRyOqEeZy9NYcWi5OOHQvrQVxeEI15PXnhXnybPj/JkCgYEAtkoox9S58b46imeqDCpkpvKnm4/TOImmc2+zZZYVjOXpbQVoWV2rtB+gtWRdtLzfY9wiQAARjDmj2nzPbJkFo3CSMcQRxITWRgSltOBAb7hulH1T5GUJXwvgfkXeLOn2PwyE2t2+jyryVr5l38mR6wY9jZVHbA4qnJKuFdrBBrECgYEAvtsB8kis8KzQ1qMPWhn1XklxY5MjTBqqSVXdL5SfDUiHzntj1dNMXXQYA33E/xjnEwfrGOD1R5SizIH0s1/hskPxXUHOL2GjPJ0Tfgk0bWsuqtnjSl8U9Wn6N4igVw5RZwuYaQyeB+sYMzBze2Fn5qy+xTNjfv/wwFAsIXInSNkCgYEAtTMIUIMzhop6dIPRM6CcRptkYizYYBXsIElDgVJr/4+2tckvTc3f1P+vZz/qKbOpNwFmBkpLJyFQr+lq4l9FjI5ktBVbOAZ3XEhYU3CqfKNPQElIL/sDEriiocJftDOotEmEcm8DchrVo8ZFS+t5Ia8lVmyx/0Yhs1vWsiCtyTECgYEAqcuctxSKg4HBp6C2typ3Au8nVVeW5xGATvV8puvIIG020wyP1F+oAC7Bzc2D79mfWUtAEt5X4na1CfUaT1xsrLcV/F6jXuGdZE0jOvO+Hd1z2Sf5k5wflXj3diQIBJEtuh2Y73KzpVe9aBrA6gO5f8J18EWwuY+m8VeXKpKNXOY=",


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
                    .PreCreate("Apple iPhone11 128G", "2234367234824", "5799.00");
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
