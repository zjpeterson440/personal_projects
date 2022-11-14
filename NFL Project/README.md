# NFL QB Weather Project


Extrated data with webscraping from [Pro Football Reference](https://www.pro-football-reference.com/) and used already compiled weather data from [here](https://www.datawithbliss.com/weather-data) which was then cross referenced with other NFL weather sites for accuracy

## Files

Imported, merged, and cleaned weather and QB data
`qb_stats_50_qbs.ipynb`
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

.CSV file created after merging and cleaning DataFrames in Python `qb_top_2000s_viz`


Added two new variables in `qb_top_2000s_viz` in `win_loss_margin_victory.ipynb`
* `win_loss`- used `Result` variable to determine if the QB won or lost (1-win, 0-loss)
* `margin_victory` - used `Result` variable to determine margin of victory (negative in a loss)

Saved as `qb_analysis_final.csv`

Imported `qb_analysis_final.csv` and created  10 degree bins for temperature rannges for each QB using PivotTables. Also added which college each QB played for and classified those colleges in to 4 new variables  `Cold `, `Neutral`,`Warm`, and `Indoor`.  Will be used in Tableau for visual analysis.
`qb_analysis_final_for_viz.xlsx`
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
* `home_away` - Converted the H/A variable to a numerical value
* `college class` - Categorizes each QB into the weather they had while playing for their college. `Cold `, `Neutral`,`Warm`, and `Indoor`. 
        (Temperatures are based off the school they played their final year at ex. Russell Wilson NC State -> Wisconson)

Used weather .CSV to determine average stadium temperatures for all games since 2000.

* `date` - Game date
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
* `GameLoc ` - Location of the game (team abbreviation)



