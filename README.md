NWPS Installer App User Guide
	Designed to aide in installing/uninstalling/Triaging of NWPS Mobile, CAD, MSP Records
	The Application is designed to start in ADMIN mode (as if it was ran with elevated permissions)
		NOTE: ON RARE OCCASIONS YOU MAY NEED TO RUN AS A LOCAL ADMIN (LEFT CNTRL + SHIFT + RIGHT CLICK on application and select "Run as a Different User and log in)
		      This is dependent on the specific machine/account you are logged in as, I have only dealt with this once in three years.

Table of Contents
1.	Install/Uninstall Options
2.	Install/Uninstall Customs Options
3.	Configure Updater Util
4.	Pre Req Checker
5.	NWPSInstallLog.txt file
6.	NWPSInstallApp.xml file

1.	Install/ Uninstall Options
a.	64 Bit Machine Vs. 32 bit machine
i.	The client is designed to detect whether the machine is 64bit or 32 bit. Of course since these are checkboxes you CAN change which version of the clients you want to install. I do not recommend you attempt to install 64bit and 32bit at the same time.
ii.	These check boxes are very important as the ENTIRE client depends on this to know where and what to modify/install/uninstall.
b.	Uninstall Mobile Client and Pre Reqs, Install Mobile Client Pre Reqs, and *Both*
i.	Uninstall Mobile Client and Pre Reqs
1.	Uninstalls Mobile ONLY, removes mobile entries from the Updater, removes pre Reqs, restarts machine
2.	Typically used when pre Reqs are corrupt and need to blow mobile away.
ii.	Install Mobile Client Pre Reqs
1.	Installs Pre Reqs for Mobile ONLY, Sets Folder permissions, adds updater entries, restarts machine
2.	Typically used on a machine was never installed or new clients.
iii.	Uninstall/Re-Install Mobile Pre Reqs
1.	this will remove everything related to mobile like above without restarting machine.
2.	after this will install everything related to move like above but will restart machine after the process is done.
3.	typically used with older clients that are upgrading.
4.	NOTE: Currently supports upgrading from 10.x to anything above.
c.	File Path to Pre Req or Network Path to IMS Pre Req Folder AND Copy Button
i.	This will download All Pre Reqs Required to install Mobile, CAD, and MSP or will download all files/folders within the end folder
1.	if end location is from IMS: this will download all pre Reqs in a known location (supports 18.2 and Up)
A.	MSP Environment it is usually: \\MSPServerName\
B.	RMS Environment it is usually: \\CADServerName\
2.	if end location is a flash drive/network folder: this will download all files/folders
A.	I recommend if you/the client are going to keep your own collection of the pre Reqs per version. Keep the pre Reqs in one folder not in multiple. My T:\Davitt has an example.
ii.	The Copy Portion is supposed to allow you configure what you want to do.
1.	If you want to configure the Updater while the copy portion is happening please feel free to do so. Likewise, for customizing what you want to install/uninstall.
2.	The client NEEDS the pre Reqs locally to install so if the download throughput is not high *fast* then this will take a while.

2.	Install/Uninstall Custom Options
a.	Uninstall Options
1.	Depending on what you specify the client will uninstall said Pre Req
2.	This will check the pre req checker tab, if the status of a pre req is "Uninstalled" then the app will skip said pre req.
b.	Install Options
1.	Depending on what you the client will run/install said pre req
2.	This will check the pre req checker tab, if the status of a pre req is "Installed" then the app will skip said pre req
c.	Typical Mobile Client Triage
1.	Depending on what you specify the client will do the typical initial triage for mobile
2.	If you specify ANY of the addon utilities or the Ublox Work around the client will look for the NWS Addons folder to be in C:\Temp\MobileInstaller folder
A.	If this is not present you will be prompted with a download form for this folder.
B.	If it is present, then the process will start/run what is required. 
C.	T:\Davitt has the NWS Addons Folder you need

3.	Configure Updater Util
a.	Generate Button and Number
i.	Type the number you need in the text box directly to the right of the "Generate" Button
ii.	Will create the number of ORI/ FDID fields as requested
a.	Type in "10" and the generate button will create 10 ORI and FDID text boxes
b.	Mobile Server Name
i.	This is the name/IP of the mobile server the Update will look to for updates
c.	Police Client/Fire Client/ Merge Client
i.	Depending on which client is selected, said client will be added into the updater file to come down in the Updater.
d.	ORIs/FDIDs
1.	Depending on which ORIs and/or FDIDs are entered into the Textboxes, they will be added to the Updater file to come down in the Updater

4.	Pre Req Checker
a.	Pre Reqs and Status
i.	Each Pre Req that can be tracked by name has it's status set/Updated here.
ii.	This is not saved across sessions using the application *close and reopen*
b.	Interoperability
i.	If another portion of the Application modifies the status of a tracked pre req it's status is set here.
1.	Installed
2.	Uninstalled
ii.	if the status is known (installed or uninstalled) the pre req checker will skip that specific check.
1.	The other portions of the application ALSO check this tab to save time on doing repeated steps.

5.	NWPSAPPLog.txt File
a.	Designed to keep track of everything the application does at the date and time it was completed.
i.	What files were downloaded
ii.	What was installed/uninstalled and if it was successful
iii.	What folders or files were modified
iv.	If the file does exist when the program starts up one will be created in the same folder.
b.	The Primary way errors or exceptions are handled to ensure the stability of the program
i.	Exception stack traces are passed to this file and are written in full.
ii.	Depending on the Error code there may be a translation from Error Code to English to better take care of issue.

6.	NWPSInstallApp.xml file
a.	Designed to retain pre req download/copy folder location for subsequent use
b.	Designed to retain Configure Updater Util tab configuration for subsequent use
