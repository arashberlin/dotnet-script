﻿using System.IO;
using Xunit;

namespace Dotnet.Script.Tests
{
    public class ScaffoldingTests
    {
        [Fact]
        public void ShouldInitializeScriptFolder()
        {
            using (var scriptFolder = new DisposableFolder())
            {                
                var result = Execute("init", scriptFolder.Path);
                Assert.Equal(0, result.exitCode);
                Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "main.csx")));
                Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "omnisharp.json")));
                Assert.True(File.Exists(Path.Combine(scriptFolder.Path, ".vscode", "launch.json")));                
            }
        }

        [Fact]
        public void ShouldCreateNewScript()
        {
            using (var scriptFolder = new DisposableFolder())
            {
                var result = Execute("new script.csx", scriptFolder.Path);
                Assert.Equal(0, result.exitCode);
                Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "script.csx")));
            }
        }

        [Fact]
        public void ShouldInitFolderWithCustomFileNAme()
        {
            using (var scriptFolder = new DisposableFolder())
            {
                var result = Execute("init custom.csx", scriptFolder.Path);
                Assert.Equal(0, result.exitCode);
                Assert.True(File.Exists(Path.Combine(scriptFolder.Path, "custom.csx")));
            }
        }

        [Fact]
        public void ShouldNotCreateDefaultFileForFolderWithExistingScriptFiles()
        {
            using (var scriptFolder = new DisposableFolder())
            {
                Execute("init custom.csx", scriptFolder.Path);
                Execute("init", scriptFolder.Path);
                Assert.False(File.Exists(Path.Combine(scriptFolder.Path, "main.csx")));
            }
        }

        /// <summary>
        /// Use this if you need to debug.
        /// </summary>        
        private static int ExecuteInProcess(params string[] args)
        {            
            return Program.Main(args);
        }

        private static (string output, int exitCode) Execute(string args, string workingDirectory)
        {
            var result = ProcessHelper.RunAndCaptureOutput("dotnet", GetDotnetScriptArguments(args), workingDirectory);
            return result;
        }

        private static string[] GetDotnetScriptArguments(string args)
        {
            string configuration;
#if DEBUG
            configuration = "Debug";
#else
            configuration = "Release";
#endif
            return new[] { "exec", Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "Dotnet.Script", "bin", configuration, "netcoreapp2.0", "dotnet-script.dll"), args };
        }
    }

}