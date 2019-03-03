﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common.Exceptions;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Linker;
using Mosa.Compiler.MosaTypeSystem;
using System;
using System.Diagnostics;

namespace Mosa.Workspace.Experiment.Debug
{
	internal static class Program
	{
		private static void Main()
		{
			var platform = "x86";

			var compilerOptions = new CompilerOptions()
			{
				EnableSSA = true,
				EnableIROptimizations = true,
				EnableSparseConditionalConstantPropagation = true,
				EnableInlinedMethods = true,
				EnableIRLongExpansion = true,
				EnableValueNumbering = true,
				TwoPassOptimizations = true,
				EnableMethodScanner = false,

				MultibootSpecification = MultibootSpecification.V1,
				LinkerFormatType = LinkerFormatType.Elf32,
				InlinedIRMaximum = 12,

				BaseAddress = 0x00500000,
				EmitStaticRelocations = false,
				EmitAllSymbols = false,

				EmitBinary = false
			};

			compilerOptions.Architecture = SelectArchitecture(platform);

			compilerOptions.AddSourceFile($"Mosa.TestWorld.{platform}.exe");
			compilerOptions.AddSourceFile("Mosa.Plug.Korlib.dll");
			compilerOptions.AddSourceFile($"Mosa.Plug.Korlib.{platform}.dll");

			var stopwatch = new Stopwatch();

			var compiler = new MosaCompiler
			{
				CompilerOptions = compilerOptions
			};

			compiler.Load();

			var method1 = GetMethod("Mosa.Kernel.x86.IDT::SetTableEntries", compiler.TypeSystem);
			var method2 = GetMethod("System.Void Mosa.TestWorld.x86.Boot::Thread1", compiler.TypeSystem);

			compiler.Initialize();
			compiler.PreCompile();

			stopwatch.Start();

			for (int i = 0; i < 5; i++)
			{
				var start = stopwatch.ElapsedMilliseconds;

				compiler.Schedule(method1);
				compiler.Compile();

				Console.WriteLine("Elapsed: " + (stopwatch.ElapsedMilliseconds - start).ToString("F2") + " ms");
			}

			for (int i = 0; i < 5; i++)
			{
				var start = stopwatch.ElapsedMilliseconds;

				compiler.Schedule(method2);
				compiler.Compile();

				Console.WriteLine("Elapsed: " + (stopwatch.ElapsedMilliseconds - start).ToString("F2") + " ms");
			}

			return;
		}

		private static MosaMethod GetMethod(string partial, TypeSystem typeSystem)
		{
			foreach (var type in typeSystem.AllTypes)
			{
				foreach (var method in type.Methods)
				{
					if (method.FullName.Contains(partial))
						return method;
				}
			}

			return null;
		}

		private static BaseArchitecture SelectArchitecture(string architecture)
		{
			switch (architecture.ToLower())
			{
				case "x86": return Platform.x86.Architecture.CreateArchitecture(Platform.x86.ArchitectureFeatureFlags.AutoDetect);
				case "x64": return Platform.x64.Architecture.CreateArchitecture(Platform.x64.ArchitectureFeatureFlags.AutoDetect);
				case "armv6": return Platform.ARMv6.Architecture.CreateArchitecture(Platform.ARMv6.ArchitectureFeatureFlags.AutoDetect);
				default: throw new NotImplementCompilerException(string.Format("Unknown or unsupported Architecture {0}.", architecture));
			}
		}
	}
}
