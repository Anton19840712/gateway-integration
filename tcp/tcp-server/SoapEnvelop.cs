using System;
using System.Xml.Serialization;

namespace tcp_server
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

    public class Card112ChangedRequest
    {
        [XmlElement(ElementName = "globalId")]
        public required string GlobalId { get; set; }

        [XmlElement(ElementName = "nEmergencyCardId")]
        public int EmergencyCardId { get; set; }

        [XmlElement(ElementName = "dtCreate")]
        public DateTime DtCreate { get; set; }

        [XmlElement(ElementName = "nCallTypeId")]
        public int CallTypeId { get; set; }

        [XmlElement(ElementName = "nCardSyntheticState")]
        public int CardSyntheticState { get; set; }

        [XmlElement(ElementName = "lWithCall")]
        public int WithCall { get; set; }

        [XmlElement(ElementName = "strCreator")]
        public required string Creator { get; set; }

        [XmlElement(ElementName = "strAddressLevel1", IsNullable = true)]
        public required string AddressLevel1 { get; set; }

        [XmlElement(ElementName = "strIncidentDescription", IsNullable = true)]
        public required string IncidentDescription { get; set; }

        // Добавьте остальные поля, соответствующие вашей XML-структуре
    }
}
