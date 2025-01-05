#!/bin/bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./release/win-x64 BEDROCKDNS.csproj
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o ./release/linux-x64 BEDROCKDNS.csproj
echo "Build completed for Windows and Linux."