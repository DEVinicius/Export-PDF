using ExportPdf_Web.Application.Services.FileExport;
using ExportPdf_Web.Domain.Model;
using ExportPdf_Web.Domain.Tools;
using Microsoft.AspNetCore.Mvc;

namespace ExportPdf_Web.API.Controllers;

public class FileController : ControllerBase
{
    [HttpPost]
    [Route("/file")]
    public async Task<IActionResult> ExportPdf(
        [FromServices] IExportPdf exportPdf,
        [FromBody] Patient patient
    )
    {
        try
        {
            var exportFilePdf = new ExportPdfService(exportPdf);
            await exportFilePdf.Execute(patient);
            return Ok(patient);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}