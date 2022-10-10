
#Imports
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns


#saves all files to a list
file_names = ['bike_trip_data2021_09.csv', 'bike_trip_data2021_10.csv', 'bike_trip_data2021_11.csv',
             'bike_trip_data2021_12.csv', 'bike_trip_data2022_01.csv', 'bike_trip_data2022_02.csv', 
              'bike_trip_data2022_03.csv','bike_trip_data2022_04.csv', 'bike_trip_data2022_05.csv', 
              'bike_trip_data2022_06.csv', 'bike_trip_data2022_07.csv', 'bike_trip_data2022_08.csv',]


#concats all CSV files in file_names list into one dataframe

data_all = pd.concat((pd.read_csv(i) for i in file_names)).reset_index(drop = True)



#converts dataframe to csv for further analysis
data_all.to_csv('one_year_092021_082022_bike_data.csv', index=False)



df = pd.read_csv('one_year_092021_082022_bike_data.csv')


df.info()



df.head()



#Used to determine the number of dropped tables while cleaning data
initial_length = len(df.index)
print(initial_length)



#dropped duplicate ride IDs
df = df.drop_duplicates(subset='ride_id', keep='first')



drop_dupes = len(df.index)
print(f'{initial_length - drop_dupes} rows were dropped')



#creates a heatmap of all NaN values in the DF

#set figure size
plt.figure(figsize=(10,10))

#generate heatmap
sns.heatmap(df.isnull(), cbar=False)



#all NaN values located in station_name and station_id fields
#drops all rows with NaN values
df.dropna(inplace=True)



#checks number of dropped rows
after_na = len(df.index)
print(f'{drop_dupes - after_na} rows were dropped')


#Sorts the time variable to check for outliers
df['time_hrs'].sort_values(ascending=False)



#It appears there are trips way over 24-48 hours in time as well as trips with negative time


#filters out rows where ride time was >24 hours and less than 1 minute for casual riders
#ride times greater than 24 hours as a casual biker does not fit with the "full day pass" or "single ride"
#offered to casual riders. No information given if Members can rent bikes for longer than a full day. Their
#time will be capped at 48 hours to remove significant outliers
#ride times less than a minute will filter out quick returns/unintential rentals/errors
#also filters out negative values from time errors. 

df = df[((df['time_hrs']<=24) & (df['time_min']>1) & (df['member_casual']=='casual')) | 
        ((df['time_hrs']<=48) & (df['time_min']>1) & (df['member_casual']=='member'))]



df.reset_index(inplace=True)



#checks number of rows dropped
time_filter = len(df.index)
print(f'{after_na - time_filter} rows were dropped')



#checks time values
df['time_hrs'].sort_values(ascending=False)



#checks casual time values to make sure filter worked as intended
df[df['member_casual']=='casual']['time_hrs'].sort_values(ascending=False)


#Creates a numeric month column (1=Jan, 2=Feb)
df['month'] = pd.to_datetime(df['started_at']).dt.month




df.drop('index', axis=1, inplace=True)




df.to_csv('one_year_bike_data_cleaned.csv', index=False)


#Basic Analysis

ride_time_avg_cas= df[df['member_casual']=='casual']['time_min'].mean()
ride_time_median_cas = df[df['member_casual']=='casual']['time_min'].median()
ride_time_avg_member = df[df['member_casual']=='member']['time_min'].mean()
ride_time_median_member = df[df['member_casual']=='member']['time_min'].median()

print(f'Average ride time for a casual rider is {round(ride_time_avg_cas,1)} minutes')
print(f'Median ride time for a casual rider is {round(ride_time_median_cas,1)} minutes')
print(f'Average ride time for a member is {round(ride_time_avg_member,1)} minutes')
print(f'Median ride time for a member is {round(ride_time_median_member,1)} minutes')




cas_ride_num = df[df['member_casual']=='casual']['ride_id'].count()
mem_ride_num = df[df['member_casual']=='member']['ride_id'].count()
print(f'Total number of casual bike rides is {cas_ride_num}')
print(f'Total number of member bike rides is {mem_ride_num}')



weekend = [1,7]
weekend_casual = df[(df.day_of_week.isin(weekend))&(df['member_casual']=='casual')]['ride_id'].count()
weekend_member = df[(df.day_of_week.isin(weekend))&(df['member_casual']=='member')]['ride_id'].count()
print(f'Number of casual bike rides on the weekend is {weekend_casual}')
print(f'Number of member bike rides on the weekend is {weekend_member}')



winter_months = [1,2,12]
winter_casual = df[(df.day_of_week.isin(winter_months))&(df['member_casual']=='casual')]['ride_id'].count()
winter_member = df[(df.day_of_week.isin(winter_months))&(df['member_casual']=='member')]['ride_id'].count()
print(f'Number of casual bike rides in winter months is {winter_casual}')
print(f'Number of member bike rides in winter months is {winter_member}')




summer_months = [6,7,8]
summer_casual = df[(df.day_of_week.isin(summer_months))&(df['member_casual']=='casual')]['ride_id'].count()
summer_member = df[(df.day_of_week.isin(summer_months))&(df['member_casual']=='member')]['ride_id'].count()

print(f'Number of casual bike rides in summer months is {summer_casual}')
print(f'Number of member bike rides in summer months is {summer_member}')






