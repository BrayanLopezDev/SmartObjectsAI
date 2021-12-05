
Project:
- Smart Objects + Reputation

Contributors:
- Brayan Lopez
- Lindy Plunkett
- Julian Blackstone

Notes:
- The EXE is in the SmartObjects+/Build folder
- The SmartObjects+ folder is a Unity folder so you can open the project with Unity
    we used Unity version 2021.1.10f1
- You can toggle fullscreen and Windowed mode in the simulation by pressing Alt Enter
- You can restart the simulation by clicking on the "Restart with" button on the top left, and use the slider
    to select the desired amount of sims to restart with
- You can adjust the speed of the simulation by adjusting the slider on the top right of the screen

Controls:
- W,S for forward and backward
- A,D for left and right
- Q,E for vertical movement
- Hold down Right Mouse Button and move mouse to rotate the camera
- Click on a sim to view their stats
    0 Their needs will appear above them
    0 The list of people they are sus of will appear on the bottom left of the screen, you can scroll through that list
        as well as click on an entry in that list to have the camera follow that other sim instead
- You can choose to have the camera automatically follow the chosen sim with the follow button on bottom right corner
- You can choose to kill the sim with the button on bottom right corner

Features:
- Smart Objects:
    0 The objects (toilet, mud, bush, fridge, goose, tree, arcade machine) are smart,
        they broadcast to nearby sims "I give you food/fun/pee/shade"
    0 The smart objects control their own animations as well as the sims' animations when interacting with them
    0 When a sim is critically low on a need, and killing a sim they are waiting to finish interacting with the 
        smart object will benefit them, they have the smart object kill them because the sims dont know how to kill
    0 When a sim reaches a smartobject, they have to first ask the smartobject if they will fulfill their need, but
        if the smartobject's capacity is full, meaning they can't concurrently help anymore sims, they refuse to help the sim
- Reputation:
    0 The sims like to assume the worst
    0 They see anyone near a "sus object" (dead body, bush, goose, mud) and they quickly draw their conclusion and become sus of that other sim
    0 When they see others, they gossip about the sims they are sus of and that the sims they tell, learn that information
    0 They try their best to avoid the sims they are sus of, even if they are interacting with a smartobject
        * This is what gives the sims their "nervous behavior" when they try to get somewhere, because they see someone they are sus of and no longer 
            want to go in that direction UNLESS they are critically low on that need
    0 When a sim sees another sim being sus, the one they saw being sus turns into the color which represents the "crime"
        * green for eating wildlife
        * brown for playing in the mud
        * yellow for peeing in the bushes
        * red for killing somebody
    0 The other sim doesnt even have to be doing any of those things, but their "assume the worst" mentality means they assume the other sim is 
        committing that crime just because they are near a "sus object"
    0 When all of the sims that are sus of someone die, that sim turns back to white because now no one is sus of them
    0 When a sim cant find a smartobject to satisfy their need, they ask nearby sims
        * if the sim they ask is sus of them, that sim will lie and give them a random coordinate
    0 A sim will patiently wait their turn to interact with a smartobject UNLESS they are critically low on their need, so they have the smartobject
        kill a sim they are helping
    0 When a sim dies, they turn into a dead amongus with the color they had which represents their most up-to-date reputation when they were alive



