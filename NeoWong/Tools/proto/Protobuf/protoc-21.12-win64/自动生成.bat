
(for %%a in (java,csharp,cpp,python,ruby,objc) do ( 
   IF NOT EXIST "output\%%a" MD "output\%%a"
   for %%i in (proto\*.proto) do bin\protoc.exe -I=proto --%%a_out=output/%%a %%~ni.proto
))
echo "OK"
pause

::for %%i in (protos\*.proto) do bin\protoc.exe -I=protos --java_out=output/java %%~ni.proto

