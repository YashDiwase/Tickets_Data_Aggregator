// See https://aka.ms/new-console-template for more information
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

Console.WriteLine("Hello, World!");

string currentDirectory = Directory.GetCurrentDirectory();

DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
string directoryPath = "";
int levelsUp = 4; // Change this to the number of levels you want to go up
for (int i = 0; i < levelsUp; i++)
{
    if (directoryInfo.Parent != null)
    {
        directoryInfo = directoryInfo.Parent;
    }
    else
    {
        Console.WriteLine("Reached the root directory.");
        break;
    }
}
directoryPath = directoryInfo.FullName;

directoryPath +="\\Tickets";

Dictionary<string, string> domainToCultureMapping = new()
{
    ["com"] = "en-US",
    ["fr"] = "fr-FR",
    ["jp"] = "ja-JP"
};

// Get all PDF files in the directory
string[] pdfFiles = Directory.GetFiles(directoryPath, "*.pdf");
List<string> contents = new List<string>();
foreach (string pdfFile in pdfFiles)
{
    Console.WriteLine($"Reading file: {pdfFile}");

    // Open the PDF document
    using (PdfDocument document = PdfDocument.Open(pdfFile))
    {
        // Iterate through each page
        foreach (Page page in document.GetPages())
        {
            contents.Add(page.Text);
        }
    }
}
string filePath = "output.txt";
using StreamWriter writer = new StreamWriter(filePath, true);
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
foreach (var input in contents)
{
 
    string[] parts = input.Split(new string[] { "Title:", "Date:", "Time:" }, StringSplitOptions.None);
    string webAddress = parts[parts.Length - 1].Trim();
    // Find the last index of '.'
    int lastDotIndex = webAddress.LastIndexOf('.');
    string lastPart = "";
    if (lastDotIndex != -1 && lastDotIndex < webAddress.Length - 1)
    {
        // Extract the substring after the last '.'
         lastPart = webAddress.Substring(lastDotIndex + 1);
    }
    string ticketCulture = domainToCultureMapping[lastPart];

    for (int i = 1; i < parts.Length; i += 3)
    {
        
        var title = parts[i].Trim();
        var dateString = parts[i + 1].Trim();
        var timeString = parts[i + 2].Trim().Split("Visit us", StringSplitOptions.None)[0]; ;

        var date=DateOnly.Parse(dateString,new CultureInfo(ticketCulture));
        var time=TimeOnly.Parse(timeString,new CultureInfo(ticketCulture));
        
        var invariantDate= date.ToString(CultureInfo.InvariantCulture);
        var invariantTime = time.ToString(CultureInfo.InvariantCulture);


        writer.WriteLine($"{title,-40}\t\t\t\t|{invariantDate}|{invariantTime}");
       
    }
}