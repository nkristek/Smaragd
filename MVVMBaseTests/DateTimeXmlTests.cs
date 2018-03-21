using Microsoft.VisualStudio.TestTools.UnitTesting;
using nkristek.MVVMBase.XmlTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace nkristek.MVVMBaseTest
{
    [TestClass]
    public class DateTimeXmlTests
    {
        [Serializable]
        public class DateTimeXmlTestClass
        {
            [XmlElement("Date", typeof(DateTimeXml))]
            public DateTimeXml Date;

            public string ToXml()
            {
                var xmlSerializer = new XmlSerializer(typeof(DateTimeXmlTestClass));
                using (var stringWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(stringWriter, this);
                    return stringWriter.ToString();
                }
            }

            public static DateTimeXmlTestClass CreateFromXml(string xmlString)
            {
                using (var reader = new StringReader(xmlString))
                    return new XmlSerializer(typeof(DateTimeXmlTestClass)).Deserialize(reader) as DateTimeXmlTestClass;
            }
        }

        [TestMethod]
        public void TestDateTimeXml()
        {
            var date = DateTimeXml.TrimDateTimeToXmlAccuracy(DateTime.Now);
            var instance = new DateTimeXmlTestClass
            {
                Date = date
            };
            var serializedInstance = instance.ToXml();
            var deserializedInstance = DateTimeXmlTestClass.CreateFromXml(serializedInstance);
            Assert.AreEqual(date, deserializedInstance.Date.Date);
        }
    }
}
