
function main()
    print("running main");
    while true do
        -- print("running a frame");
        emu.yield() -- frameadvance() also works
        local x = unityhawk.callmethod("IncrementBoshiScore", "fart fart");
        print(x);
    end
end

main()