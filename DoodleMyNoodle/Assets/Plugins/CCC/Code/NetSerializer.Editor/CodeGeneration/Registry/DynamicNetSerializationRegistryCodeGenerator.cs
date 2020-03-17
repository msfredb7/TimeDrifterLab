﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;

public static class DynamicNetSerializationRegistryCodeGenerator
{
    static readonly string CompletePath = NetSerializationCodeGenSettings.Registry_FilePath + '/' + NetSerializationCodeGenSettings.Registry_FileName;


    [MenuItem(NetSerializationCodeGenSettings.MenuName_Generate_Registry)]
    public static void Generate()
    {
        var types = NetSerializationCodeGenUtility.GetNetSerializableTypes();
        types.RemoveAll((t) => t.IsAbstract);
        GenerateCode(types.ToArray());

        AssetDatabase.Refresh();
    }

    [MenuItem(NetSerializationCodeGenSettings.MenuName_Clear_Registry)]
    public static void Clear()
    {
        GenerateCode(new Type[0]);

        AssetDatabase.Refresh();
    }

    static ulong GetHashFromNetMessageTypes(Type[] netMessageTypes)
    {
        StringBuilder concatenatedNamesBuilder = new StringBuilder();
        foreach (var t in netMessageTypes)
        {
            concatenatedNamesBuilder.Append(t.FullName);
        }

        string concatenatedNames = concatenatedNamesBuilder.ToString();
        byte[] crcData = new byte[concatenatedNames.Length * 2]; // each char is 2 bytes

        BitStreamWriter writer = new BitStreamWriter(crcData);
        for (int i = 0; i < concatenatedNames.Length; i++)
        {
            writer.WriteChar(concatenatedNames[i]);
        }

        return Crc64.Compute(crcData);
    }

    static void GenerateCode(Type[] netMessageTypes)
    {
        ulong crc = GetHashFromNetMessageTypes(netMessageTypes);

        if (!Directory.Exists(NetSerializationCodeGenSettings.Registry_FilePath))
        {
            Directory.CreateDirectory(NetSerializationCodeGenSettings.Registry_FilePath);
        }

        if (!File.Exists(CompletePath))
        {
            File.Create(CompletePath).Close();
        }

        using (FileStream fileStream = File.Open(CompletePath, FileMode.Truncate))
        {
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Flush();

                writer.WriteLine("// THIS CODE IS GENERATED");
                writer.WriteLine("// DO NOT MODIFY IT");
                writer.WriteLine();
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine();
                writer.WriteLine("public static class " + NetSerializationCodeGenSettings.Registry_ClassName);
                writer.WriteLine("{");

                writer.WriteLine("    public static readonly ulong crc = " + crc + ";");

                writer.WriteLine();

                writer.WriteLine("    public static readonly Type[] types = new Type[]");
                writer.WriteLine("    {");
                bool addComma = false;
                foreach (Type type in netMessageTypes)
                {
                    if (addComma)
                        writer.WriteLine("        ,");
                    writer.WriteLine("        typeof(" + type.GetNiceFullName() + ")");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine();


                writer.WriteLine("    public static readonly Dictionary<Type, Func<object, int>> map_GetBitSize = new Dictionary<Type, Func<object, int>>()");
                writer.WriteLine("    {");
                addComma = false;
                for (int i = 0; i < netMessageTypes.Length; i++)
                {
                    Type t = netMessageTypes[i];

                    if (addComma)
                        writer.WriteLine("        ,");
                    writer.WriteLine("        [typeof(" + t.GetNiceFullName() + ")] = (obj) =>");
                    writer.WriteLine("        {");
                    writer.WriteLine("            " + t.GetNiceFullName() + " castedObj = (" + t.GetNiceFullName() + ")obj;");
                    if (NetSerializationCodeGenUtility.ConsideredAsValueType(t))
                        writer.WriteLine("            return " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".GetNetBitSize(ref castedObj);");
                    else
                        writer.WriteLine("            return " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".GetNetBitSize(castedObj);");
                    writer.WriteLine("        }");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine();


                writer.WriteLine("    public static readonly Dictionary<Type, Action<object, BitStreamWriter>> map_Serialize = new Dictionary<Type, Action<object, BitStreamWriter>>()");
                writer.WriteLine("    {");
                addComma = false;
                for (int i = 0; i < netMessageTypes.Length; i++)
                {
                    Type t = netMessageTypes[i];

                    if (addComma)
                        writer.WriteLine("        ,");
                    writer.WriteLine("        [typeof(" + t.GetNiceFullName() + ")] = (obj, writer) =>");
                    writer.WriteLine("        {");
                    writer.WriteLine("            " + t.GetNiceFullName() + " castedObj = (" + t.GetNiceFullName() + ")obj;");
                    if (NetSerializationCodeGenUtility.ConsideredAsValueType(t))
                        writer.WriteLine("            " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".NetSerialize(ref castedObj, writer);");
                    else
                        writer.WriteLine("            " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".NetSerialize(castedObj, writer);");
                    writer.WriteLine("        }");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine();


                writer.WriteLine("    public static readonly Dictionary<UInt16, Func<BitStreamReader, object>> map_Deserialize = new Dictionary<UInt16, Func<BitStreamReader, object>>()");
                writer.WriteLine("    {");
                addComma = false;
                for (int i = 0; i < netMessageTypes.Length; i++)
                {
                    Type t = netMessageTypes[i];

                    if (addComma)
                        writer.WriteLine("        ,");
                    writer.WriteLine("        [" + i + "] = (reader) =>");
                    writer.WriteLine("        {");
                    writer.WriteLine("            " + t.GetNiceFullName() + " obj = new " + t.GetNiceFullName() + "();");
                    if (NetSerializationCodeGenUtility.ConsideredAsValueType(t))
                        writer.WriteLine("            " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".NetDeserialize(ref obj, reader);");
                    else
                        writer.WriteLine("            " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".NetDeserialize(obj, reader);");
                    writer.WriteLine("            return obj;");
                    writer.WriteLine("        }");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine("}");
            }
        }
    }
}