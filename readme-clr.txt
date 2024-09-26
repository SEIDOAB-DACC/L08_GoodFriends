To create the L07_GoodFriends to L10_GoodFriends

1. With Terminal in folder .scripts 
   macOs run: ./database-rebuild-all.sh
   Windows run: .\database-rebuild-all.ps1
   Ensure no errors from build, migration or database update

2. From Azure Data Studio you can now connect to the database
   Use connection string from user secrets:
   connection string corresponding to key
   "SQLServer-goodfriendsefc-docker-sysadmin"

   Verify application execution
   The only controller implemented is Health->Heartbeat

3. Run AppGoodFriendsWebApi with or without debugger
   Without debugger: Opeb a Terminal in folder AppGoodFriendsWebApi run: 
   dotnet run -lp https 

4. Use endpoint Admin->Seed to seed the database

