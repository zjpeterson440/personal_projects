USE bike_sharing

IF OBJECT_ID('bike_data') IS NOT NULL DROP TABLE bike_data
GO

CREATE TABLE bike_data
	(ride_id VARCHAR(100),
	rideable_type VARCHAR(100),
	started_at DATETIME,
	ended_at DATETIME,
	start_station_name VARCHAR(100),
	start_station_id VARCHAR(100),
	end_station_name VARCHAR(100),
	end_station_id VARCHAR(100),
	start_lat FLOAT,
	start_long FLOAT,
	end_lat FLOAT,
	end_long FLOAT,
	memeber_casual VARCHAR(55),
	trip_time VARCHAR(25),
	time_min FLOAT,
	time_hrs FLOAT,
	day_of_week TINYINT,
	[month] TINYINT)


TRUNCATE TABLE bike_data
GO

BULK INSERT bike_data
FROM 'C:\Users\Admin\Desktop\Google Cert\Bike data\one_year_bike_data_cleaned.csv'
WITH
(
	FORMAT = 'CSV',
	FIRSTROW=2
)
GO

--SQL can't convert times over 23:59:59 to TIME. Column was removed in favor of min/hr duration columns
ALTER TABLE bike_data
DROP COLUMN trip_time

--Check column names
SELECT name 
FROM sys.columns 
WHERE object_id = OBJECT_ID('bike_data')

--Check data
 SELECT TOP 10 * FROM bike_data

 SELECT TOP 10
 COUNT(*)
 ,started_at
 FROM bike_data
 GROUP BY started_at
 ORDER BY COUNT(*) DESC

SELECT CAST(started_at As date)
FROM bike_data

--Number of Rides for each type of rider
SELECT
	COUNT(*) AS number_rides
	,memeber_casual
FROM bike_data
GROUP BY memeber_casual
--2.6 million rides for members, 1.85 million rides for casual riders


SELECT
	COUNT(*) AS number_rides
	,memeber_casual
FROM bike_data
GROUP BY memeber_casual

--Most popular bike types
SELECT
	COUNT(*)
	,rideable_type
FROM bike_data
GROUP BY rideable_type
--Classic bike at 2.8 million rides, electric bikes in second with 1.4 million, followed by docked bikes at 200,000




--Lets find the 10 most popular dates of the year for rides for casual riders
SELECT TOP 10
	COUNT(*) AS 'Number of Rides'
	,CAST(started_at As date) AS 'Date'
	,DATENAME(WEEKDAY, started_at) AS 'Day of the Week'
FROM bike_data
WHERE memeber_casual = 'casual'
GROUP BY CAST(started_at As date), DATENAME(WEEKDAY, started_at)
ORDER BY [Number of Rides] DESC
			--Results show a lot of weekend days in May, June, and July.
			--There are several dates in September. The top date is a holiday weekend (Labor Day weekend) and the only weekday (2021-09-06) is Labor Day

SELECT AVG(number_of_rides)
FROM (
SELECT TOP 10
		COUNT(*) AS number_of_rides
FROM bike_data
WHERE memeber_casual = 'casual'
GROUP BY CAST(started_at As date)
ORDER BY number_of_rides DESC) AS A

--Least popular days for casual riders
SELECT TOP 10
	COUNT(*) AS 'Number of Rides'
	,CAST(started_at As date) AS 'Date'
	,DATENAME(WEEKDAY, started_at) AS 'Day of the Week'
FROM bike_data
WHERE memeber_casual = 'casual'
GROUP BY CAST(started_at As date), DATENAME(WEEKDAY, started_at)
ORDER BY [Number of Rides] ASC
		--All days are weekdays and in Janurary or Febuary


--10 most popular dates of the year for rides for members
SELECT TOP 10
	COUNT(*) AS 'Number of Rides'
	,CAST(started_at As date) AS 'Date'
	,DATENAME(WEEKDAY, started_at) AS 'Day of the Week'
FROM bike_data
WHERE memeber_casual = 'member'
GROUP BY CAST(started_at As date), DATENAME(WEEKDAY, started_at)
ORDER BY [Number of Rides] DESC

--AVG
SELECT AVG(number_of_rides)
FROM (
SELECT TOP 10
		COUNT(*) AS number_of_rides
FROM bike_data
WHERE memeber_casual = 'member'
GROUP BY CAST(started_at As date)
ORDER BY number_of_rides DESC) AS A
		--Highest number of rides are also in the May-September range, however they are on weekedays (particularly wednesday)
		--And the number of rides is significantly lower for popular days even though members have more rides (12,474 avg for members, 16,401 avg for casual riders)
		--This shows that member rides are more spread out throughout the year while casual riders are more concentrated at the top. Lets confirm this with the next query

--Least popular days
SELECT TOP 10
	COUNT(*) AS number_of_rides
	,CAST(started_at As date) AS 'Date'
	,DATENAME(WEEKDAY, started_at) AS day_of_the_week
FROM bike_data
WHERE memeber_casual = 'member'
GROUP BY CAST(started_at As date), DATENAME(WEEKDAY, started_at)
ORDER BY number_of_rides ASC
		--As hypothesized in the previous query, lowest number of rides for members is much higher than casual riders




