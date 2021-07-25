//Da acceso a API de bancos
module AppBancariaInterfazWPFParteFSharp.Api 

open AppBancariaInterfazWPFParteFSharp.Operacions
open AppBancariaInterfazWPFParteFSharp.Domain
open System

//Le unha conta do disco. Se non existe unha conta, crease unha valeira automaticamente
let LerConta cliente =
    cliente.Nome
    |> RepositorioDeArquivos.tryAtoparTransaccionsEnDisco
    |> Option.map Operacions.lerConta
    |> defaultArg <|
        DentroDeCredito(ContaDeCredito { IdConta = Guid.NewGuid()
                                         Saldo = 0M
                                         Propietario = cliente })
 
 //Ingresa fondos nunha conta
 let Ingresar cantidade cliente= 
    let contaClasificada = LerConta cliente 
    let idConta = contaClasificada.GetField (fun c -> c.IdConta)
    let propietario = contaClasificada.GetField(fun c -> c.Propietario)
    auditarComo "ingresar" Auditar.loggerComposto ingresar cantidade contaClasificada idConta propietario

///Retira fondos nunha conta que esta dentro de creido
let Retirar cantidade cliente =
    let conta = LerConta cliente
    match conta with
    | DentroDeCredito(ContaDeCredito conta as contaDeCredito) -> auditarComo "retirar" Auditar.loggerComposto retirar cantidade contaDeCredito conta.IdConta conta.Propietario
    | conta -> conta

/// Lee o historial de operacions dun propietario
let LerHistorialOperacions cliente =
    cliente.Nome
    |> RepositorioDeArquivos.tryAtoparTransaccionsEnDisco
    |> Option.map(fun (_,_, txns) -> txns)
    |> defaultArg <| Seq.empty