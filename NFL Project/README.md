# NFL QB Weather Project


Extrated data with webscraping from [Pro Football Reference](https://www.pro-football-reference.com/) and used already compiled weather data from [here](https://www.datawithbliss.com/weather-data) which was then cross referenced with other NFL weather sites for accuracy

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
* `Longitude` - Stadium coordinates (decimal format)
* `Latitude` - Stadium coordinates (decimal format)

.CSV file created after merging and cleaning DataFrames in Python `qbs_for_viz.csv`


Added two new variables in `add_win_loss_column.ipynb`
* `win_loss`- used `Result` variable to determine if the QB won or lost (1-win, 0-loss)
* `margin_victory` - used `Result` variable to determine margin of victory (negative in a loss)

Exported .CSV for further cleaning and manipulation in Excel `qb_analysis_winloss_marginvictory.csv`


Created 5 and 10 degree bins for temperature rannges for each QB. Will be used in Tableau for visual analysis.
`qb_bins_cleaned_for_viz.csv`
* `qb_name` - Name of the QB
* `min_t` - Minimum temperature in the bin range (used for Excel formulas)
* `max_t` - Maximum temperature in the bin range (used for Excel formulas)
* `qbr ` - Quarterback Rating 
* `Cmp%` - Completion Percentage 
* `yds` - Yards thrown 
* `td` - TDs thrown 
* `int` - Interceptions 
* `humidity` - Humidity for the game
* `percip` - Ammount of precpitation
* `windspeed` - Wind strength in mph
* `margin_victory` - Margin of victory for the game. Negative number is a loss, positive a win
* `win_percent` - Win percentage of games played in the temperature ranges
* `game_count` - Number of games played in the temperature ranges
* `temps` - Temperature range

