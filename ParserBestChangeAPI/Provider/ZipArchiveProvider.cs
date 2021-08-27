using ParserBestChangeAPI.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserBestChangeAPI.Provider
{
    public class ZipArchiveProvider : IDisposable
    {
        //ZipArchive archive;
        string _fileName;
       public ZipArchiveProvider(string fileName)
        {
            _fileName = fileName;
            //ZipArchive archive = ZipFile.Open(fileName, ZipArchiveMode.Read);
            
        }

        public async Task<Dictionary<string, List<double>>> GetMassData(string FileName)
        {

            using (ZipArchive archive = ZipFile.Open(_fileName, ZipArchiveMode.Read))
            {
                ZipArchiveEntry entry = archive.GetEntry(FileName);
                using (StreamReader reader = new StreamReader(entry.Open()))
                {
                    Rates objrates = new Rates();
                    // List<Rates> ratesList = new List<Rates>();
                    var dictionary = new Dictionary<string, List<double>>();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] splitData = line.Split(";");

                        if (!dictionary.ContainsKey(splitData[0] + "-" + splitData[1]))
                        {
                            //add
                            List<double> lr = new List<double>();
                            lr.Add(Math.Max(double.Parse(splitData[3], CultureInfo.InvariantCulture), double.Parse(splitData[4], CultureInfo.InvariantCulture)));
                            dictionary.Add(splitData[0] + "-" + splitData[1], lr);
                        }
                        else
                        {
                            dictionary[splitData[0] + "-" + splitData[1]].Add(Math.Max(double.Parse(splitData[3], CultureInfo.InvariantCulture), double.Parse(splitData[4], CultureInfo.InvariantCulture)));
                        }

                        //dictionary.Add(splitData[0]+"-"+splitData[1], new Rates {ID1=splitData[0], ID2 = splitData[1], ExchangeRates1 = splitData[3], ExchangeRates2 = splitData[4] });
                        // rates.Append(line+"\n");
                    }
                    return dictionary;
                }

                return null;
            }

                
        }


        public async Task<Dictionary<string, string>> GetCurrencys (string FileName)
        {
            using (ZipArchive archive = ZipFile.Open(_fileName, ZipArchiveMode.Read))
            {
                ZipArchiveEntry entry = archive.GetEntry(FileName);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using (StreamReader reader = new StreamReader(entry.Open(),Encoding.GetEncoding(1251)))
                {
                    var dictionary = new Dictionary<string, string>();

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] splitData = line.Split(";");
                        byte[] bytes = Encoding.Default.GetBytes(splitData[2]);

                        dictionary.Add(splitData[0], Encoding.UTF8.GetString(bytes));


                    }
                    return dictionary;
                }

            }

        }
        public void Dispose()
        {
            
        }
    }
}
