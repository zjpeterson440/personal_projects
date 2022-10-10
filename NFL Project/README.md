# NFL QB Weather Project


Extrated data with webscraping from [Pro Football Reference](https://www.pro-football-reference.com/) and used already compiled weather data from [here](https://github.com/ThompsonJamesBliss/WeatherData) which was then cross referenced with various sites for accuracy.

## Files

Imported, merged, and cleaned weather and QB data
`qb_stats_and_weather.ipynb`
* `Date` - Name of the QB
* `H/A` - Home game (H) or away game (A)
* `Opp` - Team opponent using 3 letter abreviation (Used in Pro Football Reference)
* `GameLoc ` - Location of the game (team abbreviation)
* `Result` - Game score (W - win/L - loss) 
* `Comp%` - Completion Percentage 
* `Yds1` - Yards thrown 
* `TD1` - TDs thrown 
* `Int` - Interceptions
* `Rate` - QBR (quaterback rating)
* `Tm` - What team the QB played for that game
* `DewPoint` - Average dewpoint
* `Humidity` - Average humidity
* `Precipitation` - Precipitation
* `WindSpeed` - Wind speed measured in mph
* `EstimatedCondition` - Estimated weather condition based on game time weather if there is no information
* `StadiumName` - Name of the stadium the game is played in
* `RT` - Rounded temperature of the game
* `RoofType` - Roof type (0 - Outdoor/Partially Covered, 1 - Dome)

.CSV file created after merging and cleaning DataFrames in Python. Will used in Excel for further cleaning.
`qb_for_analysis.csv`

Added a win/loss column
`add_win_loss_column.ipynb`

Created 5 and 10 degree bins for temperature rannges for each QB. Will be used in Tableau for visual analysis.
`top_30_qbs_temp_bins_w_win_percent_cleaned.csv`
* `qb_name` - Name of the QB
* `min_t` - Minimum temperature in the bin range (used for Excel formulas)
* `max_t` - Maximum temperature in the bin range (used for Excel formulas)
* `qbr ` - Quarterback Rating 
* `Cmp%` - Completion Percentage 
* `yds` - Yards thrown 
* `td` - TDs thrown 
* `int` - Interceptions 
* `dew_point` - Dew point for the game
* `humidity` - Humidity for the game
* `percip` - Ammount of precpitation
* `wind` - Wind strength in mph
* `pressure` - Atmospheric Pressure
* `game_count` - Number of games played in the temperature ranges
* `win_percent` - Win percentage of games played in the temperature ranges
* `temps` - Temperature range

