#pragma once

#include "ECS.h"
#include "SDL.h"

class TileComponent : public Component
{
public:
	SDL_Texture* texture;
	SDL_Rect srcRect, destRect;
	TileComponent() = default;
	Vector2D position;
	TileComponent(int srcX, int srcY, int xpos, int ypos, const char* path)
	{
		texture = TextureManager::LoadTexture(path);
		srcRect.x = srcX;
		srcRect.y = srcY;
		srcRect.w = 32;
		srcRect.h = 32;

		position.x = xpos;
		position.y = ypos;

		destRect.x = xpos;
		destRect.y = ypos;
		destRect.w = 32;
		destRect.h = 32;
	}

	~TileComponent()
	{
		SDL_DestroyTexture(texture);
	}

	void Update() override
	{
		destRect.x = position.x - Game::camera.x;
		destRect.y = position.y - Game::camera.y;
	}

	void Draw() override
	{
		TextureManager::Draw(texture, srcRect, destRect, SDL_FLIP_NONE);
	}

};