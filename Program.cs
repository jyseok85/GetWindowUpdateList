using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;

namespace GetWindowUpdateList
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            bool isError = IsInstalledErrorHotFix();
        }

        public static bool IsInstalledErrorHotFix()
        {
            List<string> installedHotfixlist = GetHotFixList();
            foreach (var x in installedHotfixlist)
            {
                Console.WriteLine(x);
            }

            if (installedHotfixlist.Count == 0)
                return false;
            else
            {

                string filePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ErrorHotFix.txt");
                string[] list =  System.IO.File.ReadAllLines(filePath);

                List<string> result = installedHotfixlist.Where(x => list.Count(s => x.Contains(s)) != 0).ToList();
                if (result.Count > 0)
                    return true;
                else
                    return false;
            }
        }
        public static List<string> GetHotFixList()
        {
            List<string> list = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(new ObjectQuery("select * from Win32_QuickFixEngineering"));
            searcher.Options.UseAmendedQualifiers = true;
            searcher.Scope.Options.Locale = "MS_" + CultureInfo.CurrentCulture.LCID.ToString("X");
            ManagementObjectCollection results = searcher.Get();

            foreach (ManagementObject item in results)
            {
                foreach (var x in item.Properties)
                {
                    if (x.Name.ToUpper() == "HOTFIXID")
                        list.Add(item[x.Name].ToString());
                }
            }

            return list;
        }
    }
}
