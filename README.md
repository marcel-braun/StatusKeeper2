# Debugging

```
cd StatusKeeperTerminalApp
dotnet run --launch-profile "StatusKeeperTerminalApp"
oder
dotnet run
```

# Build

**Windows**
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/windows

**Apple Intel**
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/macos-x64

**Apple Silicon**
dotnet publish -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -o ./publish/macos-arm64

# Release

Verwendung
PowerShell (Windows):

```ps
# Mit Standard-Version 1.0.0
.\build-releases.ps1

# Mit spezifischer Version
.\build-releases.ps1 -Version "2.1.0"
```

Bash (Linux/macOS):

```
# Skript ausführbar machen
chmod +x build-releases.sh

# Mit Standard-Version 1.0.0
./build-releases.sh

# Mit spezifischer Version
./build-releases.sh "2.1.0"
```