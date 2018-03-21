using Microsoft.VisualStudio.TestTools.UnitTesting;
using nkristek.MVVMBase.XmlTypes;
using System;
using System.IO;
using System.Xml.Serialization;

namespace nkristek.MVVMBaseTest
{
    [TestClass]
    public class VersionXmlTests
    {
        [Serializable]
        public class VersionXmlTestClass
        {
            [XmlElement("Version", typeof(VersionXml))]
            public Version Version;

            public string ToXml()
            {
                var xmlSerializer = new XmlSerializer(typeof(VersionXmlTestClass));
                using (var stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, this);
                    return stringWriter.ToString();
                }
            }

            public static VersionXmlTestClass CreateFromXml(string xmlString)
            {
                using (var reader = new StringReader(xmlString))
                    return new XmlSerializer(typeof(VersionXmlTestClass)).Deserialize(reader) as VersionXmlTestClass;
            }
        }

        [TestMethod]
        public void TestVersionXml()
        {
            var version = new Version(1, 2, 3, 4);
            var instance = new VersionXmlTestClass
            {
                Version = version
            };
            var serializedInstance = instance.ToXml();
            var deserializedInstance = VersionXmlTestClass.CreateFromXml(serializedInstance);
            Assert.AreEqual(version, deserializedInstance.Version);
        }
    }
}
