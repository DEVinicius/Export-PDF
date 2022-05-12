using System.Net;
using ExportPdf_Web.Domain.Model;
using ExportPdf_Web.Domain.Tools;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ExportPdf_Web.Application.Tools;

public class ExportPdfITextSharp : IExportPdf
{
    private BaseFont fontBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

    private void CreateTableCell(PdfPTable table, string text, BaseColor backgroundColor, BaseColor fontColor, int horizontalAlignment = PdfPCell.ALIGN_CENTER,
        bool bold = false, bool italic = false, int fontSize = 12, int cellHeight = 25, int padding = 0)
    {

        int styleFont = iTextSharp.text.Font.NORMAL;

        if (bold && italic)
        {
            styleFont = iTextSharp.text.Font.BOLDITALIC;
        } else if (bold)
        {
            styleFont = iTextSharp.text.Font.BOLD;
        } else if (italic)
        {
            styleFont = iTextSharp.text.Font.ITALIC;
        }
        
        var fonteCelula = new iTextSharp.text.Font(fontBase, fontSize, styleFont, fontColor);
        var celula = new PdfPCell(new Phrase(text, fonteCelula));

        celula.BackgroundColor = backgroundColor;
        celula.HorizontalAlignment = horizontalAlignment;
        celula.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
        celula.Border = 0;
        celula.Padding = padding;
        celula.FixedHeight = cellHeight;
                
        table.AddCell(celula);
    }

    private void CreateDosageCell(PdfPTable table, string applicationDate, string applicationPlace,
        BaseColor backgroundColor, BaseColor fontColor, int horizontalAlignment = PdfPCell.ALIGN_CENTER,
        bool bold = false, bool italic = false, int fontSize = 10, int cellHeight = 55)
    {
        int styleFont = iTextSharp.text.Font.NORMAL;

        if (bold && italic)
        {
            styleFont = iTextSharp.text.Font.BOLDITALIC;
        } else if (bold)
        {
            styleFont = iTextSharp.text.Font.BOLD;
        } else if (italic)
        {
            styleFont = iTextSharp.text.Font.ITALIC;
        }
        var fonteCelula = new iTextSharp.text.Font(fontBase, fontSize, styleFont, fontColor);

        PdfPTable DosageTable = new PdfPTable(1);
        
        CreateTableCell(DosageTable, "Data de Aplicação", new BaseColor(243, 246, 249), BaseColor.Black,PdfPCell.ALIGN_CENTER, true, false,6, 10);

        CreateTableCell(DosageTable, applicationDate, new BaseColor(243, 246, 249), BaseColor.Black,PdfPCell.ALIGN_CENTER, false, false,6, 13);
        CreateTableCell(DosageTable, "Local", new BaseColor(243, 246, 249), BaseColor.Black,PdfPCell.ALIGN_CENTER, true, false,6, 10);
        CreateTableCell(DosageTable, applicationPlace, new BaseColor(243, 246, 249), BaseColor.Black,PdfPCell.ALIGN_CENTER, false, false,6, 13);

        var celula = new PdfPCell(DosageTable);

        celula.BackgroundColor = backgroundColor;
        celula.HorizontalAlignment = horizontalAlignment;
        celula.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
        celula.Border = 0;
        celula.Padding = 1;
        celula.FixedHeight = cellHeight;
        
        table.AddCell(celula);
    }

    private void CreateEmptyDosageCell(PdfPTable table, int horizontalAlignment = PdfPCell.ALIGN_CENTER,
        bool bold = false, bool italic = false, int fontSize = 10, int cellHeight = 55)
    {
        int styleFont = iTextSharp.text.Font.NORMAL;

        if (bold && italic)
        {
            styleFont = iTextSharp.text.Font.BOLDITALIC;
        } else if (bold)
        {
            styleFont = iTextSharp.text.Font.BOLD;
        } else if (italic)
        {
            styleFont = iTextSharp.text.Font.ITALIC;
        }

        PdfPTable DosageTable = new PdfPTable(1);
        
        CreateTableCell(DosageTable, "", BaseColor.White, BaseColor.Black,PdfPCell.ALIGN_CENTER, true, false,6, 10);

        var celula = new PdfPCell(DosageTable);

        celula.BackgroundColor = BaseColor.White;
        celula.HorizontalAlignment = horizontalAlignment;
        celula.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
        celula.Border = 0;
        celula.Padding = 1;
        celula.FixedHeight = cellHeight;
        
        table.AddCell(celula);
    }

    private void WriteText(PdfWriter writer, string text, int xPosition, int yPosition, BaseFont baseFont, int fontSize = 12 )
    {
        var textWriter = writer.DirectContent;
        textWriter.SaveState();
        textWriter.BeginText();
        textWriter.MoveText(xPosition, yPosition);
        textWriter.SetFontAndSize(baseFont, fontSize);
        textWriter.ShowText(text);
        textWriter.EndText();
        textWriter.RestoreState();
    }

    private void AddImage(PdfWriter writer, string image, float imageHeight, float marginLeft, float marginTop)
    {
        if (File.Exists(image))
        {
            iTextSharp.text.Image writerImage = iTextSharp.text.Image.GetInstance(image);
            float razaoAlturaLargura = writerImage.Width / writerImage.Height;
            
            float imageWidth = imageHeight * razaoAlturaLargura;
            writerImage.ScaleToFit(imageWidth, imageHeight);
            
            writerImage.SetAbsolutePosition(marginLeft, marginTop);
            writer.DirectContent.AddImage(writerImage, false);
        }
    }

    private void CreateFooter(PdfWriter writer, float width )
    {
        PdfPTable footer = new PdfPTable(3);
        footer.TotalWidth = width;
        
        CreateTableCell(footer, "Este documento é para fins informativos, tenha sempre em mãos sua caderneta física.", new BaseColor(181, 216, 241), BaseColor.Black,PdfPCell.ALIGN_CENTER, false,false,6);
        
        CreateTableCell(footer, "Para dúvidas: (11) 2151-1233 ", new BaseColor(181, 216, 241), BaseColor.Black,PdfPCell.ALIGN_CENTER, false,false,8);
        
        CreateTableCell(footer, "Página 1 de 1", new BaseColor(181, 216, 241),BaseColor.Black, PdfPCell.ALIGN_RIGHT, false,false,8, 25, 3);

        footer.WriteSelectedRows(0, -1, 0, footer.TotalHeight - 5, writer.DirectContent);
    }
    
    public void CreateFile(Patient patient)
    {
        var pxPorMm = 72 / 25.2F;

        var pdf = new Document(PageSize.A4.Rotate(), 15 * pxPorMm, 15 * pxPorMm, 0 * pxPorMm, 0 * pxPorMm);
        var nomeArquivo = $"file.{DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss")}.pdf";

        var arquivo = new FileStream(nomeArquivo, FileMode.Create);
        var writer = PdfWriter.GetInstance(pdf, arquivo);
        
        pdf.Open();

        var fonteParagrafo = new iTextSharp.text.Font(fontBase, 24, iTextSharp.text.Font.BOLD, new BaseColor(0, 90, 156));

        var titulo = new Paragraph("Carteirinha de Vacinação\n\n", fonteParagrafo);
        titulo.Alignment = Element.ALIGN_LEFT;

        pdf.Add(titulo);

        var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "detalhe.png");
        var companyLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "companyLogo.png");
        var patientIcon = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patientIcon.png");

        if (!File.Exists(imagePath))
        {
            using (WebClient web1 = new WebClient())
                web1.DownloadFile("https://upload-icons-einstein-conecta-hml.s3.amazonaws.com/FaixaEinstein.png", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "detalhe.png")); 
        }

        if (!File.Exists(companyLogo))
        {
            using (WebClient web1 = new WebClient())
                web1.DownloadFile("https://upload-icons-einstein-conecta-hml.s3.amazonaws.com/NovoLogo.png", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "companyLogo.png")); 

        }
        
        if (!File.Exists(patientIcon))
        {
            using (WebClient web1 = new WebClient())
                web1.DownloadFile("https://upload-icons-einstein-conecta-hml.s3.amazonaws.com/paciente.png", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "patientIcon.png"));
        }

        AddImage(writer, imagePath, 6, 1, pdf.PageSize.Height - 4);
        AddImage(writer, companyLogo, 40, pdf.PageSize.Width - 200, pdf.PageSize.Height - 60);
        AddImage(writer, patientIcon, 25, 40, pdf.PageSize.Height - 100);

        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        WriteText(writer, "Nome: " + patient.Name, 80, (int) (pdf.PageSize.Height - 85), bf);
        WriteText(writer, "Nascimento: " + patient.BirthDate.ToString("dd/MM/yyyy"), 80, (int) (pdf.PageSize.Height - 105), bf);
        WriteText(writer, "CPF ou Passaporte: " + patient.Document, 80, (int) (pdf.PageSize.Height - 125), bf);
        WriteText(writer, "Prontuário: " + patient.Chart, 80, (int) (pdf.PageSize.Height - 145), bf);
        WriteText(writer, "Fonte: " + patient.Origin, 80, (int) (pdf.PageSize.Height - 165), bf);

        var tabela = new PdfPTable(7);
        tabela.DefaultCell.BorderWidth = 0;
        tabela.TotalWidth = pdf.PageSize.Width - pdf.LeftMargin - pdf.RightMargin;;
        tabela.WidthPercentage = 85;
        
        float[] widths = new float[] { 10f, 25f, 25f, 25f, 25f, 25f, 25f};
        tabela.SetWidths(widths);
        tabela.SpacingAfter = 1;

        CreateTableCell(tabela, "", BaseColor.White, BaseColor.White,PdfPCell.ALIGN_CENTER, true);
        foreach (var vac in patient.Vaccines)
        {
            CreateTableCell(tabela, vac.Name, new BaseColor(0, 90, 156), BaseColor.White,PdfPCell.ALIGN_CENTER, true, false,10);
        }
        
        var dosages = new List<string>();
        int maiorTamanhoVacinas = 0;
        string TipoDeVacinaComMaiorDosagem = string.Empty;

        foreach (var vac in patient.Vaccines)
        {
            if (vac.Dosages.Count > maiorTamanhoVacinas)
            {
                maiorTamanhoVacinas = vac.Dosages.Count;
                TipoDeVacinaComMaiorDosagem = vac.Name;

                var newDosages = new List<string>();

                foreach (var dos in vac.Dosages)
                {
                    newDosages.Add(dos.Name);
                }

                dosages = newDosages;
            }
        }
        
        //Casos de Teste
        
        /*
         * 1 - Caso Feliz, todos com a mesma quantidades de doses [X]
         * 2 - Caso em que algumas vacinas não possuem doses de reforço
         * 3 - Caso em que vacinas possuem mesmo tamanho em doses, porém nomes diferentes.
         * 4 - Caso em que uma vacina possui mais de 6 doses
         * 5 - Caso em que todas as vacinas são menores que 6 doses [X]
         */

        string[] dosagesArray = dosages.ToArray();
        
        for (int counter = 0; counter < maiorTamanhoVacinas; counter++)
        {
            CreateTableCell(tabela, dosagesArray[counter], new BaseColor(0, 90, 156), BaseColor.White,PdfPCell.ALIGN_CENTER, true, false,10,15);

            foreach (var vac in patient.Vaccines)
            {
                var vacDosageToArray = vac.Dosages.ToArray();
                if ((counter + 1) > vacDosageToArray.Length)
                {
                    CreateEmptyDosageCell(tabela);
                }
                else
                {
                    CreateDosageCell(tabela, vacDosageToArray[counter].ApplicationDate.ToString(), vacDosageToArray[counter].ApplicationPlace, new BaseColor(243, 246, 249), BaseColor.Black,PdfPCell.ALIGN_CENTER, false);
                }
                
            }
        }
        
        tabela.WriteSelectedRows(0, -1, pdf.LeftMargin, tabela.TotalHeight + 50, writer.DirectContent);
        
        CreateFooter(writer, pdf.PageSize.Width);

        pdf.Close();
        arquivo.Close();

        var caminhoPdf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeArquivo);
    }
}