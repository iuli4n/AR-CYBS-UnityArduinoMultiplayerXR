* load: put it in the original position under NetworkedSceneRoot 
* prev / next: works even if autosafe disabled; also saves in wrong index



* Hand Menu

* EffectMenu SpawnButton: put it on the hand, and/or in the main buttons
* >> Button on hand for erase all drawings [have RPC already]








# AT END

* >> Clean up example scenes folder
* >> Example docs: Color cube, text cube


# HL QUICK

* Example arduino scene: Manual Data Channel make it smaller



# HL TODO TEST

* way to calibrate HL finger without keyboard. Ideally use PC GUI dropdown and RPC
* does ObjectMenu and ImageMenu move in the same way [colliders are different]
** object/image menu RPC for open/close 
* trash can



# TEST GENERAL

* Can you use the cyb prefab in new scene properly ? [might have changed prefabs]




# SOON AFTER RELEASE


* Trash
** Trash can collider [will require talking through SSM]


* Effect Menu

** BUG: after closing effect menu, there's some listeners (probably from sensorchart) which got broken. Need to unlisten.

** Need to do something when the current target's effects menu changes remotely, so that the current UI updates [send RPC so that others find the object, based on PhotonView]
	** Check if the last edited object is per user or per network
	** Networked open/close only works properly if everyone's last edited object is in sync

** > BUG: draw effects ? changing drawing color I think causes a crash because can't change other draw effects
/** [noteasy] Plugins: GUI quick: "plugins GUIs"


* BUG: deleting signals still keeps some listeners around; someone doesn't unregister themselves





* Object / Image menu
** [not done, seems complicated because menus are spawned] position networked

** Plugins: remove additional plugins ?
** > Plugins: activating plugins has weird GUI popups; also make sure the populs show near the plugins menu

* EffectsMenu Make networked position





* tooltips and arrows - test creation with clicker; make it so people can create without



* KNOWN BUG: creating objects from library has error in nearinteractiongrabbable aboutcollider


** Scene saving doesn't save effects model



# MAYBE LATER


* Phone UI/RPC for swithing into a specific scene [not sure if this is possible because phone tries to start in specific place]

* Effects scaling magnitude should start at 1 but have range -3 to 3
