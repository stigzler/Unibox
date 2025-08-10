# Unibox

## Building

**Note: You cannot build this as it comes due to the Screenscraper API credentials being ommited from this repo. If you wish to build it yourself, you will need to include your own screenscraper credentials by adding a specific file.**


The file containing the credentials for Screenscraper is not included in the repository. You will need to create a Directory called `Files` in Unibox\Resources and then create a file called `info.txt` within it. Then set the Build action of this file to `EmbeddedResource`

You can then run the Unibox.Admin application, enter your own screenscraper.fr API credentials and an Encryption key. Run the encryption process and copy and paste the text generated to `info.txt`.

## Known Snags
**Media Folders not being detected following changing a Platform's Name in Launchbox**: Due to some idiosyncracies of how Launchbox handles Platform Name changes, this leaves Unibox unable to update media folders. This is due to Launchbox's inconsistent updating of Platforms.xml. For instance, if you change the PLatform name for "Atari 2600" to "Atari 2600 Renamed" and choose to update media folders, the actual folders will get updated, but not the PLatforms.xml PLatformFolders entries, with `<PlatformFolder><Platform>Atari 2600</Platform>` **not** being renamed to `<Platform>Atari 2600 Renamed</Platform>`. Also seemed incosistent on initial investigations, getting renamed at some point. Beyond this app. Choose your Platform names wisely!

