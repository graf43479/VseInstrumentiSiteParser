--Select count(*) from Product /*621*/
--Select count(distinct(code)) from Product 
--union all
--select count(*) from Price
--delete  from Statistic
--delete  from Product

--Select count(distinct(ProductID)) from Price
SELECT * FROM Statistic
--DELETE FROM Statistic
WHERE CreationDate='2019-06-28'

--select * from Statistic

--alter table Price 
--Drop column CreationDate

--alter table Price 
--Add CreationDate datetime2

--UPDATE Statistic
--SET CreationDate=GETDATE()-1

--select GETDATE()-1

Select A.PriceValue
FROM 
	(
		select Price.ProductID, PriceValue, count(ProductID)  from Price
		group by ProductID, PriceValue
		having count(ProductID)<2
	) as A
	
--	exec sp_helpdb ToolStore
--DELETE FROM Statistic
--DELETE FROM Product


--количество товаров в группе по статистике
Select StatisticID, count(*) from Price
group by StatisticID


--количество товаров в группе по производителю
Select v.Name, count(*) from Product p inner join Vendor v on p.VendorID=v.VendorID
group by v.Name

Select * from Statistic

--Количество товаров в разбивке по датам
select s.CreationDate, count(*) from Price p
INNER JOIN Statistic s ON s.StatisticID = p.StatisticID
INNER JOIN Product pr ON pr.ProductID=p.ProductID
GROUP BY s.CreationDate

--отличия между товарами за даты
select pr.Name from Price p
--select count(*) from Price p
INNER JOIN Statistic s ON s.StatisticID = p.StatisticID
INNER JOIN Product pr ON pr.ProductID=p.ProductID
WHERE s.CreationDate='2019-07-01 00:00:00.0000000'
EXCEPT
select pr.Name from Price p
--select count(*) from Price p
INNER JOIN Statistic s ON s.StatisticID = p.StatisticID
INNER JOIN Product pr ON pr.ProductID=p.ProductID
WHERE s.CreationDate='2019-06-28'


--2019-06-21 17:46:36.1540299
--2019-06-24 09:53:06.0239009


select count(*) from Price p
INNER JOIN Product pr ON pr.Code=p.ProductID
--GROUP BY s.CreationDate
--3439


select count(*) from Price p

--15827122

select * from Product p
ORDER BY ProductID DESC
--WHERE p.Code=15827122


--количество товаров появившихся в даты
select CreationDate, count(*) 
from Product p
GROUP BY CreationDate
ORDER BY CreationDate DESC


--количество товаров обновленных в даты
select UpdateDate, count(*) 
from Product p
GROUP BY UpdateDate
ORDER BY UpdateDate DESC

update Product
set UpdateDate='2019-06-27'
where UpdateDate='2019-06-28' 


select * from Price 
where PriceValue=0
ORDER BY ProductID DESC


--Проверка на уникальность товара 
Select count(Code), count(distinct(Code)) from Product


Select * from Product
--where CreationDate!='0001-01-01'
order by UpdateDate  desc


--вывести последние добавленные товары
SELECT * 
FROM Product
WHERE CreationDate=
	(
	SELECT MAX(CreationDate)
	FROM Product
	)


--вывести последние обновленные товары
SELECT * 
FROM Product
WHERE UpdateDate=
	(
	SELECT MAX(UpdateDate)
	FROM Product
	)

--параметрический скрипт, возвращающий товары, цены на которые менялись в период отслеживания

DECLARE @cols AS NVARCHAR(MAX)
DECLARE @colsHint AS NVARCHAR(MAX)
DECLARE @colsCount AS NVARCHAR(MAX)
DECLARE @query  AS NVARCHAR(MAX)
DECLARE @query2  AS NVARCHAR(MAX)
DECLARE @query3  AS NVARCHAR(MAX)

DECLARE @queryStatisticCount  AS NVARCHAR(MAX)

SET @queryStatisticCount = 'SELECT TOP 5 *					
					FROM Statistic
					ORDER BY CreationDate DESC'


SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(convert(date, c.CreationDate)) 
            FROM 
				(
					SELECT TOP 5 *
					--SELECT * 
					FROM Statistic
					ORDER BY CreationDate DESC
				) c
			--ORDER BY c.CreationDate
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');

		--print @cols

		SET @colsHint = STUFF((SELECT distinct ',' + QUOTENAME(convert(date, c.CreationDate)) + ' AS Dates' 
            FROM 
				(
					SELECT TOP 5 *
					--SELECT * 
					FROM Statistic
					ORDER BY CreationDate DESC
				) c
			--ORDER BY c.CreationDate
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');

SET @colsCount = STUFF((SELECT distinct ',' + 'COUNT('+QUOTENAME(convert(date, c.CreationDate)) + ') as ' + QUOTENAME(convert(date, c.CreationDate)) 
            FROM 
			(
					SELECT TOP 5 *
					--SELECT * 
					FROM Statistic
					ORDER BY CreationDate DESC
			) c
				
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');

		

set @query = 'SELECT Vendor, Name, ' + @cols + ' 
            FROM 
			(
				SELECT v.Name as Vendor, p.Name as Name, s.CreationDate as CreationDate, pr.PriceValue as PriceValue
				FROM Price pr
				INNER JOIN Product p ON pr.ProductID=p.ProductID
				INNER JOIN Statistic s ON s.StatisticID=pr.StatisticID
				INNER JOIN Vendor v ON v.VendorID=p.VendorID
			) x
            pivot 
            (                 
				AVG(PriceValue)
                for CreationDate in (' + @cols + ')
            ) p '			

--print @query
--execute(@query)

SET @query2 = 'SELECT Vendor, ' + @colsCount + ' 
				FROM (' + @query + ') m
				GROUP BY Vendor'
--print @query2 
--execute(@query)


	DECLARE @customWhereA AS NVARCHAR(MAX)	--starterExpression
	DECLARE @customWhereB AS NVARCHAR(MAX)  --final expression
	DECLARE @customWhereC AS INT  --count
	DECLARE @customWhereNULL AS NVARCHAR(MAX)  --null checker

	SET @customWhereC = (SELECT distinct(count(c.CreationDate)) 
								 FROM 
									(
											SELECT TOP 5 *
											--SELECT * 
											FROM Statistic
											ORDER BY CreationDate DESC
									) c
								FOR XML PATH(''), TYPE
							).value('.', 'INT')
			
			DECLARE @customWhereNULLtmp AS NVARCHAR(MAX)  
			SET @customWhereNULLtmp = STUFF((SELECT distinct  + QUOTENAME(convert(date, c.CreationDate)) + ' <> NULL AND ' 
             FROM 
			(
						SELECT TOP 5 *
					--SELECT * 
					FROM Statistic
					ORDER BY CreationDate DESC
			) c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,0,'');
				
			
	SET @customWhereNULL = SUBSTRING(@customWhereNULLtmp,0, len(@customWhereNULLtmp)-3)
				--print  @customWhereNULL 

	SET @customWhereA = STUFF((SELECT distinct  + QUOTENAME(convert(date, c.CreationDate)) + ' OR ' + QUOTENAME(convert(date, c.CreationDate)) + '<>'  
             FROM 
			(
						SELECT TOP 5 *
					--SELECT * 
					FROM Statistic
					ORDER BY CreationDate DESC
			) c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');

	SET @customWhereB = SUBSTRING(@customWhereA, 16  , len(@customWhereA)-32);    --27 --6
	
	--print @customWhereB 

--	SET @query3 = 'SELECT Vendor, Name, ' + @cols + ' 
	SET @query3 = 'SELECT Vendor, Name, ' + @colsHint + ' 
				FROM (' + @query + ') m
				WHERE ' + @customWhereB  + ' AND ' + @customWhereNULL	+ '				
				ORDER BY Vendor'
				
	
	print @query3
	execute(@query3)




	Select * From Price pr join Product p on pr.ProductID=p.ProductID join Statistic s on pr.StatisticID=s.StatisticID
	WHERE p.Code=15748538 OR p.Code=15774003 or p.Code=15746480
	order by p.Code, s.CreationDate


------------------------------------------------------------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------------------------------------------------------------------------------------------------------------


	--[2019-07-01],[2019-07-02],[2019-07-03],[2019-07-04],[2019-07-05]

	
DECLARE @cols AS NVARCHAR(MAX)
DECLARE @colsHint AS NVARCHAR(MAX)

	SET @cols = STUFF((SELECT distinct 'Date' + QUOTENAME(convert(date, c.CreationDate)) 
            FROM 
				(
					SELECT TOP 5 *
					--SELECT * 
					FROM Statistic
					ORDER BY CreationDate DESC
				) c
			--ORDER BY c.CreationDate
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');



		SET @colsHint = STUFF((SELECT distinct ',' + QUOTENAME(convert(date, c.CreationDate)) + ' AS Date' 
            FROM 
				(
					SELECT TOP 5 *
					--SELECT * 
					FROM Statistic
					ORDER BY CreationDate DESC
				) c
			--ORDER BY c.CreationDate
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');



		print @colsHint

	DECLARE @a as int
	DECLARE @i as int
	--DECLARE @s aS NVARCHAR(MAX)
	DECLARE @pos as int
	--DECLARE @resultHint as NVARCHAR(MAX)

	Set @a = 0
	--Set @s = ''
	Set @pos=0
	--Set @resultHint  = @colsHint
	Set @i = 0;

	while(@a<5)
		BEGIN
			SET  @i = CHARINDEX('Date', @colsHint, @pos)
			SET @pos = @i+1
			print @i
			if(@i<>0)
				BEGIN
					--SET @colsHint = STUFF(@colsHint, @pos-2, 2, CONVERT(NVARCHAR(MAX), @a)+',')
					SET @colsHint = STUFF(@colsHint, @pos-1, 4, 'Date' +CONVERT(NVARCHAR(MAX), @a))
				END
			--SET @colsHint = REPLACE(@colsHint, )
			--Set @s = @s+ 'Date' + CONVERT(NVARCHAR(MAX), @a) + ','
			--Set @a=@a+1
		--	SET @colsHint = STUFF(@colsHint, @pos-2, 2, CONVERT(NVARCHAR(MAX), @a)+',')
		--	print  CONVERT(NVARCHAR(MAX), @a)+': ' +@colsHint 
			--SET @pos=@pos+1
			SET @a = @a+1
		END

		--SET @s=SUBSTRING(@s, 0, Len(@s))
		--print @s
		print @colsHint
		--print @resultHint
	------------------------------------------------------------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @cols AS NVARCHAR(MAX)
DECLARE @query  AS NVARCHAR(MAX)
DECLARE @query3  AS NVARCHAR(MAX)

DECLARE @dateStart AS NVARCHAR(MAX)
DECLARE @dateEnd AS NVARCHAR(MAX)

SET @cols = QUOTENAME(convert(date, GETDATE()-1)) + ',' + QUOTENAME(convert(date, GETDATE()))
print @cols

SET @dateStart = '[' + convert(NVARCHAR(MAX), convert(date, GETDATE()-1))  + ']'
SET @dateEnd = '[' + convert(NVARCHAR(MAX), convert(date, GETDATE())) + ']'

set @query = 'SELECT Vendor, Name, ' + @cols + ' 
            FROM 
			(
				SELECT v.Name as Vendor, p.Name as Name, s.CreationDate as CreationDate, pr.PriceValue as PriceValue
				FROM Price pr
				INNER JOIN Product p ON pr.ProductID=p.ProductID
				INNER JOIN Statistic s ON s.StatisticID=pr.StatisticID
				INNER JOIN Vendor v ON v.VendorID=p.VendorID
			) x
            pivot 
            (                 
				AVG(PriceValue)
                for CreationDate in (' + @cols + ')
            ) p '			

--print @query
--execute(@query)


	--print @customWhereB 

	SET @query3 = 'SELECT Vendor, Name, ' + @cols + ' 
				FROM (' + @query + ') m
				WHERE ' + @dateStart + '<>' +@dateEnd+ 
				'AND '+@dateStart  + '<> 0 AND '+@dateEnd  + '<>0 ' +
				--'AND '+@dateStart  + '> '+@dateEnd   +
				--'AND '+@dateStart  + '> '+@dateEnd   +
				--'AND '+ISNUMERIC(@dateStart)  + '> '+ISNUMERIC(@dateEnd)   +
				--'AND ISNUMERIC('+@dateStart    + ')>' +'ISNUMERIC('+@dateEnd    + ')' +
				'AND convert(int, '+@dateStart    + ')>convert(int, '+@dateEnd    + ')*1.3'+ 
				--'AND '+@dateEnd  + '<> 0 AND '+@dateEnd  + '<>NULL ' +
				'ORDER BY Vendor'
				
	
	print @query3
	execute(@query3)




	exec GetAllStatisticPrices

	DECLARE @dateStart AS Date
	DECLARE @dateEnd AS Date
	DECLARE @paramPercent AS int
	DECLARE @paramChoosen AS bit

	SET @dateStart = GETDATE()-2
	SET @dateEnd = GETDATE() -1 
	
	exec GetTwoDaysChanges @param1=@dateStart, @param2=@dateEnd, @paramPercent=25, @paramChoosen=0

	




	Select * from Product
	WHERE IsFavorite=1

	select * from Statistic











	DROP PROCEDURE [dbo].[GetTwoDaysChanges]
	GO
	CREATE PROCEDURE [dbo].[GetTwoDaysChanges]
	@param1 date,
	@param2 date,
	@paramPercent float,
	@paramChoosen bit
AS
	BEGIN
	------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------------------------------------------------------
	DECLARE @cols AS NVARCHAR(MAX)
	DECLARE @colsNames AS NVARCHAR(MAX)
	DECLARE @query  AS NVARCHAR(MAX)
	DECLARE @query3  AS NVARCHAR(MAX)

	DECLARE @dateStart AS NVARCHAR(MAX)
	DECLARE @dateEnd AS NVARCHAR(MAX)

	DECLARE @percent AS NVARCHAR(MAX)
	DECLARE @chosen AS NVARCHAR(MAX)

	------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------------------------------------------------------
	SET @cols = QUOTENAME(convert(date, @param1)) + ',' + QUOTENAME(convert(date, @param2))
	SET @colsNames = QUOTENAME(convert(date, @param1))   + ' as DateStart, ' + QUOTENAME(convert(date, @param2)) +  ' as DateEnd '
	SET @dateStart = '[' + convert(NVARCHAR(MAX), convert(date, @param1))  + ']'
	SET @dateEnd = '[' + convert(NVARCHAR(MAX), convert(date, @param2)) + ']'
	------------------------------------------------------------------------------------------------------------	
	------------------------------------------------------------------------------------------------------------
	IF @paramPercent>=1		
		SET @percent = ' * ' + convert(NVARCHAR(MAX),1+@paramPercent/100)
	ELSE 
		SET @percent = ' '
	
	IF @paramChoosen<>0
		SET @chosen = ' WHERE p.IsFavorite = 1 '

	ELSE
			SET @chosen = ' '
			--SET @chosen = convert(NVARCHAR(MAX),@paramChoosen)
	------------------------------------------------------------------------------------------------------------	
	------------------------------------------------------------------------------------------------------------
	set @query = 'SELECT Vendor, Code, Name, ' + @cols + ' 
            FROM 
			(
				SELECT v.Name as Vendor, p.Code as Code, p.Name as Name, s.CreationDate as CreationDate, pr.PriceValue as PriceValue
				FROM Price pr
				INNER JOIN Product p ON pr.ProductID=p.ProductID
				INNER JOIN Statistic s ON s.StatisticID=pr.StatisticID
				INNER JOIN Vendor v ON v.VendorID=p.VendorID
				'+ @chosen+'
			) x
            pivot 
            (                 
				AVG(PriceValue)
                for CreationDate in (' + @cols + ')
            ) p '

	------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------------------------------------------------------
	SET @query3 = 'SELECT Vendor, Code, Name, ' + @colsNames + ' 
				FROM (' + @query + ') m
				WHERE ' + @dateStart + '<>' +@dateEnd+ 
				'AND '+@dateStart  + '<> 0 AND '+@dateEnd  + '<>0 ' +
				--'AND '+@dateStart  + '> '+@dateEnd   +
				--'AND '+@dateStart  + '> '+@dateEnd   +
				--'AND '+ISNUMERIC(@dateStart)  + '> '+ISNUMERIC(@dateEnd)   +
				--'AND ISNUMERIC('+@dateStart    + ')>' +'ISNUMERIC('+@dateEnd    + ')' +
				'AND convert(int, '+@dateStart    + ')>convert(int, '+@dateEnd    + ')'+
				@percent +
				--'AND '+@dateEnd  + '<> 0 AND '+@dateEnd  + '<>NULL ' +
				'ORDER BY Vendor'

	------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------------------------------------------------------
	execute(@query3)

	------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------------------------------------------------------
	END
	--SELECT @param1, @pa







	Select * from Product p
	WHERE p.IsFavorite !=0


	
	Select * from Statistic s
	group By YEAR(s.CreationDate), month(s.CreationDate)
	--order by s.CreationDate 




	

	exec GetAllStatisticPrices @param1=1
	exec GetAllStatisticPrices @param1=0


	exec GetAllStatisticPrices @param1=0, @param2=0, @param3=0

	exec GetMonthlyStatistic
	

	drop procedure GetMonthlyStatistic
	GO
	CREATE PROCEDURE [dbo].[GetMonthlyStatistic]
	AS
		BEGIN
			Select res.CreationDate from 
			(
				SELECT Convert(date,s.CreationDate) as CreationDate, month(CreationDate) as s1, LAG(month(CreationDate), 1,0) over (partition by  month(CreationDate), year(CreationDate) order by  CreationDate ASC) as s2
				FROM Statistic s
			) as res
			WHERE res.s1<>res.s2
		END



	--shit



	drop procedure GetAllStatisticPrices
	GO
CREATE PROCEDURE [dbo].[GetAllStatisticPrices]
	@param1 bit, --monthly/daily
	@param2 bit, --favorite only
	@param3 bit --notnulled
AS
	BEGIN
		DECLARE @cols AS NVARCHAR(MAX)
		DECLARE @colsHint AS NVARCHAR(MAX)
		DECLARE @colsCount AS NVARCHAR(MAX)
		DECLARE @query  AS NVARCHAR(MAX)
		DECLARE @query2  AS NVARCHAR(MAX)
		DECLARE @query3  AS NVARCHAR(MAX)
		DECLARE @datesCount as INT
		
		------------------------------------------------------------------------------------------------------------
		DECLARE @TEMP table (CreationDate date)
		if(@param1>0)
			BEGIN
				INSERT INTO @TEMP 
						SELECT TOP 7 s.CreationDate					
						FROM Statistic s
						ORDER BY CreationDate DESC				
			END
		ELSE
			BEGIN
				INSERT INTO @TEMP 
						exec GetMonthlyStatistic		
			END

			SET @datesCount = (Select count(*) from @TEMP)
		------------------------------------------------------------------------------------------------------------

		SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(convert(date, c.CreationDate)) 
            FROM @TEMP c
			--ORDER BY c.CreationDate
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');
		--print @cols
		------------------------------------------------------------------------------------------------------------
		------------------------------------------------------------------------------------------------------------
		SET @colsHint = STUFF((SELECT distinct ',' + QUOTENAME(convert(date, c.CreationDate)) + ' AS Date' 
            FROM @TEMP c
			--ORDER BY c.CreationDate
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');

		DECLARE @a as int
		DECLARE @i as int
		DECLARE @pos as int
	
		Set @a = 0
		Set @pos=0
		Set @i = 0;

		DECLARE @rowcount AS INT 

		IF(@datesCount<7)
			SET @rowcount = @datesCount
		ELSE
			SET @rowcount = 7

		while(@a<@rowcount)
			BEGIN
				SET  @i = CHARINDEX('Date', @colsHint, @pos)
				SET @pos = @i+1
				if(@i<>0)
					BEGIN
						SET @colsHint = STUFF(@colsHint, @pos-1, 4, 'Date' +CONVERT(NVARCHAR(MAX), @a))
					END
				SET @a = @a+1
			END


		------------------------------------------------------------------------------------------------------------
		------------------------------------------------------------------------------------------------------------
		SET @colsCount = STUFF((SELECT distinct ',' + 'COUNT('+QUOTENAME(convert(date, c.CreationDate)) + ') as ' + QUOTENAME(convert(date, c.CreationDate)) 
            FROM @TEMP c
				
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');
		------------------------------------------------------------------------------------------------------------
		set @query = 'SELECT Vendor, Name, Code, IsFavorite, ' + @cols + ' 
            FROM 
			(
				SELECT v.Name as Vendor, p.Name as Name, p.Code as Code, p.IsFavorite as IsFavorite, s.CreationDate as CreationDate, pr.PriceValue as PriceValue
				FROM Price pr
				INNER JOIN Product p ON pr.ProductID=p.ProductID
				INNER JOIN Statistic s ON s.StatisticID=pr.StatisticID
				INNER JOIN Vendor v ON v.VendorID=p.VendorID
			) x
            pivot 
            (                 
				AVG(PriceValue)
                for CreationDate in (' + @cols + ')
            ) p '		
			------------------------------------------------------------------------------------------------------------
			SET @query2 = 'SELECT Vendor, ' + @colsCount + ' 
				FROM (' + @query + ') m
				GROUP BY Vendor'

			------------------------------------------------------------------------------------------------------------
		DECLARE @constWhere AS NVARCHAR(MAX)
			DECLARE @customWhereA AS NVARCHAR(MAX)	--starterExpression
			DECLARE @customWhereB AS NVARCHAR(MAX)  --final expression
			DECLARE @customWhereC AS INT  --count
			DECLARE @customWhereNULL AS NVARCHAR(MAX)  --null checker
			------------------------------------------------------------------------------------------------------------
			SET @customWhereC = (SELECT distinct(count(c.CreationDate)) 
								 FROM @TEMP c
								FOR XML PATH(''), TYPE
							).value('.', 'INT')
			
			DECLARE @customWhereNULLtmp AS NVARCHAR(MAX)  
			SET @customWhereNULLtmp = STUFF((SELECT distinct  + QUOTENAME(convert(date, c.CreationDate)) + ' <> NULL AND ' 
             FROM @TEMP c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
			 ,1,0,'');
				
			
			SET @customWhereNULL = SUBSTRING(@customWhereNULLtmp,0, len(@customWhereNULLtmp)-3)
						--print  @customWhereNULL 

			SET @customWhereA = STUFF((SELECT distinct  + QUOTENAME(convert(date, c.CreationDate)) + ' OR ' + QUOTENAME(convert(date, c.CreationDate)) + '<>'  
					 FROM @TEMP c
					FOR XML PATH(''), TYPE
					).value('.', 'NVARCHAR(MAX)') 
				,1,1,'');

			SET @customWhereB = SUBSTRING(@customWhereA, 16  , len(@customWhereA)-32);    --27 --6
			SET @constWhere = 'WHERE 1=1'

			IF(@param2>0)
				SET @constWhere = 'WHERE IsFavorite=1 '

			IF(@param3>0)
				SET @constWhere = CONCAT(@constWhere, ' AND (',  @customWhereB, ' AND ', @customWhereNULL, ') ') 
			------------------------------------------------------------------------------------------------------------
			--SET @query3 = 'SELECT Vendor, Name, Code, IsFavorite, ' + @cols + ' 
			SET @query3 = 'SELECT Vendor, Name, Code, IsFavorite, ' + @colsHint + ' 
				FROM (' + @query + ') m ' +
				@constWhere 
				--WHERE ' + @customWhereB  + ' AND ' + @customWhereNULL	+ '				
				+ ' ORDER BY Vendor'
			------------------------------------------------------------------------------------------------------------
			execute(@query3)
			------------------------------------------------------------------------------------------------------------

	END
	--SELECT @param1, @param2
--RETURN 0






////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	


		DECLARE @param1 AS bit  --monthly/daily
		DECLARE @param2 AS bit	--favorite only
		DECLARE @param3 AS bit  --notnulled
		DECLARE @cols AS NVARCHAR(MAX)
		DECLARE @colsHint AS NVARCHAR(MAX)
		DECLARE @colsCount AS NVARCHAR(MAX)
		DECLARE @query  AS NVARCHAR(MAX)
		DECLARE @query2  AS NVARCHAR(MAX)
		DECLARE @query3  AS NVARCHAR(MAX)
		DECLARE @datesCount as INT
		
		SET @param1 = 1
		SET @param2 = 1
		SET @param3 = 1
		------------------------------------------------------------------------------------------------------------
		DECLARE @TEMP table (CreationDate date)
		if(@param1>0)
			BEGIN
				INSERT INTO @TEMP 
						SELECT TOP 7 s.CreationDate					
						FROM Statistic s
						ORDER BY CreationDate DESC				
			END
		ELSE
			BEGIN
				INSERT INTO @TEMP 
						Select res.CreationDate  from 
						(
							SELECT Convert(date,s.CreationDate) as CreationDate, month(CreationDate) as s1, LAG(month(CreationDate), 1,0) over (partition by  month(CreationDate), year(CreationDate) order by  CreationDate ASC) as s2
							FROM Statistic s
						) as res
						WHERE res.s1<>res.s2			
			END

			SET @datesCount = (Select count(*) from @TEMP)
		------------------------------------------------------------------------------------------------------------

		SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(convert(date, c.CreationDate)) 
            FROM @TEMP c
			--ORDER BY c.CreationDate
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');
		--print @cols
		------------------------------------------------------------------------------------------------------------
		------------------------------------------------------------------------------------------------------------
		SET @colsHint = STUFF((SELECT distinct ',' + QUOTENAME(convert(date, c.CreationDate)) + ' AS Date' 
            FROM @TEMP c
			--ORDER BY c.CreationDate
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');

		--print @colsHint

		DECLARE @a as int
		DECLARE @i as int
		DECLARE @pos as int
	
		Set @a = 0
		Set @pos=0
		Set @i = 0;

		DECLARE @rowcount AS INT 

		IF(@datesCount<7)
			SET @rowcount = @datesCount
		ELSE
			SET @rowcount = 7

		while(@a<@rowcount)
			BEGIN
				SET  @i = CHARINDEX('Date', @colsHint, @pos)
				SET @pos = @i+1
				if(@i<>0)
					BEGIN
						SET @colsHint = STUFF(@colsHint, @pos-1, 4, 'Date' +CONVERT(NVARCHAR(MAX), @a))
					END
				SET @a = @a+1
			END

			--print @colsHint
		------------------------------------------------------------------------------------------------------------
		------------------------------------------------------------------------------------------------------------
		SET @colsCount = STUFF((SELECT distinct ',' + 'COUNT('+QUOTENAME(convert(date, c.CreationDate)) + ') as ' + QUOTENAME(convert(date, c.CreationDate)) 
            FROM @TEMP c
				
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'');

		--print @colsCount
		------------------------------------------------------------------------------------------------------------
		set @query = 'SELECT Vendor, Name, Code, IsFavorite, ' + @cols + ' 
            FROM 
			(
				SELECT v.Name as Vendor, p.Name as Name, p.Code as Code, p.IsFavorite as IsFavorite, s.CreationDate as CreationDate, pr.PriceValue as PriceValue
				FROM Price pr
				INNER JOIN Product p ON pr.ProductID=p.ProductID
				INNER JOIN Statistic s ON s.StatisticID=pr.StatisticID
				INNER JOIN Vendor v ON v.VendorID=p.VendorID
			) x
            pivot 
            (                 
				AVG(PriceValue)
                for CreationDate in (' + @cols + ')
            ) p '
			
			--execute(@query)
			------------------------------------------------------------------------------------------------------------
			SET @query2 = 'SELECT Vendor, ' + @colsCount + ' 
				FROM (' + @query + ') m
				GROUP BY Vendor'
--execute(@query2)
			------------------------------------------------------------------------------------------------------------
			DECLARE @constWhere AS NVARCHAR(MAX)
			DECLARE @customWhereA AS NVARCHAR(MAX)	--starterExpression
			DECLARE @customWhereB AS NVARCHAR(MAX)  --final expression
			DECLARE @customWhereC AS INT  --count
			DECLARE @customWhereNULL AS NVARCHAR(MAX)  --null checker
			------------------------------------------------------------------------------------------------------------
			SET @customWhereC = (SELECT distinct(count(c.CreationDate)) 
								 FROM @TEMP c
								FOR XML PATH(''), TYPE
							).value('.', 'INT')
			
			DECLARE @customWhereNULLtmp AS NVARCHAR(MAX)  
			SET @customWhereNULLtmp = STUFF((SELECT distinct  + QUOTENAME(convert(date, c.CreationDate)) + ' <> NULL AND ' 
             FROM @TEMP c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
			 ,1,0,'');
				
			
			SET @customWhereNULL = SUBSTRING(@customWhereNULLtmp,0, len(@customWhereNULLtmp)-3)
						--print  @customWhereNULL 

			SET @customWhereA = STUFF((SELECT distinct  + QUOTENAME(convert(date, c.CreationDate)) + ' OR ' + QUOTENAME(convert(date, c.CreationDate)) + '<>'  
					 FROM @TEMP c
					FOR XML PATH(''), TYPE
					).value('.', 'NVARCHAR(MAX)') 
				,1,1,'');

			SET @customWhereB = SUBSTRING(@customWhereA, 16  , len(@customWhereA)-32);    --27 --6
			SET @constWhere = 'WHERE 1=1'

			IF(@param2>0)
				SET @constWhere = 'WHERE IsFavorite=1 '

			IF(@param3>0)
				SET @constWhere = CONCAT(@constWhere, ' AND (',  @customWhereB, ' AND ', @customWhereNULL, ') ') 
			------------------------------------------------------------------------------------------------------------
			--SET @query3 = 'SELECT Vendor, Name, Code, IsFavorite, ' + @cols + ' 
			SET @query3 = 'SELECT Vendor, Name, Code, IsFavorite, ' + @colsHint + ' 
				FROM (' + @query + ') m ' +
				@constWhere 
				--WHERE ' + @customWhereB  + ' AND ' + @customWhereNULL	+ '				
				+ ' ORDER BY Vendor'
			------------------------------------------------------------------------------------------------------------
		--print @query3
			execute(@query3)
			------------------------------------------------------------------------------------------------------------




			Select * from Product

			Select count(*) from Product
			Union ALL
			Select count(*) from Product p
			where p.[Url] is null


			Select * from Product p
			where p.UpdateDate!='2019-09-11'