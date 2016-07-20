Imports FloseCode.SettingsLib
Imports NUnit.Framework

<TestFixture()> _
Public Class Test_Empty_File

    <Test()> _
    Public Sub Test_Open_Not_existing_File()
        Dim temp As String = "fjdfjksdfijeavösjarskdlffjaklsdfjkljsdfasdlk"
        Dim settings As New SettingsFile(temp)
    End Sub

    <Test()> _
    Public Sub Test_Open_Empty_File()
        Dim temp As String = IO.Path.GetTempFileName
        Dim settings As New SettingsFile(temp)
    End Sub

    <Test()> _
    Public Sub Test_Save_empty_File()
        Dim settings As New SettingsFile()
        Dim temp As String = IO.Path.GetTempFileName
        settings.Save(temp)
    End Sub

    <Test()> _
    Public Sub Test_Save_and_Open_empty_File()
        Dim settings As New SettingsFile()
        Dim temp As String = IO.Path.GetTempFileName
        settings.Save(temp)
        settings = New SettingsFile(temp)
    End Sub

    <Test()> _
    Public Sub Test_Get_All_Empty()
        Dim settings As New SettingsFile()
        Assert.AreEqual(0, settings.GetAll.Count)
    End Sub

    <Test()> _
    Public Sub Test_Get_All_Key_Empty()
        Dim settings As New SettingsFile()
        Assert.AreEqual(0, settings.GetAll(Of Object)("test").Count)
    End Sub

    <Test()> _
    Public Sub Test_Get_All_Add_One()
        Dim settings As New SettingsFile()
        Dim key As String = "jfds03/fjdskj84jl"
        Dim correctKey As String = DirectCast(UnitTestUtilities.RunInstanceMethod("CorrectKey", settings, New String() {key}), String)
        Dim value As String = "fjsad83fjds229/$)§"
        settings.PutString(key, value)
        Assert.AreEqual(1, settings.GetAll.Count)
        Assert.AreEqual(value, settings.GetAll.Item(correctKey))
    End Sub

    <Test()> _
    Public Sub Test_Get_All_Key_Add_One()
        Dim settings As New SettingsFile()
        Dim key As String = "jfds03/fjdskj84jl"
        Dim correctKey As String = DirectCast(UnitTestUtilities.RunInstanceMethod("CorrectKey", settings, New String() {key}), String)
        Dim value As String = "fjsad83fjds229/$)§"
        settings.PutString(key, value)
        Assert.AreEqual(1, settings.GetAll(Of String)("jfds03").Count)
        Assert.AreEqual(0, settings.GetAll(Of String)("jfds3").Count)
        Assert.AreEqual(0, settings.GetAll(Of Integer)("jfds03").Count)
        Assert.AreEqual(1, settings.GetAll(Of Object)("jfds03").Count)
        Assert.AreEqual(value, settings.GetAll(Of String)("jfds03").Item(correctKey))
        Assert.AreEqual(value, settings.GetAll(Of Object)("jfds03").Item(correctKey))
        Assert.AreEqual(value, settings.GetAll(Of String)().Item(correctKey))
    End Sub

    <Test()> _
    Public Sub Test_Get_All_Add_One_Thrice()
        Dim settings As New SettingsFile()
        Dim key As String = "jfds03/fjdskj84jl"
        Dim correctKey As String = DirectCast(UnitTestUtilities.RunInstanceMethod("CorrectKey", settings, New String() {key}), String)
        Dim value As String = "fjsad83fjds229/$)§"
        settings.PutString(key, value)
        Assert.AreEqual(1, settings.GetAll.Count)
        Assert.AreEqual(value, settings.GetAll.Item(correctKey))

        value = "fjafsdjf93474/(/)$§sad83fjds229/$)§"
        settings.PutString(key, value)

        Assert.AreEqual(1, settings.GetAll.Count)
        Assert.AreEqual(value, settings.GetAll.Item(correctKey))

        Dim value2 As DateTime = New DateTime(48934384339)
        settings.PutDateTime(key, value2)

        Assert.AreEqual(1, settings.GetAll.Count)
        Assert.AreEqual(value2, settings.GetAll.Item(correctKey))
    End Sub

    <Test()> _
    Public Sub Test_Get_All_Add_Three()
        Dim settings As New SettingsFile()
        Dim key As String = "jfds03/fjdskj84jl"
        Dim correctKey As String = DirectCast(UnitTestUtilities.RunInstanceMethod("CorrectKey", settings, New String() {key}), String)
        Dim value As String = "fjsad83fjds229/$)§"
        settings.PutString(key, value)
        Assert.AreEqual(1, settings.GetAll.Count)
        Assert.AreEqual(value, settings.GetAll.Item(correctKey))

        key = "jfds03/43__fjdskj84jl"
        correctKey = DirectCast(UnitTestUtilities.RunInstanceMethod("CorrectKey", settings, New String() {key}), String)
        value = "fjafsdjf93474/(/)$§sad83fjds229/$)§"
        settings.PutString(key, value)
        Assert.AreEqual(2, settings.GetAll.Count)
        Assert.AreEqual(value, settings.GetAll.Item(correctKey))

        key = "///__""%&/jfds03/43__fjdskj84jl"
        correctKey = DirectCast(UnitTestUtilities.RunInstanceMethod("CorrectKey", settings, New String() {key}), String)
        Dim value2 As Integer = 489343843
        settings.PutInteger(key, value2)
        Assert.AreEqual(3, settings.GetAll.Count)
        Assert.AreEqual(value2, settings.GetAll.Item(correctKey))
    End Sub

    <Test()> _
    Public Sub Test_Get_All_Key_Add_Four()
        Dim settings As New SettingsFile()
        Dim key As String = "jfds03"
        Dim value As String = "fjsad83fjds229/$)§"
        settings.PutString(key, value)

        Dim key2 As String = "jfds03/43__fjds456kj84jl"
        value = "fjafsdjf93474/(/)$§sad83fjds229/$)§"
        settings.PutString(key2, value)

        Dim key2b As String = "jfds03/43__fjdskj84j"
        settings.PutInteger(key2b, 1)

        Dim key3 As String = "///__""%&/jfds03/43__fjdskj84jl"
        Dim value2 As Integer = 489343843
        settings.PutInteger(key3, value2)


        Assert.AreEqual(2, settings.GetAll(Of Object)("jfds03").Count)
        Assert.AreEqual(1, settings.GetAll(Of String)("jfds03").Count)
        Assert.AreEqual(1, settings.GetAll(Of Integer)("jfds03").Count)
        Assert.AreEqual(2, settings.GetAll(Of Integer)().Count)
        Assert.AreEqual(2, settings.GetAll(Of String)().Count)
    End Sub

    <Test()> _
    Public Sub Test_Remove_Key()
        Dim settings As New SettingsFile()
        Dim key As String = "jfds03"
        Dim value As String = "fjsad83fjds229/$)§"
        settings.PutString(key, value)

        Dim key2 As String = "jfds03/43__fjds456kj84jl"
        value = "fjafsdjf93474/(/)$§sad83fjds229/$)§"
        settings.PutString(key2, value)

        Dim key2b As String = "jfds03/43__fjdskj84j"
        settings.PutInteger(key2b, 1)

        Dim key3 As String = "///__""%&/jfds03/43__fjdskj84jl"
        Dim value2 As Integer = 489343843
        settings.PutInteger(key3, value2)

        Assert.AreEqual(2, settings.GetAll(Of Object)(key).Count)
        Assert.AreEqual(1, settings.GetAll(Of String)(key).Count)
        Assert.AreEqual(1, settings.GetAll(Of Integer)(key).Count)

        settings.RemoveKey(key, False)
        Assert.AreEqual(2, settings.GetAll(Of Object)(key).Count)
        Assert.AreEqual("", settings.GetString(key))
        Assert.AreEqual(5, settings.GetInteger(key, 5))
        Assert.AreEqual(value2, settings.GetInteger(key3))
        Assert.AreEqual(value, settings.GetString(key2))

        Assert.AreEqual(3, settings.GetAll.Count)
    End Sub

    <Test()> _
    Public Sub Test_Remove_Key_And_Sub_Keys()
        Dim settings As New SettingsFile()
        Dim key As String = "jfds03"
        Dim value As String = "fjsad83fjds229/$)§"
        settings.PutString(key, value)

        Dim key2 As String = "jfds03/43__fjds456kj84jl"
        value = "fjafsdjf93474/(/)$§sad83fjds229/$)§"
        settings.PutString(key2, value)

        Dim key2b As String = "jfds03/43__fjdskj84j"
        settings.PutInteger(key2b, 1)

        Dim key3 As String = "///__""%&/jfds03/43__fjdskj84jl"
        Dim value2 As Integer = 489343843
        settings.PutInteger(key3, value2)

        Assert.AreEqual(2, settings.GetAll(Of Object)(key).Count)
        Assert.AreEqual(1, settings.GetAll(Of String)(key).Count)
        Assert.AreEqual(1, settings.GetAll(Of Integer)(key).Count)

        settings.RemoveKey(key)
        Assert.AreEqual(0, settings.GetAll(Of Object)(key).Count)
        Assert.AreEqual("", settings.GetString(key))
        Assert.AreEqual(5, settings.GetInteger(key, 5))
        Assert.AreEqual(value2, settings.GetInteger(key3))

        Assert.AreEqual(1, settings.GetAll.Count)
    End Sub

    <Test()> _
    Public Sub Test_Save_and_Open_File_Change_Line_delimiter()
        'Save File
        Dim settings As New SettingsFile()
        Dim temp As String = IO.Path.GetTempFileName
        Dim key1, key2, key3 As String
        key1 = "fjsdk"
        key2 = "fjdslkfjdslk"
        key3 = "nwiou"
        Dim val1 As Integer = 434
        Dim val2 As String = "fjdioe830fjsdak$%§)="
        Dim val3 As DateTime = Now.ToUniversalTime
        settings.PutInteger(key1, val1)
        settings.PutString(key2, val2)
        settings.PutDateTime(key3, val3)
        settings.Save(temp)

        'Open file
        settings = New SettingsFile(temp)
        Assert.AreEqual(settings.GetInteger(key1), val1)
        Assert.AreEqual(settings.GetString(key2), val2)
        Assert.AreEqual(settings.GetDateTime(key3), val3)

        'Replace line delimiters
        Dim text As String = IO.File.ReadAllText(temp)

        'with vbLf
        Dim replacedText As String = text.Replace(vbCrLf, vbLf)
        IO.File.WriteAllText(temp, replacedText)
        Assert.IsFalse(IO.File.ReadAllText(temp).Contains(vbCrLf))
        Assert.IsFalse(IO.File.ReadAllText(temp).Contains(vbCr))
        Assert.IsTrue(IO.File.ReadAllText(temp).Contains(vbLf))

        settings = New SettingsFile(temp)
        Assert.AreEqual(settings.GetInteger(key1), val1)
        Assert.AreEqual(settings.GetString(key2), val2)
        Assert.AreEqual(settings.GetDateTime(key3), val3)

        'with vbCr
        replacedText = text.Replace(vbCrLf, vbCr)
        IO.File.WriteAllText(temp, replacedText)
        Assert.IsFalse(IO.File.ReadAllText(temp).Contains(vbCrLf))
        Assert.IsFalse(IO.File.ReadAllText(temp).Contains(vbLf))
        Assert.IsTrue(IO.File.ReadAllText(temp).Contains(vbCr))

        settings = New SettingsFile(temp)
        Assert.AreEqual(settings.GetInteger(key1), val1)
        Assert.AreEqual(settings.GetString(key2), val2)
        Assert.AreEqual(settings.GetDateTime(key3), val3)

        'Clean up
        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_Get_Default_Value()
        Dim settings As New SettingsFile()
        Assert.AreEqual(settings.GetString("aber"), "")
        Assert.AreEqual(settings.GetInteger("aber"), 0)
        Assert.AreEqual(settings.GetDateTime("aber"), New DateTime(0))
        Assert.AreEqual(settings.GetDouble("aber"), 0)
        Assert.AreEqual(settings.GetBoolean("aber"), False)

        Assert.AreEqual(settings.GetString("aber", "321ge"), "321ge")
        Assert.AreEqual(settings.GetString("aber", "54554"), "54554")
        Assert.AreEqual(settings.GetInteger("aber", 4434342), 4434342)
        Assert.AreEqual(settings.GetInteger("aber", 1234), 1234)
        Assert.AreEqual(settings.GetDouble("aber", 73473874.5454), 73473874.5454)
        Assert.AreEqual(settings.GetDouble("aber", Double.MaxValue), Double.MaxValue)
        Assert.AreEqual(settings.GetDouble("aber", Double.MinValue), Double.MinValue)
        Dim d As New DateTime(43477238473)
        Assert.AreEqual(settings.GetDateTime("aber", d), d)
        d = New DateTime(434772434338473)
        Assert.AreEqual(settings.GetDateTime("aber", d), d)
        Assert.AreEqual(settings.GetBoolean("aber", True), True)
        Assert.AreEqual(settings.GetBoolean("aber", False), False)
    End Sub

    <Test()> _
    Public Sub Test_Get_Non_Existing_Category()
        Dim settings As New SettingsFile()
        Dim keyCategory As String = "/fjda/jklfdj92432/&$§)jkfd/"
        Dim keyName As String = "fdjk"

        Assert.AreEqual(1234, settings.GetInteger(keyCategory & keyName, 1234))
        settings.PutBoolean(keyCategory & "testE", True)
        Assert.AreEqual(1234, settings.GetInteger(keyCategory & keyName, 1234))
        settings.PutInteger(keyCategory & keyName, 4321)
        Assert.AreNotEqual(1234, settings.GetInteger(keyCategory & keyName, 1234))
    End Sub
End Class
