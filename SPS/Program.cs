using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    class Staff
    {
        private float hourlyRate;
        private int hWorked;

        public float TotalPay { get; protected set; }
        public float BasicPay { get; private set; }
        public string NameOfStaff { get; private set; }

        public int HoursWorked
        {
            get
            {
                return hWorked;
            }

            set
            {
                if (value > 0)
                    hWorked = value;
                else
                    hWorked = 0;
            }
        }

        public int HourWorked { get; internal set; }

        public Staff(string name, float rate)
        {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculting Pay...");

            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }

        public override string ToString()
        {
            return "\nNameOfStaff=" + NameOfStaff
                        + "\nhourlyRate" + "\nhWorked =" + TotalPay;
        }
    }

    class Manager : Staff
    {
        private const float managerHourlyRate = 50;
        private string managerHoulyRate;

        public int Allowance { get; private set; }
        public Manager(string name) : base(name, managerHourlyRate) { }

        public override void CalculatePay()
        {
            base.CalculatePay();
            Allowance = 1000;

            if (HoursWorked > 160)
                TotalPay = BasicPay + Allowance;
        }
        public override string ToString()
        {
            return "\nNameOfStaff =" + NameOfStaff + "\nmanagerHourlyRate="
            + managerHoulyRate + "\nHourlyRate +" + HoursWorked + "\nBasicPay=" + BasicPay + "\nAllowance=" + Allowance
            + "\n\nTotalPay=" + TotalPay;
        }
    }

    class Admin : Staff
    {
        private const float overtimeRate = 15.5f;

        private const float adminHourlyRate = 30f;

        public float Overtime { get; private set; }

        public Admin(string name) : base(name, adminHourlyRate) { }

        public override void CalculatePay()
        {
            base.CalculatePay();
            if (HoursWorked > 160)
                Overtime = overtimeRate * (HoursWorked - 160);
        }

        public override string ToString()
        {
            return "\nNameOfStaff =" + NameOfStaff + "\nadminHourlyRate" + adminHourlyRate + "\nHoursWorked =" + HoursWorked
            + "\nBasicPay=" + BasicPay + "\nOvertime=" + Overtime + "\n\nTotalPay =" + TotalPay;
        }
    }

    class FileReader
    {

        public List<Staff> ReadFile()
        {
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "staff.txt";
            string[] separator = { ", " };

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        result = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);

                        if (result[1] == "Admin")
                            myStaff.Add(new Admin(result[0]));
                    }
                    sr.Close();
                }
            }
            else
            {
                Console.WriteLine("Error: File does not exist");
            }
            return myStaff;
        }
    }

    class PaySlip
    {
        private int month;

        private int year;

        enum MonthsOfYear { JAN = 1, FEB = 2, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC }
        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }

        public void GeneratePaySlip(List<Staff> myStaff)

        {
            string path;
            foreach (Staff f in myStaff)
            {
                path = f.NameOfStaff + ".txt";

                using (StreamWriter sw = new StreamWriter(path))

                {
                    sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
                    sw.WriteLine("====================");
                    sw.WriteLine("Name of Staff: {0}",
                    f.NameOfStaff);
                    sw.WriteLine("Hours Worked: {0}",
                    f.HoursWorked);
                    sw.WriteLine("");
                    sw.WriteLine("Basic Pay: {0:C}",
                    f.BasicPay);
                    if (f.GetType() == typeof(Admin))
                        sw.WriteLine("Overtime: {0:C}",
                        ((Admin)f).Overtime);
                    sw.WriteLine("");
                    sw.WriteLine("====================");
                    sw.WriteLine("Total Pay: {0:C}",
                    f.TotalPay);
                    sw.WriteLine("====================");

                    sw.Close();
                }
            }


        }

        public void GenerateSummary(List<Staff> myStaff)
        {
            var result = from f in myStaff
                         where f.HourWorked < 10
                         orderby f.NameOfStaff ascending
                         select new { f.NameOfStaff, f.HoursWorked };

            string path = "summary.txt";

            using (StreamWriter sw = new StreamWriter(path))

            {
                sw.WriteLine("Staff with less than 10 working hours");
                sw.WriteLine("");
                foreach (var f in result)
                    sw.WriteLine("Name of Staff: {0}, Hours Worked: {1}", f.NameOfStaff, f.HoursWorked);

                sw.Close();
            }
        }
        public override string ToString()
        {
            return "month=" + month + "year=" + year;
        }
    }
}
