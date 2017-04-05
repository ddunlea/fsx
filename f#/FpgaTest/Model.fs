module Model

let toBinary (b:byte) =
  let bitToString index =
    let justTheBit = 1uy <<< index
    let hit = b &&& justTheBit = justTheBit
    if hit then "1" else "0"

  let bits = [7..-1..0] |> Seq.map(bitToString)
  let n1 = bits |> Seq.take(4) |> String.concat ""
  let n2 = bits |> Seq.skip(4) |> String.concat ""
  n1 + "_" + n2

[<StructuredFormatDisplay("{AsString}")>]
type PrettyByte = {value: byte} with
  member x.AsString = toBinary x.value
