using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using UAssetAPI;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace GB_BP_EDIT
{
    public partial class Form1 : Form
    {
        static void ApplyChanges()
        {
            Console.WriteLine("NO");
        }
        public Form1()
        {
            InitializeComponent();
        }

        static float readFloatFromString(string arg)
        {
            if (arg.Split('.').Length == 1)
            {
                return (float) Int32.Parse(arg);
            }
            else
            {
                return (float)(Int32.Parse(arg.Split('.')[0]) + Int32.Parse(arg.Split('.')[1]) / Math.Pow(1.0, arg.Split('.')[1].Count()));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "%UserProfile%\\Desktop";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }
            textBox1.Text = fileContent;
            //MessageBox.Show(fileContent, "File Content at path: " + filePath, MessageBoxButtons.OK);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                /*
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                    System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                }
                */
                textBox2.Text = fbd.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] input_full = textBox1.Text.Split(';');
            foreach (string input_part in input_full)
            {
                ApplyChanges();
                string[] input = input_part.Split('+');
                string name;
                if (input[0] == "E" || input[0] == "NM" || input[0] == "I")
                {
                    name = input[1];
                }
                else
                {
                    name = input[0];
                }
                var bp = new UAssetAPI.UAsset(Path.Combine(textBox2.Text, name), UAssetAPI.UE4Version.VER_UE4_27, true);
                bp.Write(Path.Combine(textBox2.Text, name + ".bak"));
                try
                {
                    if (input[0] == "NM")
                    {
                        for (int j = 2; j < (input.Length); j += 2)
                        {
                            FString fString = new FString(input[j+1]);
                            bp.SetNameReference(Int32.Parse(input[j]), fString);
                        }
                        MessageBox.Show("Success editing Name Map");
                    }
                    else if (input[0] == "I")
                    {
                        foreach (Import testImport in bp.Imports)
                        {
                            if (testImport.ClassPackage.Value.ToString() == input[2])
                            {
                                FString fString = new FString(input[3]);
                                testImport.ClassPackage.Value = fString;
                                fString = new FString(input[4]);
                                testImport.ClassName.Value = fString;
                                fString = new FString(input[5]);
                                testImport.ObjectName.Value = fString;
                                MessageBox.Show("Success editing Import");
                            }
                        }
                    }
                    else
                    {
                        int i = new int();
                        if (input[0] == "E")
                            i = 1;
                        else
                            i = 0;
                        //var bp = new UAssetAPI.UAsset(Path.Combine(textBox2.Text, name), UAssetAPI.UE4Version.VER_UE4_27, true);
                        var ourDataTableExport = bp.Exports[1] as DataTableExport;
                        foreach (Export testExport in bp.Exports)
                        {
                            //MessageBox.Show(testExport.ReferenceData.ObjectName.Value.ToString());
                            if (testExport.ReferenceData.ObjectName.Value.ToString() == input[i + 1])
                            {
                                //MessageBox.Show(testExport.GetType().ToString());
                                if (testExport is NormalExport normalTestExport)
                                {
                                    //MessageBox.Show(normalTestExport.Data.Count().ToString());
                                    PropertyData prop = normalTestExport.Data[Int32.Parse(input[i + 2])];
                                    //MessageBox.Show(prop.GetType().ToString());
                                    if (prop.Name.Value.ToString() == input[i + 3])
                                    {

                                        if (prop is StructPropertyData structPropertyData)
                                        {
                                            string[] a = { input[i + 4], input[i + 5], input[i + 6] };
                                            Debug.Write(structPropertyData.PropertyType.ToString());
                                            structPropertyData.Value[0].FromString(d: a);
                                            //Debug.WriteLine(structPropertyData.Value.First().ToString());
                                            MessageBox.Show("Success editing a vector value!");
                                        }
                                        else if (prop is FloatPropertyData floatPropertyData)
                                        {
                                            floatPropertyData.Value = readFloatFromString(input[i + 4]);
                                            MessageBox.Show("Success editing an float numerical value!");
                                        }
                                        else if (prop is IntPropertyData intPropertyData)
                                        {
                                            intPropertyData.Value = Int32.Parse(input[i + 4]);
                                            MessageBox.Show("Success editing an integer numerical value!");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Can't handle this type of data :(, yet...");
                                        }
                                    }
                                }
                            }
                        }

                    }
                    Debug.WriteLine(name);
                    
                }
                catch
                {
                    MessageBox.Show("Something bad happened...");
                }
                bp.Write(Path.Combine(textBox2.Text, name));
            }

        }
    }
}
