using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParserBestChangeAPI.Model
{
    public  class CreateLink
    {
         private string baseLink = "https://www.bestchange.ru/";
        Dictionary<string, string> nameLink = new Dictionary<string, string>();

        public CreateLink(string Name)
        {
            using (StreamReader reader = new StreamReader(Name))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] splitData = line.Split(":");

                    nameLink.Add(splitData[0], splitData[1]);
                }
                
            }
        }
        public string getLink(string pair)
        {
            string[] splitData=pair.Split(":");

            return baseLink+getLinkName(splitData[0])+"-to-"+ getLinkName(splitData[1])+".html";

        }

        private string getLinkName(string name)
        {   try
            {
                return nameLink[name];
            }
            catch(Exception ex)
            {
                Program.Logger.Info("Исключение при генерации ссылки " + ex);
                return "noCurrency";
            }
            
        }
    }
}
