using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string str1 = "200";
            string str2 = "200 400";

            string[] str1Arr = str1.Split(' ');
            string[] str2Arr = str2.Split(' ');

            for (int i = 0; i < str1Arr.Length; i++)
            {
                System.Console.WriteLine(str1Arr[i]);
            }

            System.Console.WriteLine("========================");

            for (int i = 0; i < str2Arr.Length; i++)
            {
                System.Console.WriteLine(str2Arr[i]);
            }

        }
    }
}
