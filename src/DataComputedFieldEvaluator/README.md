# Formula syntax instructions  
## Basic syntax:  

* **Arithmetic operations (+, -, \*, /, %);**  
* **Comparisons (=, !=, >, <, >=, <=);**  
* **Logical operators (&&, ||, !);**  
* **Struct values (true, false, null);**  
* **Conditionals including (ternary) operator ( ? : ).**  

**Examples:**  
```
"hello" + " " + "world"
```
— returns "hello world".  
```
50/2
```
— returns 25.  
```
true = !false
```
— returns true.  
```
true && false
```
— returns false.  
```
1 = 1 ? 1 : 0
```
— returns 1.  

## System functions:  
### Logical  
#### IIF  
**Description:** Takes three arguments as input. Returns the second argument if the first evaluates to TRUE. Otherwise returns the third argument. The "? :" operator can be used instead of IIF().  
**Example:**
```
IIF(1 = 1, 1, 0)
```
— returns 1.  
#### AND  
**Description:** Returns TRUE if two arguments are TRUE; returns FALSE if any argument is FALSE. The "&&" operator can be used instead of AND().  
**Example:** 
```
AND(2+2=4, 2+3=6)
```
— returns false.  
#### OR  
**Description:** Returns TRUE if any argument is TRUE. The "||" operator can be used instead of OR().  
**Example:** 
```
OR(2+2=4, 2+3=6) 
```
— returns true.  
#### NOT  
**Description:** Reverses the value of its argument. The "!" operator can be used instead of NOT().  
**Example:**
```
NOT(1+2=4) 
```
— returns true.  
#### SWITCHCASE  
**Description:** Evaluates an array of cases and returns a corresponding value if a true case is found, otherwise returns the default value. The casesAndValues array must have an even number of elements and contain alternating boolean conditions and corresponding values.  
**Example:**  
```
SWITCHCASE("default result", false, "not matched", true, "matched")
```
— returns "matched".  
#### SWITCHVALUE  
**Description:** Switches the target value with a corresponding value from the provided cases. If the target value matches any case, the corresponding value is returned. Otherwise, the default value is returned. The casesAndValues array must have an even number of elements and alternate between condition values and corresponding values.  
**Example:**  
```
SWITCHVALUE("targetValue", "default result", "a", "not matched", "targetValue", "matched")  
```
— returns "matched".  

### Date  
#### DATE  
**Description:** Creates a date value from three numeric values.  
**Example:** 
```
DATE(2012, 7, 4)
```
— returns the date July 4th, 2012.  
#### NOW  
**Description:** Returns today's date and time.  
**Example:** 
```
NOW()
```
— returns the current date and time.  
#### DATECUSTOMFORMAT  
**Description:** Modifies a date value to be output in a specified format.  
**Example:** 
```
DATECUSTOMFORMAT(DATE(2023,1,1),"MM-dd-yyyy")
```
— returns the string "01-01-2023".  
#### DAYOFWEEK  
**Description:** Returns the number of the weekday of a date, where 0 = Sunday, 1 = Monday, 2 = Tuesday … 6 = Saturday.  
**Example:** 
```
DAYOFWEEK(DATE(2023, 7, 12))
```
— returns 3.  

### String  
#### CONCATENATE
**Description:** Joins several text strings into one text string.
**Example:**
```
CONCATENATE("a", "b", "c")
```
— returns "abc".
#### SUBSTRING  
**Description:** Gets a sub-string of a specified length, starting at a specified point in the string.  
**Example:** 
```
SUBSTRING("abcdefg", 2, 2)
```
— returns "bc".  
#### TRIM  
**Description:** Returns string that remains after all white-space characters are removed from the start and end of the current string.  
**Example:** 
```
TRIM("   test trim  ")
```
— returns "test trim".  
#### LEN  
**Description:** Returns the number of characters in a text string.  
**Example:** 
```
LEN("example")
```
–– returns 7.  
#### REPLACE  
**Description:** Returns string that is equivalent to the current string except that all instances of oldValue are replaced with newValue.  
**Example:** 
```
REPLACE("hello world!", "world", "transfinder")
```
— returns "hello transfinder!".  
#### LEFT / RIGHT  
**Description:** Returns the first/last character(s) of a text string.  
**Example:** 
```
LEFT("patrol abc", 6) + RIGHT("def finder", 6)
```
— returns "patrolfinder".  
#### UPPER / LOWER  
**Description:** Returns string converted to uppercase/lowercase.  
**Example:** 
```
UPPER("abc") + LOWER("DEF")
```
— returns "ABCdef".  
#### NEWLINE  
**Description:** Begins a new line of text.  
**Example:** 
```
NEWLINE()
```
#### GETFROMJSON
**Description:** Returns value from JSON object based on the key.
**Example:**
```
GETFROMJSON("{\"key1\":\"aa\",\"key2\":\"bb\"}","key1")
```
— returns "aa". 
#### SPLIT
**Description:** Split a string based on a delimiter and return the content at the 
specified index.
**Example:**
```
SPLIT("a|b|c", "|", 2)
```
— returns "aa". 

### Aggregate  
#### SUM  
**Description:** Returns the sum of the values.  
**Example:**  
```
SUM(1, 2, 3)
```
— returns 6.  
#### AVG  
**Description:** Returns the average of the values.  
**Example:**  
```
AVG(0, 100)
```
— returns 50.  
#### MAX  
**Description:** Returns the maximun value.  
**Example:**  
```
SUM(-1, 0, 1)
```
— returns 1.  
#### MIN  
**Description:** Returns the minimun value.  
**Example:**  
```
SUM(-1, 0, 1)
```
— returns -1.  

### Data Type  
#### ISNULL  
**Description:** Returns True if the argument is NULL. Otherwise returns False.  
**Example:** 
```
ISNULL(null)
```
— returns true.  
#### ISLOGICAL  
**Description:** Checks if a value is TRUE or FALSE.  
**Example:**
```
ISLOGICAL(1 = 2)
```
— returns true.  
#### ISNUMERIC  
**Description:** Returns True it is of type int, float, double, or decimal. Otherwise returns False.  
**Example:** 
```
ISNUMERIC(3.14)
```
— returns true.  
#### ISSTRING  
**Description:** Returns True if it is string value. Otherwise returns False.  
**Example:** 
```
ISSTRING(3.14) 
```
— returns false.  
#### ISDATETIME  
**Description:** Returns True if it is datetime value. Otherwise returns False.  
**Example:** 
```
ISDATETIME(DATE(2023, 7, 12))
```
— returns true.  
#### TOSTRING  
**Description:** Try parsing the parameter into a String value.  
**Example:** 
```
TOSTRING(1 = 1)
```
— returns "True".  
#### TODATETIME  
**Description:** Try parsing the parameter String into a DateTime value. If it fails, return null.  
**Example:** 
```
TODATETIME("2012-07-04")
```
— returns the date July 4th, 2012.  
#### TONUMERIC  
**Description:** Try parsing the parameter String into a Decimal value. If it fails, return null.  
**Example:** 
```
TONUMERIC("1.2")
```
— returns 1.2. 