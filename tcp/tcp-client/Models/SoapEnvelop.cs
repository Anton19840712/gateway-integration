using System.Xml.Serialization;

namespace tcp_client.Models
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class SoapEnvelope
    {
        [XmlElement(ElementName = "Body")]
        public required SoapBody Body { get; set; }
    }
    public class SoapBody
    {
        [XmlElement(ElementName = "card112ChangedRequest", Namespace = "http://www.protei.ru/emergency/integration")]
        public required Card112ChangedRequest Card112ChangedRequest { get; set; }
    }
}
