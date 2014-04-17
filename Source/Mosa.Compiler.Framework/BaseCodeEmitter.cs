/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (grover) <sharpos@michaelruck.de>
 *  Simon Wollwage (rootnode) <kintaro@think-in-co.de>
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 *  Scott Balmos <sbalmos@fastmail.fm>
 */

using Mosa.Compiler.Linker;
using Mosa.Compiler.MosaTypeSystem;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Mosa.Compiler.Framework
{
	/// <summary>
	/// Base code emitter.
	/// </summary>
	public abstract class BaseCodeEmitter
	{
		#region Types

		/// <summary>
		/// Patch
		/// </summary>
		protected struct Patch
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="Patch"/> struct.
			/// </summary>
			/// <param name="label">The label.</param>
			/// <param name="position">The position.</param>
			public Patch(int label, long position)
			{
				Label = label;
				Position = position;
			}

			/// <summary>
			/// Patch label
			/// </summary>
			public int Label;

			/// <summary>
			/// The patch's position in the stream
			/// </summary>
			public long Position;

			/// <summary>
			/// Returns a <see cref="System.String"/> that represents this instance.
			/// </summary>
			/// <returns>
			/// A <see cref="System.String"/> that represents this instance.
			/// </returns>
			public override string ToString()
			{
				return "[@" + Position.ToString() + " -> " + Label.ToString() + "]";
			}
		}

		#endregion Types

		#region Data members

		/// <summary>
		/// The stream used to write machine code bytes to.
		/// </summary>
		protected Stream codeStream;

		/// <summary>
		/// The position that the code stream starts.
		/// </summary>
		protected long codeStreamBasePosition;

		/// <summary>
		/// Holds the linker used to resolve externals.
		/// </summary>
		protected BaseLinker linker;

		/// <summary>
		/// List of labels that were emitted.
		/// </summary>
		private readonly Dictionary<int, long> labels = new Dictionary<int, long>();

		/// <summary>
		/// Patches we need to perform.
		/// </summary>
		private readonly List<Patch> patches = new List<Patch>();

		#endregion Data members

		#region Properties

		/// <summary>
		/// Gets the type system.
		/// </summary>
		/// <value>
		/// The type system.
		/// </value>
		public TypeSystem TypeSystem { get; private set; }

		/// <summary>
		/// Gets the name of the method.
		/// </summary>
		/// <value>
		/// The name of the method.
		/// </value>
		protected string MethodName { get; private set; }

		/// <summary>
		/// Gets the patches.
		/// </summary>
		/// <value>
		/// The patches.
		/// </value>
		protected IList<Patch> Patches { get { return patches.AsReadOnly(); } }

		#endregion Properties

		#region BaseCodeEmitter Members

		/// <summary>
		/// Initializes a new instance of <see cref="BaseCodeEmitter" />.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="linker">The linker.</param>
		/// <param name="codeStream">The stream the machine code is written to.</param>
		/// <param name="typeSystem">The type system.</param>
		public void Initialize(string methodName, BaseLinker linker, Stream codeStream, TypeSystem typeSystem)
		{
			Debug.Assert(codeStream != null);
			Debug.Assert(linker != null);

			this.MethodName = methodName;
			this.linker = linker;
			this.codeStream = codeStream;
			this.codeStreamBasePosition = codeStream.Position;
			this.TypeSystem = typeSystem;
		}

		/// <summary>
		/// Emits a label into the code stream.
		/// </summary>
		/// <param name="label">The label name to emit.</param>
		public void Label(int label)
		{
			/*
			 * Labels are used to resolve branches inside a procedure. Branches outside
			 * of procedures are handled differently, t.b.d.
			 *
			 * So we store the current instruction offset with the label info to be able to
			 * resolve jumps to this location.
			 *
			 */

			Debug.Assert(!labels.ContainsKey(label));

			// Add this label to the label list, so we can resolve the jump later on
			labels.Add(label, codeStream.Position);

			//Debug.WriteLine("LABEL: " + label.ToString() + " @" + codeStream.Position.ToString());
		}

		/// <summary>
		/// Gets the position.
		/// </summary>
		/// <param name="label">The label.</param>
		/// <returns></returns>
		public long GetPosition(int label)
		{
			return labels[label];
		}

		/// <summary>
		/// Gets the current position.
		/// </summary>
		/// <value>The current position.</value>
		public long CurrentPosition { get { return codeStream.Position; } }

		#endregion BaseCodeEmitter Members

		#region Code Generation Members

		/// <summary>
		/// Writes the byte.
		/// </summary>
		/// <param name="data">The data.</param>
		public void WriteByte(byte data)
		{
			codeStream.WriteByte(data);
		}

		/// <summary>
		/// Writes the byte.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		public void Write(byte[] buffer, int offset, int count)
		{
			codeStream.Write(buffer, offset, count);
		}

		#endregion Code Generation Members

		protected bool TryGetLabel(int label, out long position)
		{
			return labels.TryGetValue(label, out position);
		}

		protected void AddPatch(int label, long position)
		{
			patches.Add(new Patch(label, position));
		}

		public abstract void ResolvePatches();
	}
}