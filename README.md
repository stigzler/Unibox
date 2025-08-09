# Unibox

## Building

**Note: You cannot build this as it comes due to the Screenscraper API credentials being ommited from this repo. If you wish to build it yourself, you will need to include your own screenscraper credentials by adding a specific file.**


The file containing the credentials for Screenscraper is not included in the repository. You will need to create a Directory called `Files` in Unibox\Resources and then create a file called `info.txt` within it. Then set the Build action of this file to `EmbeddedResource`

You can then run the Unibox.Admin application, enter your own screenscraper.fr API credentials and an Encryption key. Run the encryption process and copy and paste the text generated to `info.txt`.
