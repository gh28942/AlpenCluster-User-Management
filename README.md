# AlpenCluster User Management
A C# program that connects to a MongoDB cluster &amp; that can perform CRUD operations on it.

AlpenCluster User Manager
by GerH 2020, https://github.com/gh28942
This program manages the data of over 267000 austrian users. The user information is stored in a MongoDB Cluster (the "AlpenCluster") and the program is written in C#. 

<br><br>

![User list](screenshot/aum-scr1.jpg?raw=true "User list")
<p align="center">See users and their data in a list.</p>
<br><br>

> Login

You need to log in before you can use this program. This will create a connection to my SSL-protected cluster in the MongoDB cloud. To authenticate yourself, simply choose:

Connection -> connect to database

Then enter one of the following user credentials to connect to the DB:
        
Users & access:

"Der Franz"   rw
"P4ssw0rd"

"Der Jakob"   r
"P4ssw0rd"



> View Document
Enter the SVNR into the text field and press the button to see the information about a user. The SVNR is the Austrian social security number and the unique id in this database implementation. 
You can try out this functionalitiy with e.g. the following values: 0916191061, 3174240936, 4836270852, 7394070336 or 9806220478. 
Leading zeroes exist in a real SVNR, but since the value is stored as a variable of type long (Int64), they are not stored in the MongoDB cluster.

> Create
Create one MongoDB document, which represents a user in the database. Most values are optional, but you need to input the following values:

    - First name (Vorname)
    - Last name (Nachname)
    - SVNR (Social security number)
    - Phone number (Tel.Nr.)
    - Ländervorwahl (Country dial-in code)
    - E-mail
    - Geburtstag (Date of Birth)

Most of these values will be checked for correctness and consistency (e.g. there’s a connection between the SVNR and the DoB). A valid example user could have the following info:

    - Max
    - Mustermann
    - 3333010890
    - 0650 333 22 11
    - 43
    - maxm@example.com
    - August 1 90


> Read
Lets you see a number of users in a table. Useful to look up multiple users at the same time. 
Input e.g. the SVNRs 4983051034 and 5020060865 to see ~1000 users. You can also specify up to three search parameters on this UI. Choose the value "2015 Rolls-Royce Wraith" for a vehicle to return only users that have this kind of car, or search for users with the blood_type "O+". 

> Update
Enter the SVNR number of an existing users and some values to update user information. You can choose which values you want to change – empty text fields will simply be ignored.
Example: Enter the SVNR 3047280750 and change the street number (Straßennummer) to 22. 

> Delete
Choose a parameter and its value to delete the first user (MongoDB document) that does have this value. 

> Console
Allows you to run DB queries and diagnostic commands, e.g.:
    - ping
    - connectionStatus
    - dbstats
    - buildInfo
    - features
    - listCommands
    - whatsmyuri
    - listCollections
Additionally, you can use the commands cls, quit and exit here. 




Get the Windows Installer for the program here: https://www.mediafire.com/file/hqjfgbs5zyvu579/usermanager-setup.exe/file
