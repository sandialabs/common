using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace json
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                ShowUsage();
                return;
            }

            string option = args[0];
            string filename = args[1];

            string text = File.ReadAllText(filename);
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            switch (option)
            {
                case "-csv":
                    OutputCSV(dict, filename);
                    break;
                case "-sort":
                    OutputSortedJSON(dict, filename);
                    break;
                case "-clean":
                    if(args.Length < 3)
                    {
                        ShowUsage();
                        return;
                    }

                    string filename2 = args[2];
                    OutputCleaned(dict, filename2);
                    break;
                default:
                    ShowUsage();
                    break;
            }
        }

        static void OutputCSV(Dictionary<string, string> dict, string filename)
        {
            List<string> keys = new List<string>(dict.Keys);
            keys.Sort();

            string output_filename = Path.ChangeExtension(filename, "csv");
            List<string> lines = new List<string>();
            foreach(string s in keys)
                lines.Add(string.Format(@"{0},{1}", s, dict[s]));
            File.WriteAllLines(output_filename, lines.ToArray());
        }

        static void OutputSortedJSON(Dictionary<string, string> dict, string filename)
        {
            string json = JsonConvert.SerializeObject(dict);
            json = JsonUtility.NormalizeJsonString(json);
            File.WriteAllText(filename, json);
            //System.Console.WriteLine(json);
        }

        static void OutputCleaned(Dictionary<string, string> dict, string filename2)
        {
            List<string> keys = new List<string>(dict.Keys);
            foreach (string key in keys)
                dict[key] = key;
            OutputSortedJSON(dict, filename2);
        }

        static void ShowUsage()
        {
            System.Console.WriteLine("Usage: json <options> <JSON filename> [<JSON filename 2>]");
            System.Console.WriteLine(" where <options> is one of the following:");
            System.Console.WriteLine("  -csv to convert the JSON to CSV");
            System.Console.WriteLine("  -sort to sort the properties in JSON");
            System.Console.WriteLine("     the file will be updated");
            System.Console.WriteLine("  -clean to convert the values to match the keys");
            System.Console.WriteLine("     the output will be written to <JSON filename 2>");
            System.Console.WriteLine(" and <JSON filename> is the JSON file");
            System.Console.WriteLine("The JSON file should have a series of name/value pairs");
        }
    }

    public class JsonUtility
    {
        public static string NormalizeJsonString(string json)
        {
            // Parse json string into JObject.
            var parsedObject = JObject.Parse(json);

            // Sort properties of JObject.
            var normalizedObject = SortPropertiesAlphabetically(parsedObject);

            // Serialize JObject .
            return JsonConvert.SerializeObject(normalizedObject, Formatting.Indented);
        }

        private static JObject SortPropertiesAlphabetically(JObject original)
        {
            var result = new JObject();

            foreach (var property in original.Properties().ToList().OrderBy(p => p.Name))
            {
                var value = property.Value as JObject;

                if (value != null)
                {
                    value = SortPropertiesAlphabetically(value);
                    result.Add(property.Name, value);
                }
                else
                {
                    result.Add(property.Name, property.Value);
                }
            }

            return result;
        }
    }

}
