/* ****************************************************************************

    Test sample to fetch basic info from Belgium ID Cards 
    Author: Nicolas Van Wallendael <nicolasvanwallendael@icloud.com>
    Copyright (C) 2017, Nicolas Van Wallendael

    Inspired and using :: http://github.com/Fedict/eid-mw

**************************************************************************** */
using System;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;


namespace EidSamples
{
    class Program
    {
        // Dont forget to link Net.Pkcs from wrapper/build/x**
        static void Main(string[] args)
        {
            
            Console.WriteLine("Starting the Card reading ...");
            System.Diagnostics.Debug.WriteLine("Starting the Card reading ...");

            CardData cardData = new CardData("beidpkcs11.dll");

            // Get all STRING fields

            Console.WriteLine("\nAll String Info + Photo...");
            Console.WriteLine("Asking Authorization ...\n");


            String[] labels = new String[] { "surname", "firstnames", "nationality", "location_of_birth",
                                             "date_of_birth", "gender", "address_street_and_number",
                                             "address_zip", "address_municipality"};

            String[] files = new String[] { "PHOTO_FILE" };

            String[] outLabels = new String[labels.Length];
            byte[][] outFiles  = new byte  [files.Length ][];
            

            cardData.GetData( labels, files, outLabels, outFiles);

            Console.WriteLine("\n\nSave and open Img File ...");

            byte[] bitmap = outFiles[0];

            using (Image image = Image.FromStream(new System.IO.MemoryStream(bitmap)))
            {
                image.Save("output.jpg", ImageFormat.Jpeg);  // Or Png
            }

            Process.Start(@"Y:\eid-mw-4.2.0\doc\sdk\examples\CS\EidSamples\bin\x64\Debug\output.jpg");
            Console.WriteLine("\n Sleep 5");
            Thread.Sleep(5000);
        }
    }
}
