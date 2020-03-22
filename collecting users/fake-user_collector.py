# -*- coding: utf-8 -*-
"""
Created on Mar 15 2020

@author: GerH
"""

from selenium import webdriver
import time
from datetime import datetime
import threading
import sys

from random import randint
                
#this thread takes fake user data from the website and puts it into a csv file
def collector(users_num, url_part, thread_num):
    
    #measure time
    start_time = time.time()
    print("starting up..." + str(start_time) + ", " + datetime.now().strftime("%d-%m-%Y_%H-%M-%S"))

    #current url (with names-country combo)
    url = "https://www.fakenamegenerator.com/gen-random-"+url_part+".php"
    
    #save time & resources by not loading images
    firefox_profile = webdriver.FirefoxProfile()
    firefox_profile.set_preference('permissions.default.image', 2)
    firefox_profile.set_preference('extensions.contentblocker.enabled', True)
    firefox_profile.set_preference('dom.ipc.plugins.enabled.libflashplayer.so', 'false')
    driver = webdriver.Firefox(firefox_profile=firefox_profile)
    
    #create & open csv file
    f = open("users_db/userdata_"+url_part+"-"+str(thread_num)+"_"+datetime.now().strftime("%d-%m-%Y_%H-%M-%S")+".csv","a")
    
    users_collected = 0
    while(users_collected<users_num): 
        driver.get(url)
        try:
            
            #output progress
            if(users_collected % 10 == 0):
                print(url_part + " " + str(thread_num) + ": " + str(users_collected)+"/"+str(users_num))
            
            #uncomment if necessary
            #sleep(1)
            
            #find data on website
            address = driver.find_element_by_class_name("address")
            entries = driver.find_elements_by_class_name("dl-horizontal")
            address_entries= address.text.split("\n")
            name_arr = address_entries[0].rsplit(' ', 1)
            street_arr = address_entries[1].rsplit(' ', 1)
            location_arr = address_entries[2].split(' ', 1)
            
            #extract user data
            first_name = name_arr[0]
            last_name = name_arr[1]
            street_name = street_arr[0]
            street_num = street_arr[1]
            area_code = location_arr[0]
            area_name = location_arr[1]
            mother_maiden_name = entries[0].text.split("\n")[1]
            geo_coord = entries[1].text.split("\n")[1]
            phone_num = entries[2].text.split("\n")[1]
            country_code = entries[3].text.split("\n")[1]
            birthday = entries[4].text.split("\n")[1]
            age = entries[5].text.split("\n")[1]
            zodiac_sign = entries[6].text.split("\n")[1]
            email = entries[7].text.split("\n")[1].replace("\nThis is a real email address. Click here to activate it!","")
            username = entries[8].text.split("\n")[1]
            password = entries[9].text.split("\n")[1]
            website = entries[10].text.split("\n")[1]
            browser_user_agent = entries[11].text.split("\n")[1]
            card_number = entries[12].text.split("\n")[1]
            card_expiry = entries[13].text.split("\n")[1]
            card_cvc2 = entries[14].text.split("\n")[1]
            company = entries[15].text.split("\n")[1]
            occupation = entries[16].text.split("\n")[1]
            height = entries[17].text.split("\n")[1].replace("\"","''")
            weight = entries[18].text.split("\n")[1]
            blood_type = entries[19].text.split("\n")[1]
            ups_tracking_num = entries[20].text.split("\n")[1]
            western_union_num = entries[21].text.split("\n")[1]
            money_gram_num = entries[22].text.split("\n")[1]
            fav_color = entries[23].text.split("\n")[1]
            vehicle = entries[24].text.split("\n")[1]
            guid = entries[25].text.split("\n")[1]
            
            svnr_year = birthday[-2:]
            svnr_month = birthday.replace(",","").split(" ")[0]
            svnr_day = birthday.replace(",","").split(" ")[1]
            
            if svnr_month == "January":
                svnr_month = 1
            elif svnr_month == "February":
                svnr_month = 2
            elif svnr_month == "March":
                svnr_month = 3
            elif svnr_month == "April":
                svnr_month = 4
            elif svnr_month == "May":
                svnr_month = 5
            elif svnr_month == "June":
                svnr_month = 6
            elif svnr_month == "July":
                svnr_month = 7
            elif svnr_month == "August":
                svnr_month = 8
            elif svnr_month == "September":
                svnr_month = 9
            elif svnr_month == "October":
                svnr_month = 10
            elif svnr_month == "November":
                svnr_month = 11
            elif svnr_month == "December":
                svnr_month = 12
            else:
                print("ERROR!!!")
                svnr_month = 10
            
            svnr = randint(0, 9999)*1000000 + int(svnr_day)*10000 + svnr_month*100 + int(svnr_year) 
            
            #print("\nBday: "+birthday)
            #print("svnr: "+str(svnr))
            
            svnr_str = str(svnr)
            if(svnr<1000000):
                svnr_str = "0"+svnr_str
            if(svnr<10000000):
                svnr_str = "0"+svnr_str
            if(svnr<100000000):
                svnr_str = "0"+svnr_str
            if(svnr<1000000000):
                svnr_str = "0"+svnr_str
            
            #Sternzeichen übersetzen, line erstellen und dazu schreiben
            zodiac_sign=zodiac_sign.replace("Cancer","Krebs").replace("Taurus","Stier").replace("Pisces","Fische").replace("Aries","Widder").replace("Libra","Waage").replace("Aquarius","Wassermann").replace("Capricorn","Steinbock").replace("Scorpio","Skorpion").replace("Virgo","Jungfrau").replace("Sagittarius","Schütze").replace("Gemini","Zwillinge").replace("Leo","Löwe")
            csv_line = "\"" + svnr_str + "\";\"" + first_name + "\";\"" + last_name + "\";\"" + street_name + "\";\"" + street_num + "\";\"" + area_code + "\";\"" + area_name + "\";\"" + geo_coord + "\";\"" + phone_num + "\";\"" + country_code + "\";\"" + birthday + "\";\"" + age + "\";\"" + mother_maiden_name + "\";\"" + zodiac_sign + "\";\"" + email + "\";\"" + username + "\";\"" + password + "\";\"" + website + "\";\"" + browser_user_agent + "\";\"" + card_number + "\";\"" + card_expiry + "\";\"" + card_cvc2 + "\";\"" + company + "\";\"" + occupation + "\";\"" + height + "\";\"" + weight + "\";\"" + blood_type + "\";\"" + ups_tracking_num + "\";\"" + western_union_num + "\";\"" + money_gram_num + "\";\"" + fav_color + "\";\"" + vehicle + "\";\"" + guid + "\"\n"
            f.write(csv_line)
            
            users_collected+=1
        except: 
            e = sys.exc_info()[0]
            print( "Couldn't load data from "+url+": \n%s" % e )
           
    #close browser window & file, and show user how long the process took to finish
    f.close()
    driver.close()
    end_time = time.time()
    elapsed_time = end_time - start_time
    print(url_part + " thread elapsed time: " + str(elapsed_time) + "(" + str(start_time) + "-" + str(end_time) + ")") 
    
#start threads
q = threading.Thread(target=collector, args=(10000, "gr-as", 1))
q.start()
r = threading.Thread(target=collector, args=(10000, "gr-as", 2))
r.start()
s = threading.Thread(target=collector, args=(10000, "gr-as", 3))
s.start()
t = threading.Thread(target=collector, args=(10000, "gr-as", 4))
t.start()
u = threading.Thread(target=collector, args=(10000, "gr-as", 5))
u.start()
v = threading.Thread(target=collector, args=(10000, "gr-as", 6))
v.start()
w = threading.Thread(target=collector, args=(10000, "gr-as", 7))
w.start()
x = threading.Thread(target=collector, args=(10000, "gr-as", 8))
x.start()
y = threading.Thread(target=collector, args=(10000, "gr-as", 9))
y.start()
z = threading.Thread(target=collector, args=(10000, "gr-as", 10))
z.start()
