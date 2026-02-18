IF OBJECT_ID('tempdb..#TABLAS') IS NOT NULL
	DROP TABLE #TABLAS

IF OBJECT_ID('tempdb..#Data') IS NOT NULL
	DROP TABLE #Data

IF OBJECT_ID('tempdb..#TableCatalogData') IS NOT NULL
	DROP TABLE #TableCatalogData

IF OBJECT_ID('tempdb..#Information') IS NOT NULL
	DROP TABLE #Information

CREATE TABLE #TableCatalogData (
	[Schema] VARCHAR(10) NULL
	,[TableName] VARCHAR(100) NULL
	,[FullName] VARCHAR(200) NULL
	,[ColumnName] VARCHAR(100) NULL
	,[ObjectTabla] BIGINT NULL
	,[Type] NVARCHAR(200) NULL
	,[Length] VARCHAR(50) NULL
	,[Status] INT NULL
	,[ConvertValue] NVARCHAR(200) NULL
	)

CREATE TABLE #Data(
	[Schema] [varchar](10) NULL,
	[TableName] [varchar](100) NULL,
	[FullName] [varchar](200) NULL,
	[ObjectTabla] [bigint] NULL,
	[Columnas] [int] NULL,
	[ColumnsHeader] [nvarchar](max) NULL,
	[ColumnsName] [nvarchar](max) NULL,
	[ColumnsNameChar] [nvarchar](max) NULL,
	[Select] [nvarchar](max) NULL,
	[Query] [nvarchar](max) NULL,
	[Data] [nvarchar](max) NULL
)

SELECT b.NAME [Schema]
	,a.NAME [TableName]
	,QUOTENAME(b.NAME) + '.' + QUOTENAME(A.NAME) [FullName]
	,C.NAME [ColumnName]
	,A.[object_id] [ObjectTabla]
	,CASE 
		WHEN c.is_computed = 1
			THEN 'AS ' + cc.[definition]
		ELSE UPPER(tp.NAME)
		END [Type]
	,CASE 
		WHEN c.is_computed = 1
			THEN 'COMPUTED'
		ELSE CASE 
				WHEN tp.NAME IN ('varchar', 'char', 'varbinary', 'binary', 'text')
					THEN CASE 
							WHEN c.max_length = - 1
								THEN 'MAX'
							ELSE CAST(c.max_length AS VARCHAR(5))
							END
				WHEN tp.NAME IN ('nvarchar', 'nchar', 'ntext')
					THEN CASE 
							WHEN c.max_length = - 1
								THEN 'MAX'
							ELSE CAST(c.max_length / 2 AS VARCHAR(5))
							END
				WHEN tp.NAME IN ('datetime2', 'time2', 'datetimeoffset')
					THEN CAST(c.scale AS VARCHAR(5))
				WHEN tp.NAME = 'decimal'
					THEN CAST(c.[precision] AS VARCHAR(5)) --+ ',' + CAST(c.scale AS VARCHAR(5))
				WHEN tp.NAME = 'int'
					THEN CAST(c.[precision] AS VARCHAR(5)) --'2147483647'
				WHEN tp.NAME = 'bigint'
					THEN CAST(c.[precision] AS VARCHAR(5)) --'9223372036854775807'
				WHEN tp.NAME = 'smallint'
					THEN CAST(c.[precision] AS VARCHAR(5)) --'32767'
				WHEN tp.NAME = 'tinyint'
					THEN CAST(c.[precision] AS VARCHAR(5)) --'255'
				WHEN tp.NAME = 'bit'
					THEN CAST(c.[precision] AS VARCHAR(5)) --'1'
				WHEN tp.NAME = 'date'
					THEN CAST(c.[precision] AS VARCHAR(5)) 			
				WHEN tp.NAME = 'datetime'
					THEN CAST(c.[precision] AS VARCHAR(5)) 
				WHEN tp.NAME = 'uniqueidentifier'
					THEN '40'--CAST(c.[precision] AS VARCHAR(5))
				WHEN tp.NAME = 'time'
					THEN CAST(c.[precision] AS VARCHAR(5)) 		
				WHEN tp.NAME = 'decimal'
					THEN CAST(c.[precision] AS VARCHAR(5)) 	
				WHEN tp.NAME = 'numeric'
					THEN CAST(c.[precision] AS VARCHAR(5)) --+ ',' + CAST(c.scale AS VARCHAR(5))																								
				ELSE ''
				END
		END [Length]
INTO #TABLAS		
FROM sys.tables a
INNER JOIN sys.schemas b
	ON a.schema_id = b.schema_id
INNER JOIN sys.columns C
	ON C.object_id = A.object_id
JOIN sys.types tp WITH (NOWAIT)
	ON c.user_type_id = tp.user_type_id
LEFT JOIN sys.computed_columns cc WITH (NOWAIT)
	ON c.[object_id] = cc.[object_id]
		AND c.column_id = cc.column_id
LEFT JOIN sys.default_constraints dc WITH (NOWAIT)
	ON c.default_object_id != 0
		AND c.[object_id] = dc.parent_object_id
		AND c.column_id = dc.parent_column_id
LEFT JOIN sys.identity_columns ic WITH (NOWAIT)
	ON c.is_identity = 1
		AND c.[object_id] = ic.[object_id]
		AND c.column_id = ic.column_id
ORDER BY b.NAME
	,a.NAME
	,c.column_id
	
INSERT INTO #TableCatalogData
SELECT 
	 *
	,1
	,CASE  
		WHEN [Type] IN ('datetime2', 'time2', 'datetimeoffset') THEN 'CONVERT(VARCHAR(26), ISNULL('+QUOTENAME([ColumnName])+',''19000101''),23)'
		WHEN [Type] IN ('varchar', 'char', 'text') THEN 'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',''''))'
		WHEN [Type] IN ('varbinary', 'binary') THEN 'CONVERT(VARCHAR(MAX), ISNULL('+QUOTENAME([ColumnName])+',0),2)'
		WHEN [Type] IN ('nvarchar', 'nchar', 'ntext') THEN 'CONVERT(NVARCHAR, ISNULL('+QUOTENAME([ColumnName])+',''''))'
		WHEN [Type] = 'int' THEN 'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'
		WHEN [Type] = 'bigint' THEN 'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'
		WHEN [Type] = 'smallint' THEN 'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'
		WHEN [Type] = 'tinyint' THEN 'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'
		WHEN [Type] = 'bit' THEN 'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'
		WHEN [Type] = 'date' THEN 'CONVERT(VARCHAR(26), ISNULL('+QUOTENAME([ColumnName])+',''19000101''),23)'
		WHEN [Type] = 'datetime' THEN  'CONVERT(VARCHAR(20), ISNULL('+QUOTENAME([ColumnName])+',''19000101''),100)'
		WHEN [Type] = 'uniqueidentifier' THEN  'CONVERT(VARCHAR(40), ISNULL('+QUOTENAME([ColumnName])+',''00000000-0000-0000-0000-000000000000''))'
		WHEN [Type] = 'time' THEN  'CONVERT(VARCHAR(8), ISNULL('+QUOTENAME([ColumnName])+',''00:00:00''),24)' 
		WHEN [Type] = 'decimal' THEN  'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'
		WHEN [Type] = 'numeric' THEN  'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'
		ELSE ''''''
	 END
FROM #TABLAS
WHERE [FullName] IN (
		'[cat].[CatalogShipments]', '[cat].[CollisionPart]', '[cat].[DealerBrand]', '[cat].[EndItem]', '[cat].[InsuranceCompany]', '[cat].[UserBrand]', '[cat].[WebServiceUserType]', --'[cat].[EndItemData]'
		'[dbo].[AdditionalIncentive]', '[dbo].[ClientTypeName]', '[dbo].[Country]', '[dbo].[CountryStates]', '[dbo].[CSA]', '[dbo].[DdcProcess]', '[dbo].[Dealer]', '[dbo].[DealerActiveDdcProcess]', 
		'[dbo].[DealerGroup]', '[dbo].[DealersByWebServiceUser]', '[dbo].[DMS]', '[dbo].[DmsEmailAddress]', '[dbo].[EmailAddress]', '[dbo].[EmailTemplate]', '[dbo].[ErrorCode]', '[dbo].[Exceptions]', 
		'[dbo].[FileType]', '[dbo].[FillRate]', '[dbo].[FleetType]', '[dbo].[GroupProfiles]', '[dbo].[HistoricBackupFiles]', '[dbo].[Icon2WholeSalePolicyCalculator]', '[dbo].[IncentiveType]', 
		'[dbo].[IncentiveWholeSale]', '[dbo].[InformationDownloadConfiguration]', '[dbo].[KmRange]', '[dbo].[LcvLineModel]', '[dbo].[MasterPriceType]', '[dbo].[Months]', '[dbo].[NNARetention]', 
		'[dbo].[PackagingType]', '[dbo].[Parameters]', '[dbo].[PartNumberRatioAA]', '[dbo].[PartsClass]', '[dbo].[PartType]', '[dbo].[Process]', '[dbo].[ProcessMenuType]', '[dbo].[ProcessProfile]', 
		'[dbo].[Profile]', '[dbo].[RecalculateModelGroupConfig]', '[dbo].[Region]', '[dbo].[RepairShop]', '[dbo].[ReportConfig]', '[dbo].[ReportDatasetConfig]', '[dbo].[ReportFilter]', '[dbo].[SalesChannel]',
		'[dbo].[SalesClass]', '[dbo].[SalesGoalType]', '[dbo].[SchedulingType]', '[dbo].[SecurityGroups]', '[dbo].[ServiceClass]', '[dbo].[ServiceLanePartClassification]', '[dbo].[simiClass]', 
		'[dbo].[SpecialClientRFC]', '[dbo].[SsisExecutionConfig]', '[dbo].[SsisScheduling]', '[dbo].[TaskProcess]', '[dbo].[uploadFileLayout]', '[dbo].[UploadFileLayoutCol]', '[dbo].[UploadPowerPivot]', 
		'[dbo].[UrbanScienceValues]', '[dbo].[UserPassword]', '[dbo].[UserPasswordConfig]', '[dbo].[Users]', '[dbo].[VINDecoder]', '[dbo].[WebServiceBindingConfig]', '[dbo].[WebServiceUser]', '[dbo].[Zone]', 
		'[rpt].[SalesChannelReport]', '[sls].[RcnCatalogEdoCivil]', '[sls].[RcnCatalogEscolaridad]', '[sls].[RcnCatalogMarcas]', '[sls].[RegionMapCoordinates]', '[sls].[SalesErrorCode]', 
		'[sls].[SalesRegion]', '[sls].[SalesVmc]', '[sls].[SalesZone]', '[sls].[StateMapCoordinates]', '[sls].[VehicleSaleStatus]', '[sls].[ZoneMapCoordinates]'
		)
		AND [FullName] NOT IN (
		'[dbo].[simiClass]'
		,'[dbo].[EmailTemplate]'		
		,'[dbo].[FillRate]'
		
		)

INSERT INTO #Data
           ([Schema]
           ,[TableName]
           ,[FullName]
           ,[ObjectTabla]
           ,[Columnas]
           ,[ColumnsHeader]
           ,[ColumnsName]
           ,[ColumnsNameChar]
		   ,[Select]
		   ,[Query]
		   ,[Data]
		   )
SELECT 	
	 A.[Schema]
	,A.[TableName]
	,A.[FullName]
	,A.ObjectTabla
	,COUNT(*) [Columnas]
	,STUFF((  
			SELECT 	
				CASE WHEN [Length] IN ('COMPUTED', 'MAX') 
					THEN '' 
					ELSE '+ CAST(''' + B.[ColumnName] + ''' AS CHAR('+ CAST(B.[Length] AS VARCHAR) +'))' 
					END
			FROM #TableCatalogData B
			where B.[Status] = 1
			and B.FullName = A.FullName
			group by 
				 B.[Schema]
				,B.[TableName]
				,B.[FullName]
				,B.[ColumnName]
				,B.[Length]
			order by B.FullName,[ColumnName]
			FOR XML PATH('') 
		), 1, 1, '' ) 
	 + CHAR(13) AS [ColumnsHeader]
	,STUFF((  
			SELECT 	
				',', QUOTENAME(B.[ColumnName]) AS [text()] 
			FROM #TableCatalogData B
			where B.[Status] = 1
			and B.FullName = A.FullName
			group by 
				 B.[Schema]
				,B.[TableName]
				,B.[FullName]
				,B.[ColumnName]
			order by [ColumnName]
			FOR XML PATH('') 
		), 1, 1, '' ) AS [ColumnsName]
	,STUFF((  
			SELECT 	--B.FullName,[ColumnName],
				CASE WHEN [Length] IN ('COMPUTED', 'MAX') 
					THEN '' 
					ELSE '+ CAST(' + B.[ConvertValue] + ' AS CHAR('+ CAST(B.[Length] AS VARCHAR) +'))' 
					--ELSE '+ CAST(' + QUOTENAME(B.[ColumnName]) + ' AS CHAR('+ CAST(B.[Length] AS VARCHAR) +'))' 
					END
			FROM #TableCatalogData B
			where B.[Status] = 1
			and B.FullName = A.FullName
			group by 
				 B.[Schema]
				,B.[TableName]
				,B.[FullName]
				,B.[ColumnName]				
				,B.[Length]
				,B.[ConvertValue]
			order by B.FullName,B.[ColumnName]
			FOR XML PATH('') 
		), 1, 1, '' ) AS [ColumnsNameChar]
	,'' [Select]
	,'' Query
	,'' Data
FROM #TableCatalogData A
where A.[Status] = 1
group by 
	 A.[Schema]
	,A.[TableName]
	,A.[FullName]
	,A.ObjectTabla

UPDATE #data 
	SET [Select] = 'SELECT ' + [ColumnsName] + ' FROM ' + [FullName] + ' ORDER BY ' + [ColumnsName] 
		,Query = 'SELECT REPLACE(CONVERT(NVARCHAR(MAX),STUFF((SELECT''@|'',' + [ColumnsNameChar] + 'AS [text()] FROM '+[FullName]+' B FOR XML PATH(''''), root(''MyString''), type).value(''/MyString[1]'',''varchar(max)'') , 1, 2, '''' )),''@|'',CHAR(13))'

/*
	SELECT * FROM #data
	SELECT * FROM #TableCatalogData
*/

DECLARE @SQL NVARCHAR(MAX)
DECLARE @ObjectTabla INT
CREATE TABLE #Information (DATA NVARCHAR(MAX))

DECLARE POINTER CURSOR FAST_FORWARD FOR
SELECT 
	Query
	,ObjectTabla
FROM #data A

OPEN POINTER
FETCH NEXT FROM POINTER INTO @SQL, @ObjectTabla

WHILE @@FETCH_STATUS = 0
BEGIN
   
	INSERT INTO #Information
	EXEC (@SQL)

	UPDATE #data
		SET [Data] = (SELECT ISNULL(DATA,'') FROM #Information)
	WHERE 
		ObjectTabla = @ObjectTabla

	TRUNCATE TABLE #Information
   --BREAK;

    FETCH NEXT FROM POINTER INTO @SQL, @ObjectTabla
END

CLOSE POINTER
DEALLOCATE POINTER


SELECT 	
	 *
FROM #DATA