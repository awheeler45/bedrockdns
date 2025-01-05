# BEDROCKDNS

BEDROCKDNS is a custom DNS server designed to redirect specific Minecraft Bedrock Edition domains to a local server. This tool helps players connect to custom Minecraft servers on platforms that do not natively support them.

## Features
- Redirects Minecraft Bedrock Edition "Featured Servers" to a custom IP address.
- Supports both Windows and Linux platforms.
- Lightweight and easy to configure.

---

## Prerequisites

To build and run this project, you need to have .NET 8 installed on your system.

---

### Installing .NET 8

#### Windows
1. Download the .NET 8 SDK from the [official .NET website](https://dotnet.microsoft.com/download).
2. Run the installer and follow the on-screen instructions.
3. Verify the installation by opening a terminal (PowerShell or Command Prompt) and running:
   ```powershell
   dotnet --version
   ```

#### Linux
1. Add the Microsoft package repository:
   ```bash
   sudo apt-get update && sudo apt-get install -y wget apt-transport-https software-properties-common
   wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   sudo apt-get update
   ```
2. Install the .NET 8 SDK:
   ```bash
   sudo apt-get install -y dotnet-sdk-8.0
   ```
3. Verify the installation:
   ```bash
   dotnet --version
   ```

---

## Building the Project

Follow these steps to build the project for both Windows and Linux platforms:

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/bedrockdns.git
   cd bedrockdns
   ```

2. Build for Windows:
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./release/win-x64
   ```

3. Build for Linux:
   ```bash
   dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o ./release/linux-x64
   ```

4. The built executables will be located in the `release` folder:
   ```
   release/
   ├── win-x64/
   │   └── BEDROCKDNS.exe
   ├── linux-x64/
   │   └── BEDROCKDNS
   ```

---

## Running the Server

### Windows
1. Open a terminal and navigate to the `win-x64` directory:
   ```powershell
   cd release\win-x64
   ```
2. Run the server:
   ```powershell
   .\BEDROCKDNS.exe <redirect-ip>
   ```

### Linux
1. Open a terminal and navigate to the `linux-x64` directory:
   ```bash
   cd release/linux-x64
   ```
2. Make the file executable:
   ```bash
   chmod +x BEDROCKDNS
   ```
3. Run the server:
   ```bash
   ./BEDROCKDNS <redirect-ip>
   ```

Replace `<redirect-ip>` with the IP address of your custom Minecraft server.

---

## Example Usage

Run the server to redirect queries for supported Minecraft domains to `192.168.1.246`:
```bash
BEDROCKDNS.exe 192.168.1.246
```

Set your device's DNS server to point to the machine running `BEDROCKDNS`.

---

## Contributing

Feel free to submit issues or pull requests to improve this project. Contributions are always welcome!

---

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.