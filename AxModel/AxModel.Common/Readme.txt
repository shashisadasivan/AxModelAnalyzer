AX 2012 – Model dependencies and Install Order
http://shashidotnet.wordpress.com

Settings:
In the App.config file set the MinApplicationLayer to the minimum layer you want this to work from.
8 is ISV. 
Check your Layer table in the model database to find what number means what

Running the file:
Run AxModel.Common.exe
Database server: 
 - This is the SQL server instance. Put a .  (dot) if the database is on the same server you are running this command from
   If you hit enter at this stage, it will use the connection string in the App.Config file.
   You can use that instead

Database name (model):
 - This is the name of the database you want to connect into.
   For AX2012 (prior to R2) give the name of the Ax database
   For AX2012 R2 and beyond , give the model database


Once this runs, there will be a lot of verbose output coiming out.
At the end of the process, it outputs the sequence, after the text "Sequence...", between the "--------------------"

That can be copied to a csv file, or on an excel sheet
