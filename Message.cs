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
}
