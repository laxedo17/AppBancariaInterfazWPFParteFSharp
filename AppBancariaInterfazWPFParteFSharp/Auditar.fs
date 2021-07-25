module internal AppBancariaInterfazWPFParteFSharp.Auditar

open AppBancariaInterfazWPFParteFSharp.Domain

///Loguease na consola
let imprimirOperacion _ idConta transaccion =
    printfn "Conta %O: %s con saldo %M" idConta transaccion.Operacion transaccion.Cantidade

///Loguease na consola e no sistema de arquivos
let loggerComposto =
    let loggers =
        [ RepositorioDeArquivos.escribirTransaccion 
          imprimirOperacion]
    fun idConta propietario operacion ->
        loggers 
        |> List.iter(fun logger -> logger idConta propietario operacion)
