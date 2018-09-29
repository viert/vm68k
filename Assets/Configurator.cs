using System;
using System.Collections.Generic;
using System.IO;

public class Configurator {
	const string iniFilename = ".vm68k.ini";
	const string defaultConfig = @"# default config file
rom = /home/viert/src/emu/vm68krom.bytes
";
	static Dictionary<string, string> properties = new Dictionary<string, string>();

	public static string RomPath;

	static string IniFileFullPath() {
		string configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        return Path.Combine(configDirectory, iniFilename);
	}

	static void WriteDefaults(string filename) {
		StreamWriter writer = new StreamWriter(filename);
		writer.Write(defaultConfig);
		writer.Close();
	}
    
	public static void ReadConfig() {
		string line, key, value;
		string fullPath = IniFileFullPath();

		if (!File.Exists(fullPath)) {
			WriteDefaults(fullPath);
		}
		    
		properties.Clear();
		StreamReader reader = new StreamReader(fullPath);
		while ((line = reader.ReadLine()) != null) {
			line = line.Trim();
			if (line[0] == '#') {
				continue;
			}

			string[] tokens = line.Split('=');
			if (tokens.Length != 2) {
				continue;
			}
			key = tokens[0].Trim();
			value = tokens[1].Trim();
			properties[key] = value;
		}
		reader.Close();

		SetConfigProps();
	}

	static void SetConfigProps() {
		if (properties.ContainsKey("rom")) {
			RomPath = properties["rom"];
		}
	}

}
