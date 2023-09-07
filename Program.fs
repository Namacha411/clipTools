open System
open System.IO
open System.Text.RegularExpressions
open System.Windows
open System.Windows.Media.Imaging

open Microsoft.Toolkit.Uwp.Notifications

// Utils
let toastNotification message detail =
    ToastContentBuilder()
        .AddArgument("action", "viewConversation")
        .AddArgument("conversatinId", 9813)
        .AddText(message)
        .AddText(detail)
        .Show()

    0

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
        use stream = new FileStream(path, FileMode.CreateNew)
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
        use stream = new StreamWriter(path, true)
        stream.Write(text)
        toastNotification "Saved Text!" <| path.ToString () + $"\n{text}"

let help () =
    let text =
        """
clip-tools
Usage: clip-tools [-v | --version] [-h | --help] <command>
Commands:
    counter
    saveImage [path]
    saveText [path]
    """

    let text = text.Trim()

    printfn "%s" text

let version () =
    let version = "23.9.7"
    printfn "%s" version

[<STAThread>]
[<EntryPoint>]
let main args =
    let args = args |> Array.toList

    match args with
    | head :: _ when head = "--help" || head = "-h" ->
        help ()
        0
    | head :: _ when head = "--version" || head = "-v" ->
        version ()
        0
    | head :: _ when head = "counter" -> counterApp ()
    | head :: teil when head = "saveImage" ->
        match teil with
        | h :: _ -> saveImageApp <| Some h
        | [] -> saveImageApp None
    | head :: teil when head = "saveText" ->
        match teil with
        | h :: _ -> saveTextApp <| Some h
        | [] -> saveTextApp None
    | _ ->
        help ()
        1
