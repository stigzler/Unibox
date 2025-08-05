# Design Spec

## Scraping Feature

### Overview

Scrapes from screenscraper.fr to enable accurate rom/game matching (I'm not sure how launchbox does it, but I'm guessing via fuzzy matching given metadata.xml does not contain any rom names).

### Implementation

**Lb/SS PLatform/System mapping:**

The Data folder will contain a preliminary map of LaunchBox platforms to Screenscraper systems. This will be used to determine which system to scrape for a given rom.

Will need to include abaility to match other systems in the Screenscrpaer settings section. Can get Launchbox PLatform names via `Metadata\LaunchBox.Metadata.db` - sqlite file and Screenscraper systems via my wrapper. 

Ther eis also a map of Launchbox to Screenscaper media types in the data directory. 





