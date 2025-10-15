using NHapi.Base.Parser;
using NHapi.Model.V251.Message;
using Reception.Application.Models;

namespace Reception.Application.Services;

public static class Hl7MessageService
{
    public static string CreateAdtMessage(PatientHl7Model patient)
    {
        var message = new ADT_A01();
        var parser = new PipeParser();

        message.MSH.FieldSeparator.Value = "|";
        message.MSH.EncodingCharacters.Value = "^~\\&";
        message.MSH.SendingApplication.NamespaceID.Value = "RECEPTION_API";
        message.MSH.SendingFacility.NamespaceID.Value = "HOSPITAL";
        message.MSH.ReceivingApplication.NamespaceID.Value = "HIS_SRV";
        message.MSH.ReceivingFacility.NamespaceID.Value = "HOSPITAL";
        message.MSH.DateTimeOfMessage.Time.Value = DateTime.Now.ToString("yyyyMMddHHmmss");
        message.MSH.MessageType.MessageCode.Value = "ADT";
        message.MSH.MessageType.TriggerEvent.Value = patient.Hl7Action;
        message.MSH.MessageControlID.Value = Guid.NewGuid().ToString();
        message.MSH.ProcessingID.ProcessingID.Value = "P";
        message.MSH.VersionID.VersionID.Value = "2.5.1";

        message.EVN.EventTypeCode.Value = patient.Hl7Action;
        message.EVN.RecordedDateTime.Time.Value = DateTime.Now.ToString("yyyyMMddHHmmss");

        message.PID.SetIDPID.Value = "1";

        var patientId = message.PID.GetPatientIdentifierList(0);
        patientId.IDNumber.Value = patient.Id.ToString();
        patientId.IdentifierTypeCode.Value = "MR";
        patientId.AssigningAuthority.NamespaceID.Value = "HOSPITAL";

        var patientName = message.PID.GetPatientName(0);
        patientName.FamilyName.Surname.Value = patient.Surname;
        patientName.GivenName.Value = patient.Name;
        patientName.NameTypeCode.Value = "L";

        message.PID.DateTimeOfBirth.Time.Value = patient.BirthDate.ToString("yyyyMMdd");

        return parser.Encode(message);
    }
}