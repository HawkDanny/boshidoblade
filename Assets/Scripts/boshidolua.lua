
function main()
    print("running main");
    while true do
        -- print("running a frame");
        emu.yield() -- frameadvance() also works

        local p1State = memory.read_s8(0x13D794);
        local p2State = memory.read_s8(0x13D46C);

        if p1State == 2 then
            unityhawk.callmethod("IncrementBoshiScore", "" .. p1State);
        end

        if p2State == 2 then
            unityhawk.callmethod("IncrementYellowYoshiScore", "" .. p2State);
        end
    end
end

main()