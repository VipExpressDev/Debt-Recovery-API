using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.ResponseObject
{
    public class ResponseProperty
    {
        public dynamic ReturnProperty { get; set; }
        public string Token { get; set; }

        public ResponseProperty(dynamic _returnProperty, string _token)
        {
            ReturnProperty = _returnProperty;
            Token = _token;
        }
    }
}
