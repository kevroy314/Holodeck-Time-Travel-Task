using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeTravelLogFileTester
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            int numKeys = 0;
            int numBtns = 0;
            int numItems = 0;
            bool freeRun = false;

            OpenFileDialog d = new OpenFileDialog();
            d.CheckFileExists = true;
            d.DefaultExt = ".dat";
            d.Multiselect = false;
            d.Title = "Select Log File";
            d.ShowDialog();

            string filename = d.FileName;
            StreamReader reader = new StreamReader(filename);
            BinaryReader breader = new BinaryReader(reader.BaseStream);

            string headerString = breader.ReadString();

            numKeys = Regex.Matches(headerString, "key").Count;
            numBtns = Regex.Matches(headerString, "button").Count;
            numItems = Regex.Matches(headerString, "itemXYZAC").Count;

            Console.WriteLine("Header");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine(headerString.Trim());
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Header Pase Found: numKeys={0}, numBtns={1}, numItems={2}, headerLength={3}/{4}", numKeys, numBtns, numItems, headerString.Trim().Length, headerString.Length);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Lines");
            Console.WriteLine("--------------------------------------");

            int lineCount = 0;
            bool silent = false;

            while (breader.BaseStream.Position != breader.BaseStream.Length)
            {
                long systemTime = breader.ReadInt64();
                DateTime derivedSystemTime = DateTime.FromBinary(systemTime);
                float time = breader.ReadSingle();
                float timeScale = breader.ReadSingle();

                float px = breader.ReadSingle();
                float py = breader.ReadSingle();
                float pz = breader.ReadSingle();

                float rx = breader.ReadSingle();
                float ry = breader.ReadSingle();
                float rz = breader.ReadSingle();
                float rw = breader.ReadSingle();

                bool[] keys = new bool[numKeys];
                for (int i = 0; i < keys.Length; i++)
                    keys[i] = breader.ReadBoolean();
                bool[] btns = new bool[numBtns];
                for (int i = 0; i < btns.Length; i++)
                    btns[i] = breader.ReadBoolean();
                float[] ix = new float[numItems];
                float[] iy = new float[numItems];
                float[] iz = new float[numItems];
                bool[] iActive = new bool[numItems];
                bool[] iClicked = new bool[numItems];
                for (int i = 0; i < ix.Length; i++)
                {
                    ix[i] = breader.ReadSingle();
                    iy[i] = breader.ReadSingle();
                    iz[i] = breader.ReadSingle();
                    iActive[i] = breader.ReadBoolean();
                    iClicked[i] = breader.ReadBoolean();
                }
                int boundaryState = breader.ReadInt32();
                float br = breader.ReadSingle();
                float bg = breader.ReadSingle();
                float bb = breader.ReadSingle();

                lineCount++;

                if (!silent)
                {
                    Console.Write(derivedSystemTime.ToString() + "," + time + "," + timeScale + "," + px + "," + py + "," + pz + "," + rx + "," + ry + "," + rz + "," + rw + ",");
                    for (int i = 0; i < keys.Length; i++)
                        Console.Write(keys[i] + ",");
                    for (int i = 0; i < btns.Length; i++)
                        Console.Write(btns[i] + ",");
                    for (int i = 0; i < ix.Length; i++)
                        Console.Write(ix[i] + "," + iy[i] + "," + iz[i] + "," + iActive[i] + "," + iClicked[i] + ",");
                    Console.Write(boundaryState + "," + br + "," + bg + "," + bb);
                    Console.WriteLine();
                }
                
                if (!freeRun)
                {
                    string val = Console.ReadLine();
                    if (val.Contains("r"))
                        freeRun = true;
                    if (val.Contains("s"))
                        silent = true;
                }
            }

            breader.Close();
            reader.Close();

            Console.WriteLine("--------------------------------------");
            Console.WriteLine("{0} lines read.", lineCount);
            Console.WriteLine("Finished!");
            Console.ReadLine();
        }
    }
}
