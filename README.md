# clipTool

[日本語](./README_ja.md)

This is a windows clipboard tool.

## Usage

```sh
clip-tools
Usage: clip-tools [-v | --version] [-h | --help] <command>
Commands:
    counter
    saveImage [DIR]
    saveText [DIR]
```

### counter

Count the number of characters in the clipboard and notify toast.
![counter](./images/counter.png)

### saveImage

Save a clipboard image.
Save in png format.
The default destination is MyPictures (`System.Environment.SpecialFolder.MyPictures`).

### saveText

Saves text in the clipboard.
The default destination is MyDocuments (`System.Environment.SpecialFolder.MyDocuments`).
