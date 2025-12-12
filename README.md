# PAR_WS2526

Repository for Praktikum Augmented Reality.

## Getting Started

This project uses **Unity** and **Git Large File Storage (LFS)** to manage large binary assets (textures, models, audio, etc.).

### Prerequisites

Before cloning the repository, ensure you have the following installed:

1.  **Git**: [Download Git](https://git-scm.com/downloads)
2.  **Git LFS**: [Download Git LFS](https://git-lfs.com/)
3.  **Unity Hub & Editor**: Install the version specified in `ProjectSettings/ProjectVersion.txt` (or the latest stable release if not specified).

### Installation

1.  **Install Git LFS**:
    Open your terminal or command prompt and run:
    ```bash
    git lfs install
    ```

2.  **Clone the Repository**:
    ```bash
    git clone <repository-url>
    cd PAR_WS2526
    ```

3.  **Pull LFS Assets**:
    If you cloned the repository before installing Git LFS, or if assets appear missing (e.g., pink textures), run:
    ```bash
    git lfs pull
    ```

### Opening the Project

1.  Open **Unity Hub**.
2.  Click **Add** and select the `PAR_WS2526` folder.
3.  Click on the project name to open it in the Unity Editor.

## Git LFS Configuration

This repository is configured to track the following file types with Git LFS:
-   Images: `.psd`, `.png`, `.jpg`, `.tga`
-   Audio: `.wav`, `.mp3`, `.ogg`
-   Models: `.fbx`
-   Video: `.mp4`, `.avi`

To verify LFS is working, you can run:
```bash
git lfs ls-files
```

## Contributing

When adding new large binary files, ensure they are tracked by LFS. You can add new file types using:
```bash
git lfs track "*.extension"
```
This will update the `.gitattributes` file.
