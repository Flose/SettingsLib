Imports NUnit.Framework
Imports FloseCode.SettingsLib

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

        Assert.Throws(Of SettingsFileException)(Sub() settings.PutInteger(key, value), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() settings.GetInteger(key), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() settings.GetInteger(correctKey, value), errorMsg)

        value = 43234
        Assert.Throws(Of SettingsFileException)(Sub() settings.GetInteger(key), errorMsg)

        value = 4343333
        Assert.Throws(Of SettingsFileException)(Sub() settings.PutInteger(correctKey, value), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() settings.GetInteger(correctKey), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() settings.GetInteger(key), errorMsg)
    End Sub

    <Test()> _
    Public Sub Test_Get_Wrong_Type()
        Dim key As String = "fjda/jklfdj92432/&$§)jkfd/fdjk"
        Dim value As Integer = 5345

        Assert.DoesNotThrow(Sub() settings.GetInteger(key))
        Assert.DoesNotThrow(Sub() settings.GetString(key))
        Assert.DoesNotThrow(Sub() settings.GetBoolean(key))

        settings.PutInteger(key, value)

        Dim errorMsg As String
        Assert.DoesNotThrow(Sub() settings.GetInteger(key))
        errorMsg = "Tried to read """ & key & """ as ""String"", but it's a """ & value.GetType().ToString() + """"
        Assert.Throws(Of SettingsFileException)(Sub() settings.GetString(key), errorMsg)
        errorMsg = "Tried to read """ & key & """ as ""Boolean"", but it's a """ & value.GetType().ToString() + """"
        Assert.Throws(Of SettingsFileException)(Sub() settings.GetBoolean(key), errorMsg)
        errorMsg = "Tried to read """ & key & """ as ""Double"", but it's a """ & value.GetType().ToString() + """"
        Assert.Throws(Of SettingsFileException)(Sub() settings.GetDouble(key), errorMsg)
        errorMsg = "Tried to read """ & key & """ as ""DateTime"", but it's a """ & value.GetType().ToString() + """"
        Assert.Throws(Of SettingsFileException)(Sub() settings.GetDateTime(key), errorMsg)
    End Sub

    <Test()> _
    Public Sub Test_put_Integer()
        Dim key As String = "ta/te/Test"
        Dim value As Integer = 5345
        settings.PutInteger(key, value)
        Assert.AreEqual(value, settings.GetInteger(key))

        value = 43234
        Assert.AreNotEqual(value, settings.GetInteger(key))

        settings.PutInteger(key, value)
        Assert.AreEqual(value, settings.GetInteger(key))
    End Sub

    <Test()> _
    Public Sub Test_put_Double()
        Dim key As String = "//fd/fd/Tesfda1t"
        Dim value As Double = 54.1
        settings.PutDouble(key, value)
        Assert.AreEqual(value, settings.GetDouble(key))

        value = 43234
        Assert.AreNotEqual(value, settings.GetDouble(key))

        settings.PutDouble(key, value)
        Assert.AreEqual(value, settings.GetDouble(key))

        value = 43234.4738248
        Assert.AreNotEqual(value, settings.GetDouble(key))

        settings.PutDouble(key, value)
        Assert.AreEqual(value, settings.GetDouble(key))
    End Sub

    <Test()> _
    Public Sub Test_put_Boolean()
        Dim key As String = "/43/Test"
        Dim value As Boolean = True
        settings.PutBoolean(key, value)
        Assert.AreEqual(value, settings.GetBoolean(key))

        value = False
        Assert.AreNotEqual(value, settings.GetBoolean(key))

        settings.PutBoolean(key, value)
        Assert.AreEqual(value, settings.GetBoolean(key))
    End Sub

    <Test()> _
    Public Sub Test_put_String()
        Test_put_String("fdfads534f      5" & vbTab & vbCr & "fjdsk" & vbCrLf & "fjdak" & vbLf)
        Test_put_String("jfdsalf839fjsdklafj jfaefdjsia\n\\n" & vbCr & "fjdk" & vbCrLf & "fjdk?)&" & ChrW(43438) & vbLf)
        Test_put_String("jfdsalf839fjsdklafj jfaefdjsia\n\\n" & vbCr & "fjdk" & vbCrLf & "fjdk?)&" & ChrW(43438) & vbLf & """")
        Test_put_String("jfdsalf839fjsd\\\\\\\\klafj jfae         fdjsia\n\\n" & vbCr & "fj""""dk" & vbCrLf & "fjfdssfdk?)&" & vbTab & ChrW(43438) & vbLf & vbCr)
    End Sub

    Private Sub test_put_string(ByVal text As String)
        Dim key As String = "/Test"

        settings.PutString(key, text)
        Assert.AreEqual(text, settings.GetString(key))

        text = "fjsdkajflsjkfiek"
        Assert.AreNotEqual(text, settings.GetString(key))

        settings.PutString(key, text)
        Assert.AreEqual(text, settings.GetString(key))
    End Sub

    <Test()> _
    Public Sub Test_put_DateTime()
        Dim key As String = "/ta/Test"
        Dim value As Date = New Date(477843578992835735)
        settings.PutDateTime(key, value)
        Assert.AreEqual(value, settings.GetDateTime(key))

        value = value.AddDays(43)
        Assert.AreNotEqual(value, settings.GetDateTime(key))

        settings.PutDateTime(key, value)
        Assert.AreEqual(value, settings.GetDateTime(key))
    End Sub

    <Test()> _
    Public Sub Test_put_String_Save_and_Open()
        Test_put_String_Save_and_Open("jfdsalf839fjsdklafj jfaefdjsia\n\\n" & vbCr & "fjdk" & vbCrLf & "fjdk?)&" & ChrW(43438) & vbLf)
        Test_put_String_Save_and_Open("jfdsalf839fjsdklafj jfaefdjsia\n\\n" & vbCr & "fjdk" & vbCrLf & "fjdk?)&" & ChrW(43438) & vbLf & """")
        Test_put_String_Save_and_Open("jfdsalf839fjsd\\\\\\\\klafj jfae         fdjsia\n\\n" & vbCr & "fj""""dk" & vbCrLf & "fjfdssfdk?)&" & vbTab & ChrW(43438) & vbLf & vbCr)
    End Sub

    Private Sub test_put_string_save_and_open(ByVal text As String)
        Dim key As String = "/!""§$%&/()_?`/TestSt"

        settings.PutString(key, text)
        Dim temp As String = IO.Path.GetTempFileName
        settings.Save(temp)

        Dim tmpSettings As New SettingsFile(temp)

        Assert.AreEqual(text, tmpSettings.GetString(key))
        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_put_Boolean_Save_and_Open()
        Test_put_Boolean_Save_and_Open(True)
        Test_put_Boolean_Save_and_Open(False)
    End Sub

    Private Sub test_put_Boolean_save_and_open(ByVal val As Boolean)
        Dim key As String = "/f/TestSt"

        settings.PutBoolean(key, val)
        Dim temp As String = IO.Path.GetTempFileName
        settings.Save(temp)

        Dim tmpSettings As New SettingsFile(temp)

        Assert.AreEqual(val, tmpSettings.GetBoolean(key))
        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_put_Integer_Save_and_Open()
        Dim key As String = "//TestInt"
        Dim value As Integer = 5345

        settings.PutInteger(key, value)
        Dim temp As String = IO.Path.GetTempFileName
        settings.Save(temp)

        Dim tmpSettings As New SettingsFile(temp)

        Assert.AreEqual(value, tmpSettings.GetInteger(key))
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
        Dim key As String = "////TestDoub5"

        settings.PutDouble(key, Value)
        Dim temp As String = IO.Path.GetTempFileName
        settings.Save(temp)

        Dim tmpSettings As New SettingsFile(temp)

        Assert.AreEqual(Value, tmpSettings.GetDouble(key))
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
        Dim key As String = "2/TestDat"

        settings.PutDateTime(key, datum)
        Dim temp As String = IO.Path.GetTempFileName
        settings.Save(temp)

        Dim tmpSettings As New SettingsFile(temp)

        Assert.AreEqual(datum, tmpSettings.GetDateTime(key))
        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_correct_Key()
        Test_correct_Key("abc", "/abc")
        Test_correct_Key("//abc", "/abc")
        Test_correct_Key("//a///bc", "/a/bc")
        Test_correct_Key("/m__ann/t__e/fjdk""""sfjdkTe/s" & vbTab & "t_D""""a__t", "/m__ann/t__e/fjdk""""sfjdkTe/s" & vbTab & "t_D""""a__t")
        Dim errorMsg As String = "Key with illegal char accepted"
        Assert.Throws(Of SettingsFileException)(Sub() Test_correct_Key("/ ", "/_"), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() Test_correct_Key(" / ", "/ / "), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() Test_correct_Key("//// ", "/ "), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() Test_correct_Key("=abc", "/=abc"), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() Test_correct_Key("/m  ann/t==e/fjdk""""sfjdkTe///s" & vbTab & "t=D""""a  t//", "/m  ann/t==e/fjdk""""sfjdkTe/s" & vbTab & "t=D""""a  t"), errorMsg)
        errorMsg = "Key must not be empty"
        Assert.Throws(Of SettingsFileException)(Sub() Test_correct_Key("", "/"), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() Test_correct_Key("/", "/"), errorMsg)
        Assert.Throws(Of SettingsFileException)(Sub() Test_correct_Key("//////", "/"), errorMsg)
    End Sub

    Private Sub Test_correct_Key(ByVal key As String, ByVal correctKey As String)
        Try
            Dim actual As String = DirectCast(UnitTestUtilities.RunInstanceMethod("CorrectKey", settings, New String() {key}), String)
            Assert.AreEqual(correctKey, actual)

            actual = DirectCast(UnitTestUtilities.RunInstanceMethod("CorrectKey", settings, New String() {correctKey}), String)
            Assert.AreEqual(correctKey, actual)
        Catch ex As System.Reflection.TargetInvocationException
            Throw ex.InnerException
        End Try
    End Sub

    <Test()> _
    Public Sub Test_put_String_correct_Key_Open_and_Save()
        Dim key As String = "/m__ann/t__e/fjdk""""sfjdkTe///s" & vbTab & "t_D""""a__t//"
        Dim correctKey As String = "/m__ann/t__e/fjdk""""sfjdkTe/s" & vbTab & "t_D""""a__t"

        Dim value As String = "fds  fs()=="""""

        settings.PutString(key, value)
        Dim temp As String = IO.Path.GetTempFileName
        settings.Save(temp)

        Dim tmpSettings As New SettingsFile(temp)
        Assert.AreEqual(value, tmpSettings.GetString(key))
        Assert.AreEqual(value, tmpSettings.GetString(correctKey))

        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_put_String_correct_Key()
        Dim key As String = "/m__ann/t__e/fjdk""""sfjdkTe///s" & vbTab & "t_D""""a__t//"
        Dim correctKey As String = "/m__ann/t__e/fjdk""""sfjdkTe/s" & vbTab & "t_D""""a__t"

        Dim value As String = "fds  fs()=="""""

        settings.PutString(key, value)
        Assert.AreEqual(value, settings.GetString(key))

        Assert.AreEqual(value, settings.GetString(correctKey))


        value = "fdsa><dfds  fs()=fdsfasd="""""

        settings.PutString(correctKey, value)
        Assert.AreEqual(value, settings.GetString(correctKey))

        Assert.AreEqual(value, settings.GetString(key))
    End Sub
End Class
