﻿using NGame.Components.Lines;
using NGame.OsWindows;
using NGame.Renderers;
using SFML.Graphics;
using SFML.System;
using Font = SFML.Graphics.Font;
using NGameSprite = NGame.Components.Sprites.Sprite;
using NGameTexture = NGame.Components.Sprites.Texture;
using NGameTransform = NGame.Components.Transforms.Transform;
using NGameFont = NGame.Components.Texts.Font;
using NGameText = NGame.Components.Texts.Text;
using Sprite = SFML.Graphics.Sprite;
using Text = SFML.Graphics.Text;
using Texture = SFML.Graphics.Texture;


namespace NGamePlugin.Renderer2D.Sfml;



internal class RenderTextureFactory
{
	private readonly GraphicsConfiguration _graphicsConfiguration;


	public RenderTextureFactory(GraphicsConfiguration graphicsConfiguration)
	{
		_graphicsConfiguration = graphicsConfiguration;
	}


	public RenderTexture Create()
	{
		var width = (uint)_graphicsConfiguration.Width;
		var height = (uint)_graphicsConfiguration.Height;
		return new RenderTexture(width, height);
	}
}



public class SfmlRenderer : INGameRenderer
{
	private readonly IOsWindow _window;
	private readonly RenderTexture _renderTexture;

	private readonly Dictionary<NGameTexture, Texture> _textures = new();
	private readonly Dictionary<NGameFont, Font> _fonts = new();


	public SfmlRenderer(
		IOsWindow window,
		RenderTexture renderTexture
	)
	{
		_window = window;
		_renderTexture = renderTexture;
	}


	bool INGameRenderer.BeginDraw()
	{
		_renderTexture.Clear();
		return true;
	}


	void INGameRenderer.Draw(NGameSprite sprite, NGameTransform transform)
	{
		var texture = sprite.Texture;
		if (!_textures.ContainsKey(texture))
		{
			var filename = texture.FilePath;
			var image = new Texture(filename);
			_textures.Add(texture, image);
		}

		var spriteTexture = _textures[sprite.Texture];
		var sp = new Sprite(spriteTexture);

		sp.TextureRect = new IntRect(
			sprite.SourceRectangle.X,
			sprite.SourceRectangle.Y,
			sprite.SourceRectangle.Width,
			sprite.SourceRectangle.Height
		);

		sp.Position = new Vector2f(
			transform.Position.X + sprite.TargetRectangle.X,
			transform.Position.Y + sprite.TargetRectangle.Y
		);

		_renderTexture.Draw(sp);
	}


	void INGameRenderer.Draw(Line line)
	{
		var vertices =
			line
				.Vertices
				.Select(
					x => new Vertex(
						x.ToSfmlVector2(),
						line.Color.ToSfmlColor()
					)
				)
				.ToArray();


		for (int i = 0; i < vertices.Length - 1; i++)
		{
			var firstVertex = vertices[i];
			var secondVertex = vertices[i + 1];


			_renderTexture.Draw(new[] { firstVertex, secondVertex }, PrimitiveType.Lines);
		}
	}


	void INGameRenderer.Draw(NGameText nGameText, NGameTransform transform)
	{
		var nGameFont = nGameText.Font;
		if (!_fonts.ContainsKey(nGameFont))
		{
			_fonts.Add(nGameFont, new Font(nGameFont.FilePath));
		}

		var font = _fonts[nGameFont];
		var text = new Text(nGameText.Content, font);
		text.CharacterSize = (uint)nGameText.CharacterSize;
		text.Origin = nGameText.TransformOrigin.ToSfmlVector2();
		text.Position = transform.Position.ToSfmlVector2();
		text.FillColor = nGameText.Color.ToSfmlColor();

		_renderTexture.Draw(text);
	}


	void INGameRenderer.EndDraw(bool shouldPresent)
	{
		_renderTexture.Display();

		var texture = _renderTexture.Texture;
		var image = texture.CopyToImage();
		var pixels = image.Pixels;
		_window.Draw(pixels);
	}
}
