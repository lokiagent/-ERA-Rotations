StartMobAvoidance();
UseDBToRepair(true)
UseDBToSell(true)
SetQuestRepairAt(30)
SetQuestSellAt(2)
Player = GetPlayer();

function GoToFlightMaster(NPCID)
    local NPC = GetNPCPositionFromDB(NPCID);
    if (NPC ~= nil) then
        QuestGoToPoint(NPC[1], NPC[2], NPC[3], true, true);
        InteractWithUnit(NPCID);
    else
        print("NPC not found in DB");
    end
end
-- Function to take a flight point by name
function TakeFlightPointByName(flightPointName)
    -- Convert the desired flight point name to lowercase for case-insensitive comparison
    local desiredName = flightPointName:lower()

    -- Iterate through all taxi nodes
    for i = 1, NumTaxiNodes() do
        -- Get the name of the current taxi node and convert it to lowercase
        local nodeName = TaxiNodeName(i):lower()
        
        -- Check if the current node name matches the desired flight point name
        if nodeName:find(desiredName) then
            -- Take the flight to the matched node
            TakeTaxiNode(i)
            -- Optionally play a sound to confirm the action
            PlaySound(47355)
            return
        end
    end

    -- If no match is found, print a message
    print("Flight point not found: " .. flightPointName)
end

GoToFlightMaster(3615);
TakeFlightPointByName("Orgrimmar");
StopQuestProfile();

