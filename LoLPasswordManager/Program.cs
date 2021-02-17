using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoLPasswordManager
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {

            using (var fs = File.Create("html-archive.zip"))
            {
                using (var outStream = new ZipOutputStream(fs))
                {
                    outStream.Password = "Hallo";
                    outStream.PutNextEntry(new ZipEntry("content.txt"));
                    using (var sw = new StreamWriter(outStream))
                    {
                        sw.Write("Hello World");
                    }
                }
            }


            using (var fs = File.OpenRead("html-archive.zip"))
            {
                using (var zf = new ZipFile(fs))
                {
                    zf.Password = "Hallo";
                    using (var zipStream = zf.GetInputStream(zf.GetEntry("content.txt")))
                    {
                        using (var sr = new StreamReader(zipStream))
                        {
                            MessageBox.Show(sr.ReadToEnd());
                        }
                    }
                }
            }

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new mainForm());
        }
  }
}
