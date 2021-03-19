using System.Xml;

namespace WClipboard.Core.Extensions.Xml
{
    public static class XmlNodeListExtensions
    {
        public static XmlNode? FirstOrDefault(this XmlNodeList list)
        {
            return list.Count > 0 ? list.Item(0) : null;
        }
    }
}
