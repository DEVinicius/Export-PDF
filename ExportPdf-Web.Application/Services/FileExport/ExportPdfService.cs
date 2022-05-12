using System.Net;
using ExportPdf_Web.Domain.Model;
using ExportPdf_Web.Domain.Tools;

namespace ExportPdf_Web.Application.Services.FileExport;

public class ExportPdfService
{
    private IExportPdf _exportPdf;
    public ExportPdfService(IExportPdf exportPdf)
    {
        this._exportPdf = exportPdf;
    }

    public async Task Execute(Patient patient)
    {
        _exportPdf.CreateFile(patient);
       
    }
}