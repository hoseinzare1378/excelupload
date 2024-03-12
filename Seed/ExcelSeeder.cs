using System;
using System.Linq;
using ClosedXML.Excel;
using excelupload.Models;
using NPOI.SS.Formula.Functions;

namespace excelupload.Seed;

public class ExcelSeeder
{
    public static void CreateSeedExcelFile(string filePath)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("TodoItems");
        
        
        worksheet.Cell(1, 1).Value = "ListId";
        worksheet.Cell(1, 2).Value = "Title";
        worksheet.Cell(1, 3).Value = "Note";
        worksheet.Cell(1, 4).Value = "Priority";
        worksheet.Cell(1, 5).Value = "Reminder";
        Random random = new Random();
        
        for (int i = 0; i <= 5000; i++)
        {
            worksheet.Cell(i + 2, 1).Value = i; 
            worksheet.Cell(i + 2, 2).Value = $"Task {i}"; 
            worksheet.Cell(i + 2, 3).Value = $"This is a note for task {i}."; 

            var priorityValue = random.Next(1, 5);
            worksheet.Cell(i + 2, 4).Value = priorityValue.ToString(); 

            worksheet.Cell(i + 2, 5).Value = DateTime.Now.AddDays(i).ToShortDateString(); 
        }

        workbook.SaveAs(filePath);
    }
}
