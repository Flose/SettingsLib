namespace FloseCode.SettingsLib
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	/// <summary>
	/// This class implements an easy to use settings storage
	/// </summary>
	public class SettingsFile
	{
		readonly IDictionary<string, IDictionary<string, object>> settings;

		/// <summary>
		/// Initializes a new empty instance of the <see cref="FloseCode.SettingsLib.SettingsFile"/> class.
		/// </summary>
		public SettingsFile()
		{
			settings = new Dictionary<string, IDictionary<string, object>>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FloseCode.SettingsLib.SettingsFile"/> class and loads settings from file.
		/// </summary>
		/// <param name='file'>
		/// The Settings file that will be opened.
		/// </param>
		public SettingsFile(string fileName) : this()
		{
			if (fileName == null) {
				throw new ArgumentNullException("fileName");
			}

			if (File.Exists(fileName)) {
				FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

				using (StreamReader reader = new StreamReader(fs, Encoding.UTF8, true)) {
					Open(reader);
				}
			}
		}

		/// <summary>
		/// Converts a given key to a correct key by following these rules:
		/// - a key must begin with '/'
		/// - a key doesn't end with '/'
		/// - not two '/' after each other
		/// - ' ', '[', ']' and '=' are converted to '_'
		/// </summary>
		/// <param name="key">
		/// A <see cref="System.String"/> that will be converted to a correct SettingsFile key
		/// </param>
		/// <returns>
		/// A correct SettingsFile key, as a <see cref="System.String"/>
		/// </returns>
		static string CorrectKey(string key)
		{
			if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(key.Replace("/", string.Empty)))
				throw new SettingsFileException("Key must not be empty");

			StringBuilder sb = new StringBuilder(key.Length);
			if (key[0] != '/')
				sb.Append('/');

			bool previousWasSlash = false;
			foreach (char c in key) {
				if (previousWasSlash && c == '/')
					continue;

				if (c != ' ' && c != '[' && c != ']' && c != '=') {
					sb.Append(c);
				} else {
					sb.Append("_");
				}
				previousWasSlash = c == '/';
			}
			if (sb[sb.Length - 1] == '/')
				sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		void PutValue(string key, object value)
		{
			if (key == null) {
				throw new ArgumentNullException("key");
			}

			if (value == null) {
				throw new ArgumentNullException("value");
			}

			key = CorrectKey(key);
			int li = key.LastIndexOf('/');
			string category = key.Substring(0, li + 1);
			string name = key.Substring(li + 1);

			IDictionary<string, object> categoryPair;
			if (settings.ContainsKey(category)) {
				categoryPair = settings[category];
			} else {
				categoryPair = new Dictionary<string, object>();
				settings.Add(category, categoryPair);
			}

			if (categoryPair.ContainsKey(name)) {
				categoryPair[name] = value;
			} else {
				categoryPair.Add(name, value);
			}
		}

		public void PutString(string key, string value)
		{
			PutValue(key, value);
		}

		public void PutInteger(string key, int value)
		{
			PutValue(key, value);
		}

		public void PutDouble(string key, double value)
		{
			PutValue(key, value);
		}

		public void PutDateTime(string key, DateTime value)
		{
			PutValue(key, value);
		}

		public void PutBoolean(string key, bool value)
		{
			PutValue(key, value);
		}

		public IDictionary<string, object> GetAll()
		{
			return GetAll<object>();
		}

		public IDictionary<string, T> GetAll<T>(string key = null)
		{
			if (key == null)
			{
				key = "/";
			}
			else
			{
				key = CorrectKey(key);
			}

			IDictionary<string, T> tmpList = new Dictionary<string, T>();
			foreach (KeyValuePair<string, IDictionary<string, object>> c in settings)
			{
				string currectCategory = c.Key;
				if (currectCategory.StartsWith(key))
				{
					foreach (KeyValuePair<string, object> v in c.Value)
					{
						if (typeof(T).IsAssignableFrom(v.Value.GetType()))
						{
							tmpList.Add(currectCategory + v.Key, (T) v.Value);
						}
					}
				}
			}
			return tmpList;
		}

		T GetValue<T>(string key, T defaultValue)
		{
			if (key == null) {
				throw new ArgumentNullException("key");
			}

			key = CorrectKey(key);
			int li = key.LastIndexOf('/');
			string category = key.Substring(0, li + 1);
			string name = key.Substring(li + 1);

			IDictionary<string, object> categoryPair;
			if (settings.ContainsKey(category)) {
				categoryPair = settings[category];
			} else {
				return defaultValue;
			}

			object val;
			if (categoryPair.ContainsKey(name)) {
				val = categoryPair[name];
			} else {
				return defaultValue;
			}

			if (typeof(T).IsAssignableFrom(val.GetType()))
				return (T)val;

			throw new SettingsFileException("Tried to read \"" + key + "\" as \"" + typeof(T).ToString() + "\", but it's a \"" + val.GetType().ToString() + "\"");
		}

		public string GetString(string key, string defaultValue = "")
		{
			return GetValue<string>(key, defaultValue);
		}

		public int GetInteger(string key, int defaultValue = 0)
		{
			return GetValue<int>(key, defaultValue);
		}

		public double GetDouble(string key, double defaultValue = 0)
		{
			return GetValue<double>(key, defaultValue);
		}

		public DateTime GetDateTime(string key, DateTime defaultValue = default(DateTime))
		{
			return GetValue<DateTime>(key, defaultValue);
		}

		public bool GetBoolean(string key, bool defaultValue = false)
		{
			return GetValue<bool>(key, defaultValue);
		}

		public void RemoveKey(string key)
		{
			key = CorrectKey(key);
			IDictionary<string, object> tmpList = new Dictionary<string, object>();
			foreach (KeyValuePair<string, IDictionary<string, object>> c in settings) {
				string currectCategory = c.Key;
				if (key.StartsWith(key))
				{
					foreach (KeyValuePair<string, object> v in c.Value)
					{
						tmpList.Add(currectCategory + v.Key, v.Value);
					}
				}
			}
		}

		void Open(StreamReader reader)
		{
			string currentCategory = "/";
			while (!reader.EndOfStream) {
				string line = reader.ReadLine().TrimStart(' ');
				if (string.IsNullOrEmpty(line)) // Leerzeile
					continue;
				if (line[0] == ';') // Kommentarzeile
					continue;

				if (line[0] == '[') {
					// Category
					int li = line.LastIndexOf(']');
					if (li == -1) {
						WriteErrorToConsole("Error while opening: Missing ] at the end");
						WriteErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					currentCategory = line.Substring(1, li - 1) + "/";
				} else {
					//Value
					int keyEndeIndex = line.IndexOf('=');
					if (keyEndeIndex == -1) {
						WriteErrorToConsole("Error while opening: Missing = after KeyName");
						WriteErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					string key = line.Substring(0, keyEndeIndex - 1).TrimEnd(' ');

					int valAnfangIndex = line.IndexOf('"', keyEndeIndex);
					if (valAnfangIndex == -1) { // ungültige Zeile
						WriteErrorToConsole("Error while opening key \"" + key + "\": Missing \" after =");
						WriteErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					Type valueType;
					try {
						string typeString = line.Substring(keyEndeIndex + 1, valAnfangIndex - keyEndeIndex - 1).Trim();
						valueType = System.Type.GetType(typeString); // Bei ungültigem Typ ist valueType null, und Typ string wird angenommen
					} catch (Exception ex) {
						WriteErrorToConsole("Error while opening key \"" + key + "\": " + ex.Message);
						WriteErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					int valEndeIndex = line.LastIndexOf('"');
					if (valEndeIndex == -1 || valEndeIndex <= valAnfangIndex) {
						WriteErrorToConsole("Error while opening key \"" + key + "\": Missing \" at the end");
						WriteErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					string valueString = UnEscapeString(line.Substring(valAnfangIndex + 1, valEndeIndex - valAnfangIndex - 1).Trim());
					object val;
					try {
						val = GetValueFromSaveString(valueString, valueType);
					} catch (Exception ex) {
						WriteErrorToConsole("Error while opening key \"" + key + "\": " + ex.Message);
						WriteErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					PutValue(currentCategory + key, val);
				}
			}
		}

		static void WriteErrorToConsole(string message)
		{
			Console.Error.WriteLine("SettingsFile: " + message);
		}

		static object GetValueFromSaveString(string value, Type type)
		{
			if (typeof(string).IsAssignableFrom(type)) {
				return value;
			} else if (typeof(int).IsAssignableFrom(type)) {
				return int.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
			} else if (typeof(double).IsAssignableFrom(type)) {
				return double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
			} else if (typeof(DateTime).IsAssignableFrom(type)) {
				return DateTime.FromBinary(long.Parse(value, System.Globalization.CultureInfo.InvariantCulture));
			} else if (typeof(bool).IsAssignableFrom(type)) {
				if (value == "0") {
					return false;
				} else {
					return true;
				}
			} else {
				// Unknown object type
				return value;
			}
		}

		public void Save(string fileName)
		{
			using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8)) {
				writer.NewLine = "\r\n";
				Save(writer);
			}
		}

		void Save(TextWriter writer)
		{
			foreach (KeyValuePair<string, IDictionary<string, object>> category in settings) {
				writer.WriteLine("[" + EscapeString(category.Key) + "]");
				foreach (KeyValuePair<string, object> kv in category.Value) {
					string typeString = string.Empty;
					if (!(kv.Value.GetType() == typeof(string))) {
						typeString = kv.Value.GetType().ToString();
					}
					writer.WriteLine(EscapeString(kv.Key) + " = " + typeString + "\"" + EscapeString(GetValueSaveString(kv.Value)) + "\"");
				}
				writer.WriteLine();
			}
		}

		static string GetValueSaveString(object value)
		{
			if (typeof(string).IsAssignableFrom(value.GetType())) {
				return (string)value;
			} else if (typeof(int).IsAssignableFrom(value.GetType())) {
				return ((int)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
			} else if (typeof(double).IsAssignableFrom(value.GetType())) {
				return ((double)value).ToString("R", System.Globalization.CultureInfo.InvariantCulture);
			} else if (typeof(DateTime).IsAssignableFrom(value.GetType())) {
				return ((DateTime)value).ToBinary().ToString(System.Globalization.CultureInfo.InvariantCulture);
			} else if (typeof(bool).IsAssignableFrom(value.GetType())) {
				if ((bool)value == true) {
					return "1";
				} else {
					return "0";
				}
			} else {
				// Unknown object type
				return string.Empty;
			}
		}

		static string EscapeString(string text)
		{
			return text.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "\\r");
		}

		static string UnEscapeString(string text)
		{
			StringBuilder sb = new StringBuilder(text.Length);
			bool escaping = false;
			foreach (char c in text) {
				if (!escaping && c == '\\') {
					escaping = true;
					continue;
				}
				if (!escaping) {
					sb.Append(c);
					continue;
				}

				switch (c) {
				case 'n':
					sb.Append('\n');
					break;
				case 'r':
					sb.Append('\r');
					break;
				default:
					sb.Append(c);
					break;
				}
				escaping = false;
			}
			return sb.ToString();
		}
	}
}
