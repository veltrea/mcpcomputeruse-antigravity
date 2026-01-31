# mcpcomputeruse-antigravity (Optimized for AI Agents)

[Japanese README (日本語)](README.jp.md)

> [!IMPORTANT]
> This repository is a specialized fork of [kblood/MCPComputerUse](https://github.com/kblood/MCPComputerUse).
> It has been refactored and optimized to ensure stable operation with **Google Antigravity** and other advanced AI agents in Windows environments.

## 📖 Technical Background & Setup Guide (Japanese)
For the technical background, troubleshooting process, and detailed setup guides, please refer to the following blog:

- **Developer Blog (note.com)**: [https://note.com/veltrea](https://note.com/veltrea)

---

## 🚀 Objectives & Resolved Issues
The original implementation presented several technical challenges when integrated with autonomous AI agents. This fork addresses those specifically:

- **Normalization of Tool Naming**: 
  Some tool names originally contained colons (`:`), which caused recognition failures in certain MCP clients. All tool names have been renamed using underscores (`_`) to comply with the standard regex `^[a-zA-Z0-9_-]+$`, ensuring universal compatibility.
- **Full Migration to Official MCP SDK**: 
  Eliminated legacy stub definitions and fully migrated to the official `ModelContextProtocol.Core` SDK. This improves metadata consistency and resolves build warnings (such as CS0436).
- **Environment Independence**: 
  Removed hardcoded absolute paths that were dependent on specific development environments. The project is now portable and can be built/run in any directory structure.
- **Repository Cleanup**: 
  Stripped away non-essential legacy scripts and debug logs, providing a clean, production-grade codebase focused on core Windows automation capabilities.

For a detailed record of technical changes, see **[FORK_CHANGES.md](FORK_CHANGES.md)**.

## 🛠️ Features

- **Screenshot Capture**: Flexible capture options for multiple monitors and specific windows.
- **Window Management**: Enumerate, focus, and manipulate window states (maximize/minimize).
- **Native Automation**: Low-latency mouse and keyboard input via Win32 API.
- **Macro System**: Execute complex automation sequences.

---

## 🏗️ Getting Started

### MCP Configuration
Add the following to your `mcp_config.json`:

```json
{
  "mcpServers": {
    "computer-use": {
      "command": "C:/path/to/MCPComputerUse.exe",
      "args": []
    }
  }
}
```

### Building from Source
- **Requirements**: .NET 8.0 SDK, Visual Studio 2022
- **Command**: `dotnet publish src/MCPComputerUse -c Release -r win-x64 --self-contained`

---

## 📄 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
Original work by kblood. Enhanced for AI-agent compatibility by community contributors.
