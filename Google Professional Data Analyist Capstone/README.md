# Google Professional Data Analyist Capstone

This study was done using Excel, Tableau, Python, and SQL. While most of the cleaning and analysis was done in Python and Tableau respectivly further analysis will be conducted using Excel (Pivot Tables/Graphs) and SQL (with SSMS).
The filles will be uploaded here when completed

The report can be found on Kaggle [here](https://www.kaggle.com/code/zachpeterson/cyclistic-case-study-with-excel-python-tableau)

Tableau Dashboards can be found [here](https://public.tableau.com/app/profile/zach.j.peterson/viz/CyclisticViz_16650863175000/StationPopularity) that have more dynamic visuals not available in the report.

Original CSV files and cleaned CSV files can be found [here](https://1drv.ms/u/s!AuWf-xZpLg1aghujOSbZwiKdIVKL?e=XrKtXk)

Files:

`one_year_2021_2022_clean.py`

Variables:

* `ride_id` - Unique ID given to each ride
* `rideable_type` - Type of bike used (docked, electric, classic)
* `started_at` - Date/time the ride started. Formated in DD:MM:YYYY HH:MM:SS
* `ended_at` - Date/time the ride ended. Formated in DD:MM:YYYY HH:MM:SS
* `start_station_name` - Name of the starting station where the bike is taken out
* `start_station_id` - ID given to the station
* `end_station_name` - Name of the ending station where the bike is returned
* `end_station_id` - ID given to the return station
* `start_lat` - Latitude of the starting station
* `start_lng` - Longitude of the starting station
* `end_lat` - Latitude of the ending station
* `end_lng` - Longitude of the ending station
* `member_casual` - Whether or not the rider is a member or casual rider ('member', 'casual')
* `trip_time` - the difference in time between `started_at` and `ended_at`. Shows the length of the ride in HH:MM:SS format
* `time_min` - Ride time in minutes
* `time_hrs` - Ride time in hours
* `day_of_week` - The day of the week the ride started (1 = Sunday, 2 = Monday...)
