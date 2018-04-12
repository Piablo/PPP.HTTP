using System;
using System.Collections.Generic;
using System.Text;

namespace DisplayModels
{
    public class BaseResponse
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public dynamic Result { get; set; }
    }
}
