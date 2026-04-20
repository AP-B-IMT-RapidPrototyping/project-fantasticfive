@tool
extends Node3D

@export_group("Forest Starter - Settings:")
@export_range(0, 1000) var forest_size := 50
@export_range(0, 5000) var number_of_trees : int = 50
@export var vary_tree_sizes := true
@export var add_forest_floor := true




@export_tool_button("GENERATE NEW FOREST", "New")
var button = generate


var tree
var tree_node 
var tree_node_path : String
var tree_glb_paths_array : Array = []
var tree_instances_array : Array = []

var forest_floor
var forest_floor_node 
var forest_floor_node_path : String
var forest_floor_instances_array : Array = []

var forest_floor_distribution : Dictionary = {}
var boulders_glb_paths_array : Array = []
var bushes_glb_paths_array : Array = []
var grass_glb_paths_array : Array = []
var logs_and_stumps_glb_paths_array : Array = []
var rocks_glb_paths_array : Array = []
var shrooms_glb_paths_array : Array = []




func generate() -> void:
	
	# CLEAN UP
	clean_up()
	default_settings()
	
	# CREATE FOREST
	if forest_size > 0:
		
		# PLACE TREES
		if number_of_trees > 0:
			for n in range(number_of_trees):
				place_tree()
		
		
		# PLACE FOREST FLOOR
		if add_forest_floor:
			place_forest_floor(boulders_glb_paths_array, forest_floor_distribution["boulders"])
			place_forest_floor(bushes_glb_paths_array, forest_floor_distribution["bushes"])
			place_forest_floor(grass_glb_paths_array, forest_floor_distribution["grass"])
			place_forest_floor(logs_and_stumps_glb_paths_array, forest_floor_distribution["logs_and_stumps"])
			place_forest_floor(rocks_glb_paths_array, forest_floor_distribution["rocks"])
			place_forest_floor(shrooms_glb_paths_array, forest_floor_distribution["shrooms"])




# ----- FOREST FLOOR -----

func place_forest_floor(what : Array, how_many: int) -> void:
	if not what.is_empty():
		for n in range(how_many):
			forest_floor_node_path = what.pick_random()
			forest_floor_node = load(forest_floor_node_path)
			if not forest_floor_node is PackedScene:
				continue
			forest_floor = forest_floor_node.instantiate()
			$Forest/GeneratedForestFloor.add_child(forest_floor)
			forest_floor.global_position = Vector3(randf_range(-forest_size,forest_size), 0, randf_range(-forest_size,forest_size))
			forest_floor.global_rotation_degrees.y = randf_range(-360,360)
			forest_floor.owner = self
			forest_floor_instances_array.append(forest_floor)



# ----- TREES -----

func place_tree() -> void:
	if not tree_glb_paths_array.is_empty():
		tree_node_path = tree_glb_paths_array.pick_random()
		tree_node = load(tree_node_path)
		if not tree_node is PackedScene:
			return
		tree = tree_node.instantiate()
		$Forest/GeneratedTrees.add_child(tree)
		tree.global_position = Vector3(randf_range(-forest_size,forest_size), 0, randf_range(-forest_size,forest_size))
		tree.global_rotation_degrees.y = randf_range(-360,360)
		tree.owner = self
		tree_instances_array.append(tree)
		if vary_tree_sizes:
			tree.scale = tree.scale * randf_range(0.7,1.2)



func clean_up():
	if not tree_instances_array.is_empty():
		for t in tree_instances_array.duplicate():
			t.queue_free()
		tree_instances_array = []
		
	if not forest_floor_instances_array.is_empty():
		for f in forest_floor_instances_array.duplicate():
			f.queue_free()
		forest_floor_instances_array = []



func default_settings() -> void:
	@warning_ignore_start("integer_division")
	forest_floor_distribution = {
		"boulders" : forest_size / 20,
		"bushes" : forest_size / 10,
		"grass" : forest_size * 10,
		"logs_and_stumps" : forest_size / 10,
		"rocks" : forest_size / 5,
		"shrooms" : forest_size / 2,
	}
	
	boulders_glb_paths_array = [
		"res://assets/NatureAssets/forest_floor/boulders/Boulder_01_flat.glb", 
		"res://assets/forest_floor/boulders/Boulder_02_flat.glb", 
		"res://assets/forest_floor/boulders/Boulder_03_flat.glb"
	]
		
	bushes_glb_paths_array = [
		"res://assets/forest_floor/bushes/Bush_Clumps_L_flat.glb", 
		"res://assets/forest_floor/bushes/Bush_Clumps_M_flat.glb", 
		"res://assets/forest_floor/bushes/Bush_Clumps_S_flat.glb", 
		"res://assets/forest_floor/bushes/Bush_L_flat.glb", 
		"res://assets/forest_floor/bushes/Bush_M_flat.glb", 
		"res://assets/forest_floor/bushes/Bush_S_flat.glb", 
		"res://assets/forest_floor/bushes/Bush_with_Flowers_flat.glb"
	]
	
	
	grass_glb_paths_array = [
		"res://assets/forest_floor/grass/Grass_L_flat.glb", 
		"res://assets/forest_floor/grass/Grass_M_flat.glb", 
		"res://assets/forest_floor/grass/Grass_S_flat.glb"
	]
	
	logs_and_stumps_glb_paths_array = [
		"res://assets/forest_floor/logs_and_stumps/Fallen_Log_flat.glb", 
		"res://assets/forest_floor/logs_and_stumps/Tree_Stump_flat.glb"
	]
	
	rocks_glb_paths_array = [
		"res://assets/forest_floor/rocks/Rock_01_flat.glb", 
		"res://assets/forest_floor/rocks/Rock_02_flat.glb", 
		"res://assets/forest_floor/rocks/Rock_03_flat.glb", 
		"res://assets/forest_floor/rocks/Rock_04_flat.glb", 
		"res://assets/forest_floor/rocks/Rock_05_flat.glb", 
		"res://assets/forest_floor/rocks/Rock_06_flat.glb", 
		"res://assets/forest_floor/rocks/Rock_07_flat.glb", 
		"res://assets/forest_floor/rocks/Rock_08_flat.glb", 
		"res://assets/forest_floor/rocks/Rock_09_flat.glb", 
		"res://assets/forest_floor/rocks/Rock_10_flat.glb"
	]
	
	shrooms_glb_paths_array = [
		"res://assets/forest_floor/shrooms/Mushroom_Brown_L_flat.glb", 
		"res://assets/forest_floor/shrooms/Mushroom_Brown_M_flat.glb", 
		"res://assets/forest_floor/shrooms/Mushroom_Brown_S_flat.glb", 
		"res://assets/forest_floor/shrooms/Mushroom_Red_L_flat.glb", 
		"res://assets/forest_floor/shrooms/Mushroom_Red_S_flat.glb"
	]
	
	tree_glb_paths_array = [
		"res://assets/trees/birch/Birch_H_flat.glb", 
		"res://assets/trees/birch/Birch_L_flat.glb", 
		"res://assets/trees/birch/Birch_M_flat.glb", 
		"res://assets/trees/birch/Birch_S_flat.glb", 
		"res://assets/trees/cypress/Cypress_L_flat.glb", 
		"res://assets/trees/cypress/Cypress_M_flat.glb", 
		"res://assets/trees/cypress/Cypress_S_flat.glb", 
		"res://assets/trees/other/Elm_flat.glb", 
		"res://assets/trees/other/Maple_flat.glb", 
		"res://assets/trees/other/Maple_flat_red.glb", 
		"res://assets/trees/other/Mistletoe_Tree_flat.glb", 
		"res://assets/trees/other/Windswept_Tree_flat.glb", 
		"res://assets/trees/pine/Pine_H_flat.glb", 
		"res://assets/trees/pine/Pine_L_flat.glb", 
		"res://assets/trees/pine/Pine_M_flat.glb", 
		"res://assets/trees/pine/Pine_S_flat.glb", 
		"res://assets/trees/spruce/Spruce_L_flat.glb", 
		"res://assets/trees/spruce/Spruce_M_flat.glb", 
		"res://assets/trees/spruce/Spruce_S_flat.glb"
	]
