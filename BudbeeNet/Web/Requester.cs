using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace BudbeeNet.Web
{
    using ParamSet = Dictionary<string, string>;
    public class Requester
    {
        private readonly static string __defaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.135 Safari/537.36";

        private Dictionary<string, string>  m_headerSet = new Dictionary<string, string>();

        private byte[] _send(string _url, byte[] _data = null)
        {
            if (string.IsNullOrWhiteSpace(_url))
                throw new Exception("No URL specified");

            using (var _client = new WebClient())
            {
                foreach (var _entry in this.m_headerSet)
                    _client.Headers[_entry.Key] = _entry.Value;

                if (_data != null)
                    return _client.UploadData(_url, _data);
                else
                    return _client.DownloadData(_url);
            }
        }

        public Requester(string _userAgent = "")
            => this.AddHeader("User-Agent", _userAgent != string.Empty ? _userAgent : __defaultUserAgent);

        public void AddHeader(string _key, string _val)
            => this.m_headerSet[_key] = _val;

        public byte[] Get(string _url, ParamSet _params = null)
        {
            if (_params != null && _params.Count > 0)
            {
                bool _firstVal = true;
                foreach (var _entry in _params)
                {
                    _url += (_firstVal ? "?" : "&") 
                            + _entry.Key + "=" + _entry.Value;

                    _firstVal = false;
                }
            }
                

            return this._send(_url);
        }

        public byte[] Post(string _url, byte[] _data = null)
            => this._send(_url, _data);
    }
}
