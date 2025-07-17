using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kidynecat.DataComputedFieldEvaluator
{
    public class DataComputedFieldEvaluatorTests
    {
        DataComputedFieldEvaluator dataComputedFieldEvaluator;

        public DataComputedFieldEvaluatorTests()
        {
            dataComputedFieldEvaluator = new DataComputedFieldEvaluator();
        }

        [Fact]
        public async Task SystemFunctionsTest()
        {
			// Test system function name in a string
			var str1 = dataComputedFieldEvaluator.EvaluateExpression("\"IIF\"");
			Assert.Equal("IIF", str1);

			// Test replacement of functions in the system defined class
			var concatenateResult1 = dataComputedFieldEvaluator.EvaluateExpression("CONCATENATE(\"abc\",\"def\")");
			Assert.Equal("abcdef", concatenateResult1);
			var concatenateResult2 = dataComputedFieldEvaluator.EvaluateExpression("CONCATENATE(\"CONCATENATE\",\"test\")");
			Assert.Equal("CONCATENATEtest", concatenateResult2);
			var concatenateResult3 = dataComputedFieldEvaluator.EvaluateExpression("CONCATENATE    (\"CONCATENATE()\",\"!@#$%^&*()_+\")");
			Assert.Equal("CONCATENATE()!@#$%^&*()_+", concatenateResult3);

			// Test system defined functions
			var iffResult = dataComputedFieldEvaluator.EvaluateExpression("IIF(1 = 1, 1 , 0)");
            Assert.Equal((decimal)1, iffResult);
			var andorResult = dataComputedFieldEvaluator.EvaluateExpression("OR(NOT(false), AND(true,false))");
			Assert.True((bool)andorResult);
			var basicResult1 = dataComputedFieldEvaluator.EvaluateExpression("true == !false");
			Assert.True((bool)basicResult1);

			// Test Switch
			var switchValue = dataComputedFieldEvaluator.EvaluateExpression("SWITCHCASE(2, false, 3, false, 4)");
			Assert.Equal((decimal)2, (decimal)switchValue);
			var switchValue2 = dataComputedFieldEvaluator.EvaluateExpression("SWITCHCASE(\"default result\", false, \"not matched\", true, \"matched\")");
			Assert.Equal("matched", switchValue2);
			var switchValue3 = dataComputedFieldEvaluator.EvaluateExpression("SWITCHVALUE(1000, 1000, 1, 1, 2, 2, 3, 3)");
			Assert.Equal((decimal)1000, (decimal)switchValue3);
			var switchValue4 = dataComputedFieldEvaluator.EvaluateExpression("SWITCHVALUE(999, 1000, 1, 1, 2, 2, 999, 999)");
			Assert.Equal((decimal)999, (decimal)switchValue4);

			// string
			var substringResult = dataComputedFieldEvaluator.EvaluateExpression("SUBSTRING(\"abcdefg\",2,2)");
            Assert.Equal("bc", substringResult);
            var trimResult = dataComputedFieldEvaluator.EvaluateExpression("TRIM(\"  test   \")");
            Assert.Equal("test", trimResult);
            var lenResult = dataComputedFieldEvaluator.EvaluateExpression("LEN(\"transfinder\")");
            Assert.Equal(11, lenResult);
            var replaceResult = dataComputedFieldEvaluator.EvaluateExpression("REPLACE(\"hello world!\",\"world\",\"transfinder\")");
            Assert.Equal("hello transfinder!", replaceResult);
            var strResult = dataComputedFieldEvaluator.EvaluateExpression("LEFT(\"patrol    \",6) + RIGHT(\"    finder\",6)");
			Assert.Equal("patrolfinder", strResult);
            var strResult2 = dataComputedFieldEvaluator.EvaluateExpression("UPPER(\"abc\") + NEWLINE() + LOWER(\"EFG\")");
			Assert.Equal($"ABC{Environment.NewLine}efg", strResult2);

            // date
            var dateValue3 = dataComputedFieldEvaluator.EvaluateExpression("DATECUSTOMFORMAT(DATE(2023,1,1),\"MM-dd-yyyy\")");
			Assert.Equal("01-01-2023", dateValue3);
			var dateValue4 = dataComputedFieldEvaluator.EvaluateExpression("DAYOFWEEK(DATE(2023,7,12))");
			Assert.Equal(3, (int)dateValue4);

			//Null data value
			var nulldate = dataComputedFieldEvaluator.EvaluateExpression("DATEADD(\"second\", 86000 , null)");
			var nulldate2 = dataComputedFieldEvaluator.EvaluateExpression("DAYOFWEEK(null)");
			var nulldate3 = dataComputedFieldEvaluator.EvaluateExpression("DATEADD(\"day\", 20, NOW())");

			// data type
			var isnullResult = dataComputedFieldEvaluator.EvaluateExpression("ISNULL(null)");
			Assert.True((bool)isnullResult);
			var dataType1 = dataComputedFieldEvaluator.EvaluateExpression("ISDATETIME(NOW())");
			Assert.True((bool)dataType1);
			var dataType2 = dataComputedFieldEvaluator.EvaluateExpression("ISNUMERIC(9.5)");
			Assert.True((bool)dataType2);
			var dataType3 = dataComputedFieldEvaluator.EvaluateExpression("ISNUMERIC(\"9.5\")");
			Assert.False((bool)dataType3);
            var dataType4 = dataComputedFieldEvaluator.EvaluateExpression("ISSTRING(\"9.5\")");
			Assert.True((bool)dataType4);
			var dataType5 = dataComputedFieldEvaluator.EvaluateExpression("ISLOGICAL(1=2)");
			Assert.True((bool)dataType5);

            var dataType6 = dataComputedFieldEvaluator.EvaluateExpression("TOSTRING( 1 = 1 )");
			Assert.Equal("True",dataType6.ToString());
			var dataType7 = dataComputedFieldEvaluator.EvaluateExpression("TONUMERIC( \"22.34353\" )");
			Assert.Equal((decimal)22.34353, (decimal)dataType7);
			var dataType8 = dataComputedFieldEvaluator.EvaluateExpression("TODATETIME( \"2023-12-29 00:10:01\" )");
            Assert.IsType<DateTime>(dataType8);
			var dataType9 = dataComputedFieldEvaluator.EvaluateExpression("TODATETIME( \"2023-99-99\" )");
			Assert.Null(dataType9);

			// aggregate
			var sumResult1 = dataComputedFieldEvaluator.EvaluateExpression("SUM(1,2,3)");
			Assert.Equal((decimal)6, (decimal)sumResult1);
			var sumResult2 = dataComputedFieldEvaluator.EvaluateExpression("SUM(new [[]] {1,2,3,0.1})");
			Assert.Equal((decimal)6.1, (decimal)sumResult2);
			var aygResult = dataComputedFieldEvaluator.EvaluateExpression("AVG(0, 50)");
			Assert.Equal((decimal)25, (decimal)aygResult);
			var maxResult = dataComputedFieldEvaluator.EvaluateExpression("MAX(-100, 0, 100)");
			Assert.Equal((decimal)100, (decimal)maxResult);
			var minResult = dataComputedFieldEvaluator.EvaluateExpression("MIN(-100, 0, 100)");
			Assert.Equal((decimal)-100, (decimal)minResult);

			// should throw exception with innerException message
			var exception = Assert.Throws<Exception>(() => dataComputedFieldEvaluator.EvaluateExpression("SUBSTRING(\"abcdefg\",100,100) "));
            var exception2 = Assert.Throws<Exception>(() => dataComputedFieldEvaluator.EvaluateExpression("IIF(100)"));

			// Test unknow variables
			Assert.ThrowsAny<Exception>(() => dataComputedFieldEvaluator.EvaluateExpression("abcd"));
			Assert.ThrowsAny<Exception>(() => dataComputedFieldEvaluator.EvaluateExpression("abcd + efg"));

		}

		[Fact]
        public async Task InitializeTest()
        {
            // should success
            InitContext();

            // Wrong type
            var defaultFields = new Dictionary<string, object>();
            var fieldTypes = new Dictionary<string, Type>();
            defaultFields["time"] = DateTime.Now;
            fieldTypes["time"] = typeof(string);
            var exception = Assert.ThrowsAny<Exception>(() => dataComputedFieldEvaluator.InitializeFieldsContext(fieldTypes, defaultFields));

            // Field type does not match
            defaultFields.Clear();
            defaultFields["notExisting"] = "notExisting";
            var exception2 = Assert.ThrowsAny<Exception>(() => dataComputedFieldEvaluator.InitializeFieldsContext(fieldTypes, defaultFields));

            // Same name as a system field
            defaultFields.Clear();
            defaultFields["LEN"] = "LEN";
            var exception3 = Assert.ThrowsAny<Exception>(() => dataComputedFieldEvaluator.InitializeFieldsContext(fieldTypes, defaultFields));
        }


        [Fact]

        public async Task ParsingTest()
        {
            InitContext();

			// Test Switch with inited fields
			var switchValue6 = dataComputedFieldEvaluator.EvaluateExpression(@"SWITCHCASE(null, 
			UPPER([call_type]) == UPPER(""ANIMAL CONTROL COMPLAINT"")           && [gis_clear] != null, DATEADD(""day"", 1, [gis_clear]),
			UPPER([call_type]) == UPPER(""NOTIFICATION"")                       && [gis_clear] != null, [gis_clear])");

			// CONCATENATE a string array
			var concatenateResult4 = dataComputedFieldEvaluator.EvaluateExpression("CONCATENATE([stringlist1].ToArray())");

			// Check if default value is vaild
			var isVaild = dataComputedFieldEvaluator.TryParsingExpression("[string1]", out List<string> fieldsUsed);
            Assert.True(isVaild.Item1);
            Assert.Single(fieldsUsed);

            var datetimeResult = dataComputedFieldEvaluator.EvaluateExpression("[datetime1]");
            Assert.IsType<DateTime>(datetimeResult);

            var result = dataComputedFieldEvaluator.EvaluateExpression("IIF([int1] == 50, [int1]/2 , 0)");
            Assert.Equal((decimal)25, result);

            var listResult = dataComputedFieldEvaluator.EvaluateExpression("[stringlist1]");
            Assert.IsType<List<string>>(listResult);
            Assert.Equal(3, ((List<string>)listResult).Count());

            var nullableResult = dataComputedFieldEvaluator.EvaluateExpression("ISNULL([nullableDecimal1])");
			Assert.True((bool)nullableResult);

            var specificSymbolResult = dataComputedFieldEvaluator.EvaluateExpression("\"[[SymbolTest]]\"");
			Assert.Equal("[SymbolTest]", specificSymbolResult);

			var specificSymbolResult2 = dataComputedFieldEvaluator.EvaluateExpression("\"[SymbolTest]]\"");
			Assert.Equal("[SymbolTest]", specificSymbolResult2);

			var specificSymbolResult3 = dataComputedFieldEvaluator.TryParsingExpression("\"[SymbolTest]\"",out _);
			Assert.False(specificSymbolResult3.Item1);

			// New fieled values
			Dictionary<string, object> fieleds = new Dictionary<string, object>();
            fieleds["int1"] = 100;
            var newFieledsResult = dataComputedFieldEvaluator.EvaluateExpression("[int1]", fieleds);
            Assert.Equal(100, newFieledsResult);

            // Not in the new field list
			var newFieledsException = Assert.ThrowsAny<Exception>(() => dataComputedFieldEvaluator.EvaluateExpression("[string1]", fieleds));
			var newFieledsResult2 = dataComputedFieldEvaluator.TryParsingExpression("[string1]", fieleds, out _);
            Assert.False(newFieledsResult2.Item1);

			// Unknown field
			var unknownException = Assert.ThrowsAny<Exception>(() => dataComputedFieldEvaluator.EvaluateExpression("[Unknown]"));

            // Wrong value type
            fieleds.Clear();
            fieleds["string1"] = false;
            var wrongTypeException = Assert.ThrowsAny<Exception>(() => dataComputedFieldEvaluator.EvaluateExpression("[string1]", fieleds));

            // Not in default field dictionary
            fieleds.Clear();
            fieleds["NotInDictionary"] = "NotInDictionary";
            var notInDictionaryException = Assert.ThrowsAny<Exception>(() => dataComputedFieldEvaluator.EvaluateExpression("[NotInDictionary]", fieleds));

            // Empty Expression
            var emptyException = Assert.ThrowsAny<Exception>(() => dataComputedFieldEvaluator.EvaluateExpression(""));

            // Same fields
            var twoString = dataComputedFieldEvaluator.EvaluateExpression("[string1]+[string1]");
            Assert.Equal("stringTeststringTest", twoString);

			// JSON
			var notExistsJsonValue = dataComputedFieldEvaluator.EvaluateExpression("GETFROMJSON([sc3_remarks],\"DDD:\")");
			Assert.Equal("", notExistsJsonValue);

			var jsonValue = dataComputedFieldEvaluator.EvaluateExpression("GETFROMJSON([sc3_remarks],\"Lat/Lon:\")");
			Assert.Equal("41.6975934138811 / -73.9091088310827", jsonValue);

			var x = dataComputedFieldEvaluator.EvaluateExpression("SPLIT(GETFROMJSON([sc3_remarks],\"Lat/Lon:\"), \"/\", 1)");
			Assert.Equal("-73.9091088310827", x);

			var splitNotExists = dataComputedFieldEvaluator.EvaluateExpression("SPLIT(GETFROMJSON([sc3_remarks],\"Lat/Lon:\"), \"/\", 5)");
			Assert.Equal("", splitNotExists);
		}

        private void InitContext()
        {
            Dictionary<string, object> defaultFields = new Dictionary<string, object>();
            Dictionary<string, Type> fieldTypes = new Dictionary<string, Type>();

            fieldTypes["string1"] = typeof(string);
            fieldTypes["int1"] = typeof(int);
            fieldTypes["datetime1"] = typeof(DateTime);
            fieldTypes["stringlist1"] = typeof(List<string>);
            fieldTypes["nullableDecimal1"] = typeof(decimal?);
			fieldTypes["call_type"] = typeof(string);
			fieldTypes["gis_clear"] = typeof(DateTime?);
			fieldTypes["sc3_remarks"] = typeof(string);

			defaultFields["string1"] = "stringTest";
            defaultFields["int1"] = 50;
            defaultFields["datetime1"] = DateTime.UtcNow;
            defaultFields["stringlist1"] = new List<string>() { "a", "b", "c" };
            defaultFields["nullableDecimal1"] = null;
			defaultFields["call_type"] = "test";
			defaultFields["gis_clear"] = null;
			defaultFields["sc3_remarks"] = "{ \"Call-for-Service #\": 1469," +
				" \"This Report:\": \"2/7/2025 14:57:10\"," +
				" \"Fire Call Type:\": \"PIAA P3\" ," +
				" \"EMS Call Type:\": \"PIAA P3\" ," +
				" \"Nature of Call:\": \"AUTO ACCIDENT\" ," +
				" \"Call Location:\": \"703 MAIN ST, C/POUGHKEEPSIE\" ," +
				" \"Cross Streets:\": \"WORRALL AV, INNIS AV / ROOSEVELT AV\" ," +
				" \"Common Names:\": \"TD BANKNORTH-CPK - 707 MAIN ST\" ," +
				" \"Lat/Lon:\":\"41.6975934138811 / -73.9091088310827\" ," +
				" \"Addl Location Info:\":\"41.6975934138811 / -73.9091088310827\" }" +
				"";


			dataComputedFieldEvaluator.InitializeFieldsContext(fieldTypes, defaultFields);
        }
    }
}
