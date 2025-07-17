using NReco.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kidynecat.DataComputedFieldEvaluator
{
	public class DataComputedFieldEvaluator : IComputedFieldEvaluator
	{
		private LambdaParser _lambdaParser;
		private IDictionary<string, object> _fieldToDefaultValue;
		private IDictionary<string, Type> _fieldToType;
		private IDictionary<string, object> _systemDefinedContext;

		public DataComputedFieldEvaluator()
		{
			_fieldToDefaultValue = new Dictionary<string, object>();
			_fieldToType = new Dictionary<string, Type>();
			_lambdaParser = new LambdaParser();
			_lambdaParser.AllowSingleEqualSign = true;
			InitSystemFunctions();
		}

		/// <summary>
		/// Initialize fields context by field type dictionary and default values.
		/// </summary>
		/// <param name="fieldToType"></param>
		/// <param name="fieldToDefaultValue"></param>
		/// <exception cref="Exception"></exception>
		public void InitializeFieldsContext(IDictionary<string, Type> fieldToType, IDictionary<string, object> fieldToDefaultValue)
		{
			var duplicatesFieldName = SystemDefinedFunctions.Select(sf => sf.Name).Intersect(fieldToDefaultValue.Keys);
			if (duplicatesFieldName.Any())
			{
				throw new Exception($"Initialize error, Can not use the same name as any system context field: {string.Join(",", duplicatesFieldName)}");
			}

			var mismatchedFields = GetMismatchedFields(fieldToType, fieldToDefaultValue);

			if (mismatchedFields.Any())
			{
				throw new Exception($"Initialize error, The field value does not match the field type: {string.Join(",", mismatchedFields)}");
			}

			_fieldToDefaultValue = fieldToDefaultValue;
			_fieldToType = fieldToType;
		}

		/// <summary>
		/// Try parsing expression, return if it was successful, along with any error messages.
		/// </summary>
		/// <param name="expression">supports arithmetic operations (+, -, *, /, %), comparisons (==, !=, >, <, >=, <=), conditionals including (ternary) operator ( ? : )</param>
		/// <param name="fieldsUsed"></param>
		/// <returns></returns>
		public (bool, string) TryParsingExpression(string expression, out List<string> fieldsUsed)
		{
			return TryParsingExpression(expression, null, out fieldsUsed);
		}

		/// <summary>
		/// Try parsing expression with new field values, return if it was successful, along with any error messages.
		/// </summary>
		/// <param name="expression">supports arithmetic operations (+, -, *, /, %), comparisons (==, !=, >, <, >=, <=), conditionals including (ternary) operator ( ? : )</param>
		/// <param name="fieldToValue"></param>
		/// <param name="fieldsUsed"></param>
		/// <returns></returns>
		public (bool, string) TryParsingExpression(string expression, IDictionary<string, object> fieldToValue, out List<string> fieldsUsed)
		{
			fieldsUsed = new List<string>();
			string errorMessage = string.Empty;
			try
			{
				object resultValue = ParsingByFieldsContext(expression, fieldToValue, out fieldsUsed);
				return (true, errorMessage);
			}
			catch(Exception ex)
			{
				errorMessage = GetInnermostExceptionMessage(ex);
				return (false, errorMessage);
			}
		}

		/// <summary>
		/// Evaluate the expression by given field values.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="fieldToValue"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public object EvaluateExpression(string expression, IDictionary<string, object> fieldToValue = null)
		{
			try
			{
				object resultValue = ParsingByFieldsContext(expression, fieldToValue, out _);
				return resultValue;
			}
			catch (Exception ex)
			{
				var message = GetInnermostExceptionMessage(ex);
				throw new Exception($"Pasring expression error: {message}", ex);
			}
		}

		public (bool, string) TryEvaluatingExpression(string expression, out List<string> fieldsUsed, out object returnValue)
		{
			fieldsUsed = new List<string>();
			returnValue = null;
			string errorMessage = string.Empty;
			try
			{
				returnValue = ParsingByFieldsContext(expression, null, out fieldsUsed);
				return (true, errorMessage);
			}
			catch (Exception ex)
			{
				errorMessage = GetInnermostExceptionMessage(ex);
				return (false, errorMessage);
			}
		}

		public IDictionary<string, object> FieldDefaultValues { get { return _fieldToDefaultValue; } }

		public IDictionary<string, Type> FieldTypes { get { return _fieldToType; } }

		public IEnumerable<ComputedFieldFunction> SystemDefinedFunctions { get { return SystemDefinedContextMetadata.SystemDefinedFunctions; } }

		private void InitSystemFunctions()
		{
			_systemDefinedContext = new Dictionary<string, object>();
			foreach( var sysFunction in SystemDefinedContextMetadata.SystemDefinedFunctions)
			{
				_systemDefinedContext[sysFunction.Name] = sysFunction.Function;
			}

			_systemDefinedContext[SystemDefinedContextMetadata.SystemDefinedClassContextName] = new SystemDefinedContextMetadata.SystemDefinedClass();
		}

		private List<string> GetMismatchedFields(IDictionary<string, Type> fieldToType, IDictionary<string, object> fieldToDefaultValue)
		{
			List<string> mismatchedFields = new List<string>();

			if (fieldToDefaultValue == null || fieldToDefaultValue.Count() == 0)
			{
				return mismatchedFields;
			}

			foreach (var kvp in fieldToDefaultValue)
			{
				string key = kvp.Key;
				object value = kvp.Value;

				if (fieldToType.TryGetValue(key, out Type expectedType))
				{


					if (value != null && !expectedType.IsAssignableFrom(value.GetType())) //Consider the case of nullable types
					{
						if (expectedType == typeof(decimal?) && (value is int || value is long || value is float || value is double || value is decimal))
						{
							continue;
						}

						if (!mismatchedFields.Contains(key))
						{
							mismatchedFields.Add(key);
						}
					}
				}
				else
				{
					if (!mismatchedFields.Contains(key))
					{
						mismatchedFields.Add(key);
					}
				}
			}

			return mismatchedFields;
		}

		private object ParsingByFieldsContext(string expression, IDictionary<string, object> fieldToValue, out List<string> fieldsUsed)
		{
			IDictionary<string, object> tmpContextDictionary;
			if (fieldToValue != null)
			{
				var mismatchedFields = GetMismatchedFields(_fieldToType, fieldToValue);
				if (mismatchedFields.Any())
				{
					throw new Exception($"The field value does not match the field type: {string.Join(",", mismatchedFields)}");
				}
				tmpContextDictionary = fieldToValue;
			}
			else
			{
				tmpContextDictionary = _fieldToDefaultValue;
			}

			var context = _systemDefinedContext.Union(tmpContextDictionary).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			var exp = PrepareExpression(expression, context, out fieldsUsed);
			var resultValue = PasreSingleValue(exp, context);
			return resultValue;
		}

		private Dictionary<string, object> MergeValueDictionaries(IDictionary<string, object> fieldToDefaultValue, IDictionary<string, object> fieldToValue)
		{
			Dictionary<string, object> mergedDictionary = new Dictionary<string, object>(fieldToDefaultValue);

			if (fieldToValue == null)
			{
				return mergedDictionary;
			}

			foreach (var kvp in fieldToValue)
			{
				mergedDictionary[kvp.Key] = kvp.Value;
			}

			return mergedDictionary;
		}

		private string PrepareExpression(string expression, IDictionary<string, object> dictionary, out List<string> fieldsUsed)
		{
			fieldsUsed = new List<string>();
			Regex regex = new Regex(@"(?<!\[)\[(?!\[)(.*?)(?<!\])\](?!\])");
			MatchCollection matches = regex.Matches(expression);

			foreach (Match match in matches)
			{
				string key = match.Groups[1].Value;

				if (dictionary.ContainsKey(key))
				{
					if (!fieldsUsed.Contains(key))
					{
						fieldsUsed.Add(key);
					}
					expression = expression.Replace(match.Value, key);
				}
				else
				{
					throw new Exception($"Field \"{key}\" has not been initialized");
				}
			}

			// Replace double brackets with single brackets
			expression = expression.Replace("[[", "[").Replace("]]", "]");

			// Replace system function names by the mapping
			expression = ReplaceFunctionNames(expression, SystemDefinedContextMetadata.SystemDefinedFunctionMappings);

			return expression;
		}

		private string ReplaceFunctionNames(string expression, Dictionary<string, string> mapping)
		{
			string pattern = @"(?<=\b)(" + string.Join("|", mapping.Keys) + @")(?=\s*\()(?=(?:[^""]*""[^""]*"")*[^""]*$)";
			string replaced = Regex.Replace(expression, pattern, m => mapping[m.Groups[1].Value], RegexOptions.IgnoreCase);
			return replaced;
		}


		private object PasreSingleValue(string expression, IDictionary<string, object> context)
		{
			return _lambdaParser.Eval(expression, context);
		}

		private string GetInnermostExceptionMessage(Exception ex, int maxDepth = 10, int currentDepth = 0)
		{
			if (currentDepth >= maxDepth || ex.InnerException == null)
			{
				return ex.Message;
			}

			return GetInnermostExceptionMessage(ex.InnerException, maxDepth, currentDepth + 1);
		}
	}
}
