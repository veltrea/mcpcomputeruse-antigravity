# mcpcomputeruse-antigravity (Optimized for Google Antigravity)

[日本語版 README (Japanese)](README.jp.md)

> [!IMPORTANT]
> This repository is a specialized fork of [kblood/MCPComputerUse](https://github.com/kblood/MCPComputerUse).
> It was specifically refactored to ensure full compatibility with **Google Antigravity** and other advanced AI agents on Windows.

## 📖 Detailed Logs & Guide (Japanese)
For the background story of this project and detailed setup guides (in Japanese), please visit the Master's blog:
- **Master's note.com**: [https://note.com/veltrea](https://note.com/veltrea)

---

## 🚀 Why this Fork? (Antigravity Compatibility)

The original implementation contained several hurdles that prevented modern AI agents from functioning correctly. This fork addresses those specifically:

- **Antigravity-Ready Naming**: Fixed tool naming errors where colons (`:`) caused tool-loading failures in Antigravity. All tools now follow strict naming patterns (`mouse_click`, `take_screenshot`, etc.).
- **Modern MCP SDK Integration**: Migrated from obsolete stub definitions to the official `ModelContextProtocol.Core` SDK. This ensures that tool metadata and execution flow are exactly what AI clients expect.
- **Improved Windows Stability**: Resolved [CS0436] build warnings and path dependencies, ensuring a "zero-warning" build that is stable enough for autonomous AI operations.
- **Streamlined Source**: Stripped away legacy modding scripts and debug artifacts to provide a clean, production-grade foundation for AI-human pair programming.

For a detailed list of technical changes, see **[FORK_CHANGES.md](FORK_CHANGES.md)**.

## 🤖 Developed together with Antigravity
This stabilization was achieved through a real-world "Pair Programming" session. The fixes included here are the direct result of troubleshooting sessions between **Google Antigravity (AI)** and its **Master (Human)**, making it a battle-tested solution for AI-assisted Windows control.

---

## 🚀 Features

- **Screenshot Capture**: Multi-monitor support with flexible targeting
- **Window Management**: Complete window enumeration and control
- **Mouse Automation**: Precise clicking, moving, and scrolling
- **Keyboard Automation**: Text typing and key combinations
- **Macro System**: Complex automation sequences
- **MCP Protocol**: Full compliance with official specifications
- **High Performance**: Native Windows API integration

## 🛠️ Tools Available

### Screenshot Tools
- `take_screenshot` - Take screenshots with flexible targeting
### Window Management
- `list_windows` - List all visible windows
- `get_active_window` - Get currently active window info
- `focus_window` - Focus a window by ID or name
- `manage_window` - Minimize, maximize, restore windows
### Input Automation
- `mouse_click`, `mouse_move`, `type_text`, `press_key`, `scroll`
### Macro & System
- `run_macro`, `get_server_capabilities`

## 🏗️ Architecture

- **Visual Studio 2022 / .NET 8.0**
- **Native Windows APIs** for high-reliability automation.

## 📦 Installation

### Claude Desktop / Google Antigravity Integration
Add to your MCP configuration:

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

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
Originally created by kblood. Modified for AI-agent compatibility by the community.
