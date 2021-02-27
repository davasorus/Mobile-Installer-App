# Mobile-Installer-App
Application designed to ease the installation of Tyler Technologies NWPS clients.

NWPS Installer App
	Designed to aide in installing/uninstalling/Triaging 
	The Application is designed to start in ADMIN mode (as if it was ran with elevated permissions)
		NOTE: ON RARE OCCASIONS YOU MAY NEED TO RUN AS A LOCAL ADMIN (LEFT CNTRL + SHIFT + RIGHT CLICK on application and select "Run as a Different User and log in)
		      This is dependent on the specific machine/account you are logged in as, I have only dealt with this once in three years.

Table of Contents
	1. Install/Uninstall Options
	2. Install/Uninstall Customs Options
	3. Configure Updater Util
	4. Pre Req Checker
	5. NWPSInstallLog.txt file
	6. NWPSInstallApp.xml file


1. Install/ Uninstall Options
	A. 64 Bit Machine Vs. 32 bit machine
		1. The client is designed to detect whether the machine is 64bit or 32 bit. Of course since these are checkboxes you CAN change which version of the clients you want to install. I do not recommend you attempt to install 64bit and 32bit at the same time.
		2. These check boxes are very important as the ENTIRE client depends on this to know where and what to modify/install/uninstall.

	B. Uninstall Mobile Client and Pre Reqs, Install Mobile Client Pre Reqs, and *Both*
		1. Uninstall Mobile Client and Pre Reqs
			a. Uninstalls Mobile ONLY, removes mobile entries from the Updater, removes pre reqs, restarts machine
			b. Typically used when pre reqs are corrupt and need to blow mobile away.
		
		2. Install Mobile Client Pre Reqs
			a. Installs Pre reqs for Mobile ONLY, Sets Folder permissions, adds updater entries, restarts machine
			b. Typically used on a machine was never installed or new clients.
		
		3. Uninstall/Re-Install Mobile Pre Reqs
			a. this will remove everything related to mobile like above without restarting machine.
			b. after this will install everything related to move like above but will restart machine after the process is done.
			c. typically used with older clients that are upgrading.
			d. NOTE: Currently supports upgrading from 10.x to anything above.

	C. File Path to Pre Req or Network Path to IMS Pre Req Folder AND Copy Button
		1. This will download All Pre Reqs Required to install Mobile, CAD, and MSP or will download all files/folders within the end folder
			a. if end location is from IMS: this will download all pre reqs in a known location (supports 18.2 and Up)
				1. MSP Environment it is usually: \\MSPServerName\
				2. RMS Environment it is usually: \\CADServerName\
			b. if end location is a flashdrive/network folder: this will download all files/folders
				1. I recommend if you/the client are going to keep your own collection of the pre rqs per version. Keep the pre reqs in one folder not in multiple. My T:\Davitt has an example.
		
		2. The Copy Portion is supposed to allow you configure what you want to do.
			a. If you want to configure the Updater while the copy portion is happening please feel free to do so. Likewise for customizing what you want to install/uninstall.
			b. The client NEEDS the pre reqs locally to install so if the download throughput is not high *fast* then this will take a while.

2. Install/Uninstall Custom Options
	A. Uninstall Options
		1. Depending on what you specify the client will uninstall said Pre Req
		2. This will check the pre req checker tab, if the status of a pre req is "Uninstalled" then the app will skip said pre req.
	B. Install Options
		1. Depending on what you the client will run/install said pre req
		2. This will check the pre req checker tab, if the status of a pre req is "Installed" then the app will skip said pre req
	c. Typical Mobile Client Triage
		1. Depending on what you specify the client will do the typical initial triage for mobile
		2. If you specify ANY of the addon utilities or the Ublox Work around the client will look for the NWS Addons folder to be in C:\Temp\MobileInstaller folder
			a. If this is not present you will be prompted with a download form for this folder.
			b. If it is present then the process will start/run what is required. 
			c. T:\Davitt has the NWS Addons Folder you need

3. Configure Updater Util
	A. Generate Button and Number
		1. Type the number you need in the text box directly to the right of the "Generate" Button
		2. Will create the amount of ORI/ FDID fields as requested
			a. Type in "10" and the generate button will create 10 ORI and FDID text boxes
	B. Mobile Server Name
		1. This is the name/IP of the mobile server the Update will look to for updates
	C. Police Client/Fire Client/ Merge Client
		1. Depending on which client is selected, said client will be added into the updater file to come down in the Updater.
	D. ORIs/FDIDs
		1. Depending on which ORIs and/OR FDIDs are entered into the Textboxes, they will be added to the Updater file to come down in the Updater

4. Pre Req Checker
	A. Pre Reqs and Status
		1. Each Pre Req that can be tracked by name has it's status set/Updated here.
		2. This is not saved across sessions using the application *close and reopen*
	B. Interoperablity
		1. If another portion of the Application modifies the status of a tracked pre req it's status is set here.
			a. Installed
			b. Uninstalled
		2. if the status is known (installed or uninstalled) the pre req checker will skip that specific check.
			a. The other portions of the application ALSO check this tab to save time on doing repeated steps.

5. NWPSAPPLog.txt File
	A. Designed to keep track of everything the application does at the date and time it was completed.
		1. What files were downloaded
		2. What was installed/unstalled and if it was successful
		3. What folders or files were modified
		4. If the file does exist when the program starts up one will be created in the same folder.
	B. The Primary way errors or exceptions are handled to ensure the stabilility of the program
		1. Exception stack traces are passed to this file and are written in full.
		2. Depending on the Error code there may be a translation from Error Code to english to better take care of issue.

6. NWPSInstallApp.xml file
	A. Designed to retain pre req download/copy folder location for subsiquint use
	B. Designed to retain Configure Updater Util tab configuration for subsiquint use
