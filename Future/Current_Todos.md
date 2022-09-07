#################################
# FOR MIT DEMOS
#################################

* Remote calibration
* Clicker

* load: put it in the original position under NetworkedSceneRoot 
* prev / next: works even if autosafe disabled; also saves in wrong index


#################################
# BEFORE DRAFT RELEASE
#################################

* just use clicker

* load: put it in the original position under NetworkedSceneRoot 
* prev / next: works even if autosafe disabled; also saves in wrong index


* HL: Example arduino scene: Manual Data Channel make it smaller

* New scene with more objects

* Update documentation




#################################
# BEFORE FIRST RELEASE
#################################

* HL: does ObjectMenu and ImageMenu move in the same way [colliders are different]
** [not done, seems complicated because menus are spawned] Object / Image menu: position networked

** HL: TEST object/image menu RPC for open/close 
* HL TEST: trash can


* BUG: effectmenu spawn when object doesn't have things [ex: sensor chart]
* BUG: deleting signals still keeps some listeners around; someone doesn't unregister themselves


* Can you use the cyb prefab in new scene properly ? [might have changed prefabs]
* >> Clean up example scenes folder
* >> Example docs: Color cube, text cube



** Effect Menu Link to current object

** Effect Menu BUG: after closing effect menu, there's some listeners (probably from sensorchart) which got broken. Need to unlisten.




#################################
# SECOND RELEASE
#################################

* * HL: way to calibrate HL finger without keyboard. Ideally use PC GUI dropdown and RPC

** EffectMenu Remote: Need to do something when the current target's effects menu changes remotely, so that the current UI updates [send RPC so that others find the object, based on PhotonView]
	** Check if the last edited object is per user or per network
	** Networked open/close only works properly if everyone's last edited object is in sync

** EffectMenu BUG: draw effects ? changing drawing color I think causes a crash because can't change other draw effects
/** [noteasy] Plugins: GUI quick: "plugins GUIs"

* EffectsMenu Make networked position

** Scene saving doesn't save effects model





### LOWER PRIORITY

* Tooltips and arrows - test creation with clicker; make it so people can create without, or at least just use PC

** Plugins: remove additional plugins ?
** > Plugins: activating plugins has weird GUI popups; also make sure the populs show near the plugins menu


* CreationObjectManager.ExecuteButtonPress -- Object creation should be done only through SSM via RPC 



##########################
# DOCUMENTATION DONT FORGET
##########################

* ManualDataChannel: if you want it to be manual and standalone will need atomicdataswitch on it, photontransformview,pv,boxcollider, playercreatedobject




#######################
# KNOWN BUGS
#######################

* BUG: creating objects from library has error in nearinteractiongrabbable aboutcollider

* BUG: Scene saving doesn't save effects model

* BUG: If your NetworkedSceneRoot object isn't at origin in your scene, then saving/loading prefab will have issues because it saves the whole object (with local coordinates) into a prefab, then it instantiates it in world coordinates.




###########################
# MAYBE LATER
############################

* Phone UI/RPC for swithing into a specific scene [not sure if this is possible because phone tries to start in specific place]

* Effects scaling magnitude should start at 1 but have range -3 to 3


# Hand Menu
* EffectMenu SpawnButton: put it on the hand, and/or in the main buttons
* >> Button on hand for erase all drawings [have RPC already]



# REMOVE UNUSED CLASSES
* TouchMenu_CreateObjects

