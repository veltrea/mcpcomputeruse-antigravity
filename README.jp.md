# mcpcomputeruse-antigravity (Optimized for Google Antigravity)

[English README](README.md)

> [!IMPORTANT]
> 本リポジトリは [kblood/MCPComputerUse](https://github.com/kblood/MCPComputerUse) の派生（フォーク）プロジェクトです。
> **Google Antigravity** およびその他の高度な AI エージェントが Windows 環境で安定して動作することを目的として、内部構造の最適化およびリファクタリングを行っています。

## 📖 プロジェクト詳細および構築ガイド
本プロジェクトの技術的な背景、課題解決のプロセス、および詳細な導入手順については、以下の技術ブログ（note.com）を参照してください。

- **技術ブログ（note.com）**: [https://note.com/veltrea](https://note.com/veltrea)

---

## 🚀 本フォークの目的と解決された課題
オリジナルの実装（kblood版）では、現代の AI エージェント（特に Google Antigravity 等）での運用において、いくつかの技術的な不整合が確認されました。本リポジトリでは以下の問題を解消しています。

- **命名規則の正規化（エージェント互換性）**: 
  ツール名にコロン（`:`）が含まれていたため、一部の MCP クライアントでツールの認識・ロードに失敗する問題がありました。これをアンダースコア（`_`）へ置換し、正規表現 `^[a-zA-Z0-9_-]+$` に準拠させることで、すべてのエージェントでの正常な動作を保証しました。
- **公式 MCP SDK (ModelContextProtocol.Core) への完全移行**: 
  プロジェクト内に残存していた古いスタブ定義を削除し、最新の公式 SDK に準拠させました。これにより、ツールメタデータの整合性が向上し、ビルド時の警告（CS0436）を完全に排除しました。
- **実行環境の汎用化**: 
  特定の開発環境に依存していたハードコードされたパス設定を解消し、任意のディレクトリ構成でビルド・実行が可能な状態に整備しました。
- **リポジトリのクリーンアップ**: 
  Windows 自動化サーバーとしての本質に不要なレガシースクリプトやデバッグログを排除し、プロダクション品質のソースコード構成としました。

詳細な変更履歴については **[FORK_CHANGES.md](FORK_CHANGES.md)** を参照してください。

## 🛠️ 機能概要

- **スクリーンショット撮影**: マルチモニターおよび特定ウィンドウを対象とした柔軟なキャプチャ。
- **ウィンドウ管理**: ウィンドウ一覧の取得、フォーカス制御、状態操作（最大化・最小化等）。
- **入力自動化**: Win32 API を介した低レイテンシのマウス操作およびキーボード入力。
- **マクロ実行**: 複数操作をシーケンスとして実行するマクロシステム。

---

## 🏗️ セットアップ

### クライアント設定 (Claude Desktop / Google Antigravity)
`mcp_config.json` に以下の構成を追加してください：

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

### ビルド
- **環境**: .NET 8.0 SDK, Visual Studio 2022
- **コマンド**: `dotnet publish src/MCPComputerUse -c Release -r win-x64 --self-contained`

---

## 📄 ライセンス
MIT License.
Original work by kblood. Enhanced for AI-agent compatibility by community contributors.
