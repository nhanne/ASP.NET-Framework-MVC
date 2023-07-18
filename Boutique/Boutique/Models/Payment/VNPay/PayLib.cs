using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;

namespace Boutique.Models
{
    public class PayLib
    {
        private SortedList<string, string> _requestData = new SortedList<string, string>(new PayCompare());
        private SortedList<string, string> _responseData = new SortedList<string, string>(new PayCompare());

        //------------------------REQUEST DATA----------------------------------------
        public void AddRequestData(string key, string value) // Thêm dữ liệu yêu cầu
        {
            if (!String.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret) //
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            string queryString = data.ToString();

            baseUrl += "?" + queryString;
            String signData = queryString;
            if (signData.Length > 0)
            {

                signData = signData.Remove(data.Length - 1, 1);
            }
            string vnp_SecureHash = Util.HmacSHA512(vnp_HashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

            return baseUrl;
        }




        //------------------------RESPONSE DATA----------------------------------------
        public void AddResponseData(string key, string value) // Nhận vào cặp giá trị key-value vào danh sách _responseData
        {
            if (!String.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key) // Truy xuất dữ liệu trên giá trị key cung cấp
        {
            string retValue;
            if (_responseData.TryGetValue(key, out retValue))
            {
                return retValue;
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetResponseData() // Truy xuất toàn bộ dữ liệu private dưới dạng chuỗi để tạo chữ ký bảo mật 
        {

            StringBuilder data = new StringBuilder();
            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                _responseData.Remove("vnp_SecureHashType");
            }
            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                _responseData.Remove("vnp_SecureHash");
            }
            foreach (KeyValuePair<string, string> kv in _responseData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            //remove last '&'
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }
            return data.ToString();
        }
        //chữ ký bảo mật giúp đảm bảo tính toàn vẹn và xác thực của dữ liệu trong quá trình truyền thông và trao đổi thông tin,
        //đồng thời đảm bảo rằng dữ liệu không bị thay đổi và được tạo ra bởi nguồn gốc xác định.
        public bool ValidateSignature(string inputHash, string secretKey) // xác nhận chữ ký bảo mật 
        {
            string rspRaw = GetResponseData();
            string myChecksum = Util.HmacSHA512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}