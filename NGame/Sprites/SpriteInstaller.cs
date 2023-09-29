﻿using Microsoft.Extensions.DependencyInjection;
using NGame.Ecs;
using NGame.Setup;
using NGame.UpdateSchedulers;

namespace NGame.Sprites;

public static class SpriteInstaller
{
	public static INGameApplicationBuilder AddSprites(this INGameApplicationBuilder builder)
	{
		builder.Services.AddSingleton<SpriteRendererSystem>();

		return builder;
	}


	public static NGameApplication UseSprites(this NGameApplication app)
	{
		app.RegisterComponent<SpriteRenderer>();
		app.UseDrawable<SpriteRendererSystem>();
		app.RegisterSystem<SpriteRendererSystem>();


		return app;
	}
}
