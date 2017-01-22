# DNSadd

This is the beginning stages of an application that will add servers back into DNS that get removed. It will read in the server names along with their IP Addresses from a text file using a space as the delimiter.

Example servers.txt:  
SERVERNAME1 192.168.1.1  
SERVERNAME2 192.168.1.2  
SERVERNAME3 192.168.1.3  
etc...  

The location of the text file is C:\test\servers.txt. Once the servers are loaded into a List(Of String), the application will perform an asynchronous ping on that list of servers. If the server name pings, the program will do nothing. If the server name fails to ping, the server name and IP will be outputted to a .CSV file (C:\test\failedpvsdns.csv). This CSV file will feed the PowerShell command to add the servers back into DNS.

The PowerShell command is: Import-CSV -Path "c:\test\failedpvsdns.csv" | ForEach-Object { dnscmd.exe DomainController.com /RecordAdd DomainAddress.com $.name A $.IP }

The last step will be to email my team's distro if any servers had to be added back to DNS using this app.
