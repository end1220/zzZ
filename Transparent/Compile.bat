cd /d %~dp0
C:/Windows/Microsoft.NET/Framework/v4.0.30319/csc.exe /target:library /out:Floating-runtime.dll /reference:UnitySubsetV3.5/mscorlib.dll;UnitySubsetV3.5/System.dll;UnitySubsetV3.5/System.Core.dll;UnitySubsetV3.5/System.xml.dll;UnitySubsetV3.5/System.xml.linq.dll;Library/UnityAssemblies/UnityEditor.dll;Library/UnityAssemblies/UnityEngine.dll;Library/UnityAssemblies/UnityEngine.UI.dll;Library/UnityAssemblies/UnityEditor.UI.dll;Assets/Plugins/protobuf-net.dll /recurse:Assets/Scripts/*.cs


pause