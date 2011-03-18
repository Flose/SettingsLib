Imports NUnit.Framework
Imports FloseCode.Settings

<TestFixture()> _
Public Class Test_File
    Dim settings As SettingsFile

    <TestFixtureSetUp()> _
    Public Sub CreateClass()
        settings = New SettingsFile()
    End Sub

    <Test()> _
    Public Sub Test_put_Empty_Key()
        Dim key As String = ""
        Dim correctKey As String = "/"
        Dim value As Integer = 5345
        Dim errorMsg As String = "Key must not be empty"

        Assert.Throws(Of SettingsFileException)(Sub() settings.putInteger(key, value), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() settings.getInteger(key), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() settings.getInteger(correctKey, value), errorMsg)

        value = 43234
        Assert.Throws(Of SettingsFileException)(Sub() settings.getInteger(key), errorMsg)

        value = 4343333
        Assert.Throws(Of SettingsFileException)(Sub() settings.putInteger(correctKey, value), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() settings.getInteger(correctKey), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() settings.getInteger(key), errorMsg)
    End Sub

    <Test()> _
    Public Sub Test_put_Integer()
        Dim key As String = "Test"
        Dim value As Integer = 5345
        settings.putInteger(key, value)
        Assert.AreEqual(value, settings.getInteger(key))

        value = 43234
        Assert.AreNotEqual(value, settings.getInteger(key))

        settings.putInteger(key, value)
        Assert.AreEqual(value, settings.getInteger(key))
    End Sub

    <Test()> _
    Public Sub Test_put_Double()
        Dim key As String = "Tesfda1t"
        Dim value As Double = 54.1
        settings.putDouble(key, value)
        Assert.AreEqual(value, settings.getDouble(key))

        value = 43234
        Assert.AreNotEqual(value, settings.getDouble(key))

        settings.putDouble(key, value)
        Assert.AreEqual(value, settings.getDouble(key))

        value = 43234.4738248
        Assert.AreNotEqual(value, settings.getDouble(key))

        settings.putDouble(key, value)
        Assert.AreEqual(value, settings.getDouble(key))
    End Sub

    <Test()> _
    Public Sub Test_put_Boolean()
        Dim key As String = "Test"
        Dim value As Boolean = True
        settings.putBoolean(key, value)
        Assert.AreEqual(value, settings.getBoolean(key))

        value = False
        Assert.AreNotEqual(value, settings.getBoolean(key))

        settings.putBoolean(key, value)
        Assert.AreEqual(value, settings.getBoolean(key))
    End Sub

    <Test()> _
    Public Sub Test_put_String()
        Test_put_String("fdfads534f      5" & vbTab & vbCr & "fjdsk" & vbCrLf & "fjdak" & vbLf)
        Test_put_String("jfdsalf839fjsdklafj jfaefdjsia\n\\n" & vbCr & "fjdk" & vbCrLf & "fjdk?)&" & ChrW(43438) & vbLf)
        Test_put_String("jfdsalf839fjsdklafj jfaefdjsia\n\\n" & vbCr & "fjdk" & vbCrLf & "fjdk?)&" & ChrW(43438) & vbLf & """")
        Test_put_String("jfdsalf839fjsd\\\\\\\\klafj jfae         fdjsia\n\\n" & vbCr & "fj""""dk" & vbCrLf & "fjfdssfdk?)&" & vbTab & ChrW(43438) & vbLf & vbCr)
    End Sub

    Private Sub test_put_string(ByVal text As String)
        Dim key As String = "Test"

        settings.putString(key, text)
        Assert.AreEqual(text, settings.getString(key))

        text = "fjsdkajflsjkfiek"
        Assert.AreNotEqual(text, settings.getString(key))

        settings.putString(key, text)
        Assert.AreEqual(text, settings.getString(key))
    End Sub

    <Test()> _
    Public Sub Test_put_DateTime()
        Dim key As String = "Test"
        Dim value As Date = New Date(477843578992835735)
        settings.putDateTime(key, value)
        Assert.AreEqual(value, settings.getDateTime(key))

        value = value.AddDays(43)
        Assert.AreNotEqual(value, settings.getDateTime(key))

        settings.putDateTime(key, value)
        Assert.AreEqual(value, settings.getDateTime(key))
    End Sub

    <Test()> _
    Public Sub Test_put_String_Save_and_Open()
        Test_put_String_Save_and_Open("jfdsalf839fjsdklafj jfaefdjsia\n\\n" & vbCr & "fjdk" & vbCrLf & "fjdk?)&" & ChrW(43438) & vbLf)
        Test_put_String_Save_and_Open("jfdsalf839fjsdklafj jfaefdjsia\n\\n" & vbCr & "fjdk" & vbCrLf & "fjdk?)&" & ChrW(43438) & vbLf & """")
        Test_put_String_Save_and_Open("jfdsalf839fjsd\\\\\\\\klafj jfae         fdjsia\n\\n" & vbCr & "fj""""dk" & vbCrLf & "fjfdssfdk?)&" & vbTab & ChrW(43438) & vbLf & vbCr)
    End Sub

    Private Sub test_put_string_save_and_open(ByVal text As String)
        Dim key As String = "TestSt"

        settings.putString(key, text)
        Dim temp As String = IO.Path.GetTempFileName
        settings.save(temp)

        Dim tmpSettings As New SettingsFile(temp)

        Assert.AreEqual(text, tmpSettings.getString(key))
        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_put_Boolean_Save_and_Open()
        Test_put_Boolean_Save_and_Open(True)
        Test_put_Boolean_Save_and_Open(False)
    End Sub

    Private Sub test_put_Boolean_save_and_open(ByVal val As Boolean)
        Dim key As String = "TestSt"

        settings.putBoolean(key, val)
        Dim temp As String = IO.Path.GetTempFileName
        settings.save(temp)

        Dim tmpSettings As New SettingsFile(temp)

        Assert.AreEqual(val, tmpSettings.getBoolean(key))
        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_put_Integer_Save_and_Open()
        Dim key As String = "TestInt"
        Dim value As Integer = 5345

        settings.putInteger(key, value)
        Dim temp As String = IO.Path.GetTempFileName
        settings.save(temp)

        Dim tmpSettings As New SettingsFile(temp)

        Assert.AreEqual(value, tmpSettings.getInteger(key))
        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_put_double_Save_and_Open()
        Test_put_double_Save_and_Open(5542)
        Test_put_double_Save_and_Open(5542.4329)
        Test_put_double_Save_and_Open(0.4732874)
        Test_put_double_Save_and_Open(Double.MaxValue)
        Test_put_double_Save_and_Open(Double.MinValue)
        Test_put_double_Save_and_Open(Double.MaxValue / 2)
    End Sub

    Private Sub Test_put_double_Save_and_Open(ByVal Value As Double)
        Dim key As String = "TestDoub5"

        settings.putDouble(key, Value)
        Dim temp As String = IO.Path.GetTempFileName
        settings.save(temp)

        Dim tmpSettings As New SettingsFile(temp)

        Assert.AreEqual(Value, tmpSettings.getDouble(key))
        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_put_DateTime_Save_and_Open()
        Test_put_DateTime_Save_and_Open(New DateTime(477843578992835735))
        Test_put_DateTime_Save_and_Open(New DateTime(477843578992835735, DateTimeKind.Utc))
        Test_put_DateTime_Save_and_Open(New DateTime(477843578992835735, DateTimeKind.Local))
        Test_put_DateTime_Save_and_Open(New DateTime(477843578992835735, DateTimeKind.Unspecified))
        Test_put_DateTime_Save_and_Open(New DateTime(0))
        Test_put_DateTime_Save_and_Open(New DateTime(DateTime.MinValue.Ticks))
        Test_put_DateTime_Save_and_Open(New DateTime(DateTime.MaxValue.Ticks))
    End Sub

    Public Sub Test_put_DateTime_Save_and_Open(ByVal datum As DateTime)
        Dim key As String = "TestDat"

        settings.putDateTime(key, datum)
        Dim temp As String = IO.Path.GetTempFileName
        settings.save(temp)

        Dim tmpSettings As New SettingsFile(temp)

        Assert.AreEqual(datum, tmpSettings.getDateTime(key))
        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_correct_Key()
        Test_correct_Key("/ ", "/_")
        Test_correct_Key(" / ", "/_/_")
        Test_correct_Key("//// ", "/_")
        Test_correct_Key("abc", "/abc")
        Test_correct_Key("//abc", "/abc")
        Test_correct_Key("//a///bc", "/a/bc")
        Test_correct_Key("=abc", "/_abc")
        Test_correct_Key("/m  ann/t==e/fjdk""""sfjdkTe///s" & vbTab & "t=D""""a  t//", "/m__ann/t__e/fjdk""""sfjdkTe/s" & vbTab & "t_D""""a__t")
        Dim errorMsg As String = "Key must not be empty"
        Assert.Throws(Of SettingsFileException)(Sub() Test_correct_Key("", "/"), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() Test_correct_Key("/", "/"), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() Test_correct_Key("//////", "/"), errorMsg)
    End Sub

    Private Sub Test_correct_Key(ByVal key As String, ByVal correctKey As String)
        Try
            Dim actual As String = DirectCast(UnitTestUtilities.RunInstanceMethod("correctKey", settings, New String() {key}), String)
            Assert.AreEqual(correctKey, actual)

            actual = DirectCast(UnitTestUtilities.RunInstanceMethod("correctKey", settings, New String() {correctKey}), String)
            Assert.AreEqual(correctKey, actual)
        Catch ex As System.Reflection.TargetInvocationException
            Throw ex.InnerException
        End Try
    End Sub

    <Test()> _
    Public Sub Test_put_String_correct_Key()
        Dim key As String = "/m  ann/t==e/fjdk""""sfjdkTe///s" & vbTab & "t=D""""a  t//"
        Dim correctKey As String = "/m__ann/t__e/fjdk""""sfjdkTe/s" & vbTab & "t_D""""a__t"

        Dim value As String = "fds  fs()=="""""

        settings.putString(key, value)
        Dim temp As String = IO.Path.GetTempFileName
        settings.save(temp)

        Dim tmpSettings As New SettingsFile(temp)
        Assert.AreEqual(value, tmpSettings.getString(key))
        Assert.AreEqual(value, tmpSettings.getString(correctKey))

        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_put_String_correct_Key_Open_and_Save()
        Dim key As String = "/m  ann/t==e/fjdk""""sfjdkTe///s" & vbTab & "t=D""""a  t//"
        Dim correctKey As String = "/m__ann/t__e/fjdk""""sfjdkTe/s" & vbTab & "t_D""""a__t"

        Dim value As String = "fds  fs()=="""""

        settings.putString(key, value)
        Assert.AreEqual(value, settings.getString(key))

        Assert.AreEqual(value, settings.getString(correctKey))


        value = "fdsa><dfds  fs()=fdsfasd="""""

        settings.putString(correctKey, value)
        Assert.AreEqual(value, settings.getString(correctKey))

        Assert.AreEqual(value, settings.getString(key))
    End Sub
End Class

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
End Class