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
            If c <> " " AndAlso c <> "[" AndAlso c <> "]" Then
                sb.Append(c)
            End If
            previousWasSlash = (c = "/"c)
        Next
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

    Public Sub putDate(ByVal key As String, ByVal value As DateTime)
        putValue(key, value)
    End Sub

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
        'TODO: throw Exception ?
        Console.Error.WriteLine("Tried to read """ & key & """ as """ & type.ToString & """, but it's a """ & val.GetType.ToString & """")
        Return defaultValue
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

    Public Sub open(ByVal fileName As String)
        settings.Clear()
        If Not File.Exists(fileName) Then Exit Sub

        Using Reader As New StreamReader(fileName, System.Text.Encoding.UTF8, True)
            Dim currentCategory As String = "/"
            While Not Reader.EndOfStream()
                Dim line As String = Reader.ReadLine().TrimStart(New Char() {" "c})
                If String.IsNullOrEmpty(line) Then Continue While 'Leerzeile
                If line.Chars(0) = ";" Then Continue While 'Kommentarzeile

                If line.Chars(0) = "[" Then
                    Dim li As Integer = line.LastIndexOf("]")
                    If li = -1 Then Continue While 'ungültige zeile

                    currentCategory = line.Substring(1, li - 2) & "/"
                Else
                    Dim keyEndeIndex As Integer = line.IndexOf("=")
                    If keyEndeIndex = -1 Then Continue While 'ungültige zeile

                    Dim key As String = line.Substring(0, keyEndeIndex - 1).TrimEnd(" "c)

                    Dim valAnfangIndex As Integer = line.IndexOf("""", keyEndeIndex)
                    If valAnfangIndex = -1 Then Continue While 'ungültige zeile

                    Dim valueType As Type
                    Try
                        Dim typeString As String = line.Substring(keyEndeIndex + 1, valAnfangIndex - keyEndeIndex - 1).Trim
                        valueType = System.Type.GetType(typeString)
                    Catch ex As Exception
                        Console.Error.WriteLine("Error while loading key """ & key & """: " & ex.Message)
                        Console.Error.WriteLine("  Ignoring line.")
                        Continue While
                    End Try

                    Dim valEndeIndex As Integer = line.LastIndexOf("""")
                    If valEndeIndex = -1 OrElse valEndeIndex <= valAnfangIndex Then Continue While 'ungültige zeile

                    Dim valueString As String = UnEscapeString(line.Substring(valAnfangIndex + 1, valEndeIndex - valAnfangIndex - 1).Trim)
                    Dim val As Object
                    Try
                        val = getValueFromSaveString(valueString, valueType)
                    Catch ex As Exception
                        Console.Error.WriteLine("Error while loading key """ & key & """: " & ex.Message)
                        Console.Error.WriteLine("  Ignoring line.")
                        Continue While
                    End Try

                    putValue(currentCategory & key, val)
                End If
            End While
        End Using
    End Sub

    Private Function getValueFromSaveString(ByVal value As String, ByVal type As Type) As Object
        If GetType(String).IsAssignableFrom(type) Then
            Return value
        ElseIf GetType(Integer).IsAssignableFrom(type) Then
            Return Integer.Parse(value)
        ElseIf GetType(DateTime).IsAssignableFrom(type) Then
            Return DateTime.FromBinary(Long.Parse(value))
        Else
            'Unknown Object type
            Return value
        End If
    End Function

    Public Sub save(ByVal fileName As String)
        Using Writer As New StreamWriter(fileName, False, System.Text.Encoding.UTF8)
            Writer.NewLine = vbCr
            For Each category As KeyValuePair(Of String, IDictionary(Of String, Object)) In settings
                Writer.WriteLine("[" & EscapeString(category.Key) & "]")
                For Each kv As KeyValuePair(Of String, Object) In category.Value
                    Writer.WriteLine(EscapeString(kv.Key) & " = " & kv.Value.GetType.ToString & """" & EscapeString(getValueSaveString(kv.Value)) & """") 'TODO schaun wie das mit unterklassen ist
                Next
                Writer.WriteLine()
            Next
        End Using
    End Sub

    Private Function getValueSaveString(ByVal value As Object) As String
        If TypeOf value Is String Then
            Return DirectCast(value, String)
        ElseIf TypeOf value Is Integer Then
            Return value.ToString
        ElseIf TypeOf value Is DateTime Then
            Return DirectCast(value, DateTime).ToBinary.ToString
        Else
            'Unknown Object type
            Return ""
        End If
    End Function

    Private Function EscapeString(ByVal text As String) As String
        Return text.Replace("\", "\\").Replace(vbCr, "\n")
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
