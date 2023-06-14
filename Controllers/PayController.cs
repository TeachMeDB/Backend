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
                AppId = "2021000122686075",
                MerchantPrivateKey = "MIIEowIBAAKCAQEAjHGG/8qKzn5nOeu/YO2ZeqN4VCUGoOElff4z8EYufoNHApuizJ9Ks4ikHu0YJIolPQDIo5NUcvtmm+EHVhlTH1vvhywIyTtKM65j7/VopcSEXasbDISXDm2854P4n5pwi/VpGWfLH9tFCi/0IndtVSYAuU7EuOqE1hQjzPvDZpL28eFT4dv6NjfDXF9ADO2Pg+E6Kf2/X95DxKhbA4vNthydaYIBD2vC7RhPUySuc5C6bYGB7jq/jhEPLPBi5Z8QxAwZ5YrScsVg6NHqWBvkweb3oKySctaa81YyJPk6M0hC5av/wNaGkpfUxcpmHYwhLUqopRNYDz3sjDWctqvdowIDAQABAoIBAHX5d7mvXmKPdA8Hpb+6V07Zut6UK7lA58mqm96eVbgYdrIOIvdYZ9vgUF1aNQmcsiNIYUJnJG1iGZOfuBXvOBADnnYh20+O6on3WCPeNoXpSneUoCgnyOxp6mFgTp1jkl8/S4X31n1wJ9Ki7aV2RPVd8wW20TG0btmWGWy7oCysz7fjOGwrLjA+C2ysxOAoYe3ORs8bJAwmzLxRxf/axAVO9tlvmSLKDQvy2WccnQCK6aBn7SpUPIXH0nP1aO5okJOHunf8+PjUhy8EoD5Zu5gorTmynpXFWMWZmamFJ9pkk21M9jS6lBOGYbn3fYw/Sk6C4cGkVFzxx2mkL9oeNkECgYEAz73SAuielbNpK5/mawJTKnzg084k5MzlOZx27mdIqbZtxL8kglCMk0R3sqBiiLgiGMF4rpL64+MeDrRaeq2/gvrqOag66d9dpdBxLN5m/sEwJNU4v1zi1zvM724/LIn8iRYzc2/QiCcOjMinBCkM3KLSjENDcgLvXl9NHWkhU3kCgYEArRGPh11U2wlhmENTTr2lhtVDOpqPKYSxYtSEExQCZHyXOQ1KV9c0hsZLuzZ1t44yJqHaTFfDvdw4eVa260UEVPqgiWUG6lDdKAW9sNTu8R39f85ypsI7MpTVxK4jh+zVI3jAy4z4MD3EcaJJiOMoURG+7A4n0HcoRhBIowYftvsCgYBiwZc7oL/65H5DqdEM398PXKw4pE/voOP7nkD/loP5geru0/sUJByfybWpagtNeVNkN10aiG7StN5OGTjGByD6sDN2012mXnQTZmLW9Kc7xjUP3XkLp6mCrj//Noa7sqElvjMZVndoskB9dsnG9YjSvmifGz7NzU1jrKRpLdgU6QKBgQCUfP2dUXltmMXwYstGQraTUcSlslhkceHT/qv2xnMrDi6dF4cSiKUbgWUWkio7u7fRp4ppd9+MtTaK/NhiUUvfMU2ttE1Tmt+06AARiRAdNfL6xgFejEVIJ6SlZBc+FvfjwEYCHsgOc+Bo8qmO1f8QV15sn9SPKr0WKAB6RtXNIwKBgAviOiLmYjQwpf7IO/IXUUIcVR/0y7ele5RbVQLy5zaCTw1qiY/X1XjVp5tWdwbrp2vdSOnbR+Petf8sQQLtrLydUh41Q6cLUmJfwBDYQ62rb5tWHQ8/gxkbwjkIsa22UGRVMzLTPHMrY+05avRckdk/Z5c9Mmj4fTzzdV6d+ibp",


                MerchantCertPath = "Certificate/appPublicCert.crt",
                AlipayCertPath = "Certificate/alipayPublicCert.crt",
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
			Factory.Payment.Common().Create("菜品结算", oid, price.ToString(), "2088722012892870");

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
