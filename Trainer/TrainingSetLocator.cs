
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace Trainer
{
	/// <summary>
    /// Hittar alla filer med sanningar som finns i en katalog eller dess underkataloger.
    /// </summary>
    public class TrainingSetLocator
    {
		public static string[] FindTruthFiles(string rootPath)
		{
			Console.WriteLine("Searching in directory: '" + rootPath + "'.");
			return FindXmlFiles(rootPath).Where(x => IsTruthFile(x)).ToArray();
		}
		
        private static string[] FindXmlFiles(string rootPath)
		{
			return Directory.GetFiles(rootPath, "*.xml", SearchOption.AllDirectories);
		}
		
		public static bool IsTruthFile(string path)
		{
			//Console.Write("IsTruthFile: Testing file: '" + path + "'... ");
			
			var reader = new XmlTextReader(path);
			
			try 
			{
				while(reader.Read())
				{
					//Console.WriteLine("NodeType: " + reader.NodeType.ToString() + " Name: " + reader.Name);
					if(reader.NodeType == XmlNodeType.Element && string.Compare(reader.Name, "xml", true) != 0)
					{
						bool result = string.Compare(reader.Name, "TrainingSet", true) == 0;
						//Console.WriteLine(result.ToString());
						return result;
					}
				}
				//Console.WriteLine("false");
				return false;
			}
			finally
			{
				reader.Close();
			}
		}
    }
}
