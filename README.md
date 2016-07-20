# SettingsLib

SettingsLib is a .Net library that provides a simple mechanism to store settings in a strongly-typed hierarchical INI like file format.

The library is available on NuGet: https://www.nuget.org/packages/FloseCode.SettingsLib/

## Example in VB.Net
```vb.net
Dim s As New FloseCode.SettingsLib.SettingsFile(settingsFileName)

s.PutInteger("someKey", 5)
s.PutString("user/name", "Jon Doe")
s.PutDateTime("user/lastUsed", DateTime.UtcNow())
s.PutBoolean("user/registered", True)

s.Save(settingsFileName)

s.GetInteger("someKey")
s.GetString("user/name", "default NAME")
s.GetDateTime("user/lastUsed")
s.GetBoolean("user/registered")

Dim user As IDictionary(Of String, Object) = s.GetAll(Of Object)("user")

' Remove key and all subkeys
s.RemoveKey("user", True)

s.Save(settingsFileName)
```

## Example Settings File "settings"
```INI
[/]
someKey = System.Int32"5"

[/user/]
name = "Jon Doe"
lastUsed = System.DateTime"5247730360738968069"
registered = System.Boolean"1"
```

## License

Copyright: Flose 2011 - 2016 https://www.mal-was-anderes.de/

Licensed under the LGPLv3: http://www.gnu.org/licenses/lgpl-3.0.html
