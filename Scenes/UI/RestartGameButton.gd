extends Button

func OnPressed():
	var game_manager = get_node("/root/Game")
	game_manager.emit_signal("OnGameStart")
	pass

func _ready():
	var err = connect("pressed", self, "OnPressed")
	if(err != 0):
		print("Error connecting the signal - ", err)

