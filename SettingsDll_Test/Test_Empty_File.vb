Imports FloseCode.Settings
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
        settings.save(temp)
    End Sub

    <Test()> _
    Public Sub Test_Save_and_Open_empty_File()
        Dim settings As New SettingsFile()
        Dim temp As String = IO.Path.GetTempFileName
        settings.save(temp)
        settings = New SettingsFile(temp)
    End Sub

    <Test()> _
    Public Sub Test_Get_All_Empty()
        Dim settings As New SettingsFile()
        Assert.AreEqual(0, settings.getAll.Count)
    End Sub

    <Test()> _
    Public Sub Test_Get_All_Add_One()
        Dim settings As New SettingsFile()
        Dim key As String = "jfds03/fjdskj84jl"
        Dim correctKey As String = DirectCast(UnitTestUtilities.RunInstanceMethod("correctKey", settings, New String() {key}), String)
        Dim value As String = "fjsad83fjds229/$)§"
        settings.putString(key, value)
        Assert.AreEqual(1, settings.getAll.Count)
        Assert.AreEqual(value, settings.getAll.Item(correctKey))
    End Sub

    <Test()> _
    Public Sub Test_Get_All_Add_One_Thrice()
        Dim settings As New SettingsFile()
        Dim key As String = "jfds03/fjdskj84jl"
        Dim correctKey As String = DirectCast(UnitTestUtilities.RunInstanceMethod("correctKey", settings, New String() {key}), String)
        Dim value As String = "fjsad83fjds229/$)§"
        settings.putString(key, value)
        Assert.AreEqual(1, settings.getAll.Count)
        Assert.AreEqual(value, settings.getAll.Item(correctKey))

        value = "fjafsdjf93474/(/)$§sad83fjds229/$)§"
        settings.putString(key, value)

        Assert.AreEqual(1, settings.getAll.Count)
        Assert.AreEqual(value, settings.getAll.Item(correctKey))

        Dim value2 As DateTime = New DateTime(48934384339)
        settings.putDateTime(key, value2)

        Assert.AreEqual(1, settings.getAll.Count)
        Assert.AreEqual(value2, settings.getAll.Item(correctKey))
    End Sub

    <Test()> _
    Public Sub Test_Get_All_Add_Three()
        Dim settings As New SettingsFile()
        Dim key As String = "jfds03/fjdskj84jl"
        Dim correctKey As String = DirectCast(UnitTestUtilities.RunInstanceMethod("correctKey", settings, New String() {key}), String)
        Dim value As String = "fjsad83fjds229/$)§"
        settings.putString(key, value)
        Assert.AreEqual(1, settings.getAll.Count)
        Assert.AreEqual(value, settings.getAll.Item(correctKey))

        key = "jfds03/43==fjdskj84jl"
        correctKey = DirectCast(UnitTestUtilities.RunInstanceMethod("correctKey", settings, New String() {key}), String)
        value = "fjafsdjf93474/(/)$§sad83fjds229/$)§"
        settings.putString(key, value)
        Assert.AreEqual(2, settings.getAll.Count)
        Assert.AreEqual(value, settings.getAll.Item(correctKey))

        key = "///==""%&/jfds03/43==fjdskj84jl"
        correctKey = DirectCast(UnitTestUtilities.RunInstanceMethod("correctKey", settings, New String() {key}), String)
        Dim value2 As Integer = 489343843
        settings.putInteger(key, value2)
        Assert.AreEqual(3, settings.getAll.Count)
        Assert.AreEqual(value2, settings.getAll.Item(correctKey))
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
        settings.putInteger(key1, val1)
        settings.putString(key2, val2)
        settings.putDateTime(key3, val3)
        settings.save(temp)

        'Open file
        settings = New SettingsFile(temp)
        Assert.AreEqual(settings.getInteger(key1), val1)
        Assert.AreEqual(settings.getString(key2), val2)
        Assert.AreEqual(settings.getDateTime(key3), val3)

        'Replace line delimiters
        Dim text As String = IO.File.ReadAllText(temp)

        'with vbLf
        Dim replacedText As String = text.Replace(vbCrLf, vbLf)
        IO.File.WriteAllText(temp, replacedText)
        Assert.IsFalse(IO.File.ReadAllText(temp).Contains(vbCrLf))
        Assert.IsFalse(IO.File.ReadAllText(temp).Contains(vbCr))
        Assert.IsTrue(IO.File.ReadAllText(temp).Contains(vbLf))

        settings = New SettingsFile(temp)
        Assert.AreEqual(settings.getInteger(key1), val1)
        Assert.AreEqual(settings.getString(key2), val2)
        Assert.AreEqual(settings.getDateTime(key3), val3)

        'with vbCr
        replacedText = text.Replace(vbCrLf, vbCr)
        IO.File.WriteAllText(temp, replacedText)
        Assert.IsFalse(IO.File.ReadAllText(temp).Contains(vbCrLf))
        Assert.IsFalse(IO.File.ReadAllText(temp).Contains(vbLf))
        Assert.IsTrue(IO.File.ReadAllText(temp).Contains(vbCr))

        settings = New SettingsFile(temp)
        Assert.AreEqual(settings.getInteger(key1), val1)
        Assert.AreEqual(settings.getString(key2), val2)
        Assert.AreEqual(settings.getDateTime(key3), val3)

        'Clean up
        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_Get_Default_Value()
        Dim settings As New SettingsFile()
        Assert.AreEqual(settings.getString("aber"), "")
        Assert.AreEqual(settings.getInteger("aber"), 0)
        Assert.AreEqual(settings.getDateTime("aber"), New DateTime(0))
        Assert.AreEqual(settings.getDouble("aber"), 0)
        Assert.AreEqual(settings.getBoolean("aber"), False)

        Assert.AreEqual(settings.getString("aber", "321ge"), "321ge")
        Assert.AreEqual(settings.getString("aber", "54554"), "54554")
        Assert.AreEqual(settings.getInteger("aber", 4434342), 4434342)
        Assert.AreEqual(settings.getInteger("aber", 1234), 1234)
        Assert.AreEqual(settings.getDouble("aber", 73473874.5454), 73473874.5454)
        Assert.AreEqual(settings.getDouble("aber", Double.MaxValue), Double.MaxValue)
        Assert.AreEqual(settings.getDouble("aber", Double.MinValue), Double.MinValue)
        Dim d As New DateTime(43477238473)
        Assert.AreEqual(settings.getDateTime("aber", d), d)
        d = New DateTime(434772434338473)
        Assert.AreEqual(settings.getDateTime("aber", d), d)
        Assert.AreEqual(settings.getBoolean("aber", True), True)
        Assert.AreEqual(settings.getBoolean("aber", False), False)
    End Sub

    <Test()> _
    Public Sub Test_Get_Non_Existing_Category()
        Dim settings As New SettingsFile()
        Dim keyCategory As String = "/fjda/jklfdj92432/&$§)jkfd/"
        Dim keyName As String = "fdjk"

        Assert.AreEqual(1234, settings.getInteger(keyCategory & keyName, 1234))
        settings.putBoolean(keyCategory & "testE", True)
        Assert.AreEqual(1234, settings.getInteger(keyCategory & keyName, 1234))
        settings.putInteger(keyCategory & keyName, 4321)
        Assert.AreNotEqual(1234, settings.getInteger(keyCategory & keyName, 1234))
    End Sub
End Class
