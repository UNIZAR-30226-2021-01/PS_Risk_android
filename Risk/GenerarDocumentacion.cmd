docfx Documentation/docfx.json --serve
if errorlevel 1 (
	cls
	echo No se ha podido ejecutar el programa docfx
	echo Descargalo desde https://github.com/dotnet/docfx/releases
	echo y comprueba que docfx.exe este en PATH"
	pause
)