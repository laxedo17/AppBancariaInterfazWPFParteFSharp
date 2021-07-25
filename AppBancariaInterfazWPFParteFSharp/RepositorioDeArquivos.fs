module internal AppBancariaInterfazWPFParteFSharp.RepositorioDeArquivos

open AppBancariaInterfazWPFParteFSharp.Domain
open System.IO
open System
open Newtonsoft.Json

let private rutaContas =
    let ruta = @"contas"
    Directory.CreateDirectory ruta |> ignore 
    ruta 
let private tryAtoparCarpetaDaConta propietario =
    let carpetas = Directory.EnumerateDirectories(rutaContas, sprintf "%s_*" propietario) |> Seq.toList
    match carpetas with
    | [] -> None
    | carpeta :: _ -> Some(DirectoryInfo(carpeta).Name)

let private construirRuta(propietario, idConta:Guid) = sprintf @"%s\%s_%O" rutaContas propietario idConta

let lerTransaccions (carpeta:string) =
    let propietario, idConta =
        let partes = carpeta.Split '_'
        partes.[0], Guid.Parse partes.[1]
    propietario, idConta, construirRuta(propietario, idConta)
                    |> Directory.EnumerateFiles 
                    |> Seq.map (fun ruta -> JsonConvert.DeserializeObject<Transaccion>(File.ReadAllText ruta))

///Atopa todas as transaccions no disco dun propietario especifico
let tryAtoparTransaccionsEnDisco = tryAtoparCarpetaDaConta >> Option.map lerTransaccions

///Loguease no sistema de arquivos
let escribirTransaccion idConta propietario transaccion =
    let ruta = construirRuta(propietario, idConta)
    ruta |> Directory.CreateDirectory |> ignore
    let rutaArquivo = sprintf "%s/%d.txt"ruta (transaccion.Datar.ToFileTimeUtc())
    let linha = transaccion |> JsonConvert.SerializeObject
    File.WriteAllText(rutaArquivo, linha)

