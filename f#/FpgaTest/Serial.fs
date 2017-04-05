module Serial
open Model
open System.IO.Ports

let array : byte array = Array.zeroCreate 10
let writeByteToArray (address:byte) (data:byte) =
    array.[int address] <- data

let readByteFromArray (address:byte) =
    array.[int address]

let wait () =
    async {
      do! Async.Sleep(50)
    } |> Async.RunSynchronously 
let port = new SerialPort("/dev/ttyACM0", 38400)

let writeByte (address:byte) (data:byte) =
    port.Write([|119uy;address;data|], 0, 3)

let writePrettyByte (address:PrettyByte) (data:PrettyByte) =
    writeByte address.value data.value

let readByteFast (address:byte) =
    port.Write([|114uy;address|], 0, 2)
    byte (port.ReadByte())

let readByte (address:byte) =
    let result = readByteFast address
    wait()
    result

let readPrettyByteFast (address:PrettyByte) =
    readByteFast address.value

let readPrettyByte (address:PrettyByte) =
    let result = readPrettyByteFast address
    wait()
    result

let logProgress b =
  if b then printf(".") else printf("!")
  b

let makeReady () =
    port.Open()
    port.DtrEnable <- true
    port.RtsEnable <- true
    // printfn "Sending 2 x 0 to get to Idle"
    let mutable notReady = true

    while notReady do
      port.Write([|88uy|], 0, 1)
      async {
        do! Async.Sleep(300)
      } |> Async.RunSynchronously 
      while port.BytesToRead > 1 do
        port.ReadByte() |> ignore

      if (port.BytesToRead) = 1 then
        let b = port.ReadByte()
        notReady <- not (b.Equals 88)

