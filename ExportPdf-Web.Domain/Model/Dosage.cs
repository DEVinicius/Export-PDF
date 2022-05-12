namespace ExportPdf_Web.Domain.Model;

[Serializable]
public class Dosage
{
    public string Name { get; set; }
    public DateTime ApplicationDate { get; set; }
    public string ApplicationPlace { get; set; }
}