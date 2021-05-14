using Microsoft.AspNetCore.Cors;
using Microsoft.Office.Interop.Excel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TrackingPackages.Models;
using TrackingPackages.ViewModels.Packages;

namespace TrackingPackages.Controllers
{
    public class FilesController : TrackingPackagesController
    {
        public async Task MultipleCreate(HttpPostedFileBase file)
        {
            string CurrentFilePath;
            if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/File/" + file.FileName)))
            {
                Random rnd = new Random();
                int random = rnd.Next();
                CurrentFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/File/" + file.FileName + random);
                file.SaveAs(CurrentFilePath);
            }
            else
            {
                CurrentFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/File/" + file.FileName);
                file.SaveAs(CurrentFilePath);
            }
            ExcelFile excel = new ExcelFile(CurrentFilePath, 1);

            for (int i = 2; i < excel.Range.Rows.Count; i++)
            {
                //Ex. Iterate through the row's data and put in a string array
                String[] rowData = new String[excel.Range.Rows[i].Columns.Count];
                for (int j = 0; j < excel.Range.Rows[i].Columns.Count; j++)
                    rowData[j] = Convert.ToString(excel.Range.Rows[i].Cells[1, j + 1].Value2);

                if (String.IsNullOrWhiteSpace(rowData[0])) continue;

                double d = double.Parse(rowData[0]);
                DateTime date = DateTime.FromOADate(d);
                Enum.TryParse(rowData[1], out PacketSize packet);
                string weight = rowData[2];
                string zipCode = rowData[3];
                string location = rowData[4];
                bool.TryParse(rowData[5], out bool isFragile);
                bool.TryParse(rowData[6], out bool hasValueToPay);

                PackagesCreateViewModel viewModel = new PackagesCreateViewModel(zipCode, location, date);

                string trackingCode = GenerateTrackingCode(viewModel);

                if (string.IsNullOrWhiteSpace(trackingCode)) continue;

                Package package = new Package
                {
                    ReceiveDate = viewModel.ReceiveDate.Value,
                    PacketSize = packet,
                    Weight = weight,
                    IsFragile = isFragile,
                    HasValueToPay = hasValueToPay,
                    TrackingCode = trackingCode,
                    Status = Status.Received
                };
                Db.Packages.Add(package);
            }
            await Db.SaveChangesAsync();
            excel.Wb.Close(0);

            foreach (Process clsProcess in Process.GetProcesses())
                if (clsProcess.ProcessName.Equals("EXCEL"))  //Process Excel?
                    clsProcess.Kill();

            if (File.Exists(CurrentFilePath))
            {
                File.Delete(CurrentFilePath);
            }
        }

        private string GenerateTrackingCode(PackagesCreateViewModel viewModel)
        {
            try
            {
                string isoCode = GetIso(viewModel.Location);
                string zipCode = viewModel.ZipCode.Contains("-") ? viewModel.ZipCode.Replace("-", "") : viewModel.ZipCode;
                string identification = Guid.NewGuid().ToString().Split('-').LastOrDefault().Substring(0, 6);
                string date = viewModel.ReceiveDate.Value.ToString("dd/MM/yy");

                string[] dateSplit = date.Split('/');
                int day = int.Parse(dateSplit[0].ElementAt(0).ToString());
                int month = int.Parse(dateSplit[1].ElementAt(1).ToString());
                int controlDigitOne = (day + month + 20);
                while (controlDigitOne > 10)
                {
                    controlDigitOne = controlDigitOne.ToString().Select(digit => int.Parse(digit.ToString())).ToList().Aggregate((a, b) => a + b);
                }

                int isoNumericCode = GetIsoNumericCode(viewModel.Location);
                int controlDigitTWo = (isoNumericCode + int.Parse(dateSplit[2].ElementAt(1).ToString())).ToString().Select(digit => int.Parse(digit.ToString())).ToList().Aggregate((a, b) => a + b);
                return $"{isoCode}-{zipCode}-{identification}-{date.Replace("/", "")}-{controlDigitOne}{controlDigitTWo}";
            }
            catch
            {
                return "";
            }

        }
        private string GetIso(string location)
        {
            switch (location.ToUpper())
            {
                case "PORTUGAL":
                    return "PT";
                default:
                    return "";
            }
        }
        private int GetIsoNumericCode(string location)
        {
            switch (location.ToUpper())
            {
                case "PT":
                case "PORTUGAL":
                case "PRT":
                    return 620;
                case "United Kingdom":
                case "GB":
                case "GBR":
                    return 826;
                default:
                    return 0;
            }
        }
    }
}