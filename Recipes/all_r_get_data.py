#Step 1
import pandas as pd
import scrapy
from scrapy.linkextractors import LinkExtractor
from scrapy.spiders import CrawlSpider, Rule
from scrapy.crawler import CrawlerProcess
import requests
from bs4 import BeautifulSoup

class AllRecipesSpider(CrawlSpider):
    name = 'allrecipes'
    allowed_domains = ['allrecipes.com']
    start_urls = ['http://www.allrecipes.com/']

    rules = (
        Rule(LinkExtractor(allow=(r'/recipe/',)), callback='parse_item', follow=True),
        Rule(LinkExtractor(allow=(r'/instant',)), callback='parse_item', follow=True),
        Rule(LinkExtractor(allow=(r'-recipe-',)), callback='parse_item', follow=True),
    )

    def __init__(self, *args, **kwargs):
        super(AllRecipesSpider, self).__init__(*args, **kwargs)
        self.seen_urls = set()  # Initialize an empty set to store seen URLs

    def parse_item(self, response):
        url = response.url
        if url not in self.seen_urls:  # Check if the URL has already been seen
            self.seen_urls.add(url)  # Add the URL to the set of seen URLs
            item = {'url': url}
            yield item

process = CrawlerProcess(settings={
    'USER_AGENT': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3',
    'FEED_FORMAT': 'csv',
    'FEED_URI': 'C:/Users/Admin/Desktop/try_me/allrecipes_links.csv',
    'LOG_LEVEL': 'DEBUG',
    'DEPTH_LIMIT': 7,
    'CLOSESPIDER_TIMEOUT': 600  # 30 minutes time limit
})

process.crawl(AllRecipesSpider)
process.start()


import pandas as pd

df = pd.read_csv('allrecipes_links.csv')

df = df.drop_duplicates()

def is_recipe(check):
    # Check if any of the substrings is in 'check'
    if '/recipe/' in check or '-recipe-' in check:
        return True
    else:
        return False

df['is_recipe'] = df['url'].apply(is_recipe)

df = df[df['is_recipe']==True]


###################################

df2 = df.drop(columns = ['is_recipe'])



#Creating Chunks (Step 2)
step_size = 5000

for start in range(0, len(df2), step_size):
    end = start + step_size
    # Slice the DataFrame
    smaller_df = df2.iloc[start:end]
    # Save each chunk to a CSV file
    filename = f'data_chunk_{start // step_size + 1}.csv'  # Naming each file uniquely
    smaller_df.to_csv(filename, index=False)  # Saving the file without the index

chunk_number = 1  # The chunk to process
filename = f'data_chunk_{chunk_number}.csv'
chunk_df = pd.read_csv(filename)




#######################

def fetch_and_parse(url):
    try:
        response = requests.get(url)
        if response.status_code == 200:
            return BeautifulSoup(response.content, 'html.parser')
    except Exception as e:
        print(f"Error fetching {url}: {e}")
    return None

def get_cook_info(soup):
    recipe_details = []
    if soup:
        items = soup.find_all('div', class_='mntl-recipe-details__item')
        for item in items:
            label = item.find('div', class_='mntl-recipe-details__label').get_text(strip=True).rstrip(':')
            value = item.find('div', class_='mntl-recipe-details__value').get_text(strip=True)
            recipe_details.append({label: value})
    return recipe_details

def get_ingredients(soup):
    ingredients = []
    if soup:
        containers = soup.find_all('div', class_='comp mntl-structured-ingredients')
        for container in containers:
            for item in container.find_all('li', class_='mntl-structured-ingredients__list-item'):
                quantity = item.find('span', {'data-ingredient-quantity': 'true'}).get_text(strip=True) if item.find('span', {'data-ingredient-quantity': 'true'}) else None
                unit = item.find('span', {'data-ingredient-unit': 'true'}).get_text(strip=True) if item.find('span', {'data-ingredient-unit': 'true'}) else None
                name = item.find('span', {'data-ingredient-name': 'true'}).get_text(strip=True) if item.find('span', {'data-ingredient-name': 'true'}) else None
                ingredients.append({'quantity': quantity, 'unit': unit, 'name': name})
    return ingredients

def get_nutrition(soup):
    nutrition_info = []
    if soup:
        nutrition_table = soup.find('table', class_='mntl-nutrition-facts-label__table')
        if nutrition_table:
            servings_header = nutrition_table.find('tr', class_='mntl-nutrition-facts-label__servings')
            calories_header = nutrition_table.find('tr', class_='mntl-nutrition-facts-label__calories')
            if servings_header:
                servings_full_text = servings_header.get_text(separator=" ", strip=True)
                servings_parts = servings_full_text.split()
                servings_label = " ".join(servings_parts[:-1])
                servings_number = servings_parts[-1]
                nutrition_info.append({servings_label: servings_number})
            if calories_header:
                calories_full_text = calories_header.get_text(separator=" ", strip=True)
                calories_parts = calories_full_text.split()
                calories_label = " ".join(calories_parts[:-1])
                calories_number = calories_parts[-1]
                nutrition_info.append({calories_label: calories_number})
            for nut in nutrition_table.find_all('td'):
                nutrition_full = nut.get_text(separator=" ", strip=True)
                parts = nutrition_full.split()
                nut_label = " ".join(parts[:-1])
                nut_number = parts[-1]
                nut_dict = {nut_label: nut_number}
                if nut_label != '':
                    nutrition_info.append(nut_dict)
    return nutrition_info

def get_ratings(soup):
    rating_dict = {}
    if soup:
        ratings = soup.find('div', class_='comp mntl-recipe-review-bar recipe-review-bar mntl-block has-ratings has-comments js-recipe-review-bar')
        if ratings:
            overall_rating = ratings.find('div', class_='comp mntl-recipe-review-bar__rating mntl-text-block type--squirrel-bold').get_text(strip=True)
            num_ratings = ratings.find('div', class_='comp mntl-recipe-review-bar__rating-count mntl-text-block type--squirrel').get_text(strip=True).replace('(','').replace(')','')
            rating_dict = {'rating': overall_rating, 'num_ratings': num_ratings}
    return rating_dict if rating_dict else "No Ratings"

def get_tags(soup):
    tags = []
    if soup:
        location = soup.find('div', class_='loc article-header')
        if location:
            a_tags = location.find_all('a', href=True)
            for a_tag in a_tags:
                href = a_tag['href']
                if href.startswith('https://'):
                    tag = href.rstrip('/').split('/')[-1]
                    tags.append(tag)
    return tags

def get_title(soup):
    if soup:
        h1_tag = soup.find('h1')
        if h1_tag:
            return h1_tag.get_text(strip=True)
    return "Error"

def process_data(url):
    soup = fetch_and_parse(url)
    if not soup:
        return {
            'title': "Error",
            'tags': "Error",
            'ratings': "Error",
            'nutrition': "Error",
            'ingredients': "Error",
            'cook_info': "Error"
        }

    return {
        'title': get_title(soup),
        'tags': get_tags(soup),
        'ratings': get_ratings(soup),
        'nutrition': get_nutrition(soup),
        'ingredients': get_ingredients(soup),
        'cook_info': get_cook_info(soup)
    }

def main(chunk_number):
    filename = f'data_chunk_{chunk_number}.csv'
    df_chunk = pd.read_csv(filename)
    processed_data = [process_data(url) for url in df_chunk['url']]
    processed_df = pd.DataFrame(processed_data)
    final_df = pd.concat([df_chunk, processed_df], axis=1)
    final_df.to_csv(f'final_data_chunk_{chunk_number}.csv', index=False)
    return final_df

if __name__ == "__main__":
    chunk_number = 9  # Set Chunk Number
    final_df = main(chunk_number)
    print(f"Processing of chunk {chunk_number} completed. Data saved to 'final_data_chunk_{chunk_number}.csv'.")