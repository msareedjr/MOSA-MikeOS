/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (grover) <sharpos@michaelruck.de>
 */

namespace Mosa.Runtime.CompilerFramework
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	using Mosa.Runtime.Metadata.Signatures;
	using Mosa.Runtime.Vm;

	/// <summary>
	/// Schedules compilation of types/methods.
	/// </summary>
	public class MethodCompilerSchedulerStage : IAssemblyCompilerStage, ICompilationSchedulerStage, IPipelineStage
	{
		private readonly Queue<RuntimeMethod> methodQueue;

		private readonly Queue<RuntimeType> typeQueue;

		private AssemblyCompiler compiler;

		public MethodCompilerSchedulerStage()
		{
			this.methodQueue = new Queue<RuntimeMethod>();
			this.typeQueue = new Queue<RuntimeType>();
		}

		#region IPipelineStage

		string IPipelineStage.Name { get { return @"CompilerScheduler"; } }

		#endregion // IPipelineStage

		#region IAssemblyCompilerStage

		void IAssemblyCompilerStage.Setup(AssemblyCompiler compiler)
		{
			this.compiler = compiler;
		}

		void IAssemblyCompilerStage.Run()
		{
			while (typeQueue.Count != 0)
			{
				RuntimeType type = typeQueue.Dequeue();
				CompileType(type);
			}

			CompilePendingMethods();
		}

		#endregion // IAssemblyCompilerStage

		private void CompileType(RuntimeType type)
		{
			if (type.IsDelegate)
			{
				Console.WriteLine(@"Skipping delegate type " + type);
				return;
			}

			Console.WriteLine(@"Compiling " + type.FullName);
			Debug.WriteLine(@"Compiling " + type.FullName);
			foreach (RuntimeMethod method in type.Methods)
			{
				if (method.IsGeneric)
				{
					Debug.WriteLine("Skipping generic method: " + type + "." + method.Name);
					Debug.WriteLine("Generic method will not be available in compiled image.");
					continue;
				}

				if (method.IsNative)
				{
					Debug.WriteLine("Skipping native method: " + type + "." + method.Name);
					Debug.WriteLine("Method will not be available in compiled image.");
					continue;
				}

				ScheduleMethodForCompilation(method);
			}

			CompilePendingMethods();
		}

		private void CompilePendingMethods()
		{
			while (methodQueue.Count > 0)
			{
				RuntimeMethod method = methodQueue.Dequeue();
				CompileMethod(method);
			}
		}

		private void CompileMethod(RuntimeMethod method)
		{
			Console.WriteLine(@"Compiling " + method.ToString());
			Debug.WriteLine(@"Compiling " + method.ToString());
			using (IMethodCompiler mc = compiler.CreateMethodCompiler(this, method.DeclaringType, method))
			{
				try
				{
					mc.Compile();
				}
				catch (Exception e)
				{
					HandleCompilationException(e);
					throw;
				}
			}
		}

		protected virtual void HandleCompilationException(Exception e)
		{
		}

		public void ScheduleTypeForCompilation(RuntimeType type)
		{
			if (type == null)
				throw new ArgumentNullException(@"type");

			if (type.IsCompiled == true)
			{
				return;
			}

			if (type.IsGeneric == false)
			{
				Console.WriteLine(@"Scheduling {0}", type.FullName);
				Console.WriteLine(String.Format(@"Scheduling {0}", type.FullName));

				typeQueue.Enqueue(type);
				type.IsCompiled = true;
			}
		}

		public void ScheduleMethodForCompilation(RuntimeMethod method)
		{
			if (method == null)
				throw new ArgumentNullException(@"method");

			if (method.IsGeneric == false)
			{
				Console.WriteLine(@"Scheduling {1}.{0}", method.Name, method.DeclaringType.FullName);
				Debug.WriteLine(String.Format(@"Scheduling {1}.{0}", method.Name, method.DeclaringType.FullName));

				methodQueue.Enqueue(method);
			}
		}
	}
}
