using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CrimeAnalyzer
{

    class CrimeStats
    {
        public int Year;
        public int Population;
        public int ViolentCrimes;
        public int Murder;
        public int Rape;
        public int Robbery;
        public int AggravatedAssault;
        public int PropertyCrime;
        public int Burglary;
        public int Theft;
        public int MotorVehicleTheft;

        public CrimeStats(int Year, int Population, int ViolentCrimes, int Murder, int Rape, int Robbery, int AggravatedAssault, int PropertyCrime, int Burglary, int Theft, int MotorVehicleTheft)
        {
            this.Year = Year;
            this.Population = Population;
            this.ViolentCrimes = ViolentCrimes;
            this.Murder = Murder;
            this.Rape = Rape;
            this.Robbery = Robbery;
            this.AggravatedAssault = AggravatedAssault;
            this.PropertyCrime = PropertyCrime;
            this.Burglary = Burglary;
            this.Theft = Theft;
            this.MotorVehicleTheft = MotorVehicleTheft;
        }
    }

    class MainClass
    {

        static void Main(string[] args)
        {
            Crimes crimes = new Crimes(args);
        }
    }

    class Crimes
    {
        private static List<CrimeStats> CrimeStatsList = new List<CrimeStats>();

        public Crimes(string[] args)
        {
            string csvFile = string.Empty;
            string reportFile = string.Empty;
            string startPath = System.IO.Directory.GetCurrentDirectory();

            if (Debugger.IsAttached)
            {
                csvFile = Path.Combine(startPath, "CrimeData.csv");
                reportFile = Path.Combine(startPath, "CrimeReport.txt");
            }
            else
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Invalid.");
                    Console.ReadLine();

                    return;
                }
                else
                {
                    csvFile = args[0];
                    reportFile = args[1];
                }
            }
            if (File.Exists(csvFile))
            {
                try
                {
                    var file = File.Create(reportFile);

                    file.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine($"Cannot Create a Report File.");
                }
                WriteReport(reportFile);
            }
            else
            {
                Console.WriteLine($"File Does Not Exist.");
            }
            Console.ReadLine();
        }

        private static bool ReadStats(string filePath)
        {
            try
            {
                int column = 0;
                string[] crimeStatsLines = File.ReadAllLines(filePath);

                for (int index = 0; index < crimeStatsLines.Length; index++)
                {
                    string crimeStatsLine = crimeStatsLines[index];
                    string[] data = crimeStatsLine.Split(',');

                    if (column != data.Length)
                    {
                        throw new Exception($"Invalid");
                    }
                    try
                    {
                        int Year = Int32.Parse(data[0]);
                        int Population = Int32.Parse(data[1]);
                        int ViolentCrimes = Int32.Parse(data[2]);
                        int Murder = Int32.Parse(data[3]);
                        int Rape = Int32.Parse(data[4]);
                        int Robbery = Int32.Parse(data[5]);
                        int AggravatedAssault = Int32.Parse(data[6]);
                        int PropertyCrime = Int32.Parse(data[7]);
                        int Burglary = Int32.Parse(data[8]);
                        int Theft = Int32.Parse(data[9]);
                        int MotorVehicleTheft = Int32.Parse(data[10]);
                        CrimeStats crimeStats = new CrimeStats(Year, Population, ViolentCrimes, Murder, Rape, Robbery, AggravatedAssault, PropertyCrime, Burglary, Theft, MotorVehicleTheft);
                        CrimeStatsList.Add(crimeStats);
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine($"There is an invalid value.");
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Error Occurred.");
                throw ex;
            }
            return true;
        }

        private static void WriteReport(string filePath)
        {
            try
            {
                if (CrimeStatsList != null && CrimeStatsList.Any())
                {
                    StringBuilder newString = new StringBuilder();

                    newString.Append("Crime Analyzer Report\n\n");

                    int minYear = CrimeStatsList.Min(x => x.Year);
                    int maxYear = CrimeStatsList.Max(x => x.Year);

                    int years = maxYear - minYear + 1;

                    newString.Append($"Period: { minYear}-{ maxYear} ({ years} years\n\n");

                    var totalYears = from crimeStats in CrimeStatsList
                                     where crimeStats.Murder < 15000
                                     select crimeStats.Year;

                    string totalYearsStr = string.Empty;

                    for (int i = 0; i < totalYears.Count(); i++)
                    {
                        totalYearsStr += totalYears.ElementAt(i).ToString();

                        if (i < totalYears.Count() - 1) totalYearsStr += ", ";
                    }

                    newString.Append($"Years murders per year < 15000: { totalYearsStr}\n");

                    var robYears = from crimeStats in CrimeStatsList
                                   where crimeStats.Robbery > 500000
                                   select crimeStats;

                    string robYearsStr = string.Empty;

                    for (int i = 0; i < robYears.Count(); i++)
                    {
                        CrimeStats crimeStats = robYears.ElementAt(i);

                        robYearsStr += $"{ crimeStats.Year} = { crimeStats.Robbery}";

                        if (i < robYears.Count() - 1) robYearsStr += ", ";

                    }

                    newString.Append($"Robberies per year > 500000: { robYearsStr}\n");

                    var violentCrime = from crimeStats in CrimeStatsList
                                       where crimeStats.Year == 2010
                                       select crimeStats;

                    CrimeStats violentCrimeStats = violentCrime.First();

                    double violentCrimePerCapita = (double)violentCrimeStats.ViolentCrimes / (double)violentCrimeStats.Population;

                    double avgMurders = (float)CrimeStatsList.Sum(x => x.Murder) / (float)CrimeStatsList.Count;

                    newString.Append($"Average murder per year (all years): { avgMurders}\n");

                    int murd = CrimeStatsList.Where(x => x.Year >= 2010 && x.Year <= 2014).Sum(y => y.Murder);

                    double avgMurd = (float)murd / 5;

                    newString.Append($"Average murder per year (2010-2014): { avgMurd}\n");

                    int minTheft = CrimeStatsList.Where(x => x.Year >= 1999 && x.Year <= 2004).Min(x => x.Theft);

                    newString.Append($"Minimum thefts per year (1999-2004): { minTheft}\n");

                    int maxTheft = CrimeStatsList.Where(x => x.Year >= 1999 && x.Year <= 2004).Max(x => x.Theft);

                    newString.Append($"Maximum thefts per year (1999-2004): { maxTheft}\n");

                    int maxVehicleTheft = CrimeStatsList.OrderByDescending(x => x.MotorVehicleTheft).First().Year;

                    newString.Append($"Year of highest number of motor vehicle thefts: { maxVehicleTheft}\n");

                    using (var stream = new StreamWriter(filePath))
                    {
                        stream.Write(newString.ToString());
                    }

                    Console.WriteLine();
                    Console.WriteLine(newString.ToString());
                    Console.WriteLine();                 }                 else                 {                     Console.WriteLine($"No Data.");                 }             }             catch (Exception ex)             {                 Console.WriteLine("Error.");                 throw ex;             }

        }
    }
}