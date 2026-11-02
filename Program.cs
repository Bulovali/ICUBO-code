using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using PSO_Cluster;

namespace PSO_Based_Oversampling
{
    class Program
    {
        static void Main(string[] args)
        {
            int numClusters = 4;
            PSOcluster psoCluster = new PSOcluster(numClusters, numberOfParticles: 30, maxIterations: 100, inertiaWeight: 0.729, cognitiveCoefficient: 1.49445, socialCoefficient: 1.49445, randomSeed: 42);
            Minority_class minorityClass = new Minority_class();
            Show_Data show = new Show_Data();
            string[] _file = { "22. abalone19" };
            for (int file_index = 0; file_index < _file.Length; file_index++)
            {
                string _filename = "D:\\" + _file[file_index];
                int _numofatr = 0;
                int _numofsam = 0;                
                double[][] xValues;
                string[] tValues;
                string[][] _strdataset;
                double[][] _dbldataset;       
                // ============================================================
                // 1. READ DATASET
                // ============================================================
                using (StreamReader sr = new StreamReader(_filename + ".txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        sr.ReadLine();
                        _numofsam++;
                    }
                }

                xValues = new double[_numofsam][];
                tValues = new string[_numofsam];
                _strdataset = new string[_numofsam][];
                _dbldataset = new double[_numofsam][];

                using (StreamReader sr1 = new StreamReader(_filename + ".txt"))
                {
                    int _numofrow = 0;

                    while (!sr1.EndOfStream)
                    {
                        string _row = sr1.ReadLine();

                        // Handles multiple spaces/tabs between values
                        string[] st = _row.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        _numofatr = st.Length;

                        // Last column = class label
                        xValues[_numofrow] = new double[_numofatr - 1];

                        _strdataset[_numofrow] = new string[_numofatr];

                        _dbldataset[_numofrow] = new double[_numofatr];

                        for (int j = 0; j < _numofatr; j++)
                        {
                            if (j == _numofatr - 1)
                            {
                                // Class label
                                tValues[_numofrow] = st[j];
                            }
                            else
                            {
                                // Feature
                                xValues[_numofrow][j] = Convert.ToDouble(st[j]); //CultureInfo.InvariantCulture
                            }

                            _strdataset[_numofrow][j] = st[j];
                            _dbldataset[_numofrow][j] = 0.0;
                        }

                        _numofrow++;
                    }
                }

                // ============================================================
                // 2. FIND UNIQUE CLASS LABELS
                // ============================================================

                List<string> _classvalue = new List<string>();

                for (int i = 0; i < tValues.Length; i++)
                {
                    if (!_classvalue.Contains(tValues[i]))
                    {
                        _classvalue.Add(tValues[i]);
                    }
                }

                int _numofclass = _classvalue.Count;

                Console.WriteLine("Dataset: " + _file[file_index]);

                Console.WriteLine("Number of classes: " + _numofclass);

                // ============================================================
                // 3. CREATE CLASS CONTAINERS DYNAMICALLY
                // ============================================================

                List<List<List<double>>> _classes = new List<List<List<double>>>();

                for (int i = 0; i < _numofclass; i++)
                {
                    _classes.Add(new List<List<double>>());
                }

                // ============================================================
                // 4. ENCODE CLASS LABELS
                // ============================================================

                int[] tValues_code = new int[_numofsam];

                for (int i = 0; i < _numofsam; i++)
                {
                    int classIndex = _classvalue.IndexOf(tValues[i]);

                    if (classIndex == -1)
                    {
                        throw new Exception("Unknown class label: " + tValues[i]);
                    }

                    tValues_code[i] = classIndex;

                    // Replace original class label with encoded value
                    tValues[i] = classIndex.ToString();
                }
                
                List<string> _classvalcode = new List<string>();
                for (int kacsinifdongu = 0; kacsinifdongu < tValues.Length; kacsinifdongu++)
                {
                    if (!(_classvalcode.Contains(Convert.ToString(tValues[kacsinifdongu]))))
                    {
                        _classvalcode.Add(Convert.ToString(tValues[kacsinifdongu]));
                    }
                }
                // ============================================================
                // 5. CONVERT COMPLETE DATA TO DOUBLE
                // ============================================================

                for (int i = 0; i < _numofsam; i++)
                {
                    for (int j = 0; j < _numofatr; j++)
                    {
                        if (j < _numofatr - 1)
                        {
                            // Feature
                            _dbldataset[i][j] = xValues[i][j];
                        }
                        else
                        {
                            // Encoded class
                            _dbldataset[i][j] = tValues_code[i];
                        }
                    }
                }

                // ============================================================
                // 6. DISTRIBUTE SAMPLES INTO CLASSES
                // ============================================================

                List<int> _clas = new List<int>();

                for (int i = 0; i < _numofsam; i++)
                {
                    // Dynamic class index
                    int classIndex = tValues_code[i];

                    // Create a sample
                    List<double> sample = new List<double>();

                    // Copy all features + class
                    for (int k = 0; k < _numofatr; k++)
                    {
                        sample.Add(_dbldataset[i][k]);
                    }

                    // Add sample to corresponding class
                    _classes[classIndex].Add(sample);

                    // Store class
                    _clas.Add(classIndex);
                }
                for (int i = 0; i < _numofclass; i++)
                {
                    Console.WriteLine("Class " + i + ": " + _classes[i].Count + " samples");
                }

                // ============================================================
                // 7. NUMBER OF SAMPLES IN EACH CLASS
                // ============================================================

                int[] _class_samples = new int[_numofclass];

                for (int i = 0; i < _numofclass; i++)
                {
                    _class_samples[i] = _classes[i].Count;
                }

                // ============================================================
                // 8. DISPLAY CLASS INFORMATION
                // ============================================================

                Console.WriteLine("\nClass information:");

                for (int i = 0; i < _numofclass; i++)
                {
                    Console.WriteLine("Class {0} (original label = {1}): {2} samples", i, _classvalue[i], _class_samples[i]);
                }

                Console.WriteLine("\nTotal samples: " + _numofsam);

                Console.WriteLine("Number of features: " + (_numofatr - 1));

                // ============================================================
                // MAJORITY CLASS
                // ============================================================

                int index_max = Array.IndexOf(_class_samples, _class_samples.Max());

                double[][] major_class = new double[_classes[index_max].Count][];

                double[][] x_major_class = new double[_classes[index_max].Count][];

                double[] y_major_class = new double[_classes[index_max].Count];

                for (int i = 0; i < major_class.Length; i++)
                {
                    major_class[i] = new double[_numofatr];
                    x_major_class[i] = new double[_numofatr - 1];

                    for (int j = 0; j < _numofatr; j++)
                    {
                        if (j == _numofatr - 1)
                        {
                            y_major_class[i] = _classes[index_max][i][j];
                        }
                        else
                        {
                            x_major_class[i][j] = _classes[index_max][i][j];
                        }

                        major_class[i][j] = _classes[index_max][i][j];
                    }
                }

                // ============================================================
                // ALL MINORITY CLASSES
                // ============================================================

                // Store the indexes of all classes except majority
                List<int> minorityClassIndexes = new List<int>();

                for (int i = 0; i < _numofclass; i++)
                {
                    if (i != index_max)
                    {
                        minorityClassIndexes.Add(i);
                    }
                }

                Console.WriteLine("Majority class index = " + index_max);

                Console.WriteLine("Number of minority classes = " + minorityClassIndexes.Count);

                foreach (int classIndex in minorityClassIndexes)
                {
                    Console.WriteLine("Minority class " + classIndex + " (" + _classvalue[classIndex] + ")" + " = " + _classes[classIndex].Count + " samples");
                }

                List<double[]> allMinorityData = new List<double[]>();
                List<int> allMinorityClasses = new List<int>();
                List<double[]> allData = new List<double[]>();
                
                foreach (int currentMinorityClass in minorityClassIndexes)
                {
                    List<List<double>> senth_minority = new List<List<double>>(); 
                    Console.WriteLine();
                    Console.WriteLine("======================================");
                    Console.WriteLine("Processing minority class: " + currentMinorityClass);
                    Console.WriteLine("Original class label: " + _classvalue[currentMinorityClass]);
                    Console.WriteLine("Original samples: " + _classes[currentMinorityClass].Count);

                    int currentMinorityCount = _classes[currentMinorityClass].Count;
                    double[][] current_minor_class = new double[currentMinorityCount][];
                    double[][] current_x_minor_class = new double[currentMinorityCount][];
                    double[] current_y_minor_class = new double[currentMinorityCount];
                    for (int i = 0; i < currentMinorityCount; i++)
                    {
                        current_minor_class[i] = new double[_numofatr];
                        current_x_minor_class[i] = new double[_numofatr - 1];
                        for (int j = 0; j < _numofatr; j++)
                        {
                            if (j == _numofatr - 1)
                            {
                                current_y_minor_class[i] = _classes[currentMinorityClass][i][j];
                            }
                            else
                            {
                                current_x_minor_class[i][j] = _classes[currentMinorityClass][i][j];
                            }

                            current_minor_class[i][j] = _classes[currentMinorityClass][i][j];
                        }
                    }

                    int _numofsenth = major_class.Length - currentMinorityCount;                    
                    // If the minority class is already equal to
                    // or larger than the majority class, do not generate.
                    if (_numofsenth <= 0)
                    {
                        _numofsenth = 0;
                    }
                    Console.WriteLine("Samples to generate: " + _numofsenth);
                    for (int i = 0; i < currentMinorityCount; i++)
                    {
                        double[] originalSample = new double[_numofatr];
                        for (int j = 0; j < _numofatr; j++)
                        {
                            originalSample[j] = current_minor_class[i][j];
                        }
                        allMinorityData.Add(originalSample);
                        allMinorityClasses.Add(currentMinorityClass);
                    }
                    if (_numofsenth == 0)
                    {
                        for (int ll = 0; ll < current_minor_class.Length; ll++)
                        {
                            allData.Add(current_minor_class[ll]);
                        }
                        continue;
                    }

                    double[] data_major = new double[x_major_class.Length];
                    double[] means_major = new double[numClusters];
                    double[] data_minor = new double[current_x_minor_class.Length];
                    double[] means_minor = new double[numClusters];

                    for (int attributeIndex = 0; attributeIndex < _numofatr; attributeIndex++)
                    {
                        senth_minority.Add(new List<double>());                        
                        if (attributeIndex < _numofatr -1)
                        {
                            for (int j = 0; j < x_major_class.Length; j++)
                            {
                                data_major[j] = x_major_class[j][attributeIndex];
                            }
                            int[] clustering_major_label;
                            double[][] means_maj;
                            Console.WriteLine("-------------------");
                            show.ShowData(data_major, 1, true, true);
                            Console.WriteLine("\n k sayisi " + numClusters);                                                   
                            psoCluster.Cluster_Fit(data_major, out clustering_major_label, out means_maj);                       
                            List<List<double>> clusters = new List<List<double>>();                          
                            for (int k = 0; k < numClusters; k++)
                            {
                                List<double> cluster = new List<double>();
                                for (int i = 0; i < data_major.Length; i++)
                                {
                                    if (clustering_major_label[i] == k)
                                    {
                                        cluster.Add(data_major[i]);
                                    }
                                }
                                clusters.Add(cluster);
                            }
                            for (int k = 0; k < clusters.Count; k++)
                            {
                                double mean = clusters[k].Average();
                                double minimum = clusters[k].Min();
                                double maximum = clusters[k].Max();
                                Console.WriteLine("Cluster " + k);
                                Console.WriteLine("Mean = " + mean);
                                Console.WriteLine("Min  = " + minimum);
                                Console.WriteLine("Max  = " + maximum);
                            }                            
                            
                            Console.WriteLine("-------------------");
                            show.ShowVector(clustering_major_label, true);
                            show.ShowClustered(data_major, clustering_major_label, numClusters, 1);
                            for (int k = 0; k < numClusters; k++)
                            {
                                means_major[k] = means_maj[k][0];
                                Console.WriteLine("Mean_" + k + "\t" + means_major[k] + "\t");
                            }
                            Console.WriteLine("-------------------");
                            int[][] ornek_sayi_major = minorityClass.kümede_kac_ornek_var(data_major, clustering_major_label, numClusters);
                            for (int j = 0; j < current_x_minor_class.Length; j++)
                            {
                                data_minor[j] = current_x_minor_class[j][attributeIndex];
                            }
                            Console.WriteLine("-------------------");
                            show.ShowData(data_minor, 1, true, true);                          
                            int[] clustering_minor_label;
                            double[][] means_min;
                            psoCluster.Cluster_Fit(data_minor, out clustering_minor_label, out means_min);
                            Console.WriteLine("-------------------");
                            show.ShowVector(clustering_minor_label, true);
                            show.ShowClustered(data_minor, clustering_minor_label, numClusters, 1);
                            for (int k = 0; k < numClusters; k++)
                            {
                                means_minor[k] = means_min[k][0];
                                Console.WriteLine("Mean_" + k + "\t" + means_minor[k] + "\t");
                            }
                            Console.WriteLine("-------------------");                          
                            double bestICU;
                            List<double> ICUValues;
                            List<List<double[]>> Curent_minor_cluster = ClusteredData(data_minor, clustering_minor_label, numClusters, 1);
                            List<List<double[]>> Curent_major_cluster = ClusteredData(data_major, clustering_major_label, numClusters, 1);
                            int bestMinorityClusterIndex = FindBestICUMinorityCluster( Curent_minor_cluster, Curent_major_cluster, out bestICU, out ICUValues);                         
                            double totalICU = 0.0;
                            for (int i = 0; i < ICUValues.Count; i++)
                            {
                                totalICU += ICUValues[i];
                            }
                            
                            List<double> clusterPercentage = new List<double>();
                            for (int i = 0; i < ICUValues.Count; i++)
                            {
                                double CP = 0.0;
                                if (totalICU > 0)
                                {
                                    CP = ICUValues[i] / totalICU;
                                }
                                clusterPercentage.Add(CP);
                                Console.WriteLine("Minority Cluster {0}: ICU = {1:F4}, CP = {2:F4}", i, ICUValues[i], CP);
                            }                          
                            int requiredSamples = major_class.Length - currentMinorityCount;

                            if (requiredSamples < 0)
                            {
                                requiredSamples = 0;
                            }
                            // NSSC for every minority cluster
                            List<int> NSSC = new List<int>();
                            for (int i = 0; i < clusterPercentage.Count; i++)
                            {
                                int numberOfSamples = (int)Math.Round( clusterPercentage[i] * requiredSamples);
                                NSSC.Add(numberOfSamples);
                                Console.WriteLine("Cluster {0}: CP = {1:F4}, NSSC = {2}", i, clusterPercentage[i], numberOfSamples);
                            }
                            
                            int generatedTotal = 0;
                            for (int i = 0; i < NSSC.Count; i++)
                            {
                                generatedTotal += NSSC[i];
                            }
                            int difference = requiredSamples - generatedTotal;
                            if (difference != 0)
                            {                                
                                NSSC[bestMinorityClusterIndex] += difference;
                            }                            
                            List<double[]> syntheticSamples = new List<double[]>();
                            Random random = new Random();
                            // Use the optimal minority cluster
                            List<double[]> optimalCluster = Curent_minor_cluster[bestMinorityClusterIndex];
                            int numberOfFeatures = optimalCluster[0].Length;
                            double[] clusterMean = new double[numClusters];
                            double[] clusterMin = new double[numClusters];
                            double[] clusterMax = new double[numClusters];
                            // Initialize min/max
                            for (int j = 0; j < numClusters; j++)
                            {
                                clusterMin[j] = double.MaxValue;
                                clusterMax[j] = double.MinValue;
                            }
                            // Calculate mean, min and max
                            for (int j = 0; j < numClusters; j++)
                            {
                                double sum = 0.0;
                                for (int i = 0; i < Curent_minor_cluster[j].Count; i++)
                                {
                                    double value = Curent_minor_cluster[j][i][0];
                                    sum += value;

                                    if (value < clusterMin[j])
                                        clusterMin[j] = value;

                                    if (value > clusterMax[j])
                                        clusterMax[j] = value;
                                }
                                clusterMean[j] = sum / Curent_minor_cluster[j].Count;
                            }
                            double[][] syntheticSample = new double[numClusters][];
                            for (int clusterIndex = 0; clusterIndex < NSSC.Count; clusterIndex++)
                            {
                                int numberToGenerate = NSSC[clusterIndex];

                                if (numberToGenerate <= 0)
                                    continue;                               
                                syntheticSample[clusterIndex] = new double[numberToGenerate];
                                for (int n = 0; n < numberToGenerate; n++)
                                {
                                    // Random number r in [0,1]
                                    double r = random.NextDouble();                                    
                                    syntheticSample[clusterIndex][n] = clusterMean[clusterIndex] + (2.0 * r - 1.0) * ((clusterMax[clusterIndex] - clusterMin[clusterIndex]) / 2);
                                    if (syntheticSample[clusterIndex][n] > clusterMax[clusterIndex] || syntheticSample[clusterIndex][n] < clusterMin[clusterIndex])
                                    {
                                        syntheticSample[clusterIndex][n] = clusterMin[clusterIndex] + r * (clusterMax[clusterIndex] - clusterMin[clusterIndex]);
                                    }                                    
                                    senth_minority[attributeIndex].Add(syntheticSample[clusterIndex][n]);
                                }                             
                            }                                                                                   
                        }
                        else
                        {
                            for (int l = 0; l < _numofsenth; l++)
                            {
                                senth_minority[attributeIndex].Add(currentMinorityClass);
                            }
                        }                                                
                    }//For each attribute
                    Console.WriteLine("For each attribute");
                    for (int i = 0; i < _numofsenth; i++)
                    {
                        double[] senthSample = new double[_numofatr];

                        for (int j = 0; j < _numofatr; j++)
                        {
                            senthSample[j] = senth_minority[j][i];
                        }

                        allMinorityData.Add(senthSample);
                        allData = allMinorityData;                        
                        allMinorityClasses.Add(currentMinorityClass);
                    }
                }//For each minority class                
                for (int ii = 0; ii < major_class.Length; ii++)
                {
                    double[] Sampless = new double[_numofatr];
                    for (int jj = 0; jj < _numofatr; jj++)
                    {
                        Sampless[jj] = major_class[ii][jj];
                    }
                    allData.Add(Sampless);                    
                }                
                int dim = allData.Count;
                string[][] last_data = new string[dim][];
                int[] last_class = new int[dim];
                for (int i = 0; i < dim; i++)
                {
                    last_data[i] = new string[_numofatr];
                    for (int j = 0; j < _numofatr; j++)
                    {
                        last_data[i][j] = "0.0";
                    }
                    last_class[i] = 0;
                }
                StreamWriter sw = new StreamWriter(_filename + "_ICUBO_"+ numClusters +".txt"); 
                int sayac_satir = 0;
                for (int i = 0; i < allData.Count; i++)
                {
                    int sutun_sayac = 0;
                    for (int j = 0; j < _numofatr; j++)
                    {
                        if (j < _numofatr - 1)
                        {
                            string veri = allData[i][j].ToString("F2");
                            last_data[sayac_satir][sutun_sayac] = veri.ToString().Replace(",", ".");
                            sw.Write(allData[i][j] + "\t");
                        }
                        else
                        {
                            last_data[sayac_satir][sutun_sayac] = allData[i][j].ToString();
                            sw.Write(allData[i][j]);
                        }
                        Console.Write(allData[i][j] + "\t");
                        
                        sutun_sayac++;
                    }
                    sayac_satir++;
                    Console.WriteLine();
                    sw.WriteLine();
                }                
                sw.Close();
               
            }// for file
            Console.WriteLine("\nFinished.");
            Console.ReadLine();
        }//main
        private static int FindBestICUMinorityCluster(List<List<double[]>> minorityClusters,List<List<double[]>> majorityClusters,out double bestICU,out List<double> allICUValues)
        {
            bestICU = double.MinValue;
            int bestMinorityClusterIndex = -1;

            allICUValues = new List<double>();

            for (int i = 0; i < minorityClusters.Count; i++)
            {
                double maxICUForMinorityCluster = double.MinValue;

                for (int j = 0; j < majorityClusters.Count; j++)
                {
                    double ICU = CalculateICU(minorityClusters[i],majorityClusters[j]);

                    if (ICU > maxICUForMinorityCluster)
                    {
                        maxICUForMinorityCluster = ICU;
                    }
                }

                allICUValues.Add(maxICUForMinorityCluster);

                if (maxICUForMinorityCluster > bestICU)
                {
                    bestICU = maxICUForMinorityCluster;
                    bestMinorityClusterIndex = i;
                }
            }

            return bestMinorityClusterIndex;
        }//FindBestICUMinorityCluster
        private static double CalculateICU(List<double[]> minorityCluster,List<double[]> majorityCluster)
        {
            double maxDistance = double.MinValue;

            foreach (double[] minoritySample in minorityCluster)
            {
                foreach (double[] majoritySample in majorityCluster)
                {
                    double distance = EuclideanDistance(minoritySample, majoritySample);

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                    }
                }
            }

            return maxDistance;
        }//CalculateICU
        private static double EuclideanDistance(double[] sample1,double[] sample2)
        {
            double sum = 0.0;

            for (int i = 0; i < sample1.Length; i++)
            {
                double difference = sample1[i] - sample2[i];
                sum += difference * difference;
            }

            return Math.Sqrt(sum);
        }//EuclideanDistance
        public static List<List<double[]>> ClusteredData(double[] data, int[] clustering, int numClusters, int decimals)
        {
            List<List<double[]>> clustered_data = new List<List<double[]>>();
            for (int k = 0; k < numClusters; ++k)
            {
                clustered_data.Add(new List<double[]>());
                Console.WriteLine("===================");
                for (int i = 0; i < data.Length; ++i)
                {
                    int clusterID = clustering[i];
                    if (clusterID != k) continue;
                    // One-dimensional data
                    double[] sample = new double[1];
                    sample[0] = data[i];
                    Console.Write(i.ToString().PadLeft(3) + " ");
                    for (int j = 0; j < 1; ++j)
                    {
                        if (data[i] >= 0.0) Console.Write(" ");
                        Console.Write(data[i].ToString("F" + decimals) + " ");
                        clustered_data[k].Add(sample);
                    }
                    Console.WriteLine("");
                }
                Console.WriteLine("===================");
            } // k
            return clustered_data;
        }
        
    }
}
