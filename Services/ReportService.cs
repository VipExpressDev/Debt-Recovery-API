namespace DebtRecoveryPlatform.Services
{
    using DebtRecoveryPlatform.Models.DTO;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using OfficeOpenXml.Table;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Reflection.PortableExecutable;

    public class ReportService
    {
        public byte[] GenerateCollectorPerformanceReport(List<CollectorPerformanceDto> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Collector Performance");

            // Add Headers
            //worksheet.Cells[1, 1].Value = "Collector Name";
            //worksheet.Cells[1, 2].Value = "Amount Assigned";
            //worksheet.Cells[1, 3].Value = "Amount Collected";
            //worksheet.Cells[1, 4].Value = "Outstanding";
            //worksheet.Cells[1, 5].Value = "Unactioned";
            //worksheet.Cells[1, 6].Value = "Collection Rate (%)";

            // HEADERS
            string[] headers = new[]
            {
            "Collector Name",
            "Amount Assigned",
            "Amount Collected",
            "Outstanding",
            "Unactioned",
            //"Collection Rate (%)"
        };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            // 🎨 Header Styling
            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Font.Size = 12;
                range.Style.Font.Name = "Calibri";
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // background color (light blue)
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228)); // light blue

                // border
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }

            // Fill Data
            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = data[i].CollectorName;

                worksheet.Cells[i + 2, 2].Value = (double)data[i].AmountAssigned;
                worksheet.Cells[i + 2, 3].Value = (double)data[i].AmountCollected;
                worksheet.Cells[i + 2, 4].Value = (double)data[i].Outstanding;
                worksheet.Cells[i + 2, 5].Value = (double)data[i].Unactioned;
               // worksheet.Cells[i + 2, 6].Value = (data[i].CollectionRate);
               // worksheet.Cells[i + 2, 6].Style.Numberformat.Format = "0.00%";
            }

            //worksheet.Cells.AutoFitColumns();

            // 💰 Format Amount Columns (2–5) as 2-decimal numeric
            using (var amountRange = worksheet.Cells[2, 2, data.Count + 1, 5])
            {
                amountRange.Style.Numberformat.Format = "#,##0.00";
                amountRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            // 📊 Format Percentage Column
            using (var rateRange = worksheet.Cells[2, 6, data.Count + 1, 6])
            {
                rateRange.Style.Numberformat.Format = "0.00%";
                rateRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            // Auto-fit columns and freeze header
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.View.FreezePanes(2, 1);

            return package.GetAsByteArray();
        }

        public byte[] DebtorEngagementSummaryReport(List<DebtorEngagementSummaryDTO> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Debtor Engagement Summary Report");

            // Add Headers
            //worksheet.Cells[1, 1].Value = "Collector Name";
            //worksheet.Cells[1, 2].Value = "Amount Assigned";
            //worksheet.Cells[1, 3].Value = "Amount Collected";
            //worksheet.Cells[1, 4].Value = "Outstanding";
            //worksheet.Cells[1, 5].Value = "Unactioned";
            //worksheet.Cells[1, 6].Value = "Collection Rate (%)";

            // HEADERS
            string[] headers = new[]
            {
            "Status",
            "Count",
            "Amount Total",
            "PortfolioPercent (%)"
        };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            // 🎨 Header Styling
            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Font.Size = 12;
                range.Style.Font.Name = "Calibri";
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // background color (light blue)
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228)); // light blue

                // border
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }

            // Fill Data
            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = data[i].Status;

                worksheet.Cells[i + 2, 2].Value = data[i].Count;
                worksheet.Cells[i + 2, 3].Value = (double)data[i].TotalAmount;
                worksheet.Cells[i + 2, 4].Value = (data[i].PortfolioPercent);
                worksheet.Cells[i + 2, 4].Style.Numberformat.Format = "0.00%";
            }

            //worksheet.Cells.AutoFitColumns();

            // 💰 Format Amount Columns (2–5) as 2-decimal numeric
            using (var amountRange = worksheet.Cells[2, 2, data.Count + 1, 5])
            {
                amountRange.Style.Numberformat.Format = "#,##0.00";
                amountRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            // 📊 Format Percentage Column
            using (var rateRange = worksheet.Cells[2, 4, data.Count + 1, 6])
            {
                rateRange.Style.Numberformat.Format = "0.00%";
                rateRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            // Auto-fit columns and freeze header
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.View.FreezePanes(2, 1);

            return package.GetAsByteArray();
        }

        public byte[] FeedBackReport(List<DebtorFeedbackDTO> data)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Debtor FeedBack Report");

           
            // HEADERS
            string[] headers = new[]
            {
            "ContractNo",
            "DateCreated",
            "CreatedBy",
            "FeedBack"
            
        };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            // 🎨 Header Styling
            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Font.Size = 12;
                range.Style.Font.Name = "Calibri";
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // background color (light blue)
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228)); // light blue

                // border
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }

            // Fill Data
            for (int i = 0; i < data.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = data[i].ContractNo;

                worksheet.Cells[i + 2, 2].Value = data[i].DateCreated.ToString("dd/mm/yyyy");
                worksheet.Cells[i + 2, 3].Value = data[i].CreatedBy;
                worksheet.Cells[i + 2, 4].Value = data[i].Comment;


            }

            worksheet.Cells.AutoFitColumns();

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.View.FreezePanes(2, 1);

            return package.GetAsByteArray();
        }

    }

}
