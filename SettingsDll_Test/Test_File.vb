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
    Public Sub Test_Get_Wrong_Type()
        Dim key As String = "fjda/jklfdj92432/&$§)jkfd/fdjk"
        Dim value As Integer = 5345

        Assert.DoesNotThrow(Sub() settings.getInteger(key))
        Assert.DoesNotThrow(Sub() settings.getString(key))
        Assert.DoesNotThrow(Sub() settings.getBoolean(key))

        settings.putInteger(key, value)

        Dim errorMsg As String
        Assert.DoesNotThrow(Sub() settings.getInteger(key))
        errorMsg = "Tried to read """ & key & """ as ""String"", but it's a """ & value.GetType().ToString() + """"
        Assert.Throws(Of SettingsFileException)(Sub() settings.getString(key), errorMsg)
        errorMsg = "Tried to read """ & key & """ as ""Boolean"", but it's a """ & value.GetType().ToString() + """"
        Assert.Throws(Of SettingsFileException)(Sub() settings.getBoolean(key), errorMsg)
        errorMsg = "Tried to read """ & key & """ as ""Double"", but it's a """ & value.GetType().ToString() + """"
        Assert.Throws(Of SettingsFileException)(Sub() settings.getDouble(key), errorMsg)
        errorMsg = "Tried to read """ & key & """ as ""DateTime"", but it's a """ & value.GetType().ToString() + """"
        Assert.Throws(Of SettingsFileException)(Sub() settings.getDateTime(key), errorMsg)
    End Sub

    <Test()> _
    Public Sub Test_put_Integer()
        Dim key As String = "ta/te/Test"
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
        Dim key As String = "//fd/fd/Tesfda1t"
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
        Dim key As String = "/43/Test"
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
        Dim key As String = "/Test"

        settings.putString(key, text)
        Assert.AreEqual(text, settings.getString(key))

        text = "fjsdkajflsjkfiek"
        Assert.AreNotEqual(text, settings.getString(key))

        settings.putString(key, text)
        Assert.AreEqual(text, settings.getString(key))
    End Sub

    <Test()> _
    Public Sub Test_put_DateTime()
        Dim key As String = "/ta/Test"
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
        Dim key As String = "/!""§$%&/()=?`/TestSt"

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
        Dim key As String = "/f/TestSt"

        settings.putBoolean(key, val)
        Dim temp As String = IO.Path.GetTempFileName
        settings.save(temp)

        Dim tmpSettings As New SettingsFile(temp)

        Assert.AreEqual(val, tmpSettings.getBoolean(key))
        IO.File.Delete(temp)
    End Sub

    <Test()> _
    Public Sub Test_put_Integer_Save_and_Open()
        Dim key As String = "//TestInt"
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
        Dim key As String = "////TestDoub5"

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
        Dim key As String = "2/TestDat"

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
        Test_correct_Key("/m__ann/t__e/fjdk""""sfjdkTe/s" & vbTab & "t_D""""a__t", "/m__ann/t__e/fjdk""""sfjdkTe/s" & vbTab & "t_D""""a__t")
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
    Public Sub Test_put_String_correct_Key_Open_and_Save()
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
    Public Sub Test_put_String_correct_Key()
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
