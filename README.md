# Unibox

## Building

**Note: You cannot build this as it comes due to the Screenscraper API credentials being omitted from this repo. If you wish to build it yourself, you will need to include your own screenscraper credentials by adding a specific file.**


The file containing the credentials for Screenscraper is not included in the repository. You will need to create a Directory called `Files` in Unibox\Resources and then create a file called `info.txt` within it. Then set the Build action of this file to `EmbeddedResource`

You can then run the Unibox.Admin application, enter your own screenscraper.fr API credentials and an Encryption key. Run the encryption process and copy and paste the text generated to `info.txt`.

## Known Snags

### Media Folders not being detected following changing a Platform's Name in Launchbox 

If you change the Platform name in Launchbox and choose to update media folders, the Platforms.xml file is not updated properly first time (`<PlatformFolder><Platform>` is still listed as the old name). Thus, the media folder Update system in Unibox doesn't work. 

Fix:
In order to get Platforms.xml to show the right Platform for PlatformFolders, the update process in launchbox should be:

1. Change the Platform name in Launchbox + choose to update Platform Folder names
2. Close Lauchbox.
3. Open Launchbox. Choose to edit the Platform again (you don't need to change anything). Then click OK. This will update the Platforms.xml file correctly and Unibox will then be able to detect the media folders for that Platform.
4. You can then update Platforms in Unibox.



