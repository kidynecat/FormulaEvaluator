using System;
using System.Collections.Generic;

namespace Kidynecat.DataComputedFieldEvaluator
{
	public interface IComputedFieldEvaluator
	{
		void InitializeFieldsContext(IDictionary<string, Type> fieldToType, IDictionary<string, object> fieldToDefaultValue);

		(bool, string) TryParsingExpression(string expression, out List<string> fieldsUsed);

		object EvaluateExpression(string expression, IDictionary<string, object> fieldToValue);

		(bool, string) TryEvaluatingExpression(string expression, out List<string> fieldsUsed, out object returnValue);
	}
}
