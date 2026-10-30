using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO_Based_Oversampling
{
    class Show_Data
    {
        public void ShowData(double[] data, int decimals, bool indices, bool newLine)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                if (indices) Console.Write(i.ToString().PadLeft(3) + " ");
                for (int j = 0; j < 1; ++j)
                {
                    if (data[i] >= 0.0) Console.Write(" ");
                    Console.Write(data[i].ToString("F" + decimals) + " ");
                }
                Console.WriteLine("");
            }
            if (newLine) Console.WriteLine("");
        } // ShowData

        public void ShowVector(int[] vector, bool newLine)
        {
            for (int i = 0; i < vector.Length; ++i)
                Console.Write(vector[i] + " ");
            if (newLine) Console.WriteLine("\n");
        }

        public void ShowClustered(double[] data, int[] clustering, int numClusters, int decimals)
        {
            for (int k = 0; k < numClusters; ++k)
            {
                Console.WriteLine("===================");
                for (int i = 0; i < data.Length; ++i)
                {
                    int clusterID = clustering[i];
                    if (clusterID != k) continue;
                    Console.Write(i.ToString().PadLeft(3) + " ");
                    for (int j = 0; j < 1; ++j)
                    {
                        if (data[i] >= 0.0) Console.Write(" ");
                        Console.Write(data[i].ToString("F" + decimals) + " ");
                    }
                    Console.WriteLine("");
                }
                Console.WriteLine("===================");
            } // k
        }
    }
}
