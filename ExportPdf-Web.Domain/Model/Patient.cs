namespace ExportPdf_Web.Domain.Model;

[Serializable]
public class Patient
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Document { get; set; }
    public string Chart { get; set; }
    public string Origin { get; set; }
    public List<Vaccine> Vaccines { get; set; }
}