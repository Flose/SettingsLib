Imports System.Collections.Generic
Imports System.IO

'Keys are caseSensitive, leers werden durch "_" ersetzt, beginnen immer mit "/", doppelte "/" werden durch eins ersetzt

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
                Writer.WriteLine(EscapeString(kv.Key) & " = " & kv.Value.GetType.ToString & """" & EscapeString(getValueSaveString(kv.Value)) & """") 'TODO schaun wie das mit unterklassen ist
            Next
            Writer.WriteLine()
        Next
    End Sub

    Private Function getValueSaveString(ByVal value As Object) As String
        If GetType(String).IsAssignableFrom(value.GetType) Then
            Return DirectCast(value, String)
        ElseIf GetType(Integer).IsAssignableFrom(value.GetType) Then
            Return DirectCast(value, Integer).ToString(System.Globalization.CultureInfo.InvariantCulture)
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
