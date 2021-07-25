namespace AppBancariaInterfazWPFParteFSharp.Domain

open System 

type internal OperacionBancaria = Ingresar | Retirar 

//Un cliente do banco
type Cliente = { Nome : string }
//Unha conta do banco
type Conta = { IdConta : Guid; Propietario : Cliente; Saldo : decimal }
//Unha simple transaccion que tivo lugar
type Transaccion = { Datar : DateTime; Operacion : string; Cantidade : decimal }

//Representa unha conta bancaria da que se sabe que esta en credito
type ContaDeCredito = ContaDeCredito of Conta 
//Unha conta bancaria a cal ou ben esta en Credito ou EnNumerosVermellos
type ContaClasificada =
    ///Representa unha conta que se sabe que ten credito
    | DentroDeCredito of Conta:ContaDeCredito
    ///Repressenta unha conta da que se sabe que esta en numeros vermellos
    | EnNumerosVermellos of Conta:Conta
    member internal this.GetField getter =
        match this with 
        | DentroDeCredito (ContaDeCredito conta) -> getter conta 
        | EnNumerosVermellos conta -> getter conta 
    ///Get o saldo actual da conta
    member this.Saldo = this.GetField(fun c -> c.Saldo)