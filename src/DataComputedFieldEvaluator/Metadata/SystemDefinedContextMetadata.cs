using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kidynecat.DataComputedFieldEvaluator
{
	public class SystemDefinedContextMetadata
	{
		public const string SystemDefinedClassContextName = "SYS";

		public class SystemDefinedClass
		{
			public string Concatenate(params string[] texts)
			{
				return string.Join("", texts);
			}

			public decimal Sum(params decimal[] decimals)
			{
				return decimals.Sum();
			}

			public decimal Avg(params decimal[] decimals)
			{
				return decimals.Average();
			}

			public decimal Max(params decimal[] decimals)
			{
				return decimals.Max();
			}

			public decimal Min(params decimal[] decimals)
			{
				return decimals.Min();
			}

			public object SwitchCase(object defaultValue, params object[] cases)
			{
				if (cases.Length % 2 != 0 || cases.Length < 2)
				{
					throw new ArgumentException("Invalid number of cases.");
				}

				for (int i = 0; i < cases.Length; i += 2)
				{
					if (!(cases[i] is bool))
					{
						throw new ArgumentException("cases must be of type bool.");
					}

					bool condition = (bool)cases[i];
					
					if (condition)
					{
						object value = cases[i + 1];
						return value;
					}
				}

				return defaultValue;
			}

			public object SwitchValue(object targetValue, object defaultValue, params object[] cases)
			{
				if (cases.Length % 2 != 0 || cases.Length < 2)
				{
					throw new ArgumentException("Invalid number of cases.");
				}

				for (int i = 0; i < cases.Length; i += 2)
				{
					object conditionValue = cases[i];
					object value = cases[i + 1];

					if (targetValue.Equals(conditionValue))
					{
						return value;
					}
				}

				return defaultValue;
			}

			public string GetFromJson(string json, string key)
			{
				if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(key))
				{
					//throw new ArgumentException("JSON or key cannot be empty.");
					return string.Empty;
				}

				try
				{
					JObject jsonObject = JObject.Parse(json);

					JToken valueToken;
					if (jsonObject.TryGetValue(key, out valueToken))
					{
						return valueToken.ToString();
					}
				}
				catch (Exception ex)
				{
					//throw new ArgumentException($"Error parsing JSON.");
					return string.Empty;
				}

				return string.Empty;
			}
		}

		public static List<ComputedFieldFunction> SystemDefinedFunctions = new List<ComputedFieldFunction>() { 
			// Logical functions
			new ComputedFieldFunction() {
				Name = "IIF",
				Function =  (Func<bool, object, object, object>)((condition, trueValue, falseValue) => 
				condition ? trueValue : falseValue),
				Description = "Takes three arguments as input. Returns the second argument if the first evaluates to TRUE. Otherwise returns the third argument. The \"? :\" operator can be used instead of IIF().",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "condition",
						Type = typeof(bool),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "trueValue",
						Type = typeof(object),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "falseValue",
						Type = typeof(object),
						Description ="",
					},
				},
				ReturnValueType = typeof(bool),
			},
			new ComputedFieldFunction() {
				Name = "AND",
				Function =  (Func<bool, bool, bool>)((condition1, condition2) => condition1 && condition2),
				Description = "Returns TRUE if two arguments are TRUE; returns FALSE if any argument is FALSE. The \"&&\" operator can be used instead of AND().",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "condition1",
						Type = typeof(bool),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "condition2",
						Type = typeof(bool),
						Description ="",
					},
				},
				ReturnValueType = typeof(bool),
			},
			new ComputedFieldFunction() {
				Name = "OR",
				Function =  (Func<bool, bool, bool>)((condition1, condition2) => condition1 || condition2),
				Description = "Returns TRUE if any argument is TRUE. The \"||\" operator can be used instead of OR().",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "condition1",
						Type = typeof(bool),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "condition2",
						Type = typeof(bool),
						Description ="",
					},
				},
				ReturnValueType = typeof(bool),
			},
			new ComputedFieldFunction() {
				Name = "NOT",
				Function =  (Func<bool, bool>)((condition) => !condition),
				Description = "Reverses the value of its argument. The \"!\" operator can be used instead of NOT().",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "condition",
						Type = typeof(bool),
						Description ="",
					}
				},
				ReturnValueType = typeof(bool),
			},
			new ComputedFieldFunction() {
				Name = "SWITCHCASE",
				Function = null,
				Description = "Evaluates an array of cases and returns a corresponding value if a true case is found, otherwise returns the default value. The casesAndValues array must have an even number of elements and contain alternating boolean conditions and corresponding values.",
				NameMapping = SystemDefinedClassContextName + ".SwitchCase",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "defaultValue",
						Type = typeof(object),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "casesAndValues",
						Type = typeof(object[]),
						Description ="",
					}
				},
				ReturnValueType = typeof(object),
			},
			new ComputedFieldFunction() {
				Name = "SWITCHVALUE",
				Function = null,
				Description = "Switches the target value with a corresponding value from the provided cases. If the target value matches any case, the corresponding value is returned. Otherwise, the default value is returned. The casesAndValues array must have an even number of elements and alternate between condition values and corresponding values.",
				NameMapping = SystemDefinedClassContextName + ".SwitchValue",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "targetValue",
						Type = typeof(object),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "defaultValue",
						Type = typeof(object),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "casesAndValues",
						Type = typeof(object[]),
						Description ="",
					}
				},
				ReturnValueType = typeof(object),
			},

			// Date functions
			new ComputedFieldFunction() {
				Name = "DATE",
				Function = (Func<int, int, int, DateTime>)((year, month, day) => new DateTime(year, month, day)),
				Description = "Creates a date value from three numeric values.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "year",
						Type = typeof(int),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "month",
						Type = typeof(int),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "day",
						Type = typeof(int),
						Description ="",
					},
				},
				ReturnValueType = typeof(DateTime),
			},
			new ComputedFieldFunction() {
				Name = "NOW",
				Function =  (Func<DateTime>)(() =>DateTime.Now),
				Description = "Returns today's date and time (in local server time).",
				ReturnValueType = typeof(DateTime),
			},
			// PAT-4142 - Remove UTCNOW and leave NOW as the only function to return the current date/time
			// new ComputedFieldFunction() {
			// 	Name = "UTCNOW",
			// 	Function =  (Func<DateTime>)(() =>DateTime.UtcNow),
			// 	Description = "Returns today's date and time (in utc time).",
			// 	ReturnValueType = typeof(DateTime),
			// },
			new ComputedFieldFunction() {
				Name = "DATECUSTOMFORMAT",
				Function = (Func<object, string, string>)((date, format) =>
				{
					if (date == null)
					{
						return null;
					}

					if (!(date is DateTime))
					{
						throw new ArgumentException("Invalid date type. Expected DateTime.");
					}

					DateTime dateValue = (DateTime)date;
					return dateValue.ToString(format);
				}),
				Description = "Modifies a date value to be output in a specified format.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "datetime",
						Type = typeof(DateTime),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "format",
						Type = typeof(string),
						Description ="",
					},
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "DAYOFWEEK",
				Function = (Func<object, object>)((date) =>
				{
					if (date == null)
					{
						return null;
					}

					if (!(date is DateTime))
					{
						throw new ArgumentException("Invalid date type. Expected DateTime.");
					}

					DateTime dateValue = (DateTime)date;
					return (int)dateValue.DayOfWeek;
				}),
				Description = "Returns the number of the weekday of a date, where 0 = Sunday, 1 = Monday, 2 = Tuesday … 6 = Saturday.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "datetime",
						Type = typeof(DateTime),
						Description ="",
					},
				},
				ReturnValueType = typeof(int),
			},
			new ComputedFieldFunction() {
				Name = "DATEADD",
				Function = (Func<string , int, object, object>)((datePart, number, date) => 
				{
					if (datePart == null || date == null)
					{
						return null;
					}

					if (!(date is DateTime))
					{
						throw new ArgumentException("Invalid date type. Expected DateTime.");
					}

					DateTime dateValue = (DateTime)date;

					DateTime resultDate;

					switch (datePart.ToLower())
					{
						case "year":
							resultDate = dateValue.AddYears(number);
							break;
						case "quarter":
							resultDate = dateValue.AddMonths(number * 3);
							break;
						case "month":
							resultDate = dateValue.AddMonths(number);
							break;
						case "day":
							resultDate = dateValue.AddDays(number);
							break;
						case "hour":
							resultDate = dateValue.AddHours(number);
							break;
						case "minute":
							resultDate = dateValue.AddMinutes(number);
							break;
						case "second":
							resultDate = dateValue.AddSeconds(number);
							break;
						default:
							throw new ArgumentException("Invalid datePart value.");
					}

					return resultDate;
				}),
				Description = "Adds a specified interval of time to a given date, simulating the behavior of the SQL Server DATEADD function. The part of the date to add (e.g., \"year\", \"quarter\", \"month\", \"day\", \"hour\", \"minute\", \"second\".)",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "datePart",
						Type = typeof(string),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "interval",
						Type = typeof(int),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "baseDate",
						Type = typeof(DateTime),
						Description ="",
					},
				},
				ReturnValueType = typeof(DateTime?),
			},
			// String functions
			new ComputedFieldFunction() {
				Name = "CONCATENATE",
				Function =  null,
				NameMapping = SystemDefinedClassContextName + ".Concatenate",
				Description = "Joins several text strings into one text string.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "stringArray",
						Type = typeof(string[]),
						Description ="",
					},
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "SUBSTRING",
				Function =  (Func<string, int, int, string>)((stringValue, start, length) => string.IsNullOrEmpty(stringValue) ? null : stringValue.Substring(start - 1, length)),
				Description = "Gets a sub-string of a specified length, starting at a specified point in the string.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "stringValue",
						Type = typeof(string),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "start",
						Type = typeof(int),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "length",
						Type = typeof(int),
						Description ="",
					},
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "TRIM",
				Function = (Func<string, string>)((stringValue) => string.IsNullOrEmpty(stringValue) ? null : stringValue.Trim()),
				Description = "Returns string that remains after all white-space characters are removed from the start and end of the current string.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "stringValue",
						Type = typeof(string),
						Description ="",
					},
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "LEN",
				Function = (Func<string, int>)((stringValue) => string.IsNullOrEmpty(stringValue) ? 0 : stringValue.Length),
				Description = "Returns the number of characters in a text string.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "stringValue",
						Type = typeof(string),
						Description ="",
					},
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "REPLACE",
				Function = (Func<string, string, string, string>)((original, target, replacement) => string.IsNullOrEmpty(original) ? null : original.Replace(target, replacement)),
				Description = "Returns string that is equivalent to the current string except that all instances of oldValue are replaced with newValue.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "original",
						Type = typeof(string),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "target",
						Type = typeof(string),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "replacement",
						Type = typeof(string),
						Description ="",
					},
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "LEFT",
				Function = (Func<string, int, string>)((stringValue, length) =>
				{
					if (string.IsNullOrEmpty(stringValue))
					{
						return stringValue;
					}

					if (length <= 0)
					{
						return string.Empty;
					}

					int substringLength = Math.Min(length, stringValue.Length);
					return stringValue.Substring(0, substringLength);
				}),
				Description = "Returns the first character(s) of a text string.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "stringValue",
						Type = typeof(string),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "length",
						Type = typeof(int),
						Description ="",
					},
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "RIGHT",
				Function = (Func<string, int, string>)((stringValue, length) =>
				{
					if (string.IsNullOrEmpty(stringValue))
					{
						return stringValue;
					}

					if (length <= 0)
					{
						return string.Empty;
					}

					int startIndex = Math.Max(stringValue.Length - length, 0);
					return stringValue.Substring(startIndex);
				}),
				Description = "Returns the last character(s) of a text string.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "stringValue",
						Type = typeof(string),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "length",
						Type = typeof(int),
						Description ="",
					},
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "UPPER",
				Function = (Func<string, string>)(stringValue =>  string.IsNullOrEmpty(stringValue) ? null : stringValue.ToUpper()),
				Description = "Returns string converted to uppercase.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "stringValue",
						Type = typeof(string),
						Description ="",
					},
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "LOWER",
				Function = (Func<string, string>)(stringValue => string.IsNullOrEmpty(stringValue) ? null : stringValue.ToLower()),
				Description = "Returns string converted to lowercase.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "stringValue",
						Type = typeof(string),
						Description ="",
					},
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "NEWLINE",
				Function = (Func<string>)(() => Environment.NewLine),
				Description = "Begins a new line of text.",
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "GETFROMJSON",
				Function =  null,
				Description = "Returns value from JSON object .",
				NameMapping = SystemDefinedClassContextName + ".GetFromJson",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "json",
						Type = typeof(string),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "key",
						Type = typeof(string),
						Description ="",
					}
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "SPLIT",
				Function =  (Func<string, string, int, string>)((stringValue, separator, index) =>
				{
					if (string.IsNullOrEmpty(stringValue)) return string.Empty;
					var parts = stringValue.Split(new string [] { separator}, StringSplitOptions.None).Select(part => part.Trim()).ToArray();
					return index >= 0 && index < parts.Length ? parts[index] : string.Empty;
				}),
				Description = "Split a string based on a delimiter and return the content at the specified index.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "stringValue",
						Type = typeof(string),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "separator",
						Type = typeof(string),
						Description ="",
					},
					new ComputedFieldFunctionParameter()
					{
						Name = "index",
						Type = typeof(int),
						Description ="",
					}
				},
				ReturnValueType = typeof(string),
			},

			// Aggregate function
			new ComputedFieldFunction() {
				Name = "SUM",
				Function =  null,
				NameMapping = SystemDefinedClassContextName + ".Sum",
				Description = "Returns the sum of the values.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "decimalArray",
						Type = typeof(decimal[]),
						Description ="",
					},
				},
				ReturnValueType = typeof(decimal),
			},
			new ComputedFieldFunction() {
				Name = "AVG",
				Function =  null,
				NameMapping = SystemDefinedClassContextName + ".Avg",
				Description = "Returns the average of the values",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "decimalArray",
						Type = typeof(decimal[]),
						Description ="",
					},
				},
				ReturnValueType = typeof(decimal),
			},
			new ComputedFieldFunction() {
				Name = "MAX",
				Function =  null,
				NameMapping = SystemDefinedClassContextName + ".Max",
				Description = "Returns the maximum value",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "decimalArray",
						Type = typeof(decimal[]),
						Description ="",
					},
				},
				ReturnValueType = typeof(decimal),
			},
			new ComputedFieldFunction() {
				Name = "MIN",
				Function =  null,
				NameMapping = SystemDefinedClassContextName + ".Min",
				Description = "Returns the minimum value",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "decimalArray",
						Type = typeof(decimal[]),
						Description ="",
					},
				},
				ReturnValueType = typeof(decimal),
			},

			// Data type function
			new ComputedFieldFunction() {
				Name = "ISNULL",
				Function = (Func<object, bool>)((val) => val == null ? true : false ),
				Description = "Returns True if the argument is NULL. Otherwise returns False.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "value",
						Type = typeof(object),
						Description ="",
					},
				},
				ReturnValueType = typeof(bool),
			},
			new ComputedFieldFunction() {
				Name = "ISLOGICAL",
				Function = (Func<object, bool>)((val) => val is bool),
				Description = "Checks if a value is TRUE or FALSE.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "value",
						Type = typeof(object),
						Description ="",
					},
				},
				ReturnValueType = typeof(bool),
			},
			new ComputedFieldFunction() {
				Name = "ISNUMERIC",
				Function = (Func<object, bool>)((val) => val is int || val is float || val is double || val is decimal),
				Description = "Returns True it is of type int, float, double, or decimal. Otherwise returns False.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "value",
						Type = typeof(object),
						Description ="",
					},
				},
				ReturnValueType = typeof(bool),
			},
			new ComputedFieldFunction() {
				Name = "ISSTRING",
				Function = (Func<object, bool>)((val) => val is string),
				Description = "Returns True if it is string value. Otherwise returns False.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "value",
						Type = typeof(object),
						Description ="",
					},
				},
				ReturnValueType = typeof(bool),
			},
			new ComputedFieldFunction() {
				Name = "ISDATETIME",
				Function = (Func<object, bool>)((val) => val is DateTime),
				Description = "Returns True if it is datetime value. Otherwise returns False.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "value",
						Type = typeof(object),
						Description ="",
					},
				},
				ReturnValueType = typeof(bool),
			},
			new ComputedFieldFunction() {
				Name = "TOSTRING",
				Function = (Func<object, string>)((obj) => obj.ToString()),
				Description = "Try parsing the parameter into a String value.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "value",
						Type = typeof(object),
						Description ="",
					},
				},
				ReturnValueType = typeof(string),
			},
			new ComputedFieldFunction() {
				Name = "TODATETIME",
				Function = (Func<string, DateTime?>)((datetimeString) =>
				{
					if (DateTime.TryParse(datetimeString, out DateTime datetimeValue))
					{
						return datetimeValue;
					}
					else
					{
						return null;
					}
				}),
				Description = "Try parsing the parameter String into a DateTime value. If it fails, return null.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "datetimeString",
						Type = typeof(string),
						Description ="",
					},
				},
				ReturnValueType = typeof(DateTime?),
			},
			new ComputedFieldFunction() {
				Name = "TONUMERIC",
				Function = (Func<string, decimal?>)((datetimeString) =>
				{
					if (decimal.TryParse(datetimeString, out decimal numberValue))
					{
						return numberValue;
					}
					else
					{
						return null;
					}
				}),
				Description = "Try parsing the parameter String into a Decimal value. If it fails, return null.",
				Parameters = {
					new ComputedFieldFunctionParameter()
					{
						Name = "numberString",
						Type = typeof(string),
						Description ="",
					},
				},
				ReturnValueType = typeof(decimal?),
			},
		};

		public static Dictionary<string, string> SystemDefinedFunctionMappings
		{
			get 
			{
				return SystemDefinedFunctions.Where(sdf => !string.IsNullOrEmpty(sdf.NameMapping)).ToDictionary(sdf => sdf.Name, sdf => sdf.NameMapping);
			} 
		}

	}
}
