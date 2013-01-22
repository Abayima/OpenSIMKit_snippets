CSC=gmcs

all: MCard-sharp.dll

MCard-sharp.dll: *.cs Properties/AssemblyInfo.cs
	$(CSC) /out:$@ /target:library /r:../pcsc-sharp/pcsc-sharp.dll *.cs Properties/AssemblyInfo.cs
