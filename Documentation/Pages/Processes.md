# Processes

## Add Installation

````mermaid
graph TD;
    F[Get Installation Filepath]-->ChP{Check if Network or Local};

    ChP-->|Local|WrnL@{shape: rounded, label: Warning on Installation Textbox prompting user Are you sure?};

    ChP-->|Network|ChPxml{Does Platforms.xml};






