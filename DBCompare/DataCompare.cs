using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBCompare
{
    public partial class DataCompare : Form
    {
        public DataCompare()
        {
            InitializeComponent();
        }

        string Tablas = string.Empty;

        #region Metodos
        private void LoadDefault()
        {
            foreach (ListViewItem item in lwDataTables.Items)
	        {
                foreach (string table in Tablas.Split(','))
                {
                    if (table == item.Text.ToString())
                    {
                        item.Checked = true;        
                    }
                }
	        }
        }

        private void LoadDataList()
        {
            try
            {
                DatabaseConnect data = new DatabaseConnect();
                //lwDataTables.Columns[1].Text = "Tablas";
                lwDataTables.Items.Clear();
                List<Data> res = data.Datos(1, Query());
                foreach (Data item in res)
                {
                    lwDataTables.Items.Add(new ListViewItem(item.FullName.ToString()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Errores al cargar la lista de tablas {0}", ex.Message));
            }
        }

        private string Query()
        {
            StringBuilder sb =  new StringBuilder();
            sb.AppendLine("SELECT ");
            sb.AppendLine("    QUOTENAME(b.name) +'.'+ QUOTENAME(a.name) [FullName],");
            sb.AppendLine("    b.name [Schema],");
            sb.AppendLine("    a.name TableName,");
            sb.AppendLine("    'Columns' Columns, ");
            sb.AppendLine("    'data2' Data2, ");
            sb.AppendLine("    'data' Data ");
            sb.AppendLine("    FROM    sys.tables a");
		    sb.AppendLine("            inner join sys.schemas b ");
			sb.AppendLine("                on a.schema_id = b.schema_id");
            sb.AppendLine("    ORDER BY b.name, a.name, a.create_date, a.modify_date");
            return sb.ToString();
        }

        private string QueryDatos(string tablas)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("IF OBJECT_ID('tempdb..#TABLAS') IS NOT NULL");
            sb.AppendLine("            	DROP TABLE #TABLAS");
            sb.AppendLine("            IF OBJECT_ID('tempdb..#Data') IS NOT NULL");
            sb.AppendLine("            	DROP TABLE #Data");
            sb.AppendLine("            IF OBJECT_ID('tempdb..#TableCatalogData') IS NOT NULL");
            sb.AppendLine("            	DROP TABLE #TableCatalogData");
            sb.AppendLine("            IF OBJECT_ID('tempdb..#Information') IS NOT NULL");
            sb.AppendLine("            	DROP TABLE #Information");
            sb.AppendLine("            IF OBJECT_ID('tempdb..#Result') IS NOT NULL");
            sb.AppendLine("            	DROP TABLE #Result");
            sb.AppendLine("            CREATE TABLE #TableCatalogData (");
            sb.AppendLine("            	[Schema] VARCHAR(10) NULL");
            sb.AppendLine("            	,[TableName] VARCHAR(100) NULL");
            sb.AppendLine("            	,[FullName] VARCHAR(200) NULL");
            sb.AppendLine("            	,[ColumnName] VARCHAR(100) NULL");
            sb.AppendLine("            	,[ObjectTabla] BIGINT NULL");
            sb.AppendLine("            	,[Type] NVARCHAR(200) NULL");
            sb.AppendLine("            	,[Length] VARCHAR(50) NULL");
            sb.AppendLine("				,[Identity] INT NULL");
            sb.AppendLine("				,[ColumnID] INT NULL");
            sb.AppendLine("            	,[Status] INT NULL");
            sb.AppendLine("            	,[ConvertValue] NVARCHAR(200) NULL");
            sb.AppendLine("            	)");
            sb.AppendLine("            CREATE TABLE #Data(");
            sb.AppendLine("            	[Schema] [varchar](10) NULL,");
            sb.AppendLine("            	[TableName] [varchar](100) NULL,");
            sb.AppendLine("            	[FullName] [varchar](200) NULL,");
            sb.AppendLine("            	[ObjectTabla] [bigint] NULL,");
            sb.AppendLine("            	[Columnas] [int] NULL,");
            sb.AppendLine("            	[ColumnData] [nvarchar](max) NULL,");
            sb.AppendLine("            	[ColumnsName] [nvarchar](max) NULL,");
            sb.AppendLine("            	[ColumnsNameChar] [nvarchar](max) NULL,");
            sb.AppendLine("				[ColumnsNameWithOutChar] [nvarchar](max) NULL,");
            sb.AppendLine("            	[Select] [nvarchar](max) NULL");
            sb.AppendLine("            )");
            sb.AppendLine("            CREATE TABLE #Result(");
            sb.AppendLine("            	[Schema] [varchar](10) NULL,");
            sb.AppendLine("            	[TableName] [varchar](100) NULL,");
            sb.AppendLine("            	[FullName] [varchar](200) NULL,");
            sb.AppendLine("            	[Data] [nvarchar](max) NULL,");
            sb.AppendLine("				[Data2] [nvarchar](max) NULL,");
            sb.AppendLine("				[Columns] [nvarchar](max) NULL");
            sb.AppendLine("            )");
            sb.AppendLine("            SELECT b.NAME [Schema]");
            sb.AppendLine("            	,a.NAME [TableName]");
            sb.AppendLine("            	,QUOTENAME(b.NAME) + '.' + QUOTENAME(A.NAME) [FullName]");
            sb.AppendLine("            	,C.NAME [ColumnName]");
            sb.AppendLine("            	,A.[object_id] [ObjectTabla]");
            sb.AppendLine("            	,CASE ");
            sb.AppendLine("            		WHEN c.is_computed = 1");
            sb.AppendLine("            			THEN 'AS ' + cc.[definition]");
            sb.AppendLine("            		ELSE UPPER(tp.NAME)");
            sb.AppendLine("            		END [Type]");
            sb.AppendLine("            	,CASE ");
            sb.AppendLine("            		WHEN c.is_computed = 1");
            sb.AppendLine("            			THEN 'COMPUTED'");
            sb.AppendLine("            		ELSE CASE ");
            sb.AppendLine("            				WHEN tp.NAME IN ('varchar', 'char', 'varbinary', 'binary', 'text')");
            sb.AppendLine("            					THEN CASE ");
            sb.AppendLine("            							WHEN c.max_length = - 1");
            sb.AppendLine("            								THEN 'MAX'");
            sb.AppendLine("            							ELSE CAST(c.max_length AS VARCHAR(5))");
            sb.AppendLine("            							END");
            sb.AppendLine("            				WHEN tp.NAME IN ('nvarchar', 'nchar', 'ntext')");
            sb.AppendLine("            					THEN CASE ");
            sb.AppendLine("            							WHEN c.max_length = - 1");
            sb.AppendLine("            								THEN 'MAX'");
            sb.AppendLine("            							ELSE CAST(c.max_length / 2 AS VARCHAR(5))");
            sb.AppendLine("            							END");
            sb.AppendLine("            				WHEN tp.NAME IN ('datetime2', 'time2', 'datetimeoffset')");
            sb.AppendLine("            					THEN CAST(c.scale AS VARCHAR(5))");
            sb.AppendLine("            				WHEN tp.NAME = 'decimal'");
            sb.AppendLine("            					THEN CAST(c.[precision] AS VARCHAR(5)) --+ ',' + CAST(c.scale AS VARCHAR(5))");
            sb.AppendLine("            				WHEN tp.NAME = 'int'");
            sb.AppendLine("            					THEN CAST(c.[precision] AS VARCHAR(5)) --'2147483647'");
            sb.AppendLine("            				WHEN tp.NAME = 'bigint'");
            sb.AppendLine("            					THEN CAST(c.[precision] AS VARCHAR(5)) --'9223372036854775807'");
            sb.AppendLine("            				WHEN tp.NAME = 'smallint'");
            sb.AppendLine("            					THEN CAST(c.[precision] AS VARCHAR(5)) --'32767'");
            sb.AppendLine("            				WHEN tp.NAME = 'tinyint'");
            sb.AppendLine("            					THEN CAST(c.[precision] AS VARCHAR(5)) --'255'");
            sb.AppendLine("            				WHEN tp.NAME = 'bit'");
            sb.AppendLine("            					THEN CAST(c.[precision] AS VARCHAR(5)) --'1'");
            sb.AppendLine("            				WHEN tp.NAME = 'date'");
            sb.AppendLine("            					THEN CAST(c.[precision] AS VARCHAR(5)) 			");
            sb.AppendLine("            				WHEN tp.NAME = 'datetime'");
            sb.AppendLine("            					THEN CAST(c.[precision] AS VARCHAR(5)) ");
            sb.AppendLine("            				WHEN tp.NAME = 'uniqueidentifier'");
            sb.AppendLine("            					THEN '40'--CAST(c.[precision] AS VARCHAR(5))");
            sb.AppendLine("            				WHEN tp.NAME = 'time'");
            sb.AppendLine("            					THEN CAST(c.[precision] AS VARCHAR(5)) 		");
            sb.AppendLine("            				WHEN tp.NAME = 'decimal'");
            sb.AppendLine("            					THEN CAST(c.[precision] AS VARCHAR(5)) 	");
            sb.AppendLine("            				WHEN tp.NAME = 'numeric'");
            sb.AppendLine("            					THEN CAST(c.[precision] AS VARCHAR(5)) --+ ',' + CAST(c.scale AS VARCHAR(5))																								");
            sb.AppendLine("            				ELSE ''");
            sb.AppendLine("            				END");
            sb.AppendLine("            		END [Length]");
            sb.AppendLine("				,c.is_identity [Identity]");
            sb.AppendLine("				,C.column_id   [ColumnID]");
            sb.AppendLine("            INTO #TABLAS		");
            sb.AppendLine("            FROM sys.tables a");
            sb.AppendLine("            INNER JOIN sys.schemas b");
            sb.AppendLine("            	ON a.schema_id = b.schema_id");
            sb.AppendLine("            INNER JOIN sys.columns C");
            sb.AppendLine("            	ON C.object_id = A.object_id");
            sb.AppendLine("            JOIN sys.types tp WITH (NOWAIT)");
            sb.AppendLine("            	ON c.user_type_id = tp.user_type_id");
            sb.AppendLine("            LEFT JOIN sys.computed_columns cc WITH (NOWAIT)");
            sb.AppendLine("            	ON c.[object_id] = cc.[object_id]");
            sb.AppendLine("            		AND c.column_id = cc.column_id");
            sb.AppendLine("            LEFT JOIN sys.default_constraints dc WITH (NOWAIT)");
            sb.AppendLine("            	ON c.default_object_id != 0");
            sb.AppendLine("            		AND c.[object_id] = dc.parent_object_id");
            sb.AppendLine("            		AND c.column_id = dc.parent_column_id");
            sb.AppendLine("            LEFT JOIN sys.identity_columns ic WITH (NOWAIT)");
            sb.AppendLine("            	ON c.is_identity = 1");
            sb.AppendLine("            		AND c.[object_id] = ic.[object_id]");
            sb.AppendLine("            		AND c.column_id = ic.column_id");
            sb.AppendLine("            ORDER BY b.NAME");
            sb.AppendLine("            	,a.NAME");
            sb.AppendLine("            	,c.column_id");
            sb.AppendLine("            INSERT INTO #TableCatalogData");
            sb.AppendLine("            SELECT ");
            sb.AppendLine("            	 *");
            sb.AppendLine("            	,1");
            sb.AppendLine("            	,CASE  ");
            sb.AppendLine("            		WHEN [Type] IN ('datetime2', 'time2', 'datetimeoffset') THEN 'CONVERT(VARCHAR(26), ISNULL('+QUOTENAME([ColumnName])+',''19000101''),23)'");
            sb.AppendLine("            		WHEN [Type] IN ('varchar', 'char', 'text') THEN 'CONVERT(VARCHAR('+CAST([Length] AS VARCHAR)+'), ISNULL('+QUOTENAME([ColumnName])+',''''))'");
            sb.AppendLine("            		WHEN [Type] IN ('varbinary', 'binary') THEN 'CONVERT(VARCHAR(MAX), ISNULL('+QUOTENAME([ColumnName])+',0),2)'");
            sb.AppendLine("            		WHEN [Type] IN ('nvarchar', 'nchar', 'ntext') THEN 'CONVERT(NVARCHAR('+CAST([Length] AS VARCHAR)+'), ISNULL('+QUOTENAME([ColumnName])+',''''))'");
            sb.AppendLine("            		WHEN [Type] = 'int' THEN 'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'");
            sb.AppendLine("            		WHEN [Type] = 'bigint' THEN 'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'");
            sb.AppendLine("            		WHEN [Type] = 'smallint' THEN 'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'");
            sb.AppendLine("            		WHEN [Type] = 'tinyint' THEN 'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'");
            sb.AppendLine("            		WHEN [Type] = 'bit' THEN 'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'");
            sb.AppendLine("            		WHEN [Type] = 'date' THEN 'CONVERT(VARCHAR(26), ISNULL('+QUOTENAME([ColumnName])+',''19000101''),23)'");
            sb.AppendLine("            		WHEN [Type] = 'datetime' THEN  'CONVERT(VARCHAR(20), ISNULL('+QUOTENAME([ColumnName])+',''19000101''),100)'");
            sb.AppendLine("            		WHEN [Type] = 'uniqueidentifier' THEN  'CONVERT(VARCHAR(40), ISNULL('+QUOTENAME([ColumnName])+',''00000000-0000-0000-0000-000000000000''))'");
            sb.AppendLine("            		WHEN [Type] = 'time' THEN  'CONVERT(VARCHAR(8), ISNULL('+QUOTENAME([ColumnName])+',''00:00:00''),24)' ");
            sb.AppendLine("            		WHEN [Type] = 'decimal' THEN  'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'");
            sb.AppendLine("            		WHEN [Type] = 'numeric' THEN  'CONVERT(VARCHAR, ISNULL('+QUOTENAME([ColumnName])+',0))'");
            sb.AppendLine("            		ELSE ''''''");
            sb.AppendLine("            	 END");
            sb.AppendLine("            FROM #TABLAS");
            sb.AppendLine("            WHERE [FullName] IN (###)");
            sb.AppendLine("			");
            sb.AppendLine("            INSERT INTO #Data");
            sb.AppendLine("                       ([Schema]");
            sb.AppendLine("                       ,[TableName]");
            sb.AppendLine("                       ,[FullName]");
            sb.AppendLine("                       ,[ObjectTabla]");
            sb.AppendLine("                       ,[Columnas]");
            sb.AppendLine("                       ,[ColumnData]");
            sb.AppendLine("                       ,[ColumnsName]");
            sb.AppendLine("                       ,[ColumnsNameChar]");
            sb.AppendLine("					   ,[ColumnsNameWithOutChar]");
            sb.AppendLine("            		   ,[Select]");
            sb.AppendLine("            		   )");
            sb.AppendLine("            SELECT 	");
            sb.AppendLine("            	 A.[Schema]");
            sb.AppendLine("            	,A.[TableName]");
            sb.AppendLine("            	,A.[FullName]");
            sb.AppendLine("            	,A.ObjectTabla");
            sb.AppendLine("            	,COUNT(*) [Columnas]");
            sb.AppendLine("            	,STUFF((  ");
            sb.AppendLine("            			SELECT 	--B.FullName,[ColumnName],");
            sb.AppendLine("							--[ColumnName] + ' = ' +");
            sb.AppendLine("            				CASE WHEN [Length] IN ('COMPUTED', 'MAX') ");
            sb.AppendLine("            					THEN '' ");
            sb.AppendLine("            					ELSE '+ ' + ");
            sb.AppendLine("										(CASE  ");
            sb.AppendLine("            								WHEN B.[Type] IN ('DATETIME2', 'TIME2', 'DATETIMEOFFSET') ");
            sb.AppendLine("            								OR B.[Type] IN ('VARCHAR', 'CHAR', 'TEXT') ");
            sb.AppendLine("            								OR B.[Type] IN ('VARBINARY', 'BINARY') ");
            sb.AppendLine("											OR B.[Type] IN ('NVARCHAR', 'NCHAR', 'NTEXT')");
            sb.AppendLine("											OR B.[Type] = 'DATE' ");
            sb.AppendLine("            								OR B.[Type] = 'DATETIME' ");
            sb.AppendLine("            								OR B.[Type] = 'UNIQUEIDENTIFIER' ");
            sb.AppendLine("            								OR B.[Type] = 'TIME' ");
            sb.AppendLine("											THEN  ''''''''' + ' + B.[ConvertValue] + ' + '''''',''' --+ CHAR(39) = '");
            sb.AppendLine("            								WHEN B.[Type] = 'INT' ");
            sb.AppendLine("            								OR B.[Type] = 'BIGINT' ");
            sb.AppendLine("            								OR B.[Type] = 'SMALLINT' ");
            sb.AppendLine("            								OR B.[Type] = 'TINYINT' ");
            sb.AppendLine("            								OR B.[Type] = 'BIT' ");
            sb.AppendLine("											OR B.[Type] = 'DECIMAL'");
            sb.AppendLine("            								OR B.[Type] = 'NUMERIC'");
            sb.AppendLine("											THEN B.[ConvertValue] + ' + '', '' '");
            sb.AppendLine("            								ELSE ''''''");
            sb.AppendLine("            							 END)						");
            sb.AppendLine("            					END");
            sb.AppendLine("            			FROM #TableCatalogData B");
            sb.AppendLine("            			where B.[Status] = 1");
            sb.AppendLine("            			and B.FullName = A.FullName");
            sb.AppendLine("            			group by ");
            sb.AppendLine("            				 B.[Schema]");
            sb.AppendLine("            				,B.[TableName]");
            sb.AppendLine("            				,B.[FullName]");
            sb.AppendLine("            				,B.[ColumnName]				");
            sb.AppendLine("							,B.[ColumnID]");
            sb.AppendLine("							,B.[Type]");
            sb.AppendLine("            				,B.[Length]");
            sb.AppendLine("            				,B.[ConvertValue]");
            sb.AppendLine("            			order by B.FullName,B.[ColumnID]");
            sb.AppendLine("            			FOR XML PATH('') ");
            sb.AppendLine("            		), 1, 1, '' ) AS [ColumnData]");
            sb.AppendLine("            	,STUFF((  ");
            sb.AppendLine("            			SELECT 	");
            sb.AppendLine("            				',', QUOTENAME(B.[ColumnName]) AS [text()] ");
            sb.AppendLine("            			FROM #TableCatalogData B");
            sb.AppendLine("            			where B.[Status] = 1");
            sb.AppendLine("            			and B.FullName = A.FullName");
            sb.AppendLine("            			group by ");
            sb.AppendLine("            				 B.[Schema]");
            sb.AppendLine("            				,B.[TableName]");
            sb.AppendLine("            				,B.[FullName]");
            sb.AppendLine("            				,B.[ColumnName]");
            sb.AppendLine("							,B.[ColumnID]");
            sb.AppendLine("            			order by B.FullName,B.[ColumnID]");
            sb.AppendLine("            			FOR XML PATH('') ");
            sb.AppendLine("            		), 1, 1, '' ) AS [ColumnsName]");
            sb.AppendLine("            	,STUFF((  ");
            sb.AppendLine("            			SELECT 	--B.FullName,[ColumnName],");
            sb.AppendLine("            				CASE WHEN [Length] IN ('COMPUTED', 'MAX') ");
            sb.AppendLine("            					THEN '' ");
            sb.AppendLine("            					ELSE '+ CAST(' + B.[ConvertValue] + ' AS CHAR('+ CAST(B.[Length] AS VARCHAR) +'))' ");
            sb.AppendLine("            					--ELSE '+ CAST(' + QUOTENAME(B.[ColumnName]) + ' AS CHAR('+ CAST(B.[Length] AS VARCHAR) +'))' ");
            sb.AppendLine("            					END");
            sb.AppendLine("            			FROM #TableCatalogData B");
            sb.AppendLine("            			where B.[Status] = 1");
            sb.AppendLine("            			and B.FullName = A.FullName");
            sb.AppendLine("            			group by ");
            sb.AppendLine("            				 B.[Schema]");
            sb.AppendLine("            				,B.[TableName]");
            sb.AppendLine("            				,B.[FullName]");
            sb.AppendLine("            				,B.[ColumnName]				");
            sb.AppendLine("							,B.[ColumnID]");
            sb.AppendLine("            				,B.[Length]");
            sb.AppendLine("            				,B.[ConvertValue]");
            sb.AppendLine("            			order by B.FullName,B.[ColumnID]");
            sb.AppendLine("            			FOR XML PATH('') ");
            sb.AppendLine("            		), 1, 1, '' ) AS [ColumnsNameChar]");
            sb.AppendLine("            	,STUFF((  ");
            sb.AppendLine("            			SELECT 	--B.FullName,[ColumnName],");
            sb.AppendLine("							--[ColumnName] + ' = ' +");
            sb.AppendLine("            				CASE WHEN [Length] IN ('COMPUTED', 'MAX') ");
            sb.AppendLine("            					THEN '' ");
            sb.AppendLine("            					ELSE '+ ''' + QUOTENAME([ColumnName]) + ' = '' + ' +");
            sb.AppendLine("										(CASE  ");
            sb.AppendLine("            								WHEN B.[Type] IN ('DATETIME2', 'TIME2', 'DATETIMEOFFSET') ");
            sb.AppendLine("            								OR B.[Type] IN ('VARCHAR', 'CHAR', 'TEXT') ");
            sb.AppendLine("            								OR B.[Type] IN ('VARBINARY', 'BINARY') ");
            sb.AppendLine("											OR B.[Type] IN ('NVARCHAR', 'NCHAR', 'NTEXT')");
            sb.AppendLine("											OR B.[Type] = 'DATE' ");
            sb.AppendLine("            								OR B.[Type] = 'DATETIME' ");
            sb.AppendLine("            								OR B.[Type] = 'UNIQUEIDENTIFIER' ");
            sb.AppendLine("            								OR B.[Type] = 'TIME' ");
            sb.AppendLine("											THEN  ''''''''' + ' + B.[ConvertValue] + ' + '''''',''' --+ CHAR(39) = '");
            sb.AppendLine("            								WHEN B.[Type] = 'INT' ");
            sb.AppendLine("            								OR B.[Type] = 'BIGINT' ");
            sb.AppendLine("            								OR B.[Type] = 'SMALLINT' ");
            sb.AppendLine("            								OR B.[Type] = 'TINYINT' ");
            sb.AppendLine("            								OR B.[Type] = 'BIT' ");
            sb.AppendLine("											OR B.[Type] = 'DECIMAL'");
            sb.AppendLine("            								OR B.[Type] = 'NUMERIC'");
            sb.AppendLine("											THEN B.[ConvertValue] + ' + '', '' '");
            sb.AppendLine("            								ELSE ''''''");
            sb.AppendLine("            							 END)						");
            sb.AppendLine("            					END");
            sb.AppendLine("            			FROM #TableCatalogData B");
            sb.AppendLine("            			where B.[Status] = 1");
            sb.AppendLine("            			and B.FullName = A.FullName");
            sb.AppendLine("            			group by ");
            sb.AppendLine("            				 B.[Schema]");
            sb.AppendLine("            				,B.[TableName]");
            sb.AppendLine("            				,B.[FullName]");
            sb.AppendLine("            				,B.[ColumnName]				");
            sb.AppendLine("							,B.[ColumnID]");
            sb.AppendLine("							,B.[Type]");
            sb.AppendLine("            				,B.[Length]");
            sb.AppendLine("            				,B.[ConvertValue]");
            sb.AppendLine("            			order by B.FullName,B.[ColumnID]");
            sb.AppendLine("            			FOR XML PATH('') ");
            sb.AppendLine("            		), 1, 1, '' ) AS [ColumnsNameWithoutChar]");
            sb.AppendLine("            	,'' [Select]");
            sb.AppendLine("            FROM #TableCatalogData A");
            sb.AppendLine("            where A.[Status] = 1");
            sb.AppendLine("            group by ");
            sb.AppendLine("            	 A.[Schema]");
            sb.AppendLine("            	,A.[TableName]");
            sb.AppendLine("            	,A.[FullName]");
            sb.AppendLine("            	,A.ObjectTabla");
            sb.AppendLine("            ");
            sb.AppendLine("			UPDATE #data ");
            sb.AppendLine("				SET [Select] = 'SELECT '''+  [Schema] +''' AS [Schema], '''+ [TableName] + ''' AS TableName, ''' + [FullName] +''' AS Tabla, ' + ColumnsNameWithOutChar + ' AS Datos2, ' + [ColumnData] + ' AS Datos, ''' + [ColumnsName] + ''' AS [Columns] FROM ' + [FullName] + ' ORDER BY ' + [ColumnsName]");
            sb.AppendLine("DECLARE ");
            sb.AppendLine("    @Schema [varchar](10),");
            sb.AppendLine("    @TableName [varchar](100),");
            sb.AppendLine("    @FullName [varchar](200),");
            sb.AppendLine("    @ObjectTabla [bigint],");
            sb.AppendLine("    @Data [nvarchar](max),");
            sb.AppendLine("	@Query [nvarchar](max)");
            sb.AppendLine("DECLARE POINTER CURSOR FAST_FORWARD FOR");
            sb.AppendLine("SELECT [Schema],[TableName],[FullName],[ObjectTabla],[Select] FROM #data ORDER BY [Schema],[TableName]");
            sb.AppendLine("OPEN POINTER");
            sb.AppendLine("FETCH NEXT FROM POINTER INTO @Schema,@TableName,@FullName,@ObjectTabla,@Data");
            sb.AppendLine("WHILE @@FETCH_STATUS = 0");
            sb.AppendLine("BEGIN");
            sb.AppendLine("   	");
            sb.AppendLine("	SET @Query = (SELECT 'SELECT '''+  [Schema] +''' AS [Schema], '''+ [TableName] + ''' AS TableName, ''' + [FullName] +''' AS Tabla, ' + ColumnsNameWithOutChar + ' AS Datos2, ' + [ColumnData] + ' AS Datos, ''' + [ColumnsName] + ''' AS [Columns] FROM ' + [FullName] + ' ORDER BY ' + [ColumnsName]  FROM #data WHERE [ObjectTabla] = @ObjectTabla)");
            sb.AppendLine("	");
            sb.AppendLine("	--PRINT @Query");
            sb.AppendLine("	INSERT INTO #Result	");
            sb.AppendLine("	EXEC (@Query)");
            sb.AppendLine("    FETCH NEXT FROM POINTER INTO @Schema,@TableName,@FullName,@ObjectTabla,@Data");
            sb.AppendLine("END");
            sb.AppendLine("CLOSE POINTER");
            sb.AppendLine("DEALLOCATE POINTER");
            sb.AppendLine("SELECT [Schema],[TableName],[FullName],LEFT([Data], LEN([Data]) - 1) [Data], LEFT([Data2], LEN([Data2]) - 1) [Data2], [Columns] FROM #Result");
            return sb.ToString().Replace("###", tablas);
        }

        private void LoadData()
        {
            Utilidades extras = new Utilidades();
            Tablas = extras.GetIni("DataCompare", "Tablas");
        }

        private void CompareData()
        {
            Utilidades extras = new Utilidades();

            string pathBeyond = extras.GetIni("Setup", "PathBeyon");
            string pathWinMerge = extras.GetIni("Setup", "PathWinMerge");
            string ProgramCompare = extras.GetIni("Setup", "ProgramCompare");
            string pathData = extras.GetIni("Setup", "PathCache");
            string db1Path = extras.GetIni("SetupDB1", "DataBase");
            string db2Path = extras.GetIni("SetupDB2", "DataBase");
            string objDef1 = string.Empty;
            string objDef2 = string.Empty;
            string obj = string.Empty;
            string CompareData = string.Empty;

            db1Path = string.Format(@"{0}{1}\", pathData, db1Path);
            db2Path = string.Format(@"{0}{1}\", pathData, db2Path);

            if (!Directory.Exists(db1Path))
            {
                Directory.CreateDirectory(db1Path);
            }
            if (!Directory.Exists(db2Path))
            {
                Directory.CreateDirectory(db2Path);
            }

            foreach (ListViewItem item in lwCompare.Items)
            {
                if (item.Checked)
                {
                    if (string.IsNullOrEmpty(CompareData))
                    {
                        CompareData += string.Format("'{0}'", item.Text);
                    }
                    else
                    {
                        CompareData += string.Format(",'{0}'", item.Text);
                    }
                }
            }

            try
            {
                DatabaseConnect data = new DatabaseConnect();
                lwDataTables.Items.Clear();
                List<Data> res = data.Datos(1, QueryDatos(CompareData));
                List<Data> tabla = data.Datos(1, QueryDatos(CompareData)).GroupBy(p => p.FullName).Select(g => g.First()).ToList();

                foreach (Data item in tabla)
                {
                    obj = string.Format("Data {0}.txt", item.FullName);

                    List<Data> result = res.Where(s => s.FullName == item.FullName).ToList();
                    StringBuilder sb = new StringBuilder();
                    result.Sort(delegate(Data x, Data y)
                    {
                        if (x.Value == null && y.Value == null) return 0;
                        else if (x.Value == null) return -1;
                        else if (y.Value == null) return 1;
                        else return x.Value.CompareTo(y.Value);
                    });

                    foreach (Data itemData in result)
                    {
                        sb.AppendLine(itemData.ValueByColumns.ToString());
                    }
                    System.IO.File.WriteAllText(db1Path + obj, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Errores al cargar la lista de tablas {0}", ex.Message));
            }

            try
            {
                DatabaseConnect data = new DatabaseConnect();
                lwDataTables.Items.Clear();
                List<Data> res = data.Datos(2, QueryDatos(CompareData));
                List<Data> tabla = data.Datos(2, QueryDatos(CompareData)).GroupBy(p => p.FullName).Select(g => g.First()).ToList();

                foreach (Data item in tabla)
                {
                    obj = string.Format("Data {0}.txt", item.FullName);

                    List<Data> result = res.Where(s => s.FullName == item.FullName).ToList();
                    StringBuilder sb = new StringBuilder();
                    result.Sort(delegate(Data x, Data y)
                    {
                        if (x.Value == null && y.Value == null) return 0;
                        else if (x.Value == null) return -1;
                        else if (y.Value == null) return 1;
                        else return x.Value.CompareTo(y.Value);
                    });

                    foreach (Data itemData in result)
                    {
                        sb.AppendLine(itemData.ValueByColumns.ToString());
                    }
                    System.IO.File.WriteAllText(db2Path + obj, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Errores al cargar la lista de tablas {0}", ex.Message));
            }

            Process.Start(((ProgramCompare == "1") ? pathBeyond : pathWinMerge), string.Format("\"{0}\" \"{1}\"", db1Path, db2Path));

        }
        #endregion

        #region Eventos
        private void cbAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < lwDataTables.Items.Count; i++)
            {
                lwDataTables.Items[i].Checked = cbAll.Checked;
            }
        }

        private void btAceptar_Click(object sender, EventArgs e)
        {
            Utilidades extras = new Utilidades();
            string TablasSeleccionadas = string.Empty;

            for (int i = 0; i < lwDataTables.Items.Count; i++)
            {
                if (lwDataTables.Items[i].Checked)
                {
                    if (string.IsNullOrEmpty(TablasSeleccionadas))
                    {
                        TablasSeleccionadas = lwDataTables.Items[i].Text.ToString();
                    }
                    else
                    {
                        TablasSeleccionadas += "," + lwDataTables.Items[i].Text.ToString();
                    }
                }
            }

            extras.SetIni("DataCompare", "Tablas", (string.IsNullOrEmpty(TablasSeleccionadas)) ? Tablas : TablasSeleccionadas);

        }

        private void btCancelar_Click(object sender, EventArgs e)
        {
            Utilidades extras = new Utilidades();
            extras.SetIni("DataCompare", "Tablas", Tablas);
            this.Close();
        }

        private void DataCompare_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadDataList();
            LoadDefault();
        }

        private void tcBaseDatos_Selected(object sender, TabControlEventArgs e)
        {
            TabPage CurrentTab = e.TabPage;
            lwCompare.Items.Clear();

            if (e.TabPage.Name == "tabPage2")
            {
                foreach (ListViewItem item in lwDataTables.Items)
                {
                    if (item.Checked)
                    {
                        lwCompare.Items.Add((ListViewItem)item.Clone());
                    }
                }
                lwCompare.Refresh();
            }
        }

        private void cbAllData_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < lwCompare.Items.Count; i++)
            {
                lwCompare.Items[i].Checked = cbAllData.Checked;
            }
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;
            CompareData();
            // Set cursor as default arrow
            Cursor.Current = Cursors.Default;
        }
        #endregion

        private void lwCompare_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
	            {
                    foreach (ListViewItem item in lwCompare.SelectedItems)
                    {
                        sb.AppendLine(item.Text);                        
                    }
                    Clipboard.SetText(sb.ToString());
                    ((MDIMain)this.MdiParent).StatusText = string.Format("Clipboard {0} items", lwCompare.SelectedItems.Count);
	            } 
        }

        private void lwCompare_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
