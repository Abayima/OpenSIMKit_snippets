CSC=gmcs

all: SmartcardCommands.dll

SmartcardCommands.dll: *.cs Properties/AssemblyInfo.cs
	$(CSC) /out:$@ /target:library /r:../pcsc-sharp/pcsc-sharp.dll *.cs Properties/AssemblyInfo.cs
