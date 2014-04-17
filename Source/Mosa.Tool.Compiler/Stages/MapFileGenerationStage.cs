/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (grover) <sharpos@michaelruck.de>
 */

using Mosa.Compiler.Framework;
using Mosa.Compiler.Linker;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mosa.Tool.Compiler.Stages
{
	/// <summary>
	/// An compilation stage, which generates a map file of the built binary file.
	/// </summary>
	public sealed class MapFileGenerationStage : BaseCompilerStage
	{
		#region Data members

		public string MapFile { get; set; }

		/// <summary>
		/// Holds the text writer used to emit the map file.
		/// </summary>
		private TextWriter writer;

		#endregion Data members

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="MapFileGenerationStage"/> class.
		/// </summary>
		public MapFileGenerationStage()
		{
		}

		#endregion Construction

		protected override void Setup()
		{
			this.MapFile = CompilerOptions.MapFile;
		}

		protected override void Run()
		{
			if (string.IsNullOrEmpty(MapFile))
				return;

			using (writer = new StreamWriter(MapFile))
			{
				// Emit map file header
				writer.WriteLine(CompilerOptions.OutputFile);
				writer.WriteLine();
				writer.WriteLine("Timestamp is {0}", DateTime.Now);
				writer.WriteLine();
				writer.WriteLine("Preferred load address is {0:x16}", Linker.BaseAddress);
				writer.WriteLine();

				// Emit the sections
				EmitSections(Linker);
				writer.WriteLine();

				// Emit all symbols
				EmitSymbols(Linker);

				writer.Close();
			}
		}

		#region Internals

		/// <summary>
		/// Emits all the section created in the binary file.
		/// </summary>
		/// <param name="linker">The linker.</param>
		private void EmitSections(BaseLinker linker)
		{
			writer.WriteLine("Offset           Virtual          Length           Name                             Class");
			foreach (LinkerSection section in linker.Sections)
			{
				writer.WriteLine("{0:x16} {1:x16} {2:x16} {3} {4}", section.Offset, section.VirtualAddress, section.Length, section.Name.PadRight(32), section.SectionKind);
			}
		}

		private class LinkerSymbolComparerByVirtualAddress : IComparer<LinkerObject>
		{
			public int Compare(LinkerObject x, LinkerObject y)
			{
				return (int)(x.ResolvedVirtualAddress - y.ResolvedVirtualAddress);
			}
		}

		/// <summary>
		/// Emits all symbols emitted in the binary file.
		/// </summary>
		/// <param name="linker">The linker.</param>
		private void EmitSymbols(BaseLinker linker)
		{
			List<LinkerObject> sorted = new List<LinkerObject>();

			foreach (var symbol in linker.Symbols)
			{
				sorted.Add(symbol);
			}

			var comparer = new LinkerSymbolComparerByVirtualAddress();
			sorted.Sort(comparer);

			writer.WriteLine("Offset           Virtual          Length           Section Symbol");
			foreach (var symbol in sorted)
			{
				writer.WriteLine("{0:x16} {1:x16} {2:x16} {3} {4}", symbol.ResolvedSectionOffset, symbol.ResolvedVirtualAddress, symbol.Size, symbol.SectionKind.ToString().PadRight(7), symbol.Name);
			}

			var entryPoint = linker.EntryPoint;
			if (entryPoint != null)
			{
				writer.WriteLine();
				writer.WriteLine("Entry point is {0}", entryPoint.Name);
				writer.WriteLine("\tat Offset {0:x16}", entryPoint.ResolvedSectionOffset); // TODO! add section offset too?
				writer.WriteLine("\tat virtual address {0:x16}", entryPoint.ResolvedVirtualAddress);
			}
		}

		#endregion Internals
	}
}