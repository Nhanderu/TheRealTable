﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Nhanderu.TheRealTable.TruthTable
{
    public class TruthTable
    {
        private String _formula;
        private Dictionary<String, Char> _operators;
        private readonly List<Char> _defaultOperators = new List<Char> { '\'', '.', '+', ':', '>', '<', '-', '(', ')' };
        private const Int32 _churros = 13312;

        #region Operators
        public Char Not
        {
            get { return _operators["Not"]; }
            set
            {
                if (!_operators.ContainsValue(value) && Convert.ToInt32(value) < _churros)
                    _operators["Not"] = value;
            }
        }

        public Char And
        {
            get { return _operators["And"]; }
            set
            {
                if (!_operators.ContainsValue(value) && Convert.ToInt32(value) < _churros)
                    _operators["And"] = value;
            }
        }

        public Char Or
        {
            get { return _operators["Or"]; }
            set
            {
                if (!_operators.ContainsValue(value) && Convert.ToInt32(value) < _churros)
                    _operators["Or"] = value;
            }
        }

        public Char Xor
        {
            get { return _operators["Xor"]; }
            set
            {
                if (!_operators.ContainsValue(value) && Convert.ToInt32(value) < _churros)
                    _operators["Xor"] = value;
            }
        }

        public Char IfThen
        {
            get { return _operators["IfThen"]; }
            set
            {
                if (!_operators.ContainsValue(value) && Convert.ToInt32(value) < _churros)
                    _operators["IfThen"] = value;
            }
        }

        public Char ThenIf
        {
            get { return _operators["ThenIf"]; }
            set
            {
                if (!_operators.ContainsValue(value) && Convert.ToInt32(value) < _churros)
                    _operators["ThenIf"] = value;
            }
        }

        public Char IfAndOnlyIf
        {
            get { return _operators["IfAndOnlyIf"]; }
            set
            {
                if (!_operators.ContainsValue(value) && Convert.ToInt32(value) < _churros)
                    _operators["IfAndOnlyIf"] = value;
            }
        }

        public Char OpeningBracket
        {
            get { return _operators["OpeningBracket"]; }
            set
            {
                if (!_operators.ContainsValue(value) && Convert.ToInt32(value) < _churros)
                    _operators["OpeningBracket"] = value;
            }
        }

        public Char ClosingBracket
        {
            get { return _operators["ClosingBracket"]; }
            set
            {
                if (!_operators.ContainsValue(value) && Convert.ToInt32(value) < _churros)
                    _operators["ClosingBracket"] = value;
            }
        }
        #endregion

        #region Truth table data properties
        public String Formula
        {
            get { return _formula; }
            private set
            {
                _formula = value;

                Arguments = new List<Char>();
                foreach (Char item in _formula)
                    if (Char.IsLetter(item) && !Arguments.Contains(item))
                        Arguments.Add(item);

                CalculateArguments();
            }
        }

        public List<Char> Arguments { get; private set; }

        public Boolean[,] ArgumentsValues { get; private set; }

        public List<String> Expressions { get; private set; }

        public List<Boolean[]> ExpressionsValues { get; private set; }
        #endregion

        public TruthTable(String formula, IEnumerable<Char> characters = null)
        {
            IEnumerator<Char> charactersEnumerator = (characters ?? _defaultOperators).GetEnumerator();
            String[] operatorsKeys = new String[9] { "Not", "And", "Or", "Xor", "IfThen", "ThenIf", "IfAndOnlyIf", "OpeningBracket", "ClosingBracket" };
            Int32 index = 0;

            while (charactersEnumerator.MoveNext() && index < 9)
                _operators.Add(operatorsKeys[index++], charactersEnumerator.Current);

            while (index < 9)
                _operators.Add(operatorsKeys[index], _defaultOperators[index++]);

            Formula = formula;

            Expressions = new List<String>();
            ExpressionsValues = new List<Boolean[]>();
            _operators = new Dictionary<String, Char>();
        }

        public Boolean ValidateFormula(String formula = null)
        {
            String sentence = formula ?? Formula;

            List<Boolean> charactersStatus = new List<Boolean>();
            Boolean isDisallowedCharacter = true, isValid = true;

            foreach (Char character in sentence)
            {
                if (Char.IsLetter(character))
                    isDisallowedCharacter = false;
                else
                    for (Int32 index = 0; index < EnumerateOperators().Length; index++)
                        if (character == EnumerateOperators()[index])
                        {
                            isDisallowedCharacter = false;
                            break;
                        }

                charactersStatus.Add(isDisallowedCharacter);
                isDisallowedCharacter = true;
            }

            foreach (Boolean status in charactersStatus)
                if (status)
                    isValid = false;

            if (isValid && (sentence.Contains(OpeningBracket.ToString()) || sentence.Contains(ClosingBracket.ToString())))
            {
                Int32[] parenthesisCount = new Int32[2];
                foreach (Char character in sentence)
                    if (character == OpeningBracket)
                        parenthesisCount[0]++;
                    else if (character == ClosingBracket)
                        parenthesisCount[1]++;

                isValid = parenthesisCount[0] == parenthesisCount[1];
            }

            if (isValid)
                for (Int32 index = 0; index < sentence.Length; index++)
                    if (Char.IsLetter(sentence[index]))
                    {
                        if (isValid && index != 0)
                            isValid = !Char.IsLetter(sentence[index - 1]);
                        if (isValid && index != sentence.Length - 1)
                            isValid = !Char.IsLetter(sentence[index + 1]);
                    }
                    else if (sentence[index] == Not)
                    {
                        if (isValid && index != 0)
                            isValid = Char.IsLetter(sentence[index - 1]) || sentence[index - 1] == OpeningBracket || sentence[index - 1] == ClosingBracket;
                        else if (isValid)
                            isValid = false;
                        if (isValid && index != sentence.Length - 1)
                            isValid = !Char.IsLetter(sentence[index + 1]);
                    }
                    else if (sentence[index] == OpeningBracket)
                    {
                        if (isValid && index != sentence.Length - 1)
                            isValid = Char.IsLetter(sentence[index + 1]) || sentence[index + 1] == OpeningBracket;
                        else if (isValid)
                            isValid = false;
                        if (isValid && index != 0)
                            isValid = !Char.IsLetter(sentence[index - 1]);
                    }
                    else if (sentence[index] == ClosingBracket)
                    {
                        if (isValid && index != 0)
                            isValid = Char.IsLetter(sentence[index - 1]) || sentence[index - 1] == ClosingBracket || sentence[index - 1] == Not;
                        else if (isValid)
                            isValid = false;
                        if (isValid && index != sentence.Length - 1)
                            isValid = !Char.IsLetter(sentence[index + 1]);
                    }
                    else
                    {
                        isValid = index != 0 && index != sentence.Length - 1;
                        if (isValid)
                            foreach (Char item in EnumerateOperators())
                                if (item != Not && item != OpeningBracket && item != ClosingBracket)
                                {
                                    isValid = sentence[index - 1] != item && sentence[index + 1] != item;
                                    if (!isValid)
                                        break;
                                }
                    }

            return isValid;
        }

        public Char[] EnumerateOperators()
        {
            return new Char[] { Not, And, Or, Xor, IfThen, ThenIf, IfAndOnlyIf, OpeningBracket, ClosingBracket };
        }

        public void CalculateExpressions()
        {
            if (!ValidateFormula()) throw new InvalidFormulaException();
            else
            {
                String expression = "", pseudoformula = "";
                Dictionary<String, String> snips = SnipFormula();

                Int32 count = 0, counter = 0, depth = 0;
                foreach (Char item in Formula)
                    if (item == OpeningBracket)
                    {
                        count++;
                        counter++;
                        if (counter > depth)
                            depth = counter;
                    }
                    else if (item == ClosingBracket)
                        counter--;

                Int32[] actualID = new Int32[depth + 1], fatherID = new Int32[depth + 1];

                actualID[0] = 0;
                for (Int32 index = 1; index <= depth; index++)
                    actualID[index] = -1;
                for (Int32 index = 0; index <= depth; index++)
                    fatherID[index] = -1;

                while (snips.Count != 0)
                {
                    while (!snips.ContainsKey(ConvertKey(actualID)))
                    {
                        actualID[counter--] = -1;
                        fatherID[counter] = -1;
                    }

                    while (snips[ConvertKey(actualID)].Contains(OpeningBracket.ToString()))
                    {
                        fatherID[counter] = actualID[counter];
                        actualID[++counter] = 0;
                    }

                    pseudoformula = snips[ConvertKey(actualID)];
                    while (pseudoformula.Length != 1)
                    {
                        if (pseudoformula.IndexOf(Not) > 0)
                            expression = pseudoformula.Substring(pseudoformula.IndexOf(Not) - 1, 2);
                        else if (pseudoformula.IndexOf(And) > 0)
                            expression = pseudoformula.Substring(pseudoformula.IndexOf(And) - 1, 3);
                        else if (pseudoformula.IndexOf(Or) > 0)
                            expression = pseudoformula.Substring(pseudoformula.IndexOf(Or) - 1, 3);
                        else if (pseudoformula.IndexOf(Xor) > 0)
                            expression = pseudoformula.Substring(pseudoformula.IndexOf(Xor) - 1, 3);
                        else if (pseudoformula.IndexOf(IfThen) > 0)
                            expression = pseudoformula.Substring(pseudoformula.IndexOf(IfThen) - 1, 3);
                        else if (pseudoformula.IndexOf(ThenIf) > 0)
                            expression = pseudoformula.Substring(pseudoformula.IndexOf(ThenIf) - 1, 3);
                        else if (pseudoformula.IndexOf(IfAndOnlyIf) > 0)
                            expression = pseudoformula.Substring(pseudoformula.IndexOf(IfAndOnlyIf) - 1, 3);

                        pseudoformula = ReplaceFirst(pseudoformula, expression, Convert.ToChar(ExpressionsValues.Count + _churros).ToString());
                        CalculateExpression(expression, pseudoformula.Length == 1);
                    }

                    if (snips.Count > 1)
                    {
                        if (HasChurros(snips[ConvertKey(actualID)]))
                            for (Int32 index = 0; index < Expressions.Count; index++)
                                if (snips[ConvertKey(actualID)].Contains(Convert.ToChar(_churros + index).ToString()))
                                    snips[ConvertKey(actualID)] = snips[ConvertKey(actualID)].Replace(Convert.ToChar(_churros + index).ToString(), Expressions[index]);
                        snips[ConvertKey(fatherID)] = ReplaceFirst(snips[ConvertKey(fatherID)], OpeningBracket + snips[ConvertKey(actualID)] + ClosingBracket, pseudoformula);
                    }

                    snips.Remove(ConvertKey(actualID));

                    if (snips.Count > 0)
                    {
                        actualID[counter]++;
                    }
                }
            }
        }

        public override String ToString()
        {
            StringBuilder table = new StringBuilder();

            for (Int32 index = 0; index < Arguments.Count + ExpressionsValues.Count; index++)
                if (index < Arguments.Count)
                    table.Append(Arguments[index] + " ");
                else
                    table.Append(Expressions[index - Arguments.Count] + " ");

            table.AppendLine();

            for (Int32 line = 0; line < Math.Pow(2, Arguments.Count); line++)
            {
                for (Int32 column = 0; column < Arguments.Count + ExpressionsValues.Count; column++)
                    if (column < Arguments.Count)
                        table.Append(Convert.ToInt32(ArgumentsValues[line, column]).ToString() + " ");
                    else
                    {
                        if (Expressions[column - Arguments.Count].Length % 2 == 0)
                            for (Int32 i = 0; i < Expressions[column - Arguments.Count].Length / 2 - 1; i++)
                                table.Append(" ");
                        else
                            for (Int32 i = 0; i < Expressions[column - Arguments.Count].Length / 2; i++)
                                table.Append(" ");

                        table.Append(Convert.ToInt32(ExpressionsValues[column - Arguments.Count][line]).ToString() + " ");
                        for (Int32 i = 0; i < Expressions[column - Arguments.Count].Length / 2; i++)
                            table.Append(" ");
                    }

                table.AppendLine();
            }

            return table.ToString();
        }

        #region Private helper methods
        private void CalculateArguments()
        {
            try
            {
                ArgumentsValues = new Boolean[(Int32)Math.Pow(2, Arguments.Count), Arguments.Count];
            }
            catch (OutOfMemoryException) { throw new TooMuchArgumentsInTruthTableException(Arguments.Count); }

            for (Int32 line = 0; line < Math.Pow(2, Arguments.Count); line++)
            {
                Int32 calculableLine = line;
                for (Int32 column = 0; column < Arguments.Count; column++)
                {
                    ArgumentsValues[line, column] = calculableLine >= Math.Pow(2, (Arguments.Count - 1) - column);
                    if (ArgumentsValues[line, column])
                        calculableLine -= (Int32)Math.Pow(2, (Arguments.Count - 1) - column);
                }
            }
        }

        private void CalculateExpression(String expression, Boolean hasParenthesis)
        {
            Boolean[] expressionValues = new Boolean[(Int32)Math.Pow(2, Arguments.Count)];
            Char expressionOperator = expression[1];
            Boolean value1 = true, value2 = true;

            for (Int32 line = 0; line < expressionValues.Length; line++)
            {
                if (expression[0] >= _churros)
                    value1 = ExpressionsValues[expression[0] - _churros][line];
                else
                    value1 = ArgumentsValues[line, Arguments.IndexOf(expression[0])];

                if (expression.Length == 3)
                    if (expression[2] >= _churros)
                        value2 = ExpressionsValues[expression[2] - _churros][line];
                    else
                        value2 = ArgumentsValues[line, Arguments.IndexOf(expression[2])];

                if (expressionOperator == Not)
                    expressionValues[line] = !value1;
                else if (expressionOperator == And)
                    expressionValues[line] = value1 && value2;
                else if (expressionOperator == Or)
                    expressionValues[line] = value1 || value2;
                else if (expressionOperator == Xor)
                    expressionValues[line] = value1 ? !value2 : value2;
                else if (expressionOperator == IfThen)
                    expressionValues[line] = value1 ? value2 : true;
                else if (expressionOperator == ThenIf)
                    expressionValues[line] = value2 ? value1 : true;
                else if (expressionOperator == IfAndOnlyIf)
                    expressionValues[line] = value1 == value2;
            }

            String churros = "";
            while (HasChurros(expression))
            {
                foreach (Char item in expression)
                    if (item >= _churros)
                        churros = ReplaceFirst(expression, item.ToString(), Expressions[item - _churros]);
                expression = churros;
            }

            if (hasParenthesis)
                expression = OpeningBracket + expression + ClosingBracket;

            Expressions.Add(expression);
            ExpressionsValues.Add(expressionValues);
        }

        private Dictionary<String, String> SnipFormula()
        {
            Int32 counter = 0, count = 0, depth = 0;
            foreach (Char item in Formula)
                if (item == OpeningBracket)
                {
                    count++;
                    counter++;
                    if (counter > depth)
                        depth = counter;
                }
                else if (item == ClosingBracket)
                    counter--;

            Int32[] treecode = new Int32[depth + 1];
            if (depth > 0)
                for (Int32 index = 1; index <= depth; index++)
                    treecode[index] = -1;
            treecode[0] = 0;

            List<String> pseudoformulas = new List<String>(count + 1);
            List<Int32> snipStart = new List<Int32>(count), snipEnd = new List<Int32>(count);
            List<Int32[]> treecodes = new List<Int32[]>(depth);
            pseudoformulas.Add(Formula);
            treecodes.Add(treecode);

            treecode = new Int32[depth + 1];
            treecodes[treecodes.Count - 1].CopyTo(treecode, 0);

            while (snipStart.Count != count)
                for (Int32 index = 0; index < Formula.Length; index++)
                    if (Formula[index] == OpeningBracket)
                    {
                        snipStart.Add(index + 1);
                        snipEnd.Add(-1);
                        treecode[++counter]++;
                        while (ContainsCode(treecodes, treecode))
                            treecode[counter]++;
                        treecodes.Add(treecode);
                        treecode = new Int32[depth + 1];
                        treecodes[treecodes.Count - 1].CopyTo(treecode, 0);
                    }
                    else if (Formula[index] == ClosingBracket)
                    {
                        for (Int32 position = snipStart.Count - 1; position > -1; position--)
                            if (snipEnd[position] == -1)
                            {
                                snipEnd[position] = index;
                                break;
                            }
                        treecode[counter--] = -1;
                    }

            for (Int32 index = 0; index < snipStart.Count; index++)
                pseudoformulas.Add(Formula.Substring(snipStart[index], snipEnd[index] - snipStart[index]));

            return OrganizeSnips(treecodes, pseudoformulas);
        }

        private Dictionary<String, String> OrganizeSnips(List<Int32[]> codes, List<String> texts)
        {
            Dictionary<String, String> snips = null;

            if (codes.Count == texts.Count)
            {
                snips = new Dictionary<String, String>();
                for (Int32 index = 0; index < codes.Count; index++)
                    snips.Add(ConvertKey(codes[index]), texts[index]);
            }

            return snips;
        }

        private Boolean HasChurros(String expression)
        {
            Boolean hasChurros = false;

            foreach (Char item in expression)
                if (!hasChurros)
                    hasChurros = Convert.ToInt32(item) >= _churros;
                else
                    break;

            return hasChurros;
        }

        private Boolean ContainsCode(List<Int32[]> list, Int32[] code)
        {
            Boolean contains = false;

            for (Int32 listIndex = 0; listIndex < list.Count; listIndex++)
                if (!contains)
                    for (Int32 codeIndex = 0; codeIndex < code.Length; codeIndex++)
                        if (contains || codeIndex == 0)
                            contains = list[listIndex][codeIndex] == code[codeIndex];
                        else
                            break;

            return contains;
        }

        private String ConvertKey(Int32[] key)
        {
            StringBuilder newKey = new StringBuilder();

            for (Int32 index = 0; index < key.Length; index++)
            {
                newKey.Append(key[index].ToString());
                if (index != key.Length - 1)
                    newKey.Append('.');
            }

            return newKey.ToString();
        }

        private String ReplaceFirst(String text, String oldValue, String newValue)
        {
            return text.Substring(0, text.IndexOf(oldValue)) + newValue + text.Substring(text.IndexOf(oldValue) + oldValue.Length);
        }
        #endregion
    }
}