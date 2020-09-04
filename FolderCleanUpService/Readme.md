Oublish the project to the folder


Open up a PowerShell terminal as an administrator and run the following command:

 
sc.exe create FolderCleanUpService binpath= "C:\Apps\FolderCleanUpService.exe" start= auto
This will install the service on your Windows machine.

You can put your service anywhere you want, 
I choose here to put it in the Apps folder on my c drive.
I use the start=auto flag to make sure that the service is automatically started when Windows starts. To set a description on the service, you need to execute a second command:
 

sc.exe description "FolderCleanUpService" 
"This service monitors and deletes files older than 90 days"
Press (Window + R) and run the command services.msc, then you should see a list of all your services and in that list, you should be able to see the FolderCleanUpService service. You need to manually start it the first time, but since the startup type is set to automatic, it will automatically start up itself after a reboot.