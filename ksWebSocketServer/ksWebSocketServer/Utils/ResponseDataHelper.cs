using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ksWebSocketServer.Utils
{
    public class ResponseDataHelper
    {
        private ResponseDataHelper() { }

        public static byte[] GenerateResponseData(Dictionary<string, string> headerInfo, byte[] content)
        {
            if (null == headerInfo || headerInfo.Count == 0)
            {
                Logger.Instance().Error("Invalid header info when GenerateResponseData.");
                return null;
            }

            var sb = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in headerInfo)
            {
                sb.AppendFormat("{0}={1}{2}", keyValuePair.Key, keyValuePair.Value, ServerDefines.CommandSplitChar);
            }

            if (0 == sb.Length)
            {
                return null;
            }

            sb.Remove(sb.Length - 1, 1);
            var headerBytes = Encoding.UTF8.GetBytes(sb.ToString());

            return GenerateResponseData(headerBytes, content);
        }

        public static byte[] GenerateResponseData(byte[] headerBytes, byte[] bodyBytes)
        {
            if (headerBytes == null || headerBytes.Length == 0)
            {
                Logger.Instance().Error("Invalid header info when GenerateResponseData.");
                return null;
            }

            var headerLength = (UInt32)headerBytes.Length;
            var hlBytes = UInt32ToBytes(headerLength);
            UInt32 bodyLength = 0;
            if (bodyBytes != null)
            {
                bodyLength = (UInt32)bodyBytes.Length;
            }
            var blBytes = UInt32ToBytes(bodyLength);

            byte[] result;
            using (var ms = new MemoryStream())
            {
                ms.Write(hlBytes, 0, hlBytes.Length);
                ms.Write(headerBytes, 0, headerBytes.Length);
                ms.Write(blBytes, 0, blBytes.Length);

                if (bodyBytes != null)
                {
                    ms.Write(bodyBytes, 0, bodyBytes.Length);
                }

                result = ms.ToArray();
            }

            return result;
        }

        private static byte[] UInt32ToBytes(UInt32 value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }
    }
}
