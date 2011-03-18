using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace FloseCode.Settings
{
	public class SettingsFile
	{
		readonly IDictionary<string, IDictionary<string, object>> settings;

		/// <summary>
		/// Initializes a new empty instance of the <see cref="FloseCode.Settings.SettingsFile"/> class.
		/// </summary>
		public SettingsFile()
		{
			settings = new Dictionary<string, IDictionary<string, object>>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FloseCode.Settings.SettingsFile"/> class and loads settings from file.
		/// </summary>
		/// <param name='file'>
		/// The Settings file that will be opened.
		/// </param>
		public SettingsFile(string file) : this()
		{
			open(file);
		}

		/// <summary>
		/// Converts a given key to a correct key by following these rules:
		/// - a key must begin with '/'
		/// - a key doesn't end with '/'
		/// - not two '/' after each otehr
		/// - ' ', '[', ']' and '=' are converted to '_'
		/// </summary>
		/// <param name="key">
		/// A <see cref="System.String"/> that will be converted to a correct SettingsFile key
		/// </param>
		/// <returns>
		/// A correct SettingsFile key, as a <see cref="System.String"/>
		/// </returns>
		string correctKey(string key)
		{
			if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(key.Replace("/", "")))
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
				previousWasSlash = (c == '/');
			}
			if (sb[sb.Length - 1] == '/')
				sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		void putValue(string key, object value)
		{
			if (key == null)
				return;

			key = correctKey(key);
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

		public void putString(string key, string value)
		{
			putValue(key, value);
		}

		public void putInteger(string key, int value)
		{
			putValue(key, value);
		}

		public void putDouble(string key, double value)
		{
			putValue(key, value);
		}

		public void putDateTime(string key, DateTime value)
		{
			putValue(key, value);
		}

		public void putBoolean(string key, bool value)
		{
			putValue(key, value);
		}

		public IDictionary<string, object> getAll()
		{
			string currectCategory;
			IDictionary<string, object> tmpList = new Dictionary<string, object>();
			foreach (KeyValuePair<string, IDictionary<string, object>> c in settings) {
				currectCategory = c.Key;
				foreach (KeyValuePair<string, object> v in c.Value) {
					tmpList.Add(currectCategory + v.Key, v.Value);
				}
			}
			return tmpList;
		}

		object getValue(string key, object defaultValue, Type type)
		{
			if (type == null)
				return null;

			key = correctKey(key);
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

			if (type.IsAssignableFrom(val.GetType()))
				return val;

			throw new SettingsFileException("Tried to read \"" + key + "\" as \"" + type.ToString() + "\", but it's a \"" + val.GetType().ToString() + "\"");
		}

		public string getString(string key, string defaultValue = "")
		{
			object val = getValue(key, defaultValue, typeof(string));
			return (string)val;
		}

		public int getInteger(string key, int defaultValue = 0)
		{
			object val = getValue(key, defaultValue, typeof(int));
			return (int)val;
		}

		public double getDouble(string key, double defaultValue = 0)
		{
			object val = getValue(key, defaultValue, typeof(double));
			return (double)val;
		}

		public DateTime getDateTime(string key, DateTime defaultValue = default(DateTime))
		{
			object val = getValue(key, defaultValue, typeof(DateTime));
			return (DateTime)val;
		}

		public bool getBoolean(string key, bool defaultValue = false)
		{
			object val = getValue(key, defaultValue, typeof(bool));
			return (bool)val;
		}

		void open(string fileName)
		{
			settings.Clear();
			if (!File.Exists(fileName))
				return;

			using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8, true)) {
				open(reader);
			}
		}

		void open(StreamReader reader)
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
						writeErrorToConsole("Error while opening: Missing ] at the end");
						writeErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					currentCategory = line.Substring(1, li - 1) + '/';
				} else {
					//Value
					int keyEndeIndex = line.IndexOf('=');
					if (keyEndeIndex == -1) {
						writeErrorToConsole("Error while opening: Missing = after KeyName");
						writeErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					string key = line.Substring(0, keyEndeIndex - 1).TrimEnd(' ');

					int valAnfangIndex = line.IndexOf('"', keyEndeIndex);
					if (valAnfangIndex == -1) { // ungültige Zeile
						writeErrorToConsole("Error while opening key \"" + key + "\": Missing \" after =");
						writeErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					Type valueType;
					try {
						string typeString = line.Substring(keyEndeIndex + 1, valAnfangIndex - keyEndeIndex - 1).Trim();
						valueType = System.Type.GetType(typeString); // Bei ungültigem Typ ist valueType null, und Typ string wird angenommen
					} catch (Exception ex) {
						writeErrorToConsole("Error while opening key \"" + key + "\": " + ex.Message);
						writeErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					int valEndeIndex = line.LastIndexOf('"');
					if (valEndeIndex == -1 || valEndeIndex <= valAnfangIndex) {
						writeErrorToConsole("Error while opening key \"" + key + "\": Missing \" at the end");
						writeErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					string valueString = unEscapeString(line.Substring(valAnfangIndex + 1, valEndeIndex - valAnfangIndex - 1).Trim());
					object val;
					try {
						val = getValueFromSaveString(valueString, valueType);
					} catch (Exception ex) {
						writeErrorToConsole("Error while opening key \"" + key + "\": " + ex.Message);
						writeErrorToConsole("=> Ignoring line: " + line);
						continue;
					}

					putValue(currentCategory + key, val);
				}
			}
		}

		void writeErrorToConsole(string message)
		{
			Console.Error.WriteLine("SettingsFile: " + message);
		}

		static object getValueFromSaveString(string value, Type type)
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

		public void save(string fileName)
		{
			using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8)) {
				writer.NewLine = "\r\n";
				save(writer);
			}
		}

		void save(StreamWriter writer)
		{
			foreach (KeyValuePair<string, IDictionary<string, object>> category in settings) {
				writer.WriteLine("[" + escapeString(category.Key) + "]");
				foreach (KeyValuePair<string, object> kv in category.Value) {
					string typeString = "";
					if (!(kv.Value.GetType() == typeof(string))) {
						typeString = kv.Value.GetType().ToString();
					}
					writer.WriteLine(escapeString(kv.Key) + " = " + typeString + "\"" + escapeString(getValueSaveString(kv.Value)) + "\"");
				}
			}
		}

		string getValueSaveString(object value)
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

		static string escapeString(string text)
		{
			return text.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "\\r");
		}

		static string unEscapeString(string text)
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

	public class SettingsFileException : System.Exception
	{
		public SettingsFileException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		public SettingsFileException(string message) : base(message)
		{
		}

		public SettingsFileException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
