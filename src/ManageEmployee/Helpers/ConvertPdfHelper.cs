using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace ManageEmployee.Helpers;

public interface IHtmlToPdfConverter
{
    Task ConvertHtmlToPdfAsync(string htmlContent, string outputPath);
    Task ExportPdf(string html, string prefixFile);
}
public class HtmlToPdfConverter: IHtmlToPdfConverter
{
    public HtmlToPdfConverter()
    {

    }
    public async Task ConvertHtmlToPdfAsync(string htmlContent, string outputPath)
    {
        // Download the Chromium browser if not already downloaded
        var html = File.ReadAllText("invoice.html");

        var pdfOptions = new PuppeteerSharp.PdfOptions();
        //pdfOptions.Format = PuppeteerSharp.PaperFormat.A4;

        using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = "CHROME_BROWSER_PATH"
        }))
        {
            using (var page = await browser.NewPageAsync())
            {
                await page.SetContentAsync(html);
                await page.PdfAsync("invoice.pdf", pdfOptions);
            }
        }
    }


    public async Task ExportPdf(string html, string prefixFile)
    {
        //var downloadPath = Path.Combine(Directory.GetCurrentDirectory(), "Chromium");
        //var browserFetcherOptions = new BrowserFetcherOptions
        //{
        //    Path = downloadPath
        //};

        //await new BrowserFetcher(browserFetcherOptions).DownloadAsync();

        var folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\PDF");
        string fileName = prefixFile.Trim() + "_" + DateTime.Now.Ticks.ToString() + ".pdf";
        var fileSave = Path.Combine(folder, fileName);

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            //ExecutablePath = Path.Combine(downloadPath, "chrome-win", "chrome.exe")
        }))
        {
            using (var page = await browser.NewPageAsync())
            {
                await page.SetContentAsync(html);

                // Define the PDF options
                var pdfOptions = new PdfOptions
                {
                    Format = PaperFormat.A4,
                    PrintBackground = true,
                    MarginOptions = new MarginOptions
                    {
                        Left = "15",
                    },
                    FooterTemplate = "Trang [page] / [toPage]"
                };

                // Generate the PDF and save it to a file
                await page.PdfAsync(fileSave, pdfOptions);
            }
        }
    }

}
