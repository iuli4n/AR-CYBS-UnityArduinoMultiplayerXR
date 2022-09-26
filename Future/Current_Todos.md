

#################################
# BEFORE FIRST RELEASE
#################################



LATER:
- close button for effectsmenu
- effects controller more easily
- saving/loading current scene
- issue clicking on items with right hand
- Disable object menu open/close for all users



**** NEED TO DO

- Make empty scene

- Think about what documentation you need
	* >> Example docs: Color cube, text cube
	
	* Document clearly main features through some main image
	- Arduino to Unity
	- Unity to many HL
	- Data channels


**** DOCUMENTATION

* import from Shankar
* Update documentation for arduino scene: configuration prefab, channels

* Make list of known bugs
	* add object / image menu not opening/closing/moving
	* document load scene doesn't save effects




====================================

- DO YOU HAVE ARDUINO ?
-> YES
--- In Unity, open the ConfigurationProfile object and set the COM port of your arduino (you can find it through the Arduino IDE)
--- you will control C2 by changing A0 on the arduino
--- Open the Arduino IDE and the arduino project, upload it

-> NO
--- open the ConfigurationProfile object and disable Arduino_Enabled 
--- you will control C2 manually through the window


- DO YOU HAVE HOLOLENS ?
-> YES
--- build the project for hololens and upload it to your device
-> NO
--- no problem, you can use the Unity editor


run the project
- whenever you change C2 (either through the Arduino knob, or through the panel)



2. control arduino light with control panel
(-- you'll need to modify the ConfigurationProfile. You can either make a copy of it and disable the old one, or just change it.)
-- in configuration profile, remove simchannels square from C1
-- run and change data switch controller to C1 and change manually. you should see light change and C2 change

3. control unity object with arduino
- look at the ColorCube object in scene. it was created by ...- if want to manipulate, add PlayerCreatedObject and tell it to PhotonTransformView scale; and PCO don't attach to scene




IF DONT HAVE ARDUINO
- you can still run this scene but it'll give some USB errors

- run the hololens sample





====================================














MAYBE BUGS:

* closing EffectsMenu gives problem because sensorchart keeps getting updated after being destroyed [proably is forgetting to unregister somewhere]

- other features: effects, pointers, editing, files

* HL: does ObjectMenu and ImageMenu move in the same way [colliders are different]
** [not done, seems complicated because menus are spawned, how about dont regenerate menus] Object / Image menu: position networked
** HL: TEST object/image menu RPC for open/close 

* load scene: collider isn't set properly after reload
* document load scene doesn't save effects


BUGS MAYBE:
* BUG: effectmenu spawn when object doesn't have things [ex: sensor chart]
* BUG: deleting signals still keeps some listeners around; someone doesn't unregister themselves
	** Effect Menu BUG: after closing effect menu, there's some listeners (probably from sensorchart) which got broken. Need to unlisten.

* TEST: network scene load/save when person joins after scene was switched [shouldn't happen just put it under docs]


#################################
# SECOND RELEASE
#################################


* effects menu button could be turned into an actual button

** EffectMenu Remote: Need to do something when the current target's effects menu changes remotely, so that the current UI updates [send RPC so that others find the object, based on PhotonView]
	** Check if the last edited object is per user or per network
	** Networked open/close only works properly if everyone's last edited object is in sync

** EffectMenu BUG: draw effects ? changing drawing color I think causes a crash because can't change other draw effects
/** [noteasy] Plugins: GUI quick: "plugins GUIs"

* EffectsMenu Make networked position

** Scene saving doesn't save effects model

* Creation menu: doesn't show scale properly; use renderer bounds instead of collider ?


** Effect Menu Link to current object


** Effect Menu needs to be attached on player created objects in the base scene; right now effects don't work on those kinsd of objects

### LOWER PRIORITY

* creation menu: objects get rotated 90deg !?


* Tooltips and arrows - test creation with clicker; make it so people can create without, or at least just use PC

** Plugins: remove additional plugins ?
** > Plugins: activating plugins has weird GUI popups; also make sure the populs show near the plugins menu


* CreationObjectManager.ExecuteButtonPress -- Object creation should be done only through SSM via RPC 

* update the client login so it waits for the pc master [how to check if it connected ?]


##########################
# DOCUMENTATION DONT FORGET
##########################

* ManualDataChannel: if you want it to be manual and standalone will need atomicdataswitch on it, photontransformview,pv,boxcollider, playercreatedobject

* The project for working on phone - Photon seems to serialize different kinds of objects !? 



#######################
# KNOWN BUGS
#######################

* BUG: creating objects from library has error in nearinteractiongrabbable aboutcollider

* BUG: Scene saving doesn't save effects model

* BUG: If your NetworkedSceneRoot object isn't at origin in your scene, then saving/loading prefab will have issues because it saves the whole object (with local coordinates) into a prefab, then it instantiates it in world coordinates.

* CLICKER


###########################
# MAYBE LATER
############################

* knob can't be controlled [just use slider for now]


* Phone UI/RPC for swithing into a specific scene [not sure if this is possible because phone tries to start in specific place]

* Effects scaling magnitude should start at 1 but have range -3 to 3


# Hand Menu
* EffectMenu SpawnButton: put it on the hand, and/or in the main buttons
* >> Button on hand for erase all drawings [have RPC already]



# REMOVE UNUSED CLASSES
* TouchMenu_CreateObjects

