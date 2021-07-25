module internal AppBancariaInterfazWPFParteFSharp.Operacions

open System
open AppBancariaInterfazWPFParteFSharp.Domain 

let private clasificarConta conta =
    if conta.Saldo >= 0M then (DentroDeCredito(ContaDeCredito conta))
    else EnNumerosVermellos conta

///Retira unha cantidade dunha conta (se hai fondos suficientes)
let retirar cantidade (ContaDeCredito conta) =
    { conta with Saldo = conta.Saldo - cantidade}
    |> clasificarConta

///Deposita unha cantidade na conta
let ingresar cantidade conta =
    let conta =
        match conta with
        | EnNumerosVermellos conta -> conta 
        | DentroDeCredito (ContaDeCredito conta) -> conta 
    { conta with Saldo = conta.Saldo + cantidade}
    |> clasificarConta

///Corre algunhas operacions de conta como retirar ou ingresar con auditoria
let auditarComo nomeOperacion auditar operacion cantidade conta idConta propietario =
    let contaActualizada = operacion cantidade conta
    let transaccion = { Operacion = nomeOperacion; Cantidade = cantidade; Datar = DateTime.UtcNow }
    auditar idConta propietario.Nome transaccion
    contaActualizada

let tryParseOperacionSerializada operacion =
    match operacion with 
    | "retirar" -> Some Retirar
    | "ingresar" -> Some Ingresar
    | _ -> None

///Crea unha conta dun conxunto dun historico de operacions
let lerConta (propietario, idConta, transaccions) =
    let abrirConta = clasificarConta { IdConta = idConta; Saldo = 0M; Propietario = { Nome = propietario }}
    transaccions
    |> Seq.sortBy(fun txn -> txn.Datar)
    |> Seq.fold(fun conta txn -> 
        let operacion = tryParseOperacionSerializada txn.Operacion
        match operacion, conta with 
        | Some Ingresar, _ -> conta |> ingresar txn.Cantidade 
        | Some Retirar, DentroDeCredito conta -> conta |> retirar txn.Cantidade 
        | Some Retirar, EnNumerosVermellos _ -> conta 
        | None, _ -> conta) abrirConta