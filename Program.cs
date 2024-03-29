using System.Diagnostics;
using UAssetAPI;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

class Program
{
    static float readFloatFromString(string arg)
    {
        if (arg.Split('.').Length == 1)
        {
            return (float)Int32.Parse(arg);
        }
        else
        {
            return (float)(Int32.Parse(arg.Split('.')[0]) + Int32.Parse(arg.Split('.')[1]) / Math.Pow(1.0, arg.Split('.')[1].Count()));
        }
    }

    static void Main(string[] args)
    {
        // This way of handling CLI args fucking suck but, I've wasted to much time fighting 
        // the tooling already and don't wanna learn complicated way of dotnet handling of these  
        var onlyCommand = args.Length == 1;
        var withBackup = args.Length == 3;

        if (!onlyCommand && !withBackup) {
            Console.WriteLine("Invalid number of arguments. {0} provided, 1 or 3 expected.", args.Length);
            return;
        }

        string command = args[args.Length - 1];
        string[] input = command.Split('+');
        string name;
        // E, NM, I are related to experimental functions. I don't engineer this out in fear
        // of breaking something...
        if (input[0] == "E" || input[0] == "NM" || input[0] == "I")
        {
            name = input[1];
        }
        else
        {
            //...thus it branches to here every time since tool is used primarily to edit vector values.
            name = input[0]; 
        }

        if (withBackup) 
        {
            if (args[0] == "-b") {
                try 
                {
                    // Blueprint actually depends on 3 seperate files
                    var n = Path.GetFileNameWithoutExtension(name);
                    var p = Path.GetDirectoryName(name);
                    File.Copy(p + "\\" + n + ".uasset",      Path.Combine(args[1], n + "_backup.uasset"));
                    File.Copy(p + "\\" + n + ".uasset.uexp", Path.Combine(args[1], n + "_backup.uasset.uexp"));
                    File.Copy(p + "\\" + n + ".uexp",        Path.Combine(args[1], n + "_backup.uexp"));
                } 
                catch (Exception e) 
                {
                    Console.WriteLine("Error creating backup: {0}", e.Message);
                    return;
                }
            }
        }

        var bp = new UAsset(name, UE4Version.VER_UE4_27, true);
        // Would be a proper way to do a backup if I knew how it works. Left out of fear of breaking things.
        bp.Write(name + ".bak"); 
        try
        {
            if (input[0] == "NM")
            {
                for (int j = 2; j < (input.Length); j += 2)
                {
                    FString fString = new FString(input[j + 1]);
                    bp.SetNameReference(Int32.Parse(input[j]), fString);
                }
                Console.WriteLine("Success editing Name Map");
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
                        Console.WriteLine("Success editing Import");
                    }
                }
            }
            else
            {
                int i;
                if (input[0] == "E")
                    i = 1;
                else
                    i = 0;

                //var bp = new UAssetAPI.UAsset(Path.Combine(textBox2.Text, name), UAssetAPI.UE4Version.VER_UE4_27, true);
                var ourDataTableExport = bp.Exports[1] as DataTableExport;
                foreach (Export testExport in bp.Exports)
                {
                    //Console.WriteLine(testExport.ReferenceData.ObjectName.Value.ToString());
                    if (testExport.ReferenceData.ObjectName.Value.ToString() == input[i + 1])
                    {
                        //Console.WriteLine(testExport.GetType().ToString());
                        if (testExport is NormalExport normalTestExport)
                        {
                            //Console.WriteLine(normalTestExport.Data.Count().ToString());
                            PropertyData prop = normalTestExport.Data[Int32.Parse(input[i + 2])];
                            //Console.WriteLine(prop.GetType().ToString());
                            if (prop.Name.Value.ToString() == input[i + 3])
                            {
                                if (prop is StructPropertyData structPropertyData)
                                {
                                    string[] a = { input[i + 4], input[i + 5], input[i + 6] };
                                    Debug.Write(structPropertyData.PropertyType.ToString());
                                    structPropertyData.Value[0].FromString(d: a);
                                    //Debug.WriteLine(structPropertyData.Value.First().ToString());
                                    Console.WriteLine("Success editing a vector value!");
                                }
                                else if (prop is FloatPropertyData floatPropertyData)
                                {
                                    floatPropertyData.Value = readFloatFromString(input[i + 4]);
                                    Console.WriteLine("Success editing an float numerical value!");
                                }
                                else if (prop is IntPropertyData intPropertyData)
                                {
                                    intPropertyData.Value = Int32.Parse(input[i + 4]);
                                    Console.WriteLine("Success editing an integer numerical value!");
                                }
                                else
                                {
                                    Console.WriteLine("Can't handle this type of data :(, yet...");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            Debug.WriteLine(name);
        }
        catch (Exception e)
        {
            Console.WriteLine("Bad things happened: {0}", e);
        }
        bp.Write(name);
    }
}