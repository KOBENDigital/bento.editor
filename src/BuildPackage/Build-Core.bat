set msbuildpath="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"

if not exist %msbuildpath% (
  set msbuildpath="C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe"
)

call nuget.exe restore ..\Bento.sln
call %msbuildpath% Bento.Core/Package.build.xml /p:Configuration=Release