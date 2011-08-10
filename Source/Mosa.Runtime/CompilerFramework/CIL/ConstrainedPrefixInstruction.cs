﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Mosa.Runtime.Metadata;
using Mosa.Runtime.Metadata.Signatures;
using Mosa.Runtime.TypeSystem;

namespace Mosa.Runtime.CompilerFramework.CIL
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class ConstrainedPrefixInstruction : PrefixInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstrainedPrefixInstruction"/> class.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public ConstrainedPrefixInstruction(OpCode opcode)
			: base(opcode)
		{
		}

		#endregion // Construction

		#region Methods

		/// <summary>
		/// Decodes the specified instruction.
		/// </summary>
		/// <param name="ctx">The context.</param>
		/// <param name="decoder">The instruction decoder, which holds the code stream.</param>
		public override void Decode(Context ctx, IInstructionDecoder decoder)
		{
			// Decode base classes first
			//base.Decode(ctx, decoder);
			// Retrieve the type token
			Token token = decoder.DecodeTokenType();
			ctx.Other = decoder.TypeModule.GetType (token);

			if (ctx.Other == null)
			{
				var signature = decoder.GenericTypePatcher.PatchSignatureType(decoder.TypeModule, decoder.Method.DeclaringType, token);
				if (signature is BuiltInSigType)
				{
					var builtInSigType = signature as BuiltInSigType;
					switch (builtInSigType.Type)
					{
						case CilElementType.I4:
							{
								var int32type = decoder.TypeModule.TypeSystem.GetType("mscorlib", "System", "Int32");
								ctx.Other = int32type;
								return;
							}
						default:
							break;
					}
				}
			}
			/*
				_constraint = MetadataTypeReference.FromToken(decoder.Metadata, token);
				Debug.Assert(null != _constraint);
			 */
		}

		#endregion Methods

	}
}
