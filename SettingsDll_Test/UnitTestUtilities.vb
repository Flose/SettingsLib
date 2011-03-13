Imports System.Reflection

Public Class UnitTestUtilities

    Public Shared Function RunInstanceMethod(ByVal Method As String, ByVal Instance As Object, ByVal ParamArray params() As Object) As Object
        Dim eFlags As BindingFlags = BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic
        Return RunMethod(Method, Instance, eFlags, params)
    End Function

    Private Shared Function RunMethod(ByVal Method As String, ByVal Instance As Object, ByVal eFlags As BindingFlags, ByVal ParamArray params() As Object) As Object
        Dim m As MethodInfo
        m = Instance.GetType.GetMethod(Method, eFlags)
        If (m Is Nothing) Then
            Throw New ArgumentException("There is no method '" & Method & "' for type '" & Instance.GetType.ToString() & "'.")
        End If

        Return m.Invoke(Instance, params)
    End Function
End Class
