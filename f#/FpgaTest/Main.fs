module FpgaTest
open Model
open Serial
open Generators
open Expecto
open FsCheck
open Expecto.Logging
open Expecto.Logging.Message
open System.IO
open System.IO.Ports

let config = { FsCheckConfig.defaultConfig with maxTest = 200; arbitrary = [typeof<ModelGenerators>] }
let stressConfig = { FsCheckConfig.defaultConfig with maxTest = 10000000; arbitrary = [typeof<ModelGenerators>] } // ; replay = Some (1668765585,296279134)

[<Tests>]
let tests =
  testList "FsCheck" [
    testPropertyWithConfig config  "Data that is written can be read" <|
      fun addr data ->
        writeByte addr data
        let result = readByte addr
        logProgress(result.Equals data)

    testPropertyWithConfig config  "Data that is written can be read - Typed" <|
      fun addr data ->
        writePrettyByte addr data
        let result = readPrettyByte addr
        logProgress(result.Equals data.value)
        
    testPropertyWithConfig config  "Data that is written can be read later" <|
          fun addr1 addr2 data1 data2 ->
        writeByte addr1 data1
        writeByte addr2 data2
        let result1 = readByte addr1
        let result2 = readByte addr2
        logProgress((addr1.Equals addr2) ||
          (result1 = data1 && result2 = data2))
        
    testPropertyWithConfig config  "Data that is written can be read later - Typed" <|
      fun addr1 addr2 data1 data2 ->
        writePrettyByte addr1 data1
        writePrettyByte addr2 data2
        let result1 = readPrettyByte addr1
        let result2 = readPrettyByte addr2
        logProgress((addr1.Equals addr2) ||
          (result1 = data1.value && result2 = data2.value))
        
    testPropertyWithConfig stressConfig  "Data that is written can be read later fast" <|
          fun addr1 addr2 data1 data2 ->
        writeByte addr1 data1
        writeByte addr2 data2
        let result1 = readByteFast addr1
        let result2 = readByteFast addr2
        (addr1.Equals addr2) ||
          (result1 = data1 && result2 = data2)
        
    testPropertyWithConfig stressConfig  "Data that is written can be read later - Typed fast" <|
      fun addr1 addr2 data1 data2 ->
        writePrettyByte addr1 data1
        writePrettyByte addr2 data2
        let result1 = readPrettyByteFast addr1
        let result2 = readPrettyByteFast addr2
        (addr1.Equals addr2) ||
          (result1 = data1.value && result2 = data2.value)
                
    testPropertyWithConfig stressConfig "Data that is written can be read later - Stress" <|
      fun addr1 addr2 data1 data2 ->
        writePrettyByte addr1 data1
        writePrettyByte addr2 data2
        let result1 = readPrettyByte addr1
        let result2 = readPrettyByte addr2
        logProgress((addr1.Equals addr2) ||
          (result1 = data1.value && result2 = data2.value))
                
    testPropertyWithConfig stressConfig "Count" <|
      fun addr1 ->
        for x in 0 .. 100000000 do
          writeByte addr1 (byte (x % 256))
          async {
            do! Async.Sleep(100)
          } |> Async.RunSynchronously 
        true

  ]
[<EntryPoint>]
let main argv =
    makeReady() |> ignore
    Tests.runTestsInAssembly defaultConfig argv
