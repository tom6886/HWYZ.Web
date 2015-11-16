using System.Collections.Generic;
using System.Xml.Serialization;

namespace HWYZ.Models
{
    public class Menu
    {
        [XmlAttribute("id")]
        public string ID { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlAttribute("icon")]
        public string Icon { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("authVal")]
        public string AuthVal { get; set; }

        [XmlElement("Menu")]
        public List<Menu> SubMenu { get; set; }
    }
}
