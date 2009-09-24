﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.Runtime.CompilerFramework.IR2
{
    /// <summary>
    /// Intermediate representation of a signed conversion instruction.
    /// </summary>
    /// <remarks>
    /// This instruction takes the source operand and converts to the request size maintaining its sign.
    /// </remarks>
    public class ZeroExtendedMoveInstruction : TwoOperandInstruction
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeroExtendedMoveInstruction"/>.
        /// </summary>
        public ZeroExtendedMoveInstruction()
        {
        }

        #endregion // Construction

        #region TwoOperandInstruction Overrides

        /// <summary>
        /// Returns a string representation of <see cref="SignExtendedMoveInstruction"/>.
        /// </summary>
        /// <returns>A string representation of the instruction.</returns>
        public override string ToString(ref InstructionData instruction)
        {
            return String.Format(@"IR.zconv {0} <- {1}", instruction.Operand1, instruction.Operand2);
        }

		/// <summary>
		/// Allows visitor based dispatch for this instruction object.
		/// </summary>
		/// <param name="visitor">The visitor object.</param>
		/// <param name="context">The context.</param>
        public override void Visit(IIRVisitor visitor, Context context)
        {
			visitor.ZeroExtendedMoveInstruction(context);
        }

        #endregion // TwoOperandInstruction Overrides
    }
}
