using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace youAreWhatYouEat
{
    public class Message
    {
        public int errorCode { get; set; }

        public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();

        public string ReturnJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });
        }
    }
    public class EmployeeMessage : Message
    {
        public EmployeeMessage()
        {
            errorCode = 300;
            data.Add("id", null);
            data.Add("name", null);
            data.Add("gender", null);
            data.Add("occupation", null);
        }
    }

    public class OrderMessage : Message
    {
        public OrderMessage()
        {
            errorCode = 300;
            data.Add("order_id", null);
            data.Add("creation_time", null);
            data.Add("table_id", null);
            data.Add("order_status", null);
            data.Add("final_payment", null);
            data.Add("discount_price", null);
        }
    }

    public class OrderListSummaryMessage : Message
    {
        public OrderListSummaryMessage()
        {
            errorCode = 300;
            data.Add("order_count", null);
            data.Add("total_credit", null);
        }
    }

    public class OrderListMessage : Message
    {
        public OrderListMessage()
        {
            errorCode = 300;
            data.Add("data", null);
            data.Add("summary", null);
        }
    }
}
