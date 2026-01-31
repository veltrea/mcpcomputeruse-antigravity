using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.Reflection;

namespace MCPComputerUse;

/// <summary>
/// Entry point for the MCPComputerUse MCP server, which implements a Model Context Protocol (MCP) server
/// that provides Windows automation and computer use tools to Large Language Models.
/// 
/// This server is built using the official C# SDK for MCP (https://github.com/modelcontextprotocol/csharp-sdk)
/// and provides tools for:
/// - Screenshot capture with multi-monitor support
/// - Window management and enumeration
/// - Mouse and keyboard automation
/// - Macro execution for complex automation sequences
/// </summary>
public static class Program
{
    /// <summary>
    /// Main entry point for the application.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Main(string[] args)
    {
        try
        {
            // Suppress console output to avoid interfering with the MCP protocol
            Console.SetOut(TextWriter.Null);
            
            // Set up error logging to a file in the same directory for easier debugging
            var logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mcp-server.log");
            using var errorWriter = new StreamWriter(logFile, true) { AutoFlush = true };
            Console.SetError(errorWriter);
            
            Console.Error.WriteLine($"Starting MCP Computer Use server at {DateTime.Now}");
            Console.Error.WriteLine($"Log file: {logFile}");

            // Build and run the MCP server
            var builder = Host.CreateEmptyApplicationBuilder(null);
            builder.Services
                .AddMcpServer(options =>
                {
                    options.ServerInfo = new() { Name = "MCPComputerUse", Version = "1.0" };
                })
                .WithStdioServerTransport()
                .WithToolsFromAssembly(Assembly.GetExecutingAssembly());

            await builder.Build().RunAsync();
        }
        catch (Exception ex)
        {
            // Log errors to stderr so they don't interfere with the protocol
            Console.Error.WriteLine($"MCP Computer Use server error: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
            return; // Exit process on error
        }
    }
}
