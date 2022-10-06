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
	memeber_casual VARCHAR(55))


TRUNCATE TABLE bike_data
GO

BULK INSERT bike_data
FROM 'C:\Users\Admin\Desktop\Bike data\one_year_092021_082022_bike_data.csv'
WITH
(
	FORMAT = 'CSV',
	FIRSTROW=2
)
GO



