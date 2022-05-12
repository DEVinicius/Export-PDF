namespace ExportPdf_Web.Domain.Model;

[Serializable]
public class Vaccine
{
    public string Name { get; set; }
    public List<Dosage> Dosages { get; set; }
}