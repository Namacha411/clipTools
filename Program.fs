open System
open System.IO
open System.Text.RegularExpressions
open System.Windows
open System.Windows.Media.Imaging
// prerelease
open System.CommandLine
open System.CommandLine.NamingConventionBinder

open Microsoft.Toolkit.Uwp.Notifications

// Utils
let toastNotification message detail =
    ToastContentBuilder()
        .AddArgument("action", "viewConversation")
        .AddArgument("conversatinId", 9813)
        .AddText(message)
        .AddText(detail)
        .Show()

let removeWhiteSpace str = Regex.Replace(str, @"[\s]+", "")

// Apps
let counterApp () =
    let clipboardText = Clipboard.GetText()

    let len = clipboardText.Length
    let noSpaceLen = removeWhiteSpace clipboardText |> String.length
    let wordsLen = clipboardText.Split(" ") |> Array.length

    let message = $"文字数: {len}"

    let detail =
        $"空白なし: {noSpaceLen}\n" + $"単語数: {wordsLen}\n" + $"文章:\n{clipboardText}"

    toastNotification message detail

let rec saveImageApp =
    function
    | None ->
        saveImageApp
        <| Some(Environment.GetFolderPath <| Environment.SpecialFolder.MyPictures)
    | Some(saveDir) ->
        let image = Clipboard.GetImage()
        let fileName = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")
        let path = Path.Combine(saveDir, $"{fileName}.png")
        let stream = new FileStream(path, FileMode.CreateNew)
        let encoer = new PngBitmapEncoder()
        encoer.Frames.Add <| BitmapFrame.Create(image)
        encoer.Save(stream)
        toastNotification "Saved Image!" <| path.ToString()

let rec saveTextApp =
    function
    | None ->
        saveTextApp
        <| Some(Environment.GetFolderPath <| Environment.SpecialFolder.MyDocuments)
    | Some(saveDir) ->
        let text = Clipboard.GetText()
        let fileName = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")
        let path = Path.Combine(saveDir, $"{fileName}.txt")
        let stream = new StreamWriter(path)
        stream.Write(text)
        toastNotification "Saved Image!" <| path.ToString()

[<STAThread>]
[<EntryPoint>]
let main args =
    let rootCommand = new RootCommand "Clipboard-Tools"

    let counter = Command "Counter"
    counter.Handler <- CommandHandler.Create(fun _ -> counterApp ())

    let saveImage = Command "SaveImage"
    saveImage.AddOption <| Option<string> "--path"

    saveImage.Handler <-
        CommandHandler.Create(fun a b ->
            printfn a
            printfn b
            saveImageApp None)

    let saveText = Command "SaveText"
    saveText.AddOption <| Option<string> "--path"

    saveText.Handler <-
        CommandHandler.Create(fun a b ->
            printfn a
            printfn b
            saveTextApp None)

    rootCommand.AddCommand(counter)
    rootCommand.AddCommand(saveImage)
    rootCommand.AddCommand(saveText)

    rootCommand.Invoke args
