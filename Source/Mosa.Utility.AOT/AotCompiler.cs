﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.CompilerStages;
using Mosa.Compiler.Framework.Stages;

namespace Mosa.Utility.Aot
{
	public class AotCompiler : BaseCompiler
	{
		/// <summary>
		/// Extends the compiler setup.
		/// </summary>
		public override void ExtendCompilerSetup()
		{
			var bootStage = CompilerOptions.BootStageFactory != null ? CompilerOptions.BootStageFactory() : null;

			CompilePipeline.Add(new BaseCompilerStage[] {
				bootStage,
				new PlugStage(),
				new TypeInitializerSchedulerStage(),
				new MethodLookupTableStage(),
				new MethodExceptionLookupTableStage(),
				new MetadataStage(),
				new LinkerFinalizationStage(),
				CompilerOptions.MapFile != null ? new MapFileGenerationStage() : null,
				CompilerOptions.DebugFile != null ? new DebugFileGenerationStage() : null
			});
		}

		protected override BaseMethodCompilerStage[] CreateMethodPipeline()
		{
			return new BaseMethodCompilerStage[] {
				new CILDecodingStage(),
				new ExceptionPrologueStage(),
				new OperandAssignmentStage(),
				new StackSetupStage(),
				new CILProtectedRegionStage(),
				new ExceptionStage(),
				new StaticAllocationResolutionStage(),
				new CILTransformationStage(),
				new UnboxValueTypeStage(),
				(CompilerOptions.EnableInlinedMethods) ? new InlineStage() : null,
				(CompilerOptions.EnableSSA) ? new EdgeSplitStage() : null,
				(CompilerOptions.EnableSSA) ? new EnterSSAStage() : null,
				(CompilerOptions.EnableSparseConditionalConstantPropagation && CompilerOptions.EnableSSA) ? new SparseConditionalConstantPropagationStage() : null,
				(CompilerOptions.EnableIROptimizations) ? new IROptimizationStage() : null,
				new LowerIRStage(),
				(CompilerOptions.IRLongExpansion && Architecture.NativePointerSize == 4) ? new IRLongExpansionStage() : null,
				(CompilerOptions.TwoPassOptimizations && CompilerOptions.EnableIROptimizations && CompilerOptions.EnableSparseConditionalConstantPropagation && CompilerOptions.EnableSSA) ? new SparseConditionalConstantPropagationStage() : null,
				(CompilerOptions.TwoPassOptimizations && CompilerOptions.EnableIROptimizations && CompilerOptions.EnableSparseConditionalConstantPropagation && CompilerOptions.EnableSSA) ? new IROptimizationStage() : null,
				(CompilerOptions.EnableSSA) ? new LeaveSSAStage() : null,
				new IRCleanupStage(),
				(CompilerOptions.EnableInlinedMethods) ? new InlineEvaluationStage() : null,
				new DevirtualizeCallStage(),
				new CallStage(),
				new PlatformStubStage(),
				new PlatformEdgeSplitStage(),
				new VirtualRegisterRenameStage(),
				new GreedyRegisterAllocatorStage(),
				new StackLayoutStage(),
				new EmptyBlockRemovalStage(),
				new BlockOrderingStage(),
				new CodeGenerationStage(),
				new PreciseGCStage(),
				new ProtectedRegionLayoutStage(),
			};
		}
	}
}
