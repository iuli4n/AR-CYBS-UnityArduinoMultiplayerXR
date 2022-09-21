

#################################
# BEFORE FIRST RELEASE
#################################


**** NEED TO DO

- Think about what documentation you need
	* >> Example docs: Color cube, text cube
	
	* Document clearly main features through some main image
	- Arduino to Unity
	- Unity to many HL
	- Data channels




- Check all example scenes to make sure they work
	- actually just send ArduinoScene and Empty 

- Also check that you can make a new scene

-?nah Sketchy menu & RPC to switch scenes [use same as calibration]


* >> Clean up example scenes folder

- Disable object menu open/close for all users




*** HL TEST

- Check finger issue

* HL TEST: trash can

* disable StandaloneDataChannel movement




**** DOCUMENTATION

* import from Shankar
* Update documentation for arduino scene: configuration prefab, channels

* Make list of known bugs
	* add object / image menu not opening/closing/moving
	* document load scene doesn't save effects



















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

