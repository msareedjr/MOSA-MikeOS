﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Fröhlich (grover) <michael.ruck@michaelruck.de>
 *  Phil Garcia (tgiphil) <phil@thinkedge.com> 
 */

using System;
using MbUnit.Framework;

using Test.Mosa.Runtime.CompilerFramework.Numbers;

namespace Test.Mosa.Runtime.CompilerFramework.CIL
{
	[TestFixture]
	[Importance(Importance.Critical)]
	//[Category(@"Basic types")]
	//[Description(@"Tests support for the basic type System.Double")]
	public partial class DoubleFixture
	{
		private readonly FloatingPointInstructionTestRunner<double, double> arithmeticTests = new FloatingPointInstructionTestRunner<double, double>
		{
			ExpectedType = "double",
			FirstType = "double",
			SecondType = "double",
			IncludeRem = false,
			IncludeRet = true
		};

		private readonly ComparisonInstructionTestRunner<double> comparisonTests = new ComparisonInstructionTestRunner<double>
		{
			FirstType = "double"
		};

		private readonly SZArrayInstructionTestRunner<double> arrayTests = new SZArrayInstructionTestRunner<double>
		{
			FirstType = "double"
		};
	
		#region Add

		[Test, Factory(typeof(Variations), "R8_R8")]
		public void AddR8R8(double a, double b)
		{
			this.arithmeticTests.Add((a + b), a, b);
		}

		#endregion // Add

		#region Sub

		[Test, Factory(typeof(Variations), "R8_R8")]
		public void SubR8R8(double a, double b)
		{
			this.arithmeticTests.Sub((a - b), a, b);
		}

		#endregion // Sub

		#region Mul

		[Test, Factory(typeof(Variations), "R8_R8")]
		public void MulR8R8(double a, double b)
		{
			this.arithmeticTests.Mul((a * b), a, b);
		}

		#endregion // Mul

		#region Div

		[Test, Factory(typeof(Variations), "R8_R8WithoutZero")]
		public void DivR8R8(double a, double b)
		{
			this.arithmeticTests.Div((a / b), a, b);
		}

		//[Test, Factory(typeof(Variations), "R8_R8Zero")]
		//[ExpectedException(typeof(DivideByZeroException))]
		//public void DivR8R8DivideByZeroException(double a, double b)
		//{
		//    this.arithmeticTests.Div((a / b), a, b);
		//}

		#endregion // Div

		#region Rem

		[Test, Factory(typeof(Variations), "R8_R8AboveZero")]
		public void RemR8R8(double a, double b)
		{
			this.arithmeticTests.Rem((a % b), a, b);
		}

		//[Test, Factory(typeof(Variations), "R8_R8Zero")]
		//[ExpectedException(typeof(DivideByZeroException))]
		//public void RemR8R8DivideByZeroException(double a, double b)
		//{
		//    this.arithmeticTests.Rem((a % b), a, b);
		//}

		//[Test, Factory(typeof(Variations), "R8_R8BelowZero")]
		//[ExpectedException(typeof(OverflowException))]
		//public void RemR8R8OverflowException(double a, double b)
		//{
		//    this.arithmeticTests.Rem((a % b), a, b);
		//}

		#endregion // Rem

		#region Neg

		[Test, Factory(typeof(R8), "Samples")]
		public void NegR8(double first)
		{
			this.arithmeticTests.Neg(-first, first);
		}

		#endregion // Neg

		#region Ret

		[Test, Factory(typeof(R8), "Samples")]
		public void RetR8(double value)
		{
			this.arithmeticTests.Ret(value);
		}

		#endregion // Ret

		#region Ceq

		[Test, Factory(typeof(Variations), "R8_R8")]
		public void CeqR8R8(double first, double second)
		{
			this.comparisonTests.Ceq((first == second), first, second);
		}

		#endregion // Ceq

		#region Cgt

		[Test, Factory(typeof(Variations), "R8_R8")]
		public void CgtR8R8(double first, double second)
		{
			this.comparisonTests.Cgt((first > second), first, second);
		}

		#endregion // Cgt

		#region Clt

		[Test, Factory(typeof(Variations), "R8_R8")]
		public void CltR8R8(double first, double second)
		{
			this.comparisonTests.Clt((first < second), first, second);
		}

		#endregion // Clt

		#region Cge

		[Test, Factory(typeof(Variations), "R8_R8")]
		public void CgeR8R8(double first, double second)
		{
			this.comparisonTests.Cge((first >= second), first, second);
		}

		#endregion // Cge

		#region Cle

		[Test, Factory(typeof(Variations), "R8_R8")]
		public void CleR8R8(double first, double second)
		{
			this.comparisonTests.Cle((first <= second), first, second);
		}

		#endregion // Cle

		#region Newarr

		[Test]
		public void NewarrR8()
		{
			this.arrayTests.Newarr();
		}

		#endregion // Newarr

		#region Ldlen

		[Test, Factory(typeof(Variations), "SmallNumbers")]
		public void LdlenR8(int length)
		{
			this.arrayTests.Ldlen(length);
		}

		#endregion // Ldlen

		#region Stelem

		[Test, Factory(typeof(Variations), "ISmall_R8")]
		public void StelemR8(int index, double value)
		{
			this.arrayTests.Stelem(index, value);
		}

		#endregion // Stelem

		#region Ldelem

		[Test, Factory(typeof(Variations), "ISmall_R8")]
		public void LdelemR8(int index, double value)
		{
			this.arrayTests.Ldelem(index, value);
		}

		#endregion // Ldelem

		#region Ldelema

		[Test, Factory(typeof(Variations), "ISmall_R8")]
		public void LdelemaR8(int index, double value)
		{
			this.arrayTests.Ldelema(index, value);
		}

		#endregion // Ldelema
	}
}
