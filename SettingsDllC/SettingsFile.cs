using System;
using System.Collections.Generic;
using System.Text;

namespace FloseCode.Settings
{
	public class SettingsFile
	{
		readonly IDictionary<string, IDictionary<string, object>> settings;
		readonly DateTime nullDate = new DateTime(0);

		public SettingsFile()
		{
			settings = new Dictionary<string, IDictionary<string, object>>();
		}

		public SettingsFile(string file) : this()
		{
			open(file);
		}

		/// <summary>
		/// Converts a given key to a correct key by following these rules:
		/// - a key must begin with '/'
		/// - a key doesn't end with '/'
		/// </summary>
		/// <param name="key">
		/// A <see cref="System.String"/>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		private string correctKey(string key)
		{
			if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(key.Replace("/", "")))
				throw new SettingsFileException("Key must not be empty");

			System.Text.StringBuilder sb = new System.Text.StringBuilder(key.Length);
			if (key [0] != '/')
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
			if (sb [sb.Length - 1] == '/')
				sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		void putValue(string key,object value)
		{
			if (key == null)
				return;

			key = correctKey(key);
			int li = key.LastIndexOf('/');
			string category = key.Substring(0, li + 1);
			string name = key.Substring(li + 1);

			IDictionary<string, object> categoryPair;
			if (settings.ContainsKey(category)) {
				categoryPair = settings [category];
			} else {
				categoryPair = new Dictionary<string, object>();
				settings.Add(category, categoryPair);
			}

			if (categoryPair.ContainsKey(name)) {
				categoryPair [name] = value;
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

		private object getValue(string key, object defaultValue, Type type)
		{
			if (type == null)
				return null;

			key = correctKey(key);
			int li = key.LastIndexOf('/');
			string category = key.Substring(0, li + 1);
			string name = key.Substring(li + 1);

			IDictionary<string, object> categoryPair;
			if (settings.ContainsKey(category)) {
				categoryPair = settings [category];
			} else {
				return defaultValue;
			}

			object val;
			if (categoryPair.ContainsKey(name)) {
				val = categoryPair [name];
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

		private void open(string file)
		{


		}
		
		public void save(string file)
		{


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


	/*
Public Class SettingsFile
	Private ReadOnly settings As IDictionary(Of String, IDictionary(Of String, Object))

	Public Sub New()
		Me.settings = New Dictionary(Of String, IDictionary(Of String, Object))
	End Sub

	Public Sub New(ByVal file As String)
		Me.New()
		open(file)
	End Sub

	Private Function correctKey(ByVal key As String) As String
		If String.IsNullOrEmpty(key) Then Return "/"

		Dim sb As New Text.StringBuilder(key.Length)
		If key.Chars(0) <> "/" Then sb.Append("/"c)
		Dim previousWasSlash As Boolean = False
		For Each c As Char In key
			If previousWasSlash AndAlso c = "/"c Then Continue For
			If c <> " " AndAlso c <> "[" AndAlso c <> "]" AndAlso c <> "=" Then
				sb.Append(c)
			Else
				sb.Append("_")
			End If
			previousWasSlash = (c = "/"c)
		Next
		If sb.Chars(sb.Length - 1) = "/"c Then sb.Remove(sb.Length - 1, 1)
		Return sb.ToString
	End Function

	Private Sub putValue(ByVal key As String, ByVal value As Object)
		If value Is Nothing Then Exit Sub

		key = correctKey(key)
		Dim li As Integer = key.LastIndexOf("/")
		Dim category As String = key.Substring(0, li + 1)
		Dim name As String = key.Substring(li + 1)

		Dim categoryPair As IDictionary(Of String, Object)
		If settings.ContainsKey(category) Then
			categoryPair = settings.Item(category)
		Else
			categoryPair = New Dictionary(Of String, Object)
			settings.Add(category, categoryPair)
		End If

		If categoryPair.ContainsKey(name) Then
			categoryPair.Item(name) = value
		Else
			categoryPair.Add(name, value)
		End If
	End Sub

	Public Sub putString(ByVal key As String, ByVal value As String)
		putValue(key, value)
	End Sub

	Public Sub putInteger(ByVal key As String, ByVal value As Integer)
		putValue(key, value)
	End Sub

	Public Sub putDouble(ByVal key As String, ByVal value As Double)
		putValue(key, value)
	End Sub

	Public Sub putDateTime(ByVal key As String, ByVal value As DateTime)
		putValue(key, value)
	End Sub

	Public Sub putBoolean(ByVal key As String, ByVal value As Boolean)
		putValue(key, value)
	End Sub

	Public Function getAll() As Dictionary(Of String, Object)
		Dim currentCategory As String, tmpList As New Dictionary(Of String, Object)
		For Each c As KeyValuePair(Of String, IDictionary(Of String, Object)) In settings
			currentCategory = c.Key
			For Each v As KeyValuePair(Of String, Object) In c.Value
				tmpList.Add(currentCategory & v.Key, v.Value)
			Next
		Next
		Return tmpList
	End Function

	Private Function getValue(ByVal key As String, ByVal defaultValue As Object, ByVal type As Type) As Object
		If type Is Nothing Then Return Nothing

		key = correctKey(key)
		Dim li As Integer = key.LastIndexOf("/")
		Dim category As String = key.Substring(0, li + 1)
		Dim name As String = key.Substring(li + 1)

		Dim categoryPair As IDictionary(Of String, Object)
		If settings.ContainsKey(category) Then
			categoryPair = settings.Item(category)
		Else
			Return defaultValue
		End If

		Dim val As Object
		If categoryPair.ContainsKey(name) Then
			val = categoryPair.Item(name)
		Else
			Return defaultValue
		End If
		If type.IsAssignableFrom(val.GetType) Then
			Return val
		End If
		Throw New SettingsFileException("Tried to read """ & key & """ as """ & type.ToString & """, but it's a """ & val.GetType.ToString & """")
	End Function

	Public Function getString(ByVal key As String, Optional ByVal defaultValue As String = "") As String
		Dim val As Object = getValue(key, defaultValue, GetType(String))
		Return DirectCast(val, String)
	End Function

	Public Function getInteger(ByVal key As String, Optional ByVal defaultValue As Integer = 0) As Integer
		Dim val As Object = getValue(key, defaultValue, GetType(Integer))
		Return DirectCast(val, Integer)
	End Function

	Public Function getDouble(ByVal key As String, Optional ByVal defaultValue As Double = 0) As Double
		Dim val As Object = getValue(key, defaultValue, GetType(Double))
		Return DirectCast(val, Double)
	End Function

	Public Function getDateTime(ByVal key As String, Optional ByVal defaultValue As DateTime = Nothing) As DateTime
		Dim val As Object = getValue(key, defaultValue, GetType(DateTime))
		Return DirectCast(val, DateTime)
	End Function

	Public Function getBoolean(ByVal key As String, Optional ByVal defaultValue As Boolean = False) As Boolean
		Dim val As Object = getValue(key, defaultValue, GetType(Boolean))
		Return DirectCast(val, Boolean)
	End Function

	Public Sub open(ByVal fileName As String)
		settings.Clear()
		If Not File.Exists(fileName) Then Exit Sub

		Using Reader As New StreamReader(fileName, System.Text.Encoding.UTF8, True)
			open(Reader)
		End Using
	End Sub

	Private Sub open(ByVal Reader As StreamReader)
		Dim currentCategory As String = "/"
		While Not Reader.EndOfStream()
			Dim line As String = Reader.ReadLine().TrimStart(New Char() {" "c})
			If String.IsNullOrEmpty(line) Then Continue While 'Leerzeile
			If line.Chars(0) = ";" Then Continue While 'Kommentarzeile

			If line.Chars(0) = "[" Then
				Dim li As Integer = line.LastIndexOf("]")
				If li = -1 Then 'ungültige zeile
				WriteErrorToConsole("Error while opening: Missing ] at the end")
				WriteErrorToConsole("=> Ignoring line: " & line)
				Continue While
				End If

				currentCategory = line.Substring(1, li - 1) & "/"
			Else
				Dim keyEndeIndex As Integer = line.IndexOf("=")
				If keyEndeIndex = -1 Then 'ungültige zeile
				WriteErrorToConsole("Error while opening: Missing = after KeyName")
				WriteErrorToConsole("=> Ignoring line: " & line)
				Continue While
				End If

				Dim key As String = line.Substring(0, keyEndeIndex - 1).TrimEnd(" "c)

				Dim valAnfangIndex As Integer = line.IndexOf("""", keyEndeIndex)
				If valAnfangIndex = -1 Then 'ungültige zeile
				WriteErrorToConsole("Error while opening key """ & key & """: Missing "" after =")
				WriteErrorToConsole("=> Ignoring line: " & line)
				Continue While
				End If

				Dim valueType As Type
				Try
				Dim typeString As String = line.Substring(keyEndeIndex + 1, valAnfangIndex - keyEndeIndex - 1).Trim
				valueType = System.Type.GetType(typeString) 'bei ungültigem Typ ist valueType nothing, und Typ String wird angenommen
				Catch ex As Exception
				WriteErrorToConsole("Error while opening key """ & key & """: " & ex.Message)
				WriteErrorToConsole("=> Ignoring line: " & line)
				Continue While
				End Try

				Dim valEndeIndex As Integer = line.LastIndexOf("""")
				If valEndeIndex = -1 OrElse valEndeIndex <= valAnfangIndex Then 'ungültige zeile
				WriteErrorToConsole("Error while opening key """ & key & """: Missing "" at the end")
				WriteErrorToConsole("=> Ignoring line: " & line)
				Continue While
				End If


				Dim valueString As String = UnEscapeString(line.Substring(valAnfangIndex + 1, valEndeIndex - valAnfangIndex - 1).Trim)
				Dim val As Object
				Try
				val = getValueFromSaveString(valueString, valueType)
				Catch ex As Exception
				WriteErrorToConsole("Error while opening key """ & key & """: " & ex.Message)
				WriteErrorToConsole("=> Ignoring line: " & line)
				Continue While
				End Try

				putValue(currentCategory & key, val)
			End If
		End While
	End Sub

	Private Sub WriteErrorToConsole(ByVal message As String)
		Console.Error.WriteLine("SettingsFile: " & message)
	End Sub

	Private Function getValueFromSaveString(ByVal value As String, ByVal type As Type) As Object
		If GetType(String).IsAssignableFrom(type) Then
			Return value
		ElseIf GetType(Integer).IsAssignableFrom(type) Then
			Return Integer.Parse(value, Globalization.CultureInfo.InvariantCulture)
		ElseIf GetType(Double).IsAssignableFrom(type) Then
			Return Double.Parse(value, Globalization.CultureInfo.InvariantCulture)
		ElseIf GetType(DateTime).IsAssignableFrom(type) Then
			Return DateTime.FromBinary(Long.Parse(value))
		ElseIf GetType(Boolean).IsAssignableFrom(type) Then
			If value = "0" Then
				Return False
			Else
				Return True
			End If
		Else
			'Unknown Object type
			Return value
		End If
	End Function

	Public Sub save(ByVal fileName As String)
		Using Writer As New StreamWriter(fileName, False, System.Text.Encoding.UTF8)
			Writer.NewLine = vbCrLf
			save(Writer)
		End Using
	End Sub

	Private Sub save(ByVal Writer As StreamWriter)
		For Each category As KeyValuePair(Of String, IDictionary(Of String, Object)) In settings
			Writer.WriteLine("[" & EscapeString(category.Key) & "]")
			For Each kv As KeyValuePair(Of String, Object) In category.Value
				Dim typeString As String = ""
				If Not TypeOf kv.Value Is String Then
				typeString = kv.Value.GetType.ToString
				End If
				Writer.WriteLine(EscapeString(kv.Key) & " = " & typeString & """" & EscapeString(getValueSaveString(kv.Value)) & """") 'TODO schaun wie das mit unterklassen ist
			Next
			Writer.WriteLine()
		Next
	End Sub

	Private Function getValueSaveString(ByVal value As Object) As String
		If GetType(String).IsAssignableFrom(value.GetType) Then
			Return DirectCast(value, String)
		ElseIf GetType(Integer).IsAssignableFrom(value.GetType) Then
			Return DirectCast(value, Integer).ToString(System.Globalization.CultureInfo.InvariantCulture)
		ElseIf GetType(Double).IsAssignableFrom(value.GetType) Then
			Return DirectCast(value, Double).ToString("R", Globalization.CultureInfo.InvariantCulture)
		ElseIf GetType(DateTime).IsAssignableFrom(value.GetType) Then
			Return DirectCast(value, DateTime).ToBinary.ToString
		ElseIf GetType(Boolean).IsAssignableFrom(value.GetType) Then
			If DirectCast(value, Boolean) = True Then
				Return "1"
			Else
				Return "0"
			End If
		Else
			'Unknown Object type
			Return ""
		End If
	End Function

	Private Function EscapeString(ByVal text As String) As String
		Return text.Replace("\", "\\").Replace(vbCr, "\n").Replace(vbLf, "\r")
	End Function

	Private Function UnEscapeString(ByVal text As String) As String
		Dim t As New System.Text.StringBuilder(text.Length)
		Dim escaping As Boolean = False
		For Each c As Char In text
			If Not escaping AndAlso c = "\" Then
				escaping = True
			Else
				If escaping Then
				If c = "n" Then
					t.Append(vbCr)
				ElseIf c = "r" Then
					t.Append(vbLf)
				Else
					t.Append(c)
				End If
				escaping = False
				Else
				t.Append(c)
				End If
			End If
		Next
		Return t.ToString
	End Function
End Class

Public Class SettingsFileException
	Inherits System.Exception

	Public Sub New(ByVal info As Runtime.Serialization.SerializationInfo, ByVal context As Runtime.Serialization.StreamingContext)
		MyBase.new(info, context)
	End Sub

	Public Sub New(ByVal message As String)
		Me.New(message, Nothing)
	End Sub

	Public Sub New(ByVal message As String, ByVal innerException As Exception)
		MyBase.New(message, innerException)
	End Sub
End Class
*/