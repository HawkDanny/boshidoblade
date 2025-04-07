
function main()
    print("running main");
    while true do
        -- print("running a frame");
        emu.yield() -- frameadvance() also works

        print(memory.read_s8(0x0A11D4)); -- 0A11D4
        local x = unityhawk.callmethod("IncrementBoshiScore", "" .. memory.read_s8(0x0A11D4));
    end
end

main()