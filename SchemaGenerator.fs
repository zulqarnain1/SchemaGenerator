module SchemaGenerator

open System.IO
open System.Linq
open LinqToExcel
open System
open System.Text

let createBegin tableName = sprintf "CREATE TABLE [dbo].[%s] \n(" tableName

type table =
    { colName : string
      colType : string
      req : string }

let colCreate colName colType mandatory =
    match mandatory with
    | true -> sprintf "[%s] %s NOT NULL" colName colType
    | _ -> sprintf "[%s] %s NULL" colName colType

let readExcel path tb =
    let finalQuery = StringBuilder()
    let workbook = new ExcelQueryFactory(path)
    workbook.UsePersistentConnection <- true
    workbook.ReadOnly <- true
    let sheetColNames sheet = workbook.GetColumnNames(sheet)
    let containsCol sheet colName = (sheetColNames sheet).Contains colName
    let isDbTable sheet = containsCol sheet tb.colName
    let isMandatory sheet = containsCol sheet tb.req
    let sheets = workbook.GetWorksheetNames() |> Seq.filter (fun x -> isDbTable x)

    let rows (sheet : string) =
        query {
            for r in workbook.Worksheet(sheet) do
                select r
        }

    let processRow (r : Row) sheet =
        let rb = StringBuilder()

        let mandat =
            match (isMandatory sheet) with
            | true -> r.Item(tb.req).ToString().Equals("Y", StringComparison.CurrentCultureIgnoreCase)
            | _ -> false

        let res =
            colCreate (r.Item(tb.colName).ToString()) (r.Item(tb.colType).ToString()) mandat
            |> sprintf "%s,\n"
            |> rb.Append

        rb.ToString()

    let prSheet sheet =
        let sb = StringBuilder()
        sb.Append(createBegin sheet) |> ignore
        let srows = rows sheet
        for r in srows do
            let rs = processRow (r) sheet
            if r.Item(0).Value = srows.Last().Item(0).Value then sb.Append(rs.Replace(",", "")) |> ignore
            else sb.Append(rs) |> ignore
        sprintf "%s);\n" (sb.ToString())

    for s in sheets do
        finalQuery.Append((prSheet s)) |> ignore
    finalQuery.ToString()

let writeQueryToFile (query : string) (filePath) =
    use sw = new StreamWriter(path = filePath)
    sw.WriteLine(query)