using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO_Based_Oversampling
{
    class Minority_class
    {
        public double[] best_cluster_minority(double[] means_minor, double[] means_major, int numClusters)
        {
            double[] means_big = new double[numClusters];
            for (int k = 0; k < numClusters; k++)
            {
                double sum_of_def = 0;

                for (int l = 0; l < numClusters; l++)
                {
                    sum_of_def += (means_minor[k] - means_major[l]);
                }
                means_big[k] = Math.Abs(sum_of_def);
                Console.WriteLine("Sum_" + k + "\t" + means_big[k] + "\t");
            }
            return means_big;
        }
        public List<double> Get_minor_samples(double[] data, int[] clustering, int numClusters, int inedx_max_minor_samples, int decimals)
        {
            List<double> data_minor = new List<double>();
            for (int i = 0; i < data.Length; i++)
            {
                int clusterID = clustering[i];
                if (clusterID == inedx_max_minor_samples)
                {
                    data_minor.Add(data[i]);
                }
            }
            return data_minor;
        }
        public double[] minor_class_samples(double min_minor, double max_minor, int num_samples)
        {
            Random ran = new Random();

            double[] senth_samples = new double[num_samples];
            for (int i = 0; i < num_samples; i++)
            {

                senth_samples[i] = ran.NextDouble() * (max_minor - min_minor) + min_minor;
            }
            return senth_samples;
        }
        public int[][] kümede_kac_ornek_var(double[] data, int[] clustering, int numClusters)
        {
            List<List<double>> kac_ornek_var = new List<List<double>>();
            int[][] ornek_sayilari = new int[numClusters][];
            for (int k = 0; k < numClusters; ++k)
            {
                ornek_sayilari[k] = new int[1];
                kac_ornek_var.Add(new List<double>());
                for (int i = 0; i < data.Length; ++i)
                {
                    int clusterID = clustering[i];
                    if (clusterID != k) continue;
                    ornek_sayilari[k][0] += 1;

                }
            } // k
            return ornek_sayilari;
        } 
    }
}
