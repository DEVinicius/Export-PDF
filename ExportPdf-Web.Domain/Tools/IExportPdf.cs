using ExportPdf_Web.Domain.Model;

namespace ExportPdf_Web.Domain.Tools;

public interface IExportPdf
{
    public void CreateFile(Patient patient);
}