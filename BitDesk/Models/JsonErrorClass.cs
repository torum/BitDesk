using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDesk.Models
{
    public class JsonErrorData
    {
        public int code { get; set; }
        public string ja { get; set; }
    }

    public class JsonErrorObject
    {
        public int success { get; set; }
        public JsonErrorData data { get; set; }
    }

    // Undocumented error number
    /*
GetAsset: {
  "success": 0,
  "data": {
    "code": 10007,
    "ja": "ただいまメンテナンスのため一時サービスを停止しております。 今しばらくお待ちください。"
  }
}
     */

}
