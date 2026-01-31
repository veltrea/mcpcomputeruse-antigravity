# MCPComputerUse (Enhanced Fork for Windows)

> [!NOTE]
> This repository is a fork of the original [kblood/MCPComputerUse](https://github.com/kblood/MCPComputerUse).
> It has been enhanced and stabilized for better integration with MCP clients like Google Antigravity and Claude Desktop on Windows environments.

A native C# implementation of a Computer Use MCP Server for Windows, providing superior automation capabilities through direct Windows API integration.

## 🚀 Key Improvements in this Fork

This fork addresses several critical issues found in the original implementation to ensure 100% compatibility with modern MCP environments:

- **Full MCP SDK Compliance**: Resolved [CS0436] build warnings by removing obsolete stub definitions and migrating to the official `ModelContextProtocol.Core` SDK attributes (`[McpServerTool]`).
- **Standardized Naming Conventions**: Fixed tool naming errors where colons (`:`) prevented tools from being recognized by sensitive MCP clients. All tools now follow the `^[a-zA-Z0-9_-]+$` naming pattern.
- **Path Independence**: Removed hardcoded path dependencies that targeted specific developer environments, allowing for easier setup in any directory.
- **Cleaned Repository**: Removed unnecessary legacy scripts (Skyrim modding leftovers) and debug logs to provide a focused, production-ready MCP server.

For a detailed list of technical changes and the development process, see **[FORK_CHANGES.md](FORK_CHANGES.md)**.

## 🤖 Development Process
This stabilization was achieved through a collaborative "Pair Programming" session between **Google Antigravity (AI)** and a **Human Developer (Master)**. The human provided strategic analysis and error identification, while the AI executed the wide-scale refactoring and verification.

---

## 🚀 Features

- **Screenshot Capture**: Multi-monitor support with flexible targeting
- **Window Management**: Complete window enumeration and control
- **Mouse Automation**: Precise clicking, moving, and scrolling
- **Keyboard Automation**: Text typing and key combinations
- **Macro System**: Complex automation sequences
- **MCP Protocol**: Full compliance with MCP specification
- **High Performance**: Native Windows API integration
- **Zero Dependencies**: Self-contained executable

## 🛠️ Tools Available

### Screenshot Tools
- `take_screenshot` - Take screenshots with flexible targeting (Multi-monitor, Window-specific)
- Configurable output formats

### Window Management
- `list_windows` - List all visible windows
- `get_active_window` - Get currently active window info
- `focus_window` - Focus a window by ID or name
- `manage_window` - Minimize, maximize, restore windows

### Input Automation
- `mouse_click` - Click at coordinates with button options
- `mouse_move` - Move mouse to coordinates
- `type_text` - Type text with Unicode support
- `press_key` - Press keys with modifier support
- `scroll` - Scroll at specific coordinates

### Macro System
- `run_macro` - Execute complex automation sequences

### System Info
- `get_server_capabilities` - Get server capabilities and status

## 🏗️ Architecture

### Core Components
- **MCPComputerUse.exe** - Main console application with MCP protocol
- **WindowsAutomation.dll** - Native Windows API automation library
- **MCPProtocol.dll** - MCP specification implementation

### Technology Stack
- **.NET 8.0** - Latest LTS framework
- **Windows Forms** - For screen capture support
- **System.Drawing** - Image manipulation
- **Newtonsoft.Json** - JSON serialization
- **Native Windows APIs** - Direct P/Invoke integration

## 📦 Installation

### Prerequisites
- Windows 10 or Windows 11
- .NET 8.0 Runtime (if not using self-contained build)

### Claude Desktop Integration
Add to your Claude Desktop configuration:

```json
{
  "mcpServers": {
    "computer-use": {
      "command": "c:/path/to/MCPComputerUse.exe",
      "args": []
    }
  }
}
```

## 🔧 Building from Source

### Requirements
- Visual Studio 2022 or VS Code with C# extension
- .NET 8.0 SDK

### Build Steps
```bash
# Clone the repository
git clone <repository-url>
cd MCPComputerUse

# Restore dependencies
dotnet restore

# Build solution
dotnet build --configuration Release

# Publish self-contained executable
dotnet publish src/MCPComputerUse -c Release -r win-x64 --self-contained -o ./publish
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
Originally created by kblood. Enhanced and maintained by the community.

---

**Built with ❤️ for the Windows automation community**
