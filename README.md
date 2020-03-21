# AlpenCluster User Management
A C# program that connects to an AWS MongoDB cluster &amp; that can perform CRUD operations on it.

This program manages the data of over 267000 austrian users. The user information is stored in a MongoDB Cluster (the "AlpenCluster") and can be accessed through this program's User Interface. 

Get the Windows Installer for the program here: https://www.mediafire.com/file/hqjfgbs5zyvu579/usermanager-setup.exe/file
<br><br>

![User list](screenshot/aum-scr1.jpg?raw=true "User list")
<p align="center">See users and their data in a list.</p>
<br><br>

<h2>Login</h2>

You need to log in before you can use this program. This will create a connection to my SSL-protected cluster in the MongoDB cloud. To authenticate yourself, simply choose:

<i>Connection -> connect to database</i>

Then enter one of the following user credentials to connect to the DB:
        
<b>Users & access:</b>

        "Der Franz"   rw
        "P4ssw0rd"

        "Der Jakob"   r
        "P4ssw0rd"
<br><br>


<h2>View Document</h2>
Enter the SVNR into the text field and press the button to see the information about a user. The SVNR is the Austrian social security number and the unique id in this database implementation. 
You can try out this functionalitiy with e.g. the following values: 0916191061, 3174240936, 4836270852, 7394070336 or 9806220478. 
Leading zeroes exist in a real SVNR, but since the value is stored as a variable of type long (Int64), they are not stored in the MongoDB cluster.
<br><br>

![View user](screenshot/aum-scr3.jpg?raw=true "View user")
<p align="center">View data of one user.</p>
<br><br>

<h2>Create</h2>
Create one MongoDB document, which represents a user in the database. Most values are optional, but you need to input the following values:

<ul>
  <li>First name (Vorname)</li>
  <li>Last name (Nachname)</li>
  <li>SVNR (Social security number)</li>
  <li>Phone number (Tel.Nr.)</li>
  <li>Ländervorwahl (Country dial-in code)</li>
  <li>E-mail</li>
  <li>Geburtstag (Date of Birth)</li>
</ul>

Most of these values will be checked for correctness and consistency (e.g. there’s a connection between the SVNR and the DoB). A valid example user could have the following info:
<ul>
  <li>Max</li>
  <li>Mustermann</li>
  <li>3333010890</li>
  <li>0650 333 22 11</li>
  <li>43</li>
  <li>maxm@example.com</li>
  <li>August 1 90</li>
</ul>
<br><br>

<h2>Read</h2>
Lets you see a number of users in a table. Useful to look up multiple users at the same time. 
Input e.g. the SVNRs 4983051034 and 5020060865 to see ~1000 users. You can also specify up to three search parameters on this UI. Choose the value "2015 Rolls-Royce Wraith" for a vehicle to return only users that have this kind of car, or search for users with the blood_type "O+". 
<br><br>

<h2>Update</h2>
Enter the SVNR number of an existing users and some values to update user information. You can choose which values you want to change – empty text fields will simply be ignored.
Example: Enter the SVNR 3047280750 and change the street number (Straßennummer) to 22. 
<br><br>

<h2>Delete</h2>
Choose a parameter and its value to delete the first user (MongoDB document) that does have this value. 
<br><br>

<h2>Console</h2>
Allows you to run DB queries and diagnostic commands, e.g.:
<ul>
  <li>ping</li>
  <li>connectionStatus</li>
  <li>dbstats</li>
  <li>buildInfo</li>
  <li>features</li>
  <li>listCommands</li>
  <li>whatsmyuri</li>
  <li>listCollections</li>
</ul>
Additionally, you can use the commands cls, quit and exit here. 
<br>

![User manager console](screenshot/aum-scr2.jpg?raw=true "User manager console")
<p align="center">Run DB console commands in the User Manager.</p>
<br><br>



