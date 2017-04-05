module Generators
open Model
open FsCheck

let clearBit toReduce bitIndex =
  let bitToRemove = 1uy <<< bitIndex
  toReduce &&& ~~~bitToRemove
  
type ModelGenerators =
  static member PrettyByteGen() =
      {new Arbitrary<PrettyByte>() with
          override x.Generator =
            Arb.Default.Byte().Generator |> Gen.map(fun x -> {value = x})
          override x.Shrinker before =
            [0..7]
            |> Seq.map(fun bitIndex -> {value = clearBit before.value bitIndex})
            |> Seq.filter(fun after -> not (after.value.Equals before.value))
          }
