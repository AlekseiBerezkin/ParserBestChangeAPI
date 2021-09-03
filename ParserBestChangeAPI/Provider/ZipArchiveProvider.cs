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
        public Dictionary<string, List<double>> dictionaryMinus = new Dictionary<string, List<double>>();
        public Dictionary<string, List<double>> dictionaryPlus = new Dictionary<string, List<double>>();
        public async Task GetMassData(string FileName)
        {

            using (ZipArchive archive = ZipFile.Open(_fileName, ZipArchiveMode.Read))
            {
                ZipArchiveEntry entry = archive.GetEntry(FileName);
                ParseTxt pt = new ParseTxt("IdCur.txt");
                using (StreamReader reader = new StreamReader(entry.Open()))
                {
                    Rates objrates = new Rates();
                    // List<Rates> ratesList = new List<Rates>();
                    
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] splitData = line.Split(";");
                        if (pt.ConteinCurrency(splitData[0]) && pt.ConteinCurrency(splitData[1]))
                        {

                        if (!(splitData[3] == "1"))
                        {
                            if (!dictionaryMinus.ContainsKey(splitData[0] + "-" + splitData[1]))
                            {
                                //add
                                List<double> lr = new List<double>();
                                lr.Add(double.Parse(splitData[3], CultureInfo.InvariantCulture));
                                dictionaryMinus.Add(splitData[0] + "-" + splitData[1], lr);
                            }
                            else
                            {
                                dictionaryMinus[splitData[0] + "-" + splitData[1]].Add(double.Parse(splitData[3], CultureInfo.InvariantCulture));
                            }
                        }
                        else
                        {
                            if (!dictionaryPlus.ContainsKey(splitData[0] + "-" + splitData[1]))
                            {
                                //add
                                List<double> lr = new List<double>();
                                lr.Add(double.Parse(splitData[4], CultureInfo.InvariantCulture));
                                dictionaryPlus.Add(splitData[0] + "-" + splitData[1], lr);
                            }
                            else
                            {
                                dictionaryPlus[splitData[0] + "-" + splitData[1]].Add(double.Parse(splitData[4], CultureInfo.InvariantCulture));
                            }
                        }
                    }
                    }
                    
                }

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

        public async Task<string> GetDataTimeUpdate(string FileName)
        {
            using (ZipArchive archive = ZipFile.Open(_fileName, ZipArchiveMode.Read))
            {
                ZipArchiveEntry entry = archive.GetEntry(FileName);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using (StreamReader reader = new StreamReader(entry.Open(), Encoding.GetEncoding(1251)))
                {
                    var dictionary = new Dictionary<string, string>();

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] splitData = line.Split("=");

                        string[] splitData2 = splitData[1].Split(",");
                        return splitData2[0];

                    }
                    return null;
                }

            }

        }
        public void Dispose()
        {
            
        }
    }
}
