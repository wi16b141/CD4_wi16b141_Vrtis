using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CD4_Server.DataHandling
{
    public class DataHandler
    {
        /*
         * Speichert LogFiles (inkl. Messages)
         * Ruft LogFiles ab
         * Ruft Messages eines LogFiles ab
         * Löscht LogFiles
         */

        private const string folder = "Files/";
        private const string filename = "LogFile";
        private static int counter = 1;
        private const string extension = ".txt";

        //speichert LogFile ab
        public void SaveLogFile(List<string> data)
        {
            File.WriteAllLines(folder + filename + counter + extension, data.ToArray());
            counter++; //Counter erhöhen
        }

        //liefert alle LogFiles zurück
        public string[] GetAllLogFiles()
        {
            //Directory erstellen, falls noch nicht vorhanden
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            //DirectoryInfo instanzieren
            DirectoryInfo info = new DirectoryInfo(folder);

            //Alle Files mit Endung .txt aus Directory auslesen
            var result = info.GetFiles("*" + extension);

            //Files in Array abspeichern
            string[] temp = new string[result.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = result[i].Name;
            }
            return temp;
        }

        //liefert alle Messages eines LogFiles
        public string[] GetAllMessagesFromLogFile(string fileName)
        {
            string[] temp = File.ReadAllLines(folder + fileName);
            return temp;
        }

        //löscht LogFiles
        public void DeleteLogFiles(string fileName)
        {
            if (File.Exists(folder + fileName))
            {
                File.Delete(folder + fileName);
            }
        }


    }
}
