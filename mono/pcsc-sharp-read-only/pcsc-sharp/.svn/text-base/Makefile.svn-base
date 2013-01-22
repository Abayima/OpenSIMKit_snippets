CSC=gmcs

all: pcsc-sharp.dll

pcsc-sharp.dll: *.cs Properties/AssemblyInfo.cs
	$(CSC) /out:$@ /target:library *.cs Properties/AssemblyInfo.cs
