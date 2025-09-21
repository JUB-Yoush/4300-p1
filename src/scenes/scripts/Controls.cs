using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public partial class Controls : Node
{
	//GetTree().Paused = true;
	//Panel preGame;
	CanvasLayer preGame;
	CanvasLayer paused;
	BaseButton procBtn;
	
	List<TextureButton> pauseButtons = new List<TextureButton>();
	TextureButton resBtn;
	TextureButton setBtn;
	TextureButton exitBtn;
	
	int currentButton = 0;
	
	Texture2D hoverTex;
	Texture2D idleTex;
	Texture2D clickTex;
	
	public override void _Ready()
	{
		//ProcessMode = Node.ProcessModeEnum.Inherit;
		//ProcessMode = Node.ProcessModeEnum.WhenPaused;
		
		GetTree().Paused = true;
		
	
		GD.Print("Test");
		
		//preGame = GetNode<Panel>("Pre-Game/Panel");
		preGame = GetNode<CanvasLayer>("Pre-Game");
		preGame.Show();
		
		paused = GetNode<CanvasLayer>("Paused");
		paused.Hide();
		
		procBtn = GetNode<BaseButton>("Pre-Game/Panel/Proceed");
		resBtn = GetNode<TextureButton>("Paused/Resume");
		setBtn = GetNode<TextureButton>("Paused/Settings");
		exitBtn = GetNode<TextureButton>("Paused/Home");
		
		pauseButtons.Add(resBtn);
		pauseButtons.Add(setBtn);
		pauseButtons.Add(exitBtn);
		
		idleTex = GD.Load<Texture2D>("res://assets/UI/Button_Idle.svg");
		hoverTex = GD.Load<Texture2D>("res://assets/UI/Button_Hover.svg");
		clickTex = GD.Load<Texture2D>("res://assets/UI/Button_Click.svg");
	}
	
	private void OnPauseButtonPressed()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
		GD.Print("Pause");
		GetTree().Paused = true;
		ProcessMode = Node.ProcessModeEnum.WhenPaused;
		//GetTree().Paused = true;
		currentButton = 0;
		paused.Show();
		//OnPauseButtonPressed();
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Tab)
			{
				OnPauseButtonPressed();
				
			}
			
			if (paused.IsVisible())
			{
				// navigating paused buttons, starts at Resume button
				if (eventKey.Pressed && (eventKey.Keycode == Key.Up || eventKey.Keycode == Key.Down))
				{
					if (eventKey.Keycode == Key.Up)
					{
						GD.Print("Up");
						if (currentButton == 0)
						{
							currentButton = 0;
						}
						else
						{
							currentButton = currentButton - 1;
							pauseButtons[currentButton].TextureNormal = hoverTex;
							pauseButtons[currentButton + 1].TextureNormal = idleTex;
						}
					}
					else
					{
						GD.Print("Down");
						if (currentButton == 2)
						{
							GD.Print(currentButton);
							currentButton = 2;
						}
						else
						{
							GD.Print("moved");
							GD.Print(currentButton);
							currentButton = currentButton + 1;
							pauseButtons[currentButton].TextureNormal = hoverTex;
							pauseButtons[currentButton - 1].TextureNormal = idleTex;
						}
					}
					
				}
				
				if (eventKey.Pressed && eventKey.Keycode == Key.Enter)
				{
					GD.Print("Enter");
					if (currentButton == 0 || currentButton == 1) 
					{
						OnResumeButtonPressed();
					}
					else if (currentButton == 2)
					{
						OnExitButtonPressed();
					}
					
				}
			}
			else if (preGame.IsVisible())
			{
				if (eventKey.Pressed && eventKey.Keycode == Key.Enter)
				{
					OnProceedButtonPressed();
				}
			}
		}
	}

	private void OnProceedButtonPressed()
	{
		GD.Print("Proceed");
		ProcessMode = Node.ProcessModeEnum.Always;
		
		GetTree().Paused = false;
		ProcessMode = Node.ProcessModeEnum.Inherit;
		preGame.Hide();
		
		//ProcessMode = Node.ProcessModeEnum.Inherit;
	}
	
	private void OnResumeButtonPressed()
	{
		GD.Print("Resume");
		ProcessMode = Node.ProcessModeEnum.Always;
		
		GetTree().Paused = false;
		ProcessMode = Node.ProcessModeEnum.Inherit;
		paused.Hide();
	}
	
	private void OnExitButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://src/scenes/title.tscn");
	}
}
