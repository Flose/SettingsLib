﻿Imports NUnit.Framework
Imports FloseCode.Settings

<TestFixture()>
Public Class Test_File
    Dim settings As SettingsFile

    <TestFixtureSetUp()>
    Public Sub CreateClass()
        settings = New SettingsFile()
    End Sub

    <Test()>
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

    <Test()>
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

    <Test()>
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

    <Test()>
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

    <Test()>
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

    <Test()>
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

    <Test()>
    Public Sub Test_correct_Key()
        Test_correct_Key("abc", "/abc")
        Test_correct_Key("//abc", "/abc")
        Test_correct_Key("//a///bc", "/a/bc")
        Test_correct_Key("=abc", "/_abc")
        Test_correct_Key("/m  ann/t==e/fjdk""""sfjdkTe///s" & vbTab & "t=D""""a  t//", "/m__ann/t__e/fjdk""""sfjdkTe/s" & vbTab & "t_D""""a__t")
    End Sub

    Private Sub Test_correct_Key(ByVal key As String, ByVal correctKey As String)
        Dim actual As String = DirectCast(UnitTestUtilities.RunInstanceMethod("correctKey", settings, New String() {key}), String)
        Assert.AreEqual(correctKey, actual)

        actual = DirectCast(UnitTestUtilities.RunInstanceMethod("correctKey", settings, New String() {correctKey}), String)
        Assert.AreEqual(correctKey, actual)
    End Sub

    <Test()>
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

    <Test()>
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

Public Class Test_Empty_File

    <Test()>
    Public Sub Test_Open_Not_existing_File()
        Dim temp As String = "fjdfjksdfijeavösjarskdlffjaklsdfjkljsdfasdlk"
        Dim settings As New SettingsFile(temp)
    End Sub

    <Test()>
    Public Sub Test_Open_Empty_File()
        Dim temp As String = IO.Path.GetTempFileName
        Dim settings As New SettingsFile(temp)
    End Sub

    <Test()>
    Public Sub Test_Save_empty_File()
        Dim settings As New SettingsFile()
        Dim temp As String = IO.Path.GetTempFileName
        settings.save(temp)
    End Sub

    <Test()>
    Public Sub Test_Save_and_Open_empty_File()
        Dim settings As New SettingsFile()
        Dim temp As String = IO.Path.GetTempFileName
        settings.save(temp)
        settings = New SettingsFile(temp)
    End Sub
End Class