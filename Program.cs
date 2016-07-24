using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using iTextSharp.text.pdf;

namespace EmbedXMP
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: EmbedXMP input-PDF input-XMP output-PDF");
                return;
            }
            EmbedXmpToPdf(args[0], args[1], args[2]);
        }

        static void EmbedXmpToPdf(string inputPdfFile, string inputXmpFile, string outputPdfFile)
        {
            var pdfReader = new PdfReader(inputPdfFile);
            using (var output = File.Create(outputPdfFile))
            using (var pdfStamper = new PdfStamper(pdfReader, output))
            {
                pdfStamper.SetFullCompression();
                pdfStamper.XmpMetadata = MakeXmpPacket(inputXmpFile);
            }
        }

        static byte[] MakeXmpPacket(string inputXmpFile)
        {
            var meta = XNamespace.Get("adobe:ns:meta/");
            var input = XDocument.Load(inputXmpFile);
            var xmpmeta = input.Element(meta + "xmpmeta");
            var output = new XDocument(
                new XProcessingInstruction("xpacket", "begin='\xFEFF' id='W5M0MpCehiHzreSzNTczkc9d'"),
                xmpmeta,
                new XProcessingInstruction("xpacket", "end='r'")
            );
            var s = new MemoryStream();
            using (var writer = new StreamWriter(s, new UTF8Encoding(false)))
            {
                output.Save(writer, SaveOptions.DisableFormatting);
            }
            return s.ToArray();
        }
    }
}
