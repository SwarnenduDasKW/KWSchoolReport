﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KofaxAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = string.Empty;

            //If the file path is send in the command line take that
            if (args.Length > 0)
            {
                filePath = args[0];
            }

            //If file name is not sent as argument take the default path
            if (filePath.Length == 0)
            {
                filePath = @"C:\temp\SchoolPopulation.txt";
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found in {0}", filePath);
                return;
            }

            if (!ProcessFile(filePath))
            {
                Console.WriteLine("File processing failed...");
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("File processed successfully!!");
            }

            Console.ReadLine();
        }

        public static bool ProcessFile(string filePath)
        {

            Console.WriteLine("Processing started...");
           
            IList<ReportData> rd = new List<ReportData>();
            IList<SchoolData> sd = new List<SchoolData>();

            StreamReader sr = new StreamReader(filePath);
            string line = string.Empty;
            int count = 0;
            while ((line = sr.ReadLine()) != null)
            {
                count++;
                Console.WriteLine("Processing line# {0}",count.ToString());
                if (!ValidateData(line, sd)) return false;
                
            }

            var queryResult = from item in sd
                            group item by new { item.School } into grouping
                            select new SchoolData
                            {
                                School = grouping.Key.School,
                                Population = grouping.Sum(item => item.Population)
                            };

            foreach (var qr in queryResult)
            {
                rd.Add(new ReportData() { Item = qr.School, Count = qr.Population });
            }

            queryResult = from item in sd
                              group item by new { item.City } into grouping
                              select new SchoolData
                              {
                                  City = grouping.Key.City,
                                  Population = grouping.Sum(item => item.Population)
                              };

            foreach (var qr in queryResult)
            {
                rd.Add(new ReportData() { Item = qr.City, Count = qr.Population });
            }

            queryResult = from item in sd
                              group item by new { item.Province } into grouping
                              select new SchoolData
                              {
                                  Province = grouping.Key.Province,
                                  Population = grouping.Sum(item => item.Population)
                              };

            foreach (var qr in queryResult)
            {
                rd.Add(new ReportData() { Item = qr.Province, Count = qr.Population });
            }

            var result = rd.OrderBy(x => x.Count);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("-=-=-=-=-=-=-=-= Report =-=-=-=-=-=-=-=-=-");
            foreach (var rep in result)
            {
                Console.WriteLine("{0} {1}",rep.Item,rep.Count);
            }

            sr.Close();

            return true;
        }

        public static bool ValidateData(string line, IList<SchoolData> sd)
        {
            char delimiter = '\t';
            int count = 0;

            string[] words = line.Split(delimiter);

            if (words.Length < 4)
            {
                Console.WriteLine("Incorrect length of words in the file.");
                return false;
            }

            string grade = words[3].Substring(0, 1);
            string population = words[3].Substring(1, words[3].Length - 1);

            bool success = Int32.TryParse(population, out count);
            if (!success)
            {
                Console.WriteLine("Attempted conversion of '{0}' failed.",
                                   population ?? "<null>");
                return false;
            }
            Int32.TryParse(words[3].Substring(1, words[3].Length - 1), out count);           

            sd.Add(new SchoolData() { Province = words[0], City = words[1], School = words[2], Grade = grade, Population = count});
           
            return true;
        }


    }
}
