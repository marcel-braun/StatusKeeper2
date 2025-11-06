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