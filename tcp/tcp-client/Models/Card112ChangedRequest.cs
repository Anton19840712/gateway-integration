using System.Xml.Serialization;

namespace tcp_client.Models
{
    [XmlRoot(ElementName = "card112ChangedRequest", Namespace = "http://www.protei.ru/emergency/integration")]
    public class Card112ChangedRequest
    {
        [XmlElement(ElementName = "globalId")]
        public string GlobalId { get; set; } = Guid.NewGuid().ToString();

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
        public string Creator { get; set; } = string.Empty;

        [XmlElement(ElementName = "strAddressLevel1", IsNullable = true)]
        public string AddressLevel1 { get; set; } = string.Empty;

        [XmlElement(ElementName = "strAddressLevel2", IsNullable = true)]
        public string AddressLevel2 { get; set; } = string.Empty;

        [XmlElement(ElementName = "strIncidentDescription", IsNullable = true)]
        public string IncidentDescription { get; set; } = string.Empty;

        [XmlElement(ElementName = "strPriority", IsNullable = true)]
        public string Priority { get; set; } = string.Empty;

        [XmlElement(ElementName = "dtClose", IsNullable = true)]
        public DateTime? DtClose { get; set; }

        [XmlElement(ElementName = "nIncidentTypeId")]
        public int IncidentTypeId { get; set; }

        [XmlElement(ElementName = "strOperator", IsNullable = true)]
        public string Operator { get; set; } = string.Empty;

        [XmlElement(ElementName = "strAdditionalInfo", IsNullable = true)]
        public string AdditionalInfo { get; set; } = string.Empty;

        [XmlElement(ElementName = "nCardStatus")]
        public int CardStatus { get; set; }
    }
}

