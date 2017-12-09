using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace nkristek.MVVMBase.XmlTypes
{
    /// <summary>
    /// Helper class to serialize/deserialize Version to/from XML
    /// Can be used like:
    /// [XmlElement("Version", typeof(VersionXml))]
    /// public Version Version;
    /// </summary>
    [Serializable]
    [XmlType("Version")]
    public class VersionXml
    {
        // cant use default parameter null in other constructor because XML Serialization can't handle that
        public VersionXml() { }

        public VersionXml(Version Version)
        {
            this.Version = Version;
        }

        [XmlIgnore]
        public Version Version { get; set; }

        [XmlText]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string Value
        {
            get
            {
                return Version == null ? String.Empty : Version.ToString();
            }

            set
            {
                Version.TryParse(value, out Version temp);
                Version = temp;
            }
        }

        public static implicit operator Version(VersionXml VersionXml)
        {
            return VersionXml.Version;
        }

        public static implicit operator VersionXml(Version Version)
        {
            return new VersionXml(Version);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
