﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Intrinsic
{
	/// <summary>
	/// IntrinsicMethods
	/// </summary>
	internal static partial class IntrinsicMethods
	{
		[IntrinsicMethod("Mosa.Platform.x86.Intrinsic::GetMultibootEAX")]
		private static void GetMultibootEAX(Context context, MethodCompiler methodCompiler)
		{
			var MultibootEAX = Operand.CreateUnmanagedSymbolPointer(Intel.CompilerStages.MultibootV1Stage.MultibootEAX, methodCompiler.TypeSystem);

			context.SetInstruction(IRInstruction.Load32, context.Result, MultibootEAX, methodCompiler.ConstantZero32);
		}
	}
}
