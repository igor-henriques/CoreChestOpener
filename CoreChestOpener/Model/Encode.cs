using System.Text;

namespace CoreChestOpener.Model
{
    public class Encode
    {        
        public static string GB2312ToUtf8(byte[] gb2312bytes)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Encoding fromEncoding = Encoding.GetEncoding("GB2312");
            Encoding toEncoding = Encoding.UTF8;
            return EncodingConvert(gb2312bytes, fromEncoding, toEncoding);
        }
        private static string EncodingConvert(byte[] fromBytes, Encoding fromEncoding, Encoding toEncoding)
        {
            byte[] toBytes = Encoding.Convert(fromEncoding, toEncoding, fromBytes);

            string toString = toEncoding.GetString(toBytes);
            return toString;
        }
    }
}
