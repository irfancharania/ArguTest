open Argu
open System
open System.Reflection


// ----------------------------
// Types
// ----------------------------

type OtherFile =
| One
| Two


// ----------------------------
// Arguments
// ----------------------------

type CliArguments =
    | [<AltCommandLine("-f"); UniqueAttribute;>] FileInput of filePath:string
    
    // optional argument, and help will NOT show [] around argument because not marked "option"
    | [<AltCommandLine("-o"); UniqueAttribute;>] Other_File of outputType:OtherFile 
    
    // optional argument, and help will show [] around argument
    | [<AltCommandLine("-o2"); UniqueAttribute;>] Other_File2 of outputType2:OtherFile option 


    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | FileInput _       -> "specify a file"
            | Other_File _      -> "optionally, restrict to either 'One' or 'Two'"
            | Other_File2 _     -> "optionally, restrict to either 'One' or 'Two'"

let errorHandler = ProcessExiter (colorizer = function | ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)


// ----------------------------
// Assembly
// ----------------------------

let assembly = Assembly.GetExecutingAssembly().GetName()
let assemblyName = assembly.Name
let assemblyVersion = assembly.Version


// ----------------------------
// Main
// ----------------------------

[<EntryPoint>]
let main argv =

    let parser = ArgumentParser.Create<CliArguments>(programName = $"{assemblyName}.exe", errorHandler = errorHandler)

    // parse command line args
    let args = parser.Parse argv
    let filePath = FileInput (args.GetResult(FileInput, ""))


    //----------------------------------------------

    // return type: OtherFile option
    let optionalOtherFile = args.TryGetResult(Other_File) 
    
    // return type: OtherFile option, however expects parameter to be passed in
    let optionalOtherFile2 = args.GetResult(Other_File2) 
    
    // return type: OtherFile option option
    let optionalOtherFile2' = args.TryGetResult(Other_File2) 

    //----------------------------------------------


    printfn "
file path:               %A
optional other file:     %A
optional other file2:    %A
optional other file2':   %A
"  
        filePath 
        optionalOtherFile 
        optionalOtherFile2 
        optionalOtherFile2'


    0