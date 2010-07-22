﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Fröhlich (grover) <sharpos@michaelruck.de>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Mosa.Runtime;
using Mosa.Runtime.CompilerFramework;
using Mosa.Runtime.Linker;
using Mosa.Runtime.Loader;
using Mosa.Tools.Compiler.Symbols.Pdb;
using Mosa.Tools.Compiler.TypeInitializers;

namespace Mosa.Tools.Compiler
{
	public class AssemblyCompilationStage : IAssemblyCompilerStage
	{
		private readonly List<IMetadataModule> inputAssemblies;

		private AssemblyCompiler outputAssemblyCompiler;

		private IAssemblyLinker linker;

		private ITypeInitializerSchedulerStage typeInitializerSchedulerStage;

		public AssemblyCompilationStage(IEnumerable<string> inputFileNames)
		{
			inputAssemblies = new List<IMetadataModule>();

			//List<IMetadataModule> modules = new List<IMetadataModule>();

			foreach (string inputFileName in inputFileNames)
			{
				IMetadataModule assembly = LoadAssembly(RuntimeBase.Instance, inputFileName);
				inputAssemblies.Add(assembly);

				//modules.Add(assembly);
			}

			//Mosa.Runtime.Metadata.MergedMetadata merged = new Runtime.Metadata.MergedMetadata(modules);
			//inputAssemblies.Add(merged);
		}

		#region IPipelineStage members

		/// <summary>
		/// Retrieves the name of the compilation stage.
		/// </summary>
		/// <value>The name of the compilation stage.</value>
		string IPipelineStage.Name { get { return @"AssemblyCompilationStage"; } }

		#endregion // IPipelineStage

		#region IAssemblyCompilerStage

		void IAssemblyCompilerStage.Run()
		{
			foreach (IMetadataModule assembly in inputAssemblies)
				CompileAssembly(assembly);
		}

		void IAssemblyCompilerStage.Setup(AssemblyCompiler compiler)
		{
			outputAssemblyCompiler = compiler;
			typeInitializerSchedulerStage = compiler.Pipeline.FindFirst<ITypeInitializerSchedulerStage>();
			linker = compiler.Pipeline.FindFirst<IAssemblyLinker>();
		}

		#endregion IAssemblyCompilerStage

		private IMetadataModule LoadAssembly(RuntimeBase runtime, string assemblyFileName)
		{
			try
			{
				IMetadataModule assemblyModule = runtime.AssemblyLoader.Load(assemblyFileName);

				// Try to load debug information for the compilation
				LoadAssemblyDebugInfo(assemblyFileName);

				return assemblyModule;
			}
			catch (BadImageFormatException bife)
			{
				throw new CompilationException(String.Format("Couldn't load input file {0} (invalid format).", assemblyFileName), bife);
			}
		}

		private void LoadAssemblyDebugInfo(string assemblyFileName)
		{
			string dbgFile = Path.Combine(Path.GetDirectoryName(assemblyFileName), Path.GetFileNameWithoutExtension(assemblyFileName) + ".pdb") + "!!";

			if (File.Exists(dbgFile))
			{
				using (FileStream fileStream = new FileStream(dbgFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (PdbReader reader = new PdbReader(fileStream))
					{
						Debug.WriteLine(@"Global symbols:");
						foreach (CvSymbol symbol in reader.GlobalSymbols)
						{
							Debug.WriteLine("\t" + symbol.ToString());
						}

						Debug.WriteLine(@"Types:");
						foreach (PdbType type in reader.Types)
						{
							Debug.WriteLine("\t" + type.Name);
							Debug.WriteLine("\t\tSymbols:");
							foreach (CvSymbol symbol in type.Symbols)
							{
								Debug.WriteLine("\t\t\t" + symbol.ToString());
							}

							Debug.WriteLine("\t\tLines:");
							foreach (CvLine line in type.LineNumbers)
							{
								Debug.WriteLine("\t\t\t" + line.ToString());
							}
						}
					}
				}
			}
		}

		private void CompileAssembly(IMetadataModule assembly)
		{
			using (AotAssemblyCompiler assemblyCompiler = new AotAssemblyCompiler(outputAssemblyCompiler.Architecture, assembly, typeInitializerSchedulerStage, linker))
			{
				assemblyCompiler.Run();
			}
		}
	}
}
