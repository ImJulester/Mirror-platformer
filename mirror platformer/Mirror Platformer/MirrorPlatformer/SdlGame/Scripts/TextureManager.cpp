#include "TextureManager.h"
#include "Game.h"
//#include <iostream>

//Easy texture loading
SDL_Texture * TextureManager::LoadTexture(const char* texture)
{
	return IMG_LoadTexture(Game::renderer, texture);
}

void TextureManager::Draw(SDL_Texture * tex, SDL_Rect src, SDL_Rect dest, SDL_RendererFlip flip)
{
	SDL_Rect offset;
	offset = Game::camera;
	SDL_Rect temp;

	SDL_RenderCopyEx(Game::renderer, tex, &src, &dest, NULL, NULL, flip);
}
