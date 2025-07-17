using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kidynecat.DataComputedFieldEvaluator
{
	public class ComputedFieldFunction
	{
		public string Name { get; set; }

		public object Function { get; set; }

		public string Description { get; set; }

		public List<ComputedFieldFunctionParameter> Parameters { get; set; } = new List<ComputedFieldFunctionParameter>();

		public Type ReturnValueType { get; set; }

		public string NameMapping { get; set; } = null;

		public string ReturnValueTypeName
		{ 
			get
			{
				if (ReturnValueType == null)
				{
					return string.Empty;
				}

				if (ReturnValueType.IsGenericType && ReturnValueType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					return Nullable.GetUnderlyingType(ReturnValueType).Name;
				}

				return ReturnValueType.Name;
			}
		}
	}

	public class ComputedFieldFunctionParameter
	{
		public string Name { get; set; }

		public Type Type { get; set; }

		public string Description { get; set; }

		public string TypeName
		{
			get
			{
				if (Type == null)
				{
					return string.Empty;
				}

				if (Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					return Nullable.GetUnderlyingType(Type).Name;
				}

				return Type.Name;
			}
		}
	}
}
