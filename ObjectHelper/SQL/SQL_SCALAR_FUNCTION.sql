SELECT 
  SCHEMA_NAME(schema_id) [schema_name]
  ,QUOTENAME(name) [object_name]
  ,type_desc
  ,create_date
  ,modify_date
  ,Code = (SELECT DEFINITION FROM SYS.SQL_MODULES WHERE OBJECT_ID = OBJECT_ID(SCHEMA_NAME(schema_id) +'.'+ name)) 
  ,DB_Name() [DbName]
FROM sys.objects
WHERE 
type_desc IN
(
'SQL_SCALAR_FUNCTION'
)
ORDER BY 2,1