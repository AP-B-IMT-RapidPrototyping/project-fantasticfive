using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public partial class DebugConsole : VBoxContainer
{
    //! CONSOLE IS EXPERIMENTAL AND MAY *NOT* WORK IN EVERY PROJECT

    //TODO SETUP:
    //TODO 
    //TODO END OF SETUP


    private Node owner;
    // For sake of code, "cin" and "cout" don't have a _underscore, but they are still private
    private LineEdit cin;
    private RichTextLabel cout;

    private readonly Dictionary<string, (MethodInfo Method, object Target)> _commandRegistry = new();

    // Tabulate
    private List<string> _suggestions = new();
    private int _suggestionIndex = -1;
    private string _originalQuery = "";




    public override void _Ready()
    {
        cin = GetNode<LineEdit>("Console");
        cout = GetNode<RichTextLabel>("CommandOutput");


        cin.TextSubmitted += OnTextSubmitted;

        cin.GuiInput += OnConsoleGuiInput;

        RegisterCommands(this);

        if (GetOwner() != null)
        {
            RegisterCommands(GetOwner());
            owner = GetOwner();
        }
        else
        {
            GD.PrintErr($"{Name} can't find GetOwner");
        }
    }


    // ================================== //
    // GUI FUNCTIONALITY & INPUT LOGIC
    // ================================== //
    private List<string> GetAllActiveGroups()
    {
        HashSet<string> allGroups = new();

        // Search entire tree to find all available groups
        var stack = new Stack<Node>();
        stack.Push(GetTree().Root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            foreach (var group in current.GetGroups())
            {
                allGroups.Add(group.ToString());
            }

            foreach (Node child in current.GetChildren())
            {
                stack.Push(child);
            }
        }

        return allGroups.ToList();
    }

    private void OnConsoleGuiInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Tab)
            {
                AcceptEvent();
                HandleAutocomplete();
            }
            else if (keyEvent.Keycode != Key.Shift)
            {
                _suggestions.Clear();
                _suggestionIndex = -1;
            }
        }
    }

    private void HandleAutocomplete()
    {
        string currentText = cin.Text;

        if (_suggestions.Count == 0)
        {
            _suggestionIndex = -1;
            var parts = currentText.Split(' ');
            string lastPart = parts.Last();
            _originalQuery = lastPart;

            // Search for command names (se -> set)
            if (parts.Length == 1)
            {
                _suggestions = _commandRegistry.Keys.
                Where(k => k.StartsWith(lastPart.ToLower())).
                OrderBy(k => k).ToList();
            }
            // Search for groups using @ key, mc ahh commands
            else if (lastPart.StartsWith("@"))
            {
                string groupQuery = lastPart.Substring(1).ToLower();

                // Get all groups in current scene
                _suggestions = GetAllActiveGroups()
                .Where(g => g.ToLower().StartsWith(groupQuery))
                .Select(g => "@" + g)
                .OrderBy(g => g).ToList();
            }
        }

        // Go through tabulate commands
        if (_suggestions.Count > 0)
        {
            _suggestionIndex = (_suggestionIndex + 1) % _suggestions.Count;
            string match = _suggestions[_suggestionIndex];

            // Replace last word with match
            var words = cin.Text.Split(' ').ToList();
            words[words.Count - 1] = match;

            cin.Text = string.Join(" ", words);
            cin.CaretColumn = cin.Text.Length; // Put cursor at end (caret means blinking cursor, aka selected part, ykwim)
        }
    }



    // ================================== //
    // WORKING FUNCTIONS
    // ================================== //
    private void RegisterCommands(object target)
    {
        var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<ConsoleCommandAttribute>();
            if (attr != null)
            {
                _commandRegistry[attr.Name] = (method, target);
                PrintLine($"[color=green]Registered command:[/color] {attr.Name}");
            }
        }
    }

    private void OnTextSubmitted(string text)
    {
        cin.Clear();
        if (string.IsNullOrWhiteSpace(text))
        {
            cin.GrabFocus();
            return;
        }

        PrintLine($"[color=gray]> {text}[/color]");
        Execute(text);
    }

    private List<Node> FindTargets(string targetName)
    {
        List<Node> targets = new();

        if (string.IsNullOrWhiteSpace(targetName))
            return targets;

        string targetNameLower = targetName.ToLower();

        // starts with @ = search for Godot groups
        if (targetName.StartsWith("@"))
        {
            string groupName = targetName.Substring(1);
            var groupNodes = GetTree().GetNodesInGroup(groupName);

            foreach (var node in groupNodes)
            {
                targets.Add(node);
            }

            if (targets.Count == 0)
            {
                PrintLine($"[color=red]No nodes found in group '{groupName}'.[/color]");
            }

            return targets;
        }

        // check for keywords
        switch (targetNameLower)
        {
            case "me":
            case "player":
                if (owner != null)
                {
                    targets.Add(owner);
                }
                break;

            case "everything": // except player
                var everything = GetTree().CurrentScene.GetChildren();
                foreach (var node in everything)
                {
                    if (node.Name != "Player")
                    {
                        targets.Add(node);
                    }
                }
                break;

            default:
                Node found = GetTree().CurrentScene.FindChild(targetName, recursive: true, owned: false);
                if (found != null)
                {
                    targets.Add(found);
                }
                break;
        }

        if (targets.Count == 0)
        {
            PrintLine($"[color=red]Target '{targetName}' not found.[/color]");
        }

        return targets;
    }

    private bool ApplyValueToNode(Node targetNode, string variableName, string value)
    {
        // Helper function to apply value to node (health, etc)

        var type = targetNode.GetType();
        // example: search for field that is not static, no matter if it's public or private, and ignores case (HEALTH = health)
        var field = type.GetField(variableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
        var prop = type.GetProperty(variableName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

        try
        {
            // if a field or property is found , converts string to the field type, so string value becomes the type of the field ("10" -> 10f)
            // then overwrites the field value (targetNode) into new value (converted), (1 health -> 1000 health)
            if (field != null)
            {
                object converted = Convert.ChangeType(value, field.FieldType);
                field.SetValue(targetNode, converted);
                return true;
            }
            else if (prop != null && prop.CanWrite)
            {
                object converted = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(targetNode, converted);
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            PrintLine($"[color=red]Type mismatch for '{targetNode.Name}::{variableName}[/color]");
            return false;
        }
    }



    // ================================== //
    // EXECUTION FUNCTIONS
    // ================================== //
    private void Execute(string input)
    {
        // Split command into array, example: "set @player _health 10" -> ["set", "@player", "_health", "10"]
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cmdName = parts[0].ToLower();
        var args = parts.Skip(1).ToArray();

        if (_commandRegistry.TryGetValue(cmdName, out var command))
        {
            try
            {
                var methodParams = command.Method.GetParameters();
                object[] convertedArgs = new object[methodParams.Length];

                for (int i = 0; i < methodParams.Length; i++)
                {
                    if (i < args.Length)
                    {
                        convertedArgs[i] = Convert.ChangeType(args[i], methodParams[i].ParameterType);
                    }
                    else if (methodParams[i].HasDefaultValue)
                    {
                        convertedArgs[i] = methodParams[i].DefaultValue;
                    }
                    else
                    {
                        PrintLine($"[color=red]Usage Error: {cmdName} requires more arguments.[/color]");
                        CommandHelp(cmdName);
                        return;
                    }
                }

                command.Method.Invoke(command.Target, convertedArgs); // run the function
            }
            catch (Exception e)
            {
                string eMsg = e.InnerException?.Message ?? e.Message;
                PrintLine($"[color=red]Runtime Error in '{cmdName}': {eMsg}[/color]");
            }
        }
        else
        {
            PrintLine($"[color=red]Command not found:[/color] {cmdName}");
        }

        /* cin.GrabFocus(); */ // normally this should keep the focus on the lineedit, but just press enter again to refocus.. also this just doesn't work D:
    }

    private void PrintLine(string message)
    {
        cout.AppendText(message + "\n");

        var vScroll = cout.GetVScrollBar();
        vScroll.Value = vScroll.MaxValue;
    }



    // ==================================================================== //
    //                          BUILT-IN COMMANDS
    // ==================================================================== //

    // ================================== //
    // BASIC COMMANDS
    // ================================== //

    // test command
    [ConsoleCommand("test", "Prints \"Hello World!\"")]
    private void CommandTest()
    {
        PrintLine("Hello World!");
    }

    // help command
    [ConsoleCommand("help", "List all available commands")]
    private void CommandHelp(string cmdName = "")
    {
        if (string.IsNullOrWhiteSpace(cmdName))
        {
            PrintLine($"[color=yellow]Available Commands:[/color]");
            foreach (var cmd in _commandRegistry.Keys)
            {
                PrintLine($"- {cmd}");
            }
            PrintLine($"Type 'help <command>' for more info.");
            return;
        }

        if (_commandRegistry.TryGetValue(cmdName.ToLower(), out var command)){
            var attr = command.Method.GetCustomAttribute<ConsoleCommandAttribute>();
            var parameters = command.Method.GetParameters();

            string syntax = cmdName;
            foreach (var param in parameters)
            {
                syntax += param.HasDefaultValue ? $"[{param.Name}]" : $"<{param.Name}>";
            }

            PrintLine($"[color=yellow]Command: [/color] {cmdName}");
            PrintLine($"[color=white]Description: [/color] {attr.Description}");
            PrintLine($"[color=cyan]Syntax: [/color] {syntax}");
        }

        else
        {
            PrintLine($"[color=red]Error: Command '{cmdName}' does not exist.[/color]");
        }
    }

    // clear command
    [ConsoleCommand("clear", "Clears the console")]
    private void CommandClear()
    {
        cout.Clear();
        PrintLine("");
    }

    // exit command
    [ConsoleCommand("exit", "Exits the game")]
    private void CommandExit()
    {
        GetTree().Quit();
    }


    // ================================== //
    // SCENE COMMANDS
    // ================================== //

    // changescene command
    [ConsoleCommand("changescene", "Change the current scene. Syntax: changescene <scene_name.tscn>")]
    private void CommandChangeScene(string targetScene)
    {
        if (string.IsNullOrWhiteSpace(targetScene))
        {
            PrintLine($"[color=red]No scene name provided.[/color]");
            return;
        }

        string fullPath = $"{VariableManager.SceneRoot}{targetScene}.tscn";

        if (!FileAccess.FileExists(fullPath))
        {
            PrintLine($"[color=red]File '{VariableManager.SceneRoot + targetScene + ".tscn"}' not found.[/color]");
            return;
        }

        var tree = GetTree();
        if (GetTree().Paused)
        {
            GetTree().Paused = false;
        }

        Error result = tree.ChangeSceneToFile(fullPath);

        if (result == Error.Ok)
        {
            PrintLine($"[color=green]Changed scene to {targetScene}[/color]");
        }
        else
        {
            PrintLine($"[color=red]Failed to load scene. Engine Error: {result}[/color]");
        }
    }

    [ConsoleCommand("resetscene", "Reset the current scene.")]
    private void CommandResetScene()
    {
        if (GetTree().Paused)
        {
            GetTree().Paused = false;
        }

        GetTree().ReloadCurrentScene();
    }


    // ================================== //
    // ENTITY MANIPULATION COMMANDS
    // ================================== //

    // set command
    [ConsoleCommand("set", "Sets a variable on target. Syntax: set <target> <variable> <value>")]
    private void CommandSet(string targetName, string variableName, string value)
    {
        var targets = FindTargets(targetName);

        if (targets.Count == 0)
            return;

        int successCount = 0;
        foreach (var target in targets)
        {
            if (ApplyValueToNode(target, variableName, value))
            {
                successCount++;
            }
        }

        PrintLine($"[color=green]Updated {successCount} target(s).[/color]");
    }

    // up command
    [ConsoleCommand("up", "Launch target in air! Syntax: up <target> <value>")]
    private void CommandUp(string targetName, float amount)
    {
        var targets = FindTargets(targetName);

        if (targets.Count == 0)
            return;

        int successCount = 0;
        foreach (var target in targets)
        {
            if (target is Node3D targetN3)
            {
                targetN3.GlobalPosition += new Vector3(0, amount, 0);
                successCount++;
            }
        }

        if (successCount > 0)
        {
            PrintLine($"[color=green]Moved {successCount} target(s) up by {amount}m.[/color]");
        }
        else
        {
            PrintLine($"[color=red]Targets found, but they are not 3D entities.[/color]");
        }
    }

    // kill command
    [ConsoleCommand("kill", "Deletes target. Syntax: kill <target_name>")]
    private void CommandKill(string targetName)
    {
        var targets = FindTargets(targetName);

        foreach (var target in targets)
        {
            if (target == this || target == GetTree().CurrentScene)
                continue;

            PrintLine($"[color=green]Killed {target.Name}[/color]");
            target.QueueFree();
        }
    }

    // spawn command
    [ConsoleCommand("spawn", "Spawns a target. Syntax: spawn <target_name>")]
    private void CommandSpawn(string targetName)
    {
        PrintLine($"[color=yellow]This command is WIP[/color]");
    }

    // teleport to  command
    [ConsoleCommand("tp", "Teleport player to target. Syntax: teleport <target_name>")]
    private void CommandTp(string targetName)
    {
        PrintLine($"[color=yellow]This command is WIP[/color]");
    }

    // teleport here command
    [ConsoleCommand("tphere", "Teleport target to player. Syntax: tphere <target_name>")]
    private void CommandTpHere(string targetName)
    {
        PrintLine($"[color=yellow]This command is WIP[/color]");
    }
}
